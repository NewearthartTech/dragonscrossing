using System;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace DragonsCrossing.Core.Helper;

public interface IGameStateService
{
    Task ResetQuestsIfNeeded(int HeroId, bool forceItNow = false);

    Task<PlayerDto> EnsurePlayerInSeason(string playerId);
}

public class GameStateService: IGameStateService
{
    readonly ISeasonsDbService _seasonsDb;
    readonly IPerpetualDbService _perpetualDb;
    readonly ILogger _logger;
    readonly IPublishEndpoint _publishEp;
    readonly IServiceProvider _sp;

    public GameStateService(ISeasonsDbService db,
        IPerpetualDbService perpetualDb,
        IPublishEndpoint publishEp, IServiceProvider sp,
        ILogger<GameStateService> logger)
    {
        _perpetualDb = perpetualDb;
        _seasonsDb = db;
        _logger = logger;
        _publishEp = publishEp;
        _sp = sp;
    }

    public async Task<PlayerDto> EnsurePlayerInSeason(string playerId)
    {
        if (_seasonsDb.isAvailable)
        {
            var player = await _seasonsDb.getCollection<PlayerDto>().Find(p => p.Id == playerId).SingleOrDefaultAsync();

            if (null == player)
            {
                player = await _perpetualDb.getCollection<PlayerDto>().Find(p => p.Id == playerId).SingleAsync();
                await _seasonsDb.getCollection<PlayerDto>().InsertOneAsync(player);
            }

            return player;

        }
        else
        {
            return await _perpetualDb.getCollection<PlayerDto>().Find(p => p.Id == playerId).SingleAsync();
        }
    }

    public static readonly int HOURS_TILL_RESET = 23;

    public async Task ResetQuestsIfNeeded(int heroId, bool forceItNow = false)
    {
        var perPetualHero = await _perpetualDb.getCollection<HeroDto>().Find(c => c.id == heroId).SingleAsync();

        if(0 == perPetualHero.seasonId)
        {
            throw new InvalidOperationException($"Perpetual Hero {heroId}, is not registered in a season");
        }

        ISeasonsDbService tmSeasonsDb = ActivatorUtilities.CreateInstance<SeasonsDbService>(_sp, perPetualHero.seasonId);

        if (forceItNow)
        {
            var lastResetSetTo = DateTime.Now.Subtract(
            TimeSpan.FromHours(GameStateService.HOURS_TILL_RESET + 2));

            await tmSeasonsDb.getCollection<DbGameState>()
                .UpdateOneAsync(c => c.HeroId == heroId, Builders<DbGameState>
                .Update.Set(g => g.Hero.lastDailyResetAt, lastResetSetTo));

        }


        var gameState = await tmSeasonsDb.getCollection<DbGameState>().Find(g => g.HeroId == heroId).SingleAsync();


        if (gameState.CurrentEncounters.Length > 0)
        {
            _logger.LogDebug("Hero is in encounter we won't reset now");

            //FailSafe in case we fail to setup reset after encounters are over
            await _publishEp.Publish(new Sagas.ResetHeroIfNeededMessage
            {
                heroId = heroId,
                runAfter = DateTime.Now.AddMinutes(5)

            });

            return;
        }

        if(null == gameState.Hero.lastDailyResetAt)
        {
            gameState.Hero.lastDailyResetAt = DateTime.MinValue;
        }

        if (gameState.Hero.lastDailyResetAt?.AddHours(HOURS_TILL_RESET) > DateTime.Now.ToUniversalTime() )
        {
            _logger.LogDebug($"No need to reset Hero {gameState.HeroId} . last reset was at {gameState.Hero.lastDailyResetAt}",new {
                reason= "no_need_to_reset_hero"
            });
            return;
        }

        _logger.LogInformation($"Performing daily reset for HeroId {gameState.HeroId} at {DateTime.Now}, last reset was at {gameState.Hero.lastDailyResetAt}");

        var skills = gameState.Hero.skills.Select(i =>
        {
            i.skillUseInstance =
              i.skillUseInstance.Select(su => {
                  su.alreadyUsed = false;
                  return su;
              }).ToArray();

            return i;
        }).ToArray();

        //If the Hero has quested at all, then count all "unused" quests against daily quests used
        var increaseTotalQuestsBy = 0 == (gameState.Hero.dailyQuestsUsed) ? 0 : (gameState.Hero.maxDailyQuests + gameState.Hero.extraDailyQuestGiven);

        var setter = Builders<DbGameState>.Update
                            .Set(g => g.Hero.lastDailyResetAt, DateTime.Now)

                            //health set to full
                            .Set(g => g.Hero.remainingHitPointsPercentage, 100.0)

                            //skill uses given back for consumed skills
                            //.Set(g => g.Hero.usedUpSkillPoints, 0)
                            // We donot need to reset skill points.
                            // For extra secure, may be we need to calculate skill points keft and all

                            // Sets the alreadyUsed flag to be false for every skill allocated
                            .Set(g => g.Hero.skills, skills)

                            //daily quest reset
                            .Set(g => g.Hero.dailyQuestsUsed, 0)
                            .Set(g => g.Hero.extraDailyQuestGiven, 0)
                            .Set(g => g.inactiveDailyTiles, new DcxTiles[] { })

                            //total Quests used
                            .Inc(g => g.leaderStatus.totalQuestsUsed, increaseTotalQuestsBy)
                            ;

        var done = await tmSeasonsDb.getCollection<DbGameState>().UpdateOneAsync(
            c => c.HeroId == gameState.HeroId &&
            (null == c.Hero.lastDailyResetAt || c.Hero.lastDailyResetAt == gameState.Hero.lastDailyResetAt),
                            setter
                            );
        if (done.MatchedCount != 1)
        {
            //if we are here then probably some other method updated the lastDailyReset.
            //lets log it

            var newGameState = await tmSeasonsDb.getCollection<DbGameState>().Find(g => g.HeroId == heroId).SingleAsync();

            if(newGameState.Hero.lastDailyResetAt > gameState.Hero.lastDailyResetAt)
            {
                _logger.LogDebug("Hero is already reset");
            }
            else
            {
                throw new Exception($"For hero {gameState.HeroId} quest_reset_did_not_work, newRe");
            }

        }
        else
        {
            //log and set up delayed message for next reset

            _logger.LogDebug($"hero daily reset heroId{gameState.HeroId} completed", new
            {
                reason = "hero_daily_reset"
            });

            var delay = TimeSpan.FromHours(HOURS_TILL_RESET).Add(TimeSpan.FromSeconds(1));

            await _publishEp.Publish(new Sagas.ResetHeroIfNeededMessage
            {
                heroId = heroId,
                runAfter = DateTime.Now.Add(delay)
            });


        }

    }

}

