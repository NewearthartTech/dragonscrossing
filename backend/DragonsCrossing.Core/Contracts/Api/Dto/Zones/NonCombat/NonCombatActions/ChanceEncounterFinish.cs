using System;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Domain.GameStates;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using Newtonsoft.Json;
using static DragonsCrossing.Core.Contracts.Api.Dto.Zones.WildPraireState;
using static DragonsCrossing.Core.Helper.DataHelper;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones;

public partial class TileDto
{
    async Task<ChanceEncounterResultProps> foreignBerriesChanceEncounterFinish(ILogger logger,
       ISeasonsDbService _db,
       int heroId,
       string choiceSlug,
       bool isGoodOutcome,
       UpdateDefinition<DbGameState> setter
       )
    {
        var collection = _db.getCollection<DbGameState>();
        var gameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        //todo: Can we NEVER use numerical case statments
        switch (choiceSlug)
        {
            case "5":
                {
                    if (isGoodOutcome)
                    {
                        gameState.Hero.remainingHitPointsPercentage = (gameState.Hero.remainingHitPointsPercentage + 15.0 >= 100.0)
                            ? 100.0
                            : (gameState.Hero.remainingHitPointsPercentage + 15.0);
                    }
                    else
                    {
                        gameState.Hero.remainingHitPointsPercentage = gameState.Hero.remainingHitPointsPercentage - 5.0 >= 0.0 
                            ? gameState.Hero.remainingHitPointsPercentage - 5.0 
                            : 1.0;
                    }
                    break;
                }
            case "6":
                {
                    if (isGoodOutcome)
                    {
                        gameState.Hero.remainingHitPointsPercentage = gameState.Hero.remainingHitPointsPercentage + 8.0 >= 100.0
                            ? 100.0
                            : gameState.Hero.remainingHitPointsPercentage + 8.0;
                    }
                    else
                    {
                        break;
                    }
                    break;
                }
            case "7":
                {
                    break;
                }
        }

        setter = setter.Set(g => g.Hero, gameState.Hero);

        return new ChanceEncounterResultProps { setter = setter };
    }

    async Task<ChanceEncounterResultProps> freshwaterOrbChanceEncounterFinish(ILogger logger,
       ISeasonsDbService _db,
       int heroId,
       string choiceSlug,
       bool isGoodOutcome,
       UpdateDefinition<DbGameState> setter
       )
    {
        var collection = _db.getCollection<DbGameState>();
        var gameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        switch (choiceSlug)
        {
            case "8":
                {
                    if (isGoodOutcome)
                    {
                        //here we want give out 2 more quest till the max of maxDailyQuests
                        //the idea is here the remainingQuests should never go over max daily quests.

                        gameState.Hero.difficultyToHit = gameState.Hero.difficultyToHit + 100;
                    }
                    else
                    {
                        break;
                    }
                    break;
                }
            case "9":
                {
                    if (isGoodOutcome)
                    {
                        gameState.Hero.charisma = gameState.Hero.charisma + 1;
                        gameState.Hero.quickness = gameState.Hero.quickness + 1;
                    }
                    else
                    {
                        gameState.Hero.charisma = gameState.Hero.charisma - 2 <= 0
                            ? 0
                            : gameState.Hero.charisma - 1;

                        gameState.Hero.quickness = gameState.Hero.quickness - 2 <= 0
                            ? 0
                            : gameState.Hero.quickness - 1;
                    }
                    break;
                }
            case "10":
                {
                    if (isGoodOutcome)
                    {
                        // This is a quest refund so refund back to dailyQuestsUsed and not count as a quest spent in the game
                        gameState.Hero.dailyQuestsUsed = gameState.Hero.dailyQuestsUsed - 1;
                    }
                    else
                    {
                        break;
                    }
                    break;
                }
        }
        setter = setter.Set(g => g.Hero, gameState.Hero);

        return new ChanceEncounterResultProps { setter = setter };
    }

    async Task<ChanceEncounterResultProps> wonderingWizardChanceEncounterFinish(ILogger logger,
       ISeasonsDbService _db,
       int heroId,
       string choiceSlug,
       bool isGoodOutcome,
       UpdateDefinition<DbGameState> setter
       )
    {
        var collection = _db.getCollection<DbGameState>();
        var gameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        bool goBackToAdos = true;
        bool? sendDiceResult = null;

        switch (choiceSlug)
        {
            //yes
            case "15":
                logger.LogDebug("wonderingWizardChanceEncounterFinish, going back to ados nothing to do");
                // 
                break;

            //no we need to roll dice
            case "16":
                {
                    sendDiceResult = true;
                    if (isGoodOutcome)
                    {
                        goBackToAdos = false;
                        if (gameState.Hero.experiencePoints < gameState.Hero.maxExperiencePoints)
                        {
                            setter = setter
                            .Inc(g => g.Hero.experiencePoints, 1);
                        }
                    }
                    break;
                }
        }

        if (goBackToAdos)
        {
            setter = Builders<DbGameState>.Update
                .Set(g => g.CurrentEncounters, new Encounter[] { })
                .Set(g=>g.CurrentTile, DcxTiles.aedos)
                ;
        }

        return new ChanceEncounterResultProps {
            setter = setter,

            //this is a slug don't change it
            encounterResponceSlug = goBackToAdos ? "BACKTOADOS" : "",

            sendDiceResult = sendDiceResult
        };
    }


    async Task<ChanceEncounterResultProps> rustingArandomWeaponChanceEncounterFinish(ILogger logger,
       ISeasonsDbService _db,
       int heroId,
       string choiceSlug,
       bool isGoodOutcome,
       UpdateDefinition<DbGameState> setter
       )
    {
        var collection = _db.getCollection<DbGameState>();
        var gameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        var looseWeapon = true;
        var heroUpdated = false; 

        switch (choiceSlug)
        {
            //Traverse the mist
            case "17":
                if (isGoodOutcome)
                {
                    logger.LogDebug($"rustingArandomWeaponChanceEncounterFinish, current experiencePoints {gameState.Hero.experiencePoints}");
                    if (gameState.Hero.experiencePoints == gameState.Hero.maxExperiencePoints - 1)
                    {
                        gameState.Hero.experiencePoints += 1;
                    }
                    else if (gameState.Hero.experiencePoints == gameState.Hero.maxExperiencePoints)
                    {
                        // dont give any xp, already full
                    }
                    else
                    {
                        gameState.Hero.experiencePoints += 2;
                    }
                    looseWeapon = false;
                    heroUpdated = true;

                }
                break;

            //Run
            case "18":
                {
                    if (isGoodOutcome)
                    {
                        looseWeapon = false;
                        heroUpdated = true;
                        if (gameState.Hero.experiencePoints == gameState.Hero.maxExperiencePoints - 1)
                        {
                            gameState.Hero.experiencePoints += 1;
                        }
                        else if (gameState.Hero.experiencePoints == gameState.Hero.maxExperiencePoints)
                        {
                            // dont give any xp, already full
                        }
                        else
                        {
                            gameState.Hero.experiencePoints += 1;
                        }
                    }
                    break;
                }
        }

        if (looseWeapon && gameState.Hero.equippedItems.Length > 0)
        {
            var rand = ExtensionMethods.GetRandomWithSeed().Next(0, gameState.Hero.equippedItems.Length);

            var idToRemove = gameState.Hero.equippedItems[rand].id;

            gameState.Hero.equippedItems = gameState.Hero.equippedItems.Where(i => i.id != idToRemove).ToArray();

            heroUpdated = true;
        }

        if (heroUpdated)
        {
            setter = setter.Set(g => g.Hero, gameState.Hero);
        }

        return new ChanceEncounterResultProps
        {
            setter = setter,
        };
    }

    async Task<ChanceEncounterResultProps> riddlerChanceEncounterFinish(ILogger logger,
       ISeasonsDbService _db,
       int heroId,
       string choiceSlug,
       bool isGoodOutcome,
       UpdateDefinition<DbGameState> setter
       )
    {
        var collection = _db.getCollection<DbGameState>();
        var gameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        var heroUpdated = false;

        switch (choiceSlug)
        {
            //Try and answer the riddle
            case "19":
                if (isGoodOutcome)
                {
                    //1.5% chance to hit

                    gameState.Hero.baseChanceToHit = gameState.Hero.baseChanceToHit + 100;
                    heroUpdated = true;
                }
                else
                {
                    //hp is set to 1

                    gameState.Hero.remainingHitPointsPercentage = 1.0;
                    heroUpdated = true;
                }
                break;

            //Run (50 / 50) success
            case "20":
                {
                    if (isGoodOutcome)
                    {
                        //nothing
                    }
                    else
                    {
                        gameState.Hero.remainingHitPointsPercentage = gameState.Hero.remainingHitPointsPercentage - 20.0 >= 0.0
                            ? gameState.Hero.remainingHitPointsPercentage - 20.0
                            : 1.0;
                        heroUpdated = true;
                    }
                    break;
                }
        }

        if (heroUpdated)
        {
            setter = setter.Set(g => g.Hero, gameState.Hero);
        }

        return new ChanceEncounterResultProps
        {
            setter = setter,
        };
    }

    async Task<ChanceEncounterResultProps> lovecraftianMonsterChanceEncounterFinish(ILogger logger,
       ISeasonsDbService _db,
       int heroId,
       string choiceSlug,
       bool isGoodOutcome,
       UpdateDefinition<DbGameState> setter
       )
    {
        var collection = _db.getCollection<DbGameState>();
        var gameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        var heroUpdated = false;

        switch (choiceSlug)
        {
            //Try to convince
            case "21":
                if (isGoodOutcome)
                {
                    //+1 Charisma +1 Hit Point

                    gameState.Hero.charisma += 1;
                    gameState.Hero.baseHitPoints += 1;
                    heroUpdated = true;
                }
                else
                {
                    //-1 Charisma
                    if(gameState.Hero.charisma>0)
                    {
                        gameState.Hero.charisma -= 1;
                    }
                    
                    heroUpdated = true;
                }
                break;

            //Run to the nearest
            case "22":
                {
                    if (isGoodOutcome)
                    {
                        //+1 Quickness + 1 Hit Point

                        gameState.Hero.quickness += 1;
                        gameState.Hero.baseHitPoints += 1;
                        heroUpdated = true;
                    }
                    else
                    {
                        //-1 Quickness
                        if (gameState.Hero.quickness > 0)
                        {
                            gameState.Hero.quickness -= 1;
                        }
                        heroUpdated = true;
                    }
                    break;
                }
            //Hold your hands up
            case "23":
                {
                    break;
                }
        }

        if (heroUpdated)
        {
            setter = setter.Set(g => g.Hero, gameState.Hero);
        }

        return new ChanceEncounterResultProps
        {
            setter = setter,
        };
    }

    async Task<ChanceEncounterResultProps> gamblerChanceEncounterFinish(ILogger logger,
       ISeasonsDbService _db,
       int heroId,
       string choiceSlug,
       bool isGoodOutcome,
       UpdateDefinition<DbGameState> setter
       )
    {
        var collection = _db.getCollection<DbGameState>();
        var gameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        switch (choiceSlug)
        {
            case "11":
                {
                    if (isGoodOutcome)
                    {
                        gameState.Hero.baseChanceToHit = gameState.Hero.baseChanceToHit + 150;
                    }
                    else
                    {
                        //This should not go negative
                        gameState.Hero.baseChanceToHit = gameState.Hero.baseChanceToHit - 100 <= 0
                            ? 0
                            : gameState.Hero.baseChanceToHit - 100;
                    }
                    break;
                }
            case "12":
                {
                    if (isGoodOutcome)
                    {
                        gameState.Hero.baseHitPoints = gameState.Hero.baseHitPoints + 3;
                    }
                    else
                    {
                        //This should not go negative
                        gameState.Hero.baseHitPoints = gameState.Hero.baseHitPoints - 2 > 0 ? gameState.Hero.baseHitPoints - 2 : 0;
                    }
                    break;
                }
            case "13":
                {
                    if (isGoodOutcome)
                    {
                        gameState.Hero.strength = gameState.Hero.strength + 1;
                        gameState.Hero.wisdom = gameState.Hero.wisdom + 1;
                        gameState.Hero.agility = gameState.Hero.agility + 1;
                    }
                    else
                    {
                        gameState.Hero.strength = gameState.Hero.strength - 1 > 0 ? gameState.Hero.strength - 1 : 0;
                        gameState.Hero.wisdom = gameState.Hero.wisdom - 1 > 0 ? gameState.Hero.wisdom - 1 : 0;
                        gameState.Hero.agility = gameState.Hero.agility - 1 > 0 ? gameState.Hero.agility - 1 : 0 ;
                    }
                    break;
                }
        }
        setter = setter.Set(g => g.Hero, gameState.Hero);

        return new ChanceEncounterResultProps { setter = setter };
    }
}
