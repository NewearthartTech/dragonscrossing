using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.GameStates;
using MassTransit.Contracts;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using Newtonsoft.Json;
using static DragonsCrossing.Core.Helper.DataHelper;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones;

public interface ICombatZoneService
{
    MonsterDto GenerateMonster(MonsterTemplate template, bool isBossTile);
    Task CombatTileEntry(
        int HeroId,
        DcxTiles newLocation,
        IClientSessionHandle session
    );
    Task<bool> EnsureCombatIsResolved(
        ISeasonsDbService seasonsDb,
        IClientSessionHandle session,
        int HeroId
        );
}

public partial class CombatZoneService : ICombatZoneService
{
    readonly IDiceService _dice;
    readonly ISeasonsDbService _seasonsDb;
    readonly ILogger _logger;
    readonly IDataHelperService _dataHelperService;
    static int GUARANTEED_NON_COMBAT_COUNTER = 5;
    readonly NonCombatHelpers _nonCombatHelpers;

    public CombatZoneService(IDiceService dice,
        ISeasonsDbService db,
        NonCombatHelpers nonCombatHelpers,
        ILogger<CombatZoneService> logger,
        IDataHelperService dataHelperService
        )
    {
        _dice = dice;
        _nonCombatHelpers = nonCombatHelpers;

        _seasonsDb = db;
        _logger = logger;
        _dataHelperService = dataHelperService;
    }

    public MonsterDto GenerateMonster(MonsterTemplate template, bool isBossTile)
    {
        var dto = new MonsterDto
        {
            Id = Guid.NewGuid().ToString()
        };

        dto.UpdateFromTemplate(template);


        // The initial HitPoints is the same as MaxHitPoints
        dto.HitPoints = dto.MaxHitPoints;

        if (isBossTile)
        {
            dto.PersonalityType = MonsterPersonalityTypeDto.None;
        }
        else
        {

            #region randomly generate the monster personality based on the schedule.

            var monsterPersonalityTypeDic = DataHelper.CreateTypefromJsonTemplate($"ScheduleTableRef.monsterPersonalityChance.json",
                        new Dictionary<MonsterPersonalityTypeDto, Range>());

            var roll = _dice.Roll(100, DiceRollReason.MonsterPersonality);

            dto.PersonalityType = monsterPersonalityTypeDic.Where(d => d.Value.lower <= roll && d.Value.upper >= roll).Select(d => d.Key).Single();

            #endregion;
        }

        return dto;

    }

    readonly static DcxTiles[] _combatLocations =

        TileDto.QuestTiles
        .Concat(TileDto.BossTiles)
        .Concat(TileDto.DailyQuestTiles)
        .ToArray();

    public async Task<bool> EnsureCombatIsResolved(
        ISeasonsDbService seasonsDb,
        IClientSessionHandle session,
        int HeroId
        )

    {
        var gameStateCollection = seasonsDb.getCollection<DbGameState>();
        var gameState = await seasonsDb.getCollection<DbGameState>().Find(session, c => c.HeroId == HeroId).SingleAsync();

        if (!_combatLocations.Contains(gameState.CurrentTile))
        {
            return true;
        }


        await gameStateCollection.UpdateOneAsync(session, g => g.HeroId == HeroId,
            Builders<DbGameState>.Update.Set(g => g.CurrentEncounters, new Encounter[] { }));


        var nonCombatEncounter = gameState.CurrentEncounters?
            .Where(e => e.type == nameof(LocationEncounter) ||
            e.type == nameof(LoreEncounter) ||
            e.type == nameof(ChanceEncounter))
            .Select(e => (NonCombatEncounter)e).ToArray();

        var combatEncounters = gameState.CurrentEncounters?
            .Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

        // If there's any unresolved nonCombatEncounter, throw error.
        if (null != nonCombatEncounter && nonCombatEncounter.Count() > 0)
        {
            throw new InvalidDataException($"Non-combat encounter {nonCombatEncounter.First().id} has not been resolved");
        }

        // If there are no un resolved nonCombatEncounter and no combat encounters, return.
        if ((null != nonCombatEncounter && nonCombatEncounter.Count() == 0)
            && (null != combatEncounters && combatEncounters.Count() == 0))
        {
            return true;
        }

        // Take care of combat from this point
        var combat = combatEncounters?.Where(e => e.Monster != null).Single();

        if (null == combat)
            throw new InvalidOperationException("Combat cannot be null here");

        if (!combat.isHeroDead && !combat.isMonsterDead && !combat.isHeroAbleToFlee)
        {
            throw new ExceptionWithCode("Cannot leave combat");
        }

        if (combat.isHeroAbleToFlee)
        {
            return true;
        }
        else if (combat.isHeroDead)
        {
            // As required, not going to do level loss penalty
            //var levelDownsNeeded = CreateTypefromJsonTemplate($"ScheduleTableRef.LevelLoss.json", new[] {
            //new {
            //    level = 0,
            //    loss = 0,
            //} })
            //    .OrderByDescending(s => s.level)
            //    .Where(s => s.level <= gameState.Hero.level)
            //    .First();
            //;

            //for (var levelDown = 0; levelDown > levelDownsNeeded.loss; levelDown--)
            //{
            //    var statToLoose = gameState.Hero.heroLevelUpProps[gameState.Hero.level];
            //    gameState.Hero.AdjustHeroProps(true, statToLoose);
            //    gameState.Hero.level--;
            //}

            //gameState.Hero.remainingHitPointsPercentage = 100.0;

            gameState.Hero.unsecuredDCXValue = new Heroes.UnsecurdDcx();

            var chosenItem = gameState.Hero.inventory.FirstOrDefault();
            if (null == chosenItem)
                chosenItem = gameState.Hero.equippedItems.FirstOrDefault();
            gameState.Hero.inventory = gameState.Hero.inventory.Where(i => i == chosenItem).ToArray();
            gameState.Hero.equippedItems = gameState.Hero.equippedItems.Where(i => i == chosenItem).ToArray();

            gameState.Hero.AvailableQuestSetToZero();

            await gameStateCollection.UpdateOneAsync(session, g => g.HeroId == HeroId,
                Builders<DbGameState>.Update.Set(g => g.Hero, gameState.Hero));

        }


        #region checks and make sure player automatically gets the skill book or shard saved into their inventory

        if (combat != null &&
            combat.loot != null &&
            combat.loot.Items.Where(i => i.slot == ItemSlotTypeDto.unidentifiedSkill || i.slot == ItemSlotTypeDto.shard).Count() > 0)
        {
            var skillBooksAndShardItems = combat.loot.Items.Where(
                i => i.slot == ItemSlotTypeDto.unidentifiedSkill || i.slot == ItemSlotTypeDto.shard).ToArray();

            var inventoryIds = gameState.Hero.inventory.Select(i => i.id).ToArray();

            var itemsToGoToInventory = skillBooksAndShardItems.Where(i => !inventoryIds.Contains(i.id)).ToList();

            var inventorySpaceLeft = HeroDto.TotalInventorySlots - gameState.Hero.inventory.Length;

            // Shard slot is higher rank then skill book, take shard first
            var finalItems = itemsToGoToInventory.OrderBy(i => i.slot).Take(inventorySpaceLeft).ToArray();

            //wen: todo
            // throw exception here if shard or skill book we are trying to give is already in the inventory
            // or any other condition

            var findFilter = Builders<DbGameState>.Filter.And(
                Builders<DbGameState>.Filter.Where(g => g.HeroId == gameState.HeroId),
                Builders<DbGameState>.Filter.SizeLte(g => g.Hero.inventory, (HeroDto.TotalInventorySlots - finalItems.Length))
                );

            var done = await gameStateCollection.UpdateOneAsync(session,
                findFilter,
                Builders<DbGameState>.Update
                .PushEach(g => g.Hero.inventory, finalItems)
                );
        }

        #endregion checks and make sure player automatically gets the skill book or shard saved into their inventory

        #region Before leaving the boss or daily quest tile, if monster is dead, add them to the inactive tile collection.

        var isBossTile = TileDto.BossTiles.Contains(gameState.CurrentTile);
        var isDailyQuestTile = TileDto.DailyQuestTiles.Contains(gameState.CurrentTile);

        if (isBossTile && combat!.isMonsterDead)
        {
            gameState.inactiveBossTiles = gameState.inactiveBossTiles.Concat(new DcxTiles[] { gameState.CurrentTile }).ToArray();

            await gameStateCollection.UpdateOneAsync(session, g => g.HeroId == HeroId,
                    Builders<DbGameState>.Update.Set(g => g.inactiveBossTiles, gameState.inactiveBossTiles));

            // if final zone boss is beat, set the leaderstatus
            if (gameState.CurrentTile == DcxTiles.libraryOfTheArchmage)
            {
                var currentTotalQuestsSpent = gameState.leaderStatus.totalQuestsUsed + gameState.Hero.dailyQuestsUsed;

                await gameStateCollection.UpdateOneAsync(session, g => g.HeroId == HeroId,
                    Builders<DbGameState>.Update
                    .Set(g => g.leaderStatus.isFinalBossDefeated, true)
                    .Set(g => g.leaderStatus.benchmarkUsedQuests, currentTotalQuestsSpent));
            }

        }
        else if (isDailyQuestTile && combat!.isMonsterDead)
        {
            gameState.inactiveDailyTiles = gameState.inactiveDailyTiles.Concat(new DcxTiles[] { gameState.CurrentTile }).ToArray();

            await gameStateCollection.UpdateOneAsync(session, g => g.HeroId == HeroId,
                    Builders<DbGameState>.Update.Set(g => g.inactiveDailyTiles, gameState.inactiveDailyTiles));
        }

        // After defeating the final boss, we need to add some lores encounter
        if (isBossTile &&
            gameState.CurrentTile == DcxTiles.libraryOfTheArchmage &&
            combat!.isMonsterDead)
        {
            var updateStatement = TileDto.CreateLibraryOfTheArchmageNonCombatEncounter(gameState, gameState.CurrentTile);
            await gameStateCollection.UpdateOneAsync(session, g => g.HeroId == HeroId, updateStatement);
        }

        //after regular combats we need to add nonComabt chances
        if(combat!.isMonsterDead && !isBossTile && !isDailyQuestTile)
        {
            var isCocytus2Tile = TileDto.Cocytus2Tiles.Contains(gameState.CurrentTile);
            _logger.LogDebug($"isCocytus2Tile = {isCocytus2Tile}");

            if (isCocytus2Tile)
            {
                {
                    if (null == combat?.Monster || (gameState.Hero.level >= (combat!.Monster!.Level + 4)))
                    {
                        _logger.LogDebug($"NO bonus xp after Cocytus2Tile, Hro level too high hero level {gameState.Hero.level}, monster level {combat?.Monster?.Level}");
                    }
                    else
                    {
                        var diceRoll = _dice.Roll(100, reason: DiceRollReason.BonusXPAfterCocytus2Tile);

                        if (diceRoll > 50)
                        {
                            if (gameState.Hero.experiencePoints < gameState.Hero.maxExperiencePoints)
                            {
                                _logger.LogDebug("giving bonus xp after Cocytus2Tile");

                                await _seasonsDb.getCollection<DbGameState>().UpdateOneAsync(session,
                                    Builders<DbGameState>.Filter.Where(c =>
                                        c.HeroId == HeroId && c.Hero.maxExperiencePoints == gameState.Hero.maxExperiencePoints),
                                    Builders<DbGameState>.Update
                                        .Inc(g => g.Hero.experiencePoints, 1)
                                );
                            }
                            else
                            {
                                _logger.LogDebug("NO bonus xp after Cocytus2Tile, Hero at maxExperiencePoints");

                            }

                        }
                    }
                }

                var isHPUnderThershold = gameState.Hero.remainingHitPointsPercentage < 20;
                _logger.LogDebug($"isHPUnderThershold = {isHPUnderThershold}, hpPercent = {gameState.Hero.remainingHitPointsPercentage}");


                if (isHPUnderThershold)
                {
                    var nonCombatencounters = new NonCombatEncounter[]
                    {
                        _nonCombatHelpers.GetCocytus2NonCombatEncounter()
                    };

                    var setter = Builders<DbGameState>.Update
                        .Set(g => g.CurrentTile, gameState.CurrentTile)
                        .Set(g => g.CurrentEncounters, nonCombatencounters.ToArray())
                        //.Set(g => g.nonCombatTileState, gamestate.nonCombatTileState)
                        ;

                    await _seasonsDb.getCollection<DbGameState>().UpdateOneAsync(session,
                        Builders<DbGameState>.Filter.Where(c => c.HeroId == HeroId),
                        setter);

                    return false;

                }


                

            }

            {
                var giveNCE = gameState.guaranteedNonCombatCounter >= GUARANTEED_NON_COMBAT_COUNTER;

                if (giveNCE)
                {
                    _logger.LogDebug($"GUARANTEED_NON_COMBAT_COUNTER threhold met guaranteedNonCombatCounter= {gameState.guaranteedNonCombatCounter} ");
                }else
                {
                    var diceRoll = _dice.Roll(100, reason: DiceRollReason.NCEAfterCombat);
                    giveNCE = diceRoll > 85;
                }

                if (giveNCE)
                {
                    _logger.LogDebug("We have NCE after combat");

                    var setter = Builders<DbGameState>.Update
                        .Set(g => g.CurrentTile, gameState.CurrentTile)
                        .Set(g=>g.guaranteedNonCombatCounter,0)
                        ;
                    setter = createNonCombatEncounter(gameState, gameState.CurrentTile, setter);

                    await _seasonsDb.getCollection<DbGameState>().UpdateOneAsync(session,
                        Builders<DbGameState>.Filter.Where(c => c.HeroId == HeroId),
                        setter);

                    return false;
                }
                else
                {
                    await _seasonsDb.getCollection<DbGameState>().UpdateOneAsync(session,
                        Builders<DbGameState>.Filter.Where(c => c.HeroId == HeroId),
                        Builders<DbGameState>.Update.Inc(g => g.guaranteedNonCombatCounter, 1)
                        ) ;
                }
            }
        }

        #endregion


        return true;
    }


    public async Task CombatTileEntry(int HeroId,
        DcxTiles newLocation,
        IClientSessionHandle session
    )
    {

        if (!_combatLocations.Contains(newLocation))
        {
            return;
        }

        var gameStateCollection = _seasonsDb.getCollection<DbGameState>();
        var gameState = await _seasonsDb.getCollection<DbGameState>().Find(session, c => c.HeroId == HeroId).SingleAsync();

        var encounters = gameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

        var combat = encounters?.Where(e => e.Monster != null).FirstOrDefault();

        if (null != combat?.Monster)
        {
            _logger.LogDebug($"hero {HeroId} is in combat with monster");
            return;
        }

        var isBossTile = TileDto.BossTiles.Contains(newLocation);
        var isDailyQuestTile = TileDto.DailyQuestTiles.Contains(newLocation);

        // If we are trying to enter inactive boss or daily quest tile, throw error.
        if (isBossTile && gameState.inactiveBossTiles.Contains(newLocation))
        {
            throw new Exception($"Cannot enter {newLocation}. Boss has been defeated.");
        }

        if (isDailyQuestTile && gameState.inactiveDailyTiles.Contains(newLocation))
        {
            throw new Exception($"Cannot enter {newLocation}. Quest has been completed for the day.");
        }


        var gameStateSetter = Builders<DbGameState>.Update.Set(g => g.CurrentTile, newLocation);

        
        {
            var availableMonsters = (from m in TileDto.loadMonsterTemplates()
                                     where m.LocationTile == newLocation
                                     select m).ToArray();

            var totalWeight = availableMonsters.Sum(m => m.AppearChance);

            var randomMonsterRoll = _dice.Roll(totalWeight,
                Combats.DiceRollReason.ChooseMonster /*, Combat.monsterAttackResult.Dice*/);

            var template = new MonsterTemplate();

            _logger.LogDebug($"Total monster weight for {newLocation} is {totalWeight}. Monster selection roll result is {randomMonsterRoll}.");

            // Select monster based on weight.
            foreach (var monsterTemplate in availableMonsters)
            {
                // Say the roll is 500 and is smaller than the first monster appearChance, 
                // Then the first monster is selected.
                if (randomMonsterRoll <= monsterTemplate.AppearChance)
                {
                    template = monsterTemplate;
                    break;
                }
                randomMonsterRoll -= monsterTemplate.AppearChance;
            }

            if (null != template && !string.IsNullOrEmpty(template.MonsterSlug))
            {
                var monster = GenerateMonster(template, isBossTile);

                TileDto.ApplyCombatOpportunityStat(monster);

                var combatEncounter = new CombatEncounter()
                {
                    Monster = monster
                };

                #region monster stats level scaling
                // After monster is generated, enhance the monster stats if level scaling is needed.
                // HitPoints need to be updated and be same as the modified maxhitpoints if applicable
                _dataHelperService.ApplyLevelScaling(combatEncounter, gameState);
                combatEncounter.Monster.HitPoints = combatEncounter.Monster.MaxHitPoints;
                #endregion

                combatEncounter.isCharismaOpportunityAvailable = _dice.Roll(100, DiceRollReason.RollForCharismaOpportunity) > 85 ? true : false;

                gameStateSetter = gameStateSetter.Set(c => c.CurrentEncounters, new Encounter[] { combatEncounter });
            }
            else
            {
                throw new Exception("Unable to find a monster template base on weighted selection");
            }

            
        }

        await _seasonsDb.getCollection<DbGameState>().UpdateOneAsync(session, Builders<DbGameState>.Filter.And(
            Builders<DbGameState>.Filter.Where(c => c.HeroId == HeroId),
                Builders<DbGameState>.Filter.Size(c => c.CurrentEncounters, 0)),
                gameStateSetter);

        return;
    }

    public UpdateDefinition<DbGameState> createNonCombatEncounter(DbGameState gamestate,
        DcxTiles newLocation,
        UpdateDefinition<DbGameState> setter)
    {
        switch (newLocation)
        {
            case DcxTiles.enchantedFields:
                {
                    return _nonCombatHelpers.CreateWildPrairieNonCombatEncounter(gamestate, newLocation, setter);
                }
            case DcxTiles.sylvanWoodlands:
                {
                    return _nonCombatHelpers.CreateMysteriousForestNonCombatEncounter(gamestate, newLocation, setter);
                }
            case DcxTiles.odorousBog:
                {
                    return _nonCombatHelpers.CreateFoulWastesNonCombatEncounter(gamestate, newLocation, setter);
                }
            case DcxTiles.mountainFortress:
                {
                    return _nonCombatHelpers.CreateTreacherousPeaksNonCombatEncounter(gamestate, newLocation, setter);
                }
            case DcxTiles.labyrinthianDungeon:
                {
                    return _nonCombatHelpers.CreateDarkTowerNonCombatEncounter(gamestate, newLocation, setter);
                }
            case DcxTiles.feyClearing:
                return _nonCombatHelpers.CreateWondrousThicketNonCombatEncounter(gamestate, newLocation, setter);
            case DcxTiles.pillaredRuins:
                return _nonCombatHelpers.CreateFallenTemplesNonCombatEncounter(gamestate, newLocation, setter);
            default:
                throw new Exception($"New location tile {newLocation} doesn't have a non-combat encounter state.");

        }
    }


}