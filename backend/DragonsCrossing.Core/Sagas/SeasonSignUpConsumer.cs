using System;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.NewCombatLogic;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DragonsCrossing.Core.Sagas;

public class SeasonSignupMessage
{
    public int seasonId { get; set; }
    public int heroId { get; set; }

}

public class SeasonSignUpConsumer : IConsumer<SeasonSignupMessage>
{
    readonly ILogger _logger;
    readonly IPerpetualDbService _perpetualDb;
    readonly IServiceProvider _sp;

    public SeasonSignUpConsumer(
    IPerpetualDbService perpertualDb,
    IServiceProvider sp,
    ILogger<MaintenanceConsumer> logger)
    {
        _sp = sp;
        _perpetualDb = perpertualDb;
        _logger = logger;
    }


    /// <summary>
    /*
     * DROP in SeasonSignUp to force register
    {
        "messageType": [
            "urn:message:DragonsCrossing.Core.Sagas:SeasonSignupMessage"
        ],
        "message": {
            "seasonId": 101,
            "heroId": 1
        }
    }
    */
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Consume(ConsumeContext<SeasonSignupMessage> context)
    {
        _logger.LogInformation($"Consume SeasonSignupMessage", context.Message);

        using (var session = await _perpetualDb.StartTransaction())
        {

            var done = await _perpetualDb.getCollection<HeroDto>().
                UpdateOneAsync(session, c => c.id == context.Message.heroId && 0 == c.seasonId,
                Builders<HeroDto>.Update.Set(h => h.seasonId, context.Message.seasonId)
                );


            if (done.MatchedCount != 1)
            {
                throw new InvalidOperationException($"SignUpToSeasonSM:  Hero {context.Message.heroId} already has seasonId, failed to update");
            }

            var hero = await _perpetualDb.getCollection<HeroDto>().Find(session, c => c.id == context.Message.heroId
                        && context.Message.seasonId == c.seasonId).SingleAsync();

            ISeasonsDbService _seasonsDb = ActivatorUtilities.CreateInstance<SeasonsDbService>(_sp, context.Message.seasonId);

            await _seasonsDb.getCollection<DbGameState>().InsertOneAsync(new DbGameState
            {
                Hero = hero
            });

            await session.CommitTransactionAsync();
        }
    }

}

