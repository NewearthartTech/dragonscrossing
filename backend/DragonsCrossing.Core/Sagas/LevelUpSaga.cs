using System;
using System.ComponentModel.DataAnnotations;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using Newtonsoft.Json;
using DragonsCrossing.Core.Helper;
using Microsoft.Extensions.DependencyInjection;

namespace DragonsCrossing.Core.Sagas;

[BsonIgnoreExtraElements]
public class LevelUpState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public int Version { get; set; }

    public LevelUpOrder Details { get; set; } = new LevelUpOrder();

}

public class LevelUpSM : MassTransitStateMachine<LevelUpState>
{
    public State WaitingForPayment { get; private set; }

    public Event<LevelUpOrdered> OnOrdered { get; private set; }
    public Event<DcxTxConfirmed> OnPaymentReceived { get; private set; }

    public LevelUpSM(
        IServiceProvider sp,
        IConfiguration config,
        ILogger<LevelUpSM> _logger
        )
    {

        InstanceState(x => x.CurrentState);

        Initially(
            When(OnOrdered)
            /*.Then(x =>
            {
                x.Saga.Details = x.Message.Details;
            })
            .TransitionTo(WaitingForPayment)
            );

        During(WaitingForPayment,
            When(OnPaymentReceived)*/
            .ThenAsync(async x =>
            {

                x.Saga.Details = x.Message.Details;

                if (0 == x.Saga.Details.seasonId)
                {
                    throw new InvalidOperationException("seasonId is null");
                }
                ISeasonsDbService _db = ActivatorUtilities.CreateInstance<SeasonsDbService>(sp, x.Saga.Details.seasonId);

                /*
                if (x.Saga.Details.priceInDcx < x.Message.confirmation.dcxAmountInEther)
                    throw new InvalidOperationException("received less dcx");

                if (x.Saga.Details.id != x.Message.confirmation.orderId)
                    throw new InvalidOperationException("order id mismatch");
                */

                var _config = config.GetSection("levelUp").Get<LevelUpConfig>() ?? new LevelUpConfig();

                var heroId = x.Saga.Details.heroId;

                var ordersCollection = _db.getCollection<LevelUpOrder>();

                //var confirmedTxn = x.Message.confirmation;

                var levelupOrder = await ordersCollection.Find(o =>
                o.id == x.Message.Details.id
                /*o.id == confirmedTxn.orderId
                && !string.IsNullOrEmpty(o.fulfillmentTxnHash)
                */
                && !o.IsCompleted
                ).SingleAsync();

                if (levelupOrder.heroId != heroId)
                    throw new Exception("heroIds don't match");

                var gameStateCollection = _db.getCollection<DbGameState>();
                var gameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();


                if (gameState.Hero.level >= levelupOrder.newLevel)
                {
                    _logger.LogInformation($"Hero {heroId} is already at level {levelupOrder.newLevel}");
                    return;
                }

                /*
                if (confirmedTxn.dcxAmountInEther < _config.feesInDsx)
                    throw new Exception("Incorrect fees paid");
                */

                var newHero = gameState.Hero;

                var levelUpProps = new LevelUpProps
                {
                    baseSkillPoints = levelupOrder.skillPoints.total(),
                    baseHitPoints = levelupOrder.hitPoints.total()
                };

                levelupOrder.ensureStatsAllocationCorrect();

                foreach (var choosen in levelupOrder.chosenProps)
                {
                    if (0 == choosen.Value)
                        continue;

                    var prop = typeof(LevelUpProps).GetProperty(choosen.Key);
                    if (null == prop)
                        throw new InvalidOperationException($"the prop {choosen.Key} is invalid");

                    var currentValue = prop.GetValue(levelUpProps);

                    if (null == currentValue)
                    {
                        throw new InvalidOperationException("Int value should never be null. We have logic error");
                    }

                    var num = (int)currentValue;
                    var sum = 0;

                    if (choosen.Key == (nameof(LevelUpProps.baseHitPoints)))
                    {
                        // For every 2 points allocated for hitpoint, we only grant 1 extra hp
                        sum = num + choosen.Value / 2;
                    }
                    else 
                    {
                        sum = num + choosen.Value;
                    }

                    _logger.LogDebug($"Prop name is:{prop.Name} and will be set to {choosen.Value}. chosen key is {choosen.Key}");
                    prop.SetValue(levelUpProps, sum);
                }

                // Loop through the classSpecificProps and add the values to levelUpProps
                foreach (var choosen in levelupOrder.classSpecificProps)
                {
                    if (0 == choosen.Value)
                        continue;

                    var prop = typeof(LevelUpProps).GetProperty(choosen.Key);
                    if (null == prop)
                        throw new InvalidOperationException($"the class specific prop {choosen.Key} is invalid");

                    var currentValue = prop.GetValue(levelUpProps);

                    if (null == currentValue)
                    {
                        throw new InvalidOperationException("Int value should never be null. We have logic error");
                    }

                    var num = (int)currentValue;
                    var sum = num + choosen.Value;

                    _logger.LogDebug($"Prop name is:{prop.Name} and will be set to {choosen.Value}. chosen key is {choosen.Key}");
                    prop.SetValue(levelUpProps, sum);
                }

                newHero.AdjustHeroProps(false, levelUpProps);
                newHero.level++;
                newHero.heroLevelUpProps[newHero.level] = levelUpProps;
                newHero.experiencePoints = 0;

                newHero.UpdateMaxExperiencePoints();

                await gameStateCollection.UpdateOneAsync(g => g.HeroId == heroId

                    //todo: dee Ensure we don't level it twice (race condition). This is breaking right now 
                    /*&& g.Hero.Level == gameState.Hero.Level*/,

                    Builders<DbGameState>.Update.Set(g => g.Hero, newHero)
                    );

                await ordersCollection.UpdateOneAsync(o => o.id == levelupOrder.id,
                    Builders<LevelUpOrder>.Update.Set(o => o.IsCompleted, true));

            }
            )
            .Finalize()
            );

        SetCompletedWhenFinalized();

    }

    
}

public class DcxTxConfirmed : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; }
    public DcxTxConfirmation confirmation { get; }

    [JsonConstructor]
    public DcxTxConfirmed(Guid CorrelationId, DcxTxConfirmation confirmation)
    {
        this.CorrelationId = CorrelationId;
        this.confirmation = confirmation;
    }
}

public class LevelUpOrdered : CorrelatedBy<Guid>
{
    public LevelUpOrder Details { get; }
    public Guid CorrelationId { get; }

    [Newtonsoft.Json.JsonConstructor]
    public LevelUpOrdered(LevelUpOrder Details)
    {
        this.CorrelationId = Guid.Parse( Details.id);
        this.Details = Details;
    }

    
}



/// <summary>
/// The order saved in the Database
/// </summary>
[MongoCollection("DcxOrders")]
[BsonIgnoreExtraElements]
[BsonDiscriminator(Required = true, RootClass = true)]
[BsonKnownTypes(typeof(LevelUpOrder),typeof(SummonHeroOrder),
    typeof(SecuredNFTsOrder), typeof(ClaimDcxOrder),
    typeof(SignUpToSeasonOrder),
    typeof(NftActionOrder)
 )]
[System.Text.Json.Serialization.JsonConverter(typeof(PolymorphicConverter<DcxOrder>))]
public class DcxOrder : PolymorphicBase<DcxOrder>
{
    /// <summary>
    /// the order ID
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    [Required]
    public string id { get; set; } = Guid.NewGuid().ToString();

    public int seasonId { get; set; }

    [BsonIgnoreIfNull]
    /// <summary>
    /// The blockchain Transaction that completed this transaction
    /// </summary>
    public string? fulfillmentTxnHash { get; set; }

    public DateTime fulfillmentTxnHashTime {get; set;}

    [Required]
    public bool IsCompleted { get; set; }

    public DateTime created { get; set; } = DateTime.Now;

    [Required]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal priceInDcx { get; set; }

    [MongoIndex]
    public static void CreateIndexes(IMongoCollection<DcxOrder> collection)
    {
        //disallow two redemptions with the same redeem count
        collection.Indexes.CreateOne(
            new CreateIndexModel<DcxOrder>(
            new IndexKeysDefinitionBuilder<DcxOrder>()
                .Ascending(f => f.fulfillmentTxnHash)
                , new CreateIndexOptions<DcxOrder>
                {
                    Unique = true,
                    Sparse = true,
                }
            ));
    }

    public string? forWallet { get; set; }

    [Required]
    public long chainId { get; set; }

}

public class BaseAndExtra
{
    /// <summary>
    /// This is the points that's available to all level up orders
    /// </summary>
    [Required]
    public int basePoints { get; set; }

    /// <summary>
    /// This is dependent to die roll
    /// </summary>
    [Required]
    public int xtraPoints { get; set; }

    public int total()
    {
        return basePoints + xtraPoints;
    }
}

public class LevelUpOrder : DcxOrder
{
    [Required]
    public int heroId { get; set; }

    /// <summary>
    /// The Points available to be allocatied
    /// </summary>
    [Required]
    public BaseAndExtra statsPoints { get; set; } = new BaseAndExtra();

    [Required]
    public BaseAndExtra hitPoints { get; set; } = new BaseAndExtra();

    [Required]
    public BaseAndExtra skillPoints { get; set; } = new BaseAndExtra();

    /// <summary>
    /// The prop chosen by player to level up
    /// </summary>
    [Required]
    public Dictionary<string, int> chosenProps { get; set; } = new Dictionary<string, int>();

    /// <summary>
    /// The props pre defined for each hero based on hero class
    /// </summary>
    [Required]
    public Dictionary<string, int> classSpecificProps { get; set; } = new Dictionary<string, int>();

    /// <summary>
    /// The current level the hero is at
    /// Hero level are 1 indexed
    /// </summary>
    [Required]
    public int currentLevel { get; set; }

    /// <summary>
    /// The new Hero level that the hero will go to
    /// </summary>
    [Required]
    public int newLevel { get; set; }

    [Required]
    public DieResultDto[] diceRolls { get; set; } = new DieResultDto[] { };

    [MongoIndex]
    public static void CreateIndexes(IMongoCollection<LevelUpOrder> collection)
    {
        //disallow two redemptions with the same redeem count
        collection.Indexes.CreateOne(
            new CreateIndexModel<LevelUpOrder>(
            new IndexKeysDefinitionBuilder<LevelUpOrder>()
                .Ascending(f=>f.type)
                .Ascending(f => f.heroId)
                .Ascending(f => f.currentLevel)
                , new CreateIndexOptions<LevelUpOrder> {
                    Unique = true,
                    PartialFilterExpression = Builders<LevelUpOrder>.Filter.Eq(o=>o.type, nameof(LevelUpOrder))
                }
            ));
    }

    public void ensureStatsAllocationCorrect()
    {
        if (statsPoints.total() != chosenProps.Sum(p => p.Value))
        {
            throw new ExceptionWithCode("allocation is not correct");
        }

        if (chosenProps.Any(p => p.Value > 2))
        {
            throw new Exception("Each stat can only have max of 2 points allocated");
        } 
    }

}


public class LevelUpConfig
{
    public decimal feesInDsx { get; set; } = 2.25m;
}



