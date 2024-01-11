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

/// <summary>
/// used to run various maintenance tasks
/// </summary>
public class MaintenanceConsumer : IConsumer<RunMaintenanceMessage>
{
    readonly ILogger _logger;
    readonly IPerpetualDbService _perpetualDb;
    readonly IServiceProvider _sp;

    public MaintenanceConsumer(
        IPerpetualDbService perpertualDb,
        IServiceProvider sp,
        ILogger<MaintenanceConsumer> logger)
    {
        _sp = sp;
        _perpetualDb = perpertualDb;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RunMaintenanceMessage> context)
    {
        _logger.LogInformation($"Consume RunMaintenanceMessage", context.Message);

        switch (context.Message.maintenanceType)
        {
            case MaintenanceType.saveSkillsFromSeason:
                if (await saveSkillsFromSeason(context.Message.seasonId!.Value))
                {
                    //we have more data
                    await context.Defer(TimeSpan.FromTicks(1));
                }
                break;
            case MaintenanceType.updateSkillVersion:
                if (await updateSkillVersion(context.Message.newSkillsVersion!.Value))
                {
                    //we have more data
                    await context.Defer(TimeSpan.FromTicks(1));
                }
                break;
            case MaintenanceType.loadMissingSkillsfromPerpetual:
                await loadMissingSkillsfromPerpetual(context);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /*
    {
        "messageType": [
            "urn:message:DragonsCrossing.Core.Sagas:RunMaintenanceMessage"
        ],
        "message": {
            "maintenanceType": 1,
            "newSkillsVersion": 1
        }
    }
    */
    /// </summary>
    /// <param name="newSkillsVersion"></param>
    /// <returns></returns>
    async Task<bool> updateSkillVersion(int newSkillsVersion)
    {
        var herosToFix = (await _perpetualDb.getCollection<HeroDto>()
            .Find(Builders<HeroDto>.Filter.ElemMatch(
                h => h.skills, Builders<LearnedHeroSkill>.Filter.Where(s => s.version != newSkillsVersion)
                ))
            .Limit(10)
            .ToListAsync()).ToArray();


        var toRun = herosToFix.Select(hero =>
        {
            var newSkills = hero.skills.Select(s =>
            {
                var unLearnedSkill = DataHelper.CreateTypefromJsonTemplate($"templates.skills.{s.slug}.json", new UnlearnedHeroSkill());

                s.description = unLearnedSkill.description;
                s.requiredSkillPoints = unLearnedSkill.requiredSkillPoints;

                s.version = newSkillsVersion;

                return s;

            }).ToArray();

            return new UpdateOneModel<HeroDto>(
                Builders<HeroDto>.Filter.Where(r => r.id == hero.id),
                Builders<HeroDto>.Update
                    .Set(r => r.skills, newSkills)
                );

        }).ToArray();

        if (toRun.Length > 0)
        {
            var done = await _perpetualDb.getCollection<HeroDto>()
            .BulkWriteAsync(toRun);

            _logger.LogInformation($"updateSkillVersion modified: {done.ModifiedCount}");
            return true;
        }
        else
        {
            _logger.LogInformation($"No more heros for updateSkillVersion");
            return false;
        }

    }


    /// <summary>
    /*
    {
        "messageType": [
            "urn:message:DragonsCrossing.Core.Sagas:RunMaintenanceMessage"
        ],
        "message": {
            "maintenanceType": 2,
            "seasonId": 2
        }
    }
    */
    /// submit to maintennace Exchange
    /// </summary>
    /// <param name="seasonId"></param>
    /// <returns></returns>
    async Task loadMissingSkillsfromPerpetual(ConsumeContext<RunMaintenanceMessage> context)
    {
        if (null == context.Message.seasonId)
            throw new InvalidOperationException("seasonId is 0");

        ISeasonsDbService _seasonDb = ActivatorUtilities.CreateInstance<SeasonsDbService>(_sp, context.Message.seasonId);

        if(null == context.Message.heroId)
        {
            _logger.LogInformation($"loadMissingSkillsfromPerpetual: loading all heros");

            var herosinSeason = await _seasonDb.getCollection<DbGameState>()
                .Find(g => true)
                .Project(g => g.HeroId)
                .ToListAsync();


            var tasks = await Task.WhenAll( herosinSeason.Select(async heroId =>
            {
                await context.Publish(new RunMaintenanceMessage
                {
                    seasonId = context.Message.seasonId,
                    heroId = heroId,
                    maintenanceType= MaintenanceType.loadMissingSkillsfromPerpetual
                });

                return true;
            }));

            _logger.LogInformation($"loadMissingSkillsfromPerpetual: set up {tasks.Length} heros");

            return;
        }

        _logger.LogInformation($"loadMissingSkillsfromPerpetual: fixing Hero {context.Message.heroId}");

        var herosSkillsInSeason = await _seasonDb.getCollection<DbGameState>()
            .Find(g => g.HeroId == context.Message.heroId)
            .Project(g => g.Hero.skills)
            .SingleAsync();

        var existingSkillIds = herosSkillsInSeason.Select(s => s.id).ToArray();


        var herosSkillsInPerpetual = await _perpetualDb.getCollection<HeroDto>()
            .Find(g => g.id == context.Message.heroId)
            .Project(g => g.skills)
            .SingleAsync();


        var missingSkills = herosSkillsInPerpetual.Where(s => !existingSkillIds.Contains(s.id)).ToArray() ;

        if (missingSkills.Length > 0)
        {
            await _seasonDb.getCollection<DbGameState>()
                .UpdateOneAsync(g => g.HeroId == context.Message.heroId,
                Builders<DbGameState>.Update.PushEach(g => g.Hero.skills, missingSkills)
                );

            _logger.LogInformation($"loadMissingSkillsfromPerpetual: added {missingSkills.Length} skills to Hero {context.Message.heroId}");

        }
        else
        {
            _logger.LogInformation($"loadMissingSkillsfromPerpetual: to Hero {context.Message.heroId} has no missing skills");
        }

    }





    /// <summary>
    /*
    {
        "messageType": [
            "urn:message:DragonsCrossing.Core.Sagas:RunMaintenanceMessage"
        ],
        "message": {
            "maintenanceType": 0,
            "seasonId": 1
        }
    }
    */
    /// submit to maintennace Exchange
    /// </summary>
    /// <param name="seasonId"></param>
    /// <returns></returns>
    async Task<bool> saveSkillsFromSeason(int seasonId)
    {
        /*
        if (1 != seasonId)
        {
            throw new Exception("only for season 1. for next season we need to push the seasonid in skillsTransferedFromSeasons");
        }
        */

        ISeasonsDbService _seasonDb = ActivatorUtilities.CreateInstance<SeasonsDbService>(_sp, seasonId);

        var herosIdToFix = (await _perpetualDb.getCollection<HeroDto>()
            .Find(g => !g.dbHeroProps.skillsTransferedFromSeasons.Contains(seasonId))
            .Project(g => g.id)
            .Limit(10)
            .ToListAsync()).ToArray();


        var herosinOldSeason = (await _seasonDb.getCollection<DbGameState>()
                .Find(g => herosIdToFix.Contains(g.HeroId))
                .Project(g => g.Hero)
                .ToListAsync()).ToDictionary(k => k.id, v => v);

        var toRun = herosIdToFix.Select(id =>
        {
            //var setter = Builders<HeroDto>.Update.Set(g => g.dbHeroProps.skillsTransferedFromSeasons, new[] { seasonId });
            var setter = Builders<HeroDto>.Update.Push(g => g.dbHeroProps.skillsTransferedFromSeasons,  seasonId );

            if (herosinOldSeason.TryGetValue(id, out var hero))
            {
                foreach (var s in hero.skills)
                {
                    //clear out the useInstances
                    s.skillUseInstance = new SkillUseInstance[] { };
                }

                setter = setter.Set(g => g.skills, hero.skills);

            }

            return new UpdateOneModel<HeroDto>(
                Builders<HeroDto>.Filter.Where(g => g.id == id),
                setter
                );
        }).ToArray();

        if (toRun.Length > 0)
        {
            var done = await _perpetualDb.getCollection<HeroDto>().BulkWriteAsync(toRun);

            _logger.LogInformation($"saveSkillsFromSeason modified: {done.ModifiedCount}");
            _logger.LogInformation($"There might be heros to saveSkillsFromSeason for {seasonId}");
            return true;
        }
        else
        {
            _logger.LogInformation($"No more heros to saveSkillsFromSeason for {seasonId}");
            return false;
        }

    }

}

[JsonConverter(typeof(StringEnumConverter))]
public enum MaintenanceType {
    saveSkillsFromSeason,
    updateSkillVersion,
    loadMissingSkillsfromPerpetual
}

public class RunMaintenanceMessage
{
    public MaintenanceType maintenanceType { get; set; }

    public int? seasonId { get; set; }

    public int? newSkillsVersion { get; set; }

    public int? heroId { get; set; }

}



