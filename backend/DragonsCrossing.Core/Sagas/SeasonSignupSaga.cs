using System;
using System.ComponentModel.DataAnnotations;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.NewCombatLogic;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DragonsCrossing.Core.Sagas;

public class LoanerSignupDetails
{
    [Required]
    public string authorization { get; set; } = String.Empty;

    [Required]
    public string loanedToUserId { get; set; } = String.Empty;

    [Required]
    public byte[] orderHash { get; set; } = new byte[] { };
}

public class SignUpToSeasonOrder : DcxOrder
{
    [Required]
    public int heroId { get; set; }


    public LoanerSignupDetails? loanerDetails { get; set; }

    [MongoIndex]
    public static void CreateIndexes(IMongoCollection<SignUpToSeasonOrder> collection)
    {
        collection.Indexes.CreateOne(
            new CreateIndexModel<SignUpToSeasonOrder>(
            new IndexKeysDefinitionBuilder<SignUpToSeasonOrder>()
                .Ascending(f => f.type)
                .Ascending(f => f.heroId)
                .Ascending(f => f.seasonId)
                , new CreateIndexOptions<SignUpToSeasonOrder>
                {
                    Unique = true,
                    PartialFilterExpression = Builders<SignUpToSeasonOrder>.Filter.Eq(o => o.type, nameof(SignUpToSeasonOrder))
                }
            ));
    }
}

public class SignUpToSeasonOrdered : CorrelatedBy<Guid>
{
    public SignUpToSeasonOrder Details { get; }
    public Guid CorrelationId { get; }

    [Newtonsoft.Json.JsonConstructor]
    public SignUpToSeasonOrdered(SignUpToSeasonOrder Details, Guid CorrelationId)
    {
        this.CorrelationId = CorrelationId;
        this.Details = Details;
    }
}

[BsonIgnoreExtraElements]
public class SignUpToSeasonState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public int Version { get; set; }

    public SignUpToSeasonOrder Details { get; set; } = new SignUpToSeasonOrder();

}

public class SignUpToSeasonSM : MassTransitStateMachine<SignUpToSeasonState>
{
    public State WaitingForPayment { get; private set; }

    public Event<SignUpToSeasonOrdered> OnOrdered { get; private set; }
    public Event<DcxTxConfirmed> OnPaymentReceived { get; private set; }

    public SignUpToSeasonSM(
        IPerpetualDbService _perpetualDb,
        IServiceProvider sp,
        ILogger<SignUpToSeasonSM> _logger
        )
    {

        InstanceState(x => x.CurrentState);

        Initially(
            When(OnOrdered)
            .Then(x =>
            {
                x.Saga.Details = x.Message.Details;
            })
            .TransitionTo(WaitingForPayment)
            );

        During(WaitingForPayment,
            When(OnPaymentReceived)
            .ThenAsync(async x =>
            {
                var confirmedTxn = x.Message.confirmation;

                if (confirmedTxn.orderId != x.Saga.Details.id)
                    throw new InvalidOperationException("Incorrect orderId, expecting {x.Saga.Details.id}, got {confirmedTxn.orderId}");


                if (confirmedTxn.dcxAmountInEther < x.Saga.Details.priceInDcx )
                    throw new InvalidOperationException($"Incorrect fees paid expecting {x.Saga.Details.priceInDcx}, got {confirmedTxn.dcxAmountInEther}");

                _logger.LogInformation($"Hero {x.Saga.Details.heroId} signed up to season {x.Saga.Details.seasonId}");

                
                {
                    var fulfillmentTxnHash = string.Empty;
                    for (var i = 0; i < 5; i++)
                    {
                        var orderCollection = _perpetualDb.getCollection<DcxOrder>().OfType<SignUpToSeasonOrder>();
                        fulfillmentTxnHash = await orderCollection.Find(l => l.id == x.Saga.Details.id)
                            .Project(o=>o.fulfillmentTxnHash)
                            .SingleAsync();

                        if (string.IsNullOrWhiteSpace(fulfillmentTxnHash))
                        {
                            _logger.LogDebug($"fulfillmentTxnHash is not yet available for order {x.Saga.Details.id}. Wait a bit");
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                        else
                        {
                            break;
                        }
                    }

                    if(fulfillmentTxnHash != confirmedTxn.txnHash)
                    {
                        throw new InvalidOperationException($"Incorrect tx Hash expected {fulfillmentTxnHash} got {confirmedTxn.txnHash}");
                    }


                    await x.Publish(new SeasonSignupMessage
                    {
                        heroId = x.Saga.Details.heroId,
                        seasonId = x.Saga.Details.seasonId
                    });
                }
            }
            )
            .Finalize()
            );



    }


}

