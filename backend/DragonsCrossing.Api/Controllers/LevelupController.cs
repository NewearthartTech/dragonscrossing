using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.GameStates;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Sagas;
using DragonsCrossing.Core.Services;
using DragonsCrossing.Domain.GameStates;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Helper;

namespace DragonsCrossing.Api.Controllers;

/// <summary>
/// This is a child of heroes and shouldn't have a root url.
/// </summary>

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "SeasonJoined")]
public class LevelupController : ControllerBase
{
    readonly ISeasonsDbService _db;
    readonly IPublishEndpoint _publishEp;
    readonly LevelUpConfig _config;
    readonly IDiceService _dice;
    readonly ILogger _logger;

    public LevelupController(ISeasonsDbService db,
        IConfiguration config,
        IDiceService dice,
        IPublishEndpoint publishEp,
        ILogger<GameStatesController> logger)
    {
        _publishEp = publishEp;
        _db = db;
        _dice = dice;
        _config = config.GetSection("levelUp").Get<LevelUpConfig>() ?? new LevelUpConfig();
        _logger = logger;
    }


    [HttpPost]
    public async Task<LevelUpOrder> postLevelUpOrder([FromBody] LevelUpOrder order)
    {

        if (!string.IsNullOrWhiteSpace(order.fulfillmentTxnHash))
        {
            //this is a way to fix duplicate issue while we are NOT using blockchain
            order.fulfillmentTxnHash = $"DUMMY-{Guid.NewGuid()}";
        }

        var heroId = this.GetHeroId();
        var collection = _db.getCollection<DcxOrder>().OfType<LevelUpOrder>();
        var gameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();
        var hero = gameState.Hero;

        if (order.chosenProps.Count() > 0)
        {
            order.ensureStatsAllocationCorrect();
        }

        // As of now, we have a level cap at 20 and cannot go above that.
        if (order.currentLevel >= 20)
        {
            throw new Exception("Level cap is 20");
        }

        var existing = await collection.Find(l => l.heroId == heroId && l.currentLevel == hero.level).SingleAsync();

        if (!String.IsNullOrWhiteSpace(existing.fulfillmentTxnHash))
            throw new InvalidOperationException("transaction already submitted");

        await collection.UpdateOneAsync(l => l.heroId == heroId && l.currentLevel == hero.level,
            Builders<LevelUpOrder>.Update
            .Set(o => o.chosenProps, order.chosenProps)
            .Set(o => o.fulfillmentTxnHash, order.fulfillmentTxnHash)
            );

        var updatedOrder = await getLevelUpOrder(hero.level);

        if (!String.IsNullOrWhiteSpace(updatedOrder.fulfillmentTxnHash) &&
            String.IsNullOrWhiteSpace(existing.fulfillmentTxnHash))
        {
            var ordered = new LevelUpOrdered(order);
            await _publishEp.Publish(ordered);
            //await _publishEp.Publish(new CheckTxConfirmation(updatedOrder.fulfillmentTxnHash, ordered.CorrelationId));
        }

        return updatedOrder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="fromLevel">what level we are trying to level up from. Can also be used to retrieve information of a completed levelup</param>
    /// <returns></returns>
    /// <exception cref="ExceptionWithCode"></exception>
    [HttpGet("{fromLevel}")]
    public async Task<LevelUpOrder> getLevelUpOrder(int fromLevel)
    {
        var heroId = this.GetHeroId();
        var collection = _db.getCollection<DcxOrder>().OfType<LevelUpOrder>();

        var hero = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).Project(g=>g.Hero).SingleAsync();

        var existing = await collection.Find(l => l.heroId == heroId && l.currentLevel == fromLevel).SingleOrDefaultAsync();

        if (null != existing)
            return existing;

        if(fromLevel!= hero.level)
        {
            throw new ExceptionWithCode("Cannot level up beyound next level");
        }

        if (hero.experiencePoints < hero.maxExperiencePoints)
            throw new ExceptionWithCode("Not enough experience");

        // As of now, we have a level cap at 20 and cannot go above that.
        if (fromLevel >= 20)
        {
            throw new ExceptionWithCode("Level cap is 20");
        }

        var diceRolls = new List<DieResultDto>();

        var hitPointsDiceRoll = _dice.Roll(4,DiceRollReason.levelUpHitPoints, diceRolls);

        var statsDiceRoll = _dice.Roll(4, DiceRollReason.levelUpStats, diceRolls);

        var heroClass = hero.heroClass;
        var heroclassStatName = string.Empty;

        var heroClassPropsDic = new Dictionary<string, int>() { };

        #region Class based guarenty stats gain 

        Dictionary<CharacterClassDto, LevelUpClassBenefit> benefitSchedule = DataHelper.CreateTypefromJsonTemplate($"ScheduleTableRef.levelUpClassSpecificBenefit.json",
                    new Dictionary<CharacterClassDto, LevelUpClassBenefit>());

        /// Every other level, meaning when leveling up from level 2, 4, 6....18, hero will get this additional benefit.
        switch (heroClass)
        {
            case CharacterClassDto.Warrior:
                    if (benefitSchedule.ContainsKey(CharacterClassDto.Warrior) && benefitSchedule[CharacterClassDto.Warrior].baseHitPoints.ContainsKey(fromLevel.ToString()))
                    {
                        heroClassPropsDic.Add(nameof(LevelUpProps.baseHitPoints), benefitSchedule[CharacterClassDto.Warrior].baseHitPoints[fromLevel.ToString()]);
                    }
                    else 
                    {
                        _logger.LogError("Cannot find Warrior baseHitPoints benefit from levelUpClassSpecificBenefit schedule");
                    }
                    break;

            case CharacterClassDto.Ranger:
                    if (benefitSchedule.ContainsKey(CharacterClassDto.Ranger) && benefitSchedule[CharacterClassDto.Ranger].baseHitPoints.ContainsKey(fromLevel.ToString()))
                    {
                        heroClassPropsDic.Add(nameof(LevelUpProps.baseHitPoints), benefitSchedule[CharacterClassDto.Ranger].baseHitPoints[fromLevel.ToString()]);
                    }
                    else
                    {
                        _logger.LogError("Cannot find Ranger baseHitPoints benefit from levelUpClassSpecificBenefit schedule");
                    }
                    if (benefitSchedule.ContainsKey(CharacterClassDto.Ranger) && benefitSchedule[CharacterClassDto.Ranger].quickness.ContainsKey(fromLevel.ToString()))
                    {
                        heroClassPropsDic.Add(nameof(LevelUpProps.quickness), benefitSchedule[CharacterClassDto.Ranger].quickness[fromLevel.ToString()]);
                    }
                    else
                    {
                        _logger.LogError("Cannot find Ranger quickness benefit from levelUpClassSpecificBenefit schedule");
                    }
                    break;

            case CharacterClassDto.Mage:
                    if (benefitSchedule.ContainsKey(CharacterClassDto.Mage) && benefitSchedule[CharacterClassDto.Mage].baseHitPoints.ContainsKey(fromLevel.ToString()))
                    {
                        heroClassPropsDic.Add(nameof(LevelUpProps.baseHitPoints), benefitSchedule[CharacterClassDto.Mage].baseHitPoints[fromLevel.ToString()]);
                    }
                    else
                    {
                        _logger.LogError("Cannot find Mage baseHitPoints benefit from levelUpClassSpecificBenefit schedule");
                    }
                    if (benefitSchedule.ContainsKey(CharacterClassDto.Mage) && benefitSchedule[CharacterClassDto.Mage].charisma.ContainsKey(fromLevel.ToString()))
                    {
                        heroClassPropsDic.Add(nameof(LevelUpProps.charisma), benefitSchedule[CharacterClassDto.Mage].charisma[fromLevel.ToString()]);
                    }
                    else
                    {
                        _logger.LogError("Cannot find Mage charisma benefit from levelUpClassSpecificBenefit schedule");
                    }
                    break;

            default:
                break;
        }   
        #endregion
        var order = new LevelUpOrder
        {
            heroId = heroId,
            currentLevel = hero.level,
            newLevel = hero.level + 1,

            seasonId = this.GetSeasonId(),

            priceInDcx = _config.feesInDsx,

            statsPoints = new BaseAndExtra
            {
                basePoints = 2,
                xtraPoints = statsDiceRoll
            },
            hitPoints = new BaseAndExtra
            {
                basePoints = 3,
                xtraPoints = hitPointsDiceRoll
            },
            skillPoints = new BaseAndExtra
            {
                basePoints = 1
            },
            diceRolls = diceRolls.ToArray(),
            classSpecificProps = heroClassPropsDic
        };

        await collection.InsertOneAsync(order);

        return order;

    }

    public class LevelUpClassBenefit 
    {
        public Dictionary<string, int> baseHitPoints { get; set; }

        public Dictionary<string, int> quickness { get; set; }

        public Dictionary<string, int> charisma { get; set; }
    }

}

