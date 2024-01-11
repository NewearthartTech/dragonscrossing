using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.GameStates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DragonsCrossing.NewCombatLogic;
using MongoDB.Driver;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Helper;
using Microsoft.AspNetCore.Authorization;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Domain.GameStates;

namespace DragonsCrossing.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "SeasonJoined")]
    public class GameStatesController : ControllerBase
    {
        readonly ISeasonsDbService _seasonDb;
        readonly ILogger _logger;
        readonly ICombatZoneService _combatZoneService;
        readonly ICombatEngine _combatEngine;
        readonly IGameStateService _gameStateService;

        public GameStatesController(ISeasonsDbService db,
            ICombatEngine combatEngine,
            IGameStateService gameStateService,
            ICombatZoneService combatZoneService,
            ILogger<GameStatesController> logger)
        {
            _seasonDb = db;
            _combatZoneService = combatZoneService;
            _logger = logger;
            _combatEngine = combatEngine;
            _gameStateService = gameStateService;
        }


        [HttpGet("updateLocation/{newLocation}")]
        public async Task<GameStateDto> UpdateLocation(DcxTiles newLocation)
        {
            _logger.LogInformation($"Updating location to {newLocation}");

            var heroId = this.GetHeroId();
            var playerId = this.GetUserId();

            /*
            if (Core.Services.HeroesService.TEST_OWNED.Contains(heroId))
                throw new ExceptionWithCode("not yet supported");
            */

            await _gameStateService.EnsurePlayerInSeason(playerId!);

            var gameStateCollection = _seasonDb.getCollection<DbGameState>();

            var gameStateCurrent = await _seasonDb.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            var origingameState = await _seasonDb.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            ///we leave this here so that reset is trigerred if moving after combat
            await _gameStateService.ResetQuestsIfNeeded(heroId);

            using (var session = await _seasonDb.StartTransaction())
            {
                var gameState = await _seasonDb.getCollection<DbGameState>().Find(session, c => c.HeroId == heroId).SingleAsync();

                var currentZone = TileDto.ZoneFromTile(gameState.CurrentTile);
                var newZone = TileDto.ZoneFromTile(newLocation);

                if (gameState.Hero.isLoanedHero?.loanerType == Core.Contracts.Api.Dto.Heroes.LoanerHeroType.Demo && newZone > DcxZones.foulWastes)
                    throw new ExceptionWithCode("DEMO limit reached");

                if (newLocation != gameState.CurrentTile)
                {     
                    EnsureLocationAllowed(gameState, newLocation);

                    bool consumeQuest = false;

                    // Moving in or out of aedos doesn't cost quest.
                    if (newZone != DcxZones.aedos && !(currentZone == DcxZones.aedos && newZone == DcxZones.wildPrairie))
                    {
                        if (newZone != currentZone)
                        {
                            consumeQuest = true;
                        }
                        else if (gameState.CurrentTile != newLocation)
                        {

                            consumeQuest = TileDto.QuestTiles
                                                .Concat(TileDto.DailyQuestTiles)
                                                .Concat(TileDto.BossTiles)
                                                .Contains(newLocation);
                        }

                        if (consumeQuest)
                        {
                            if (gameState.Hero.remainingQuests < 1)
                            {
                                throw new ExceptionWithCode("Exhaustion has overwhelmed you. Rest now and try again tomorrow.");
                            }
                        }
                    }

                    var tile = new TileDto
                    {
                        Slug = newLocation

                    };

                    if (await _combatZoneService.EnsureCombatIsResolved(_seasonDb, session, heroId))
                    {

                        await _combatZoneService.CombatTileEntry(heroId, newLocation, session);

                        // If the tile is a new zone and is higher level than the HighestZoneVisited in gameState, update HighestZoneVisited
                        if (Enum.TryParse<DcxZones>(newLocation.ToString(), true, out var newVisitedZone))
                        {
                            if (ZoneDto.MapZoneOrders[newVisitedZone] > ZoneDto.MapZoneOrders[gameState.HighestZoneVisited])
                            {
                                // Total quests spent to get the the furthest zone is totalQuestsUsed so far plus all daily quests used for the day and then the extra 1 is the quest used for entering this zone.
                                var currentTotalQuestsSpent = gameState.leaderStatus.totalQuestsUsed + gameState.Hero.dailyQuestsUsed + 1;

                                await _seasonDb.getCollection<DbGameState>().UpdateOneAsync(session, c => c.HeroId == heroId,
                                    Builders<DbGameState>.Update
                                    .Set(g => g.HighestZoneVisited, newVisitedZone)

                                    .Set(g => g.leaderStatus.farthestZoneDiscovered, newVisitedZone)
                                    .Set(g => g.leaderStatus.farthestZoneDiscoveredOrder, ZoneDto.MapZoneOrders[newVisitedZone])

                                    .Set(g => g.leaderStatus.benchmarkUsedQuests, currentTotalQuestsSpent));
                            }

                            // When entering a new zone, we need to add the noncombat state for the zone
                            if (newVisitedZone != DcxZones.aedos && !gameState.nonCombatTileState.TryGetValue(newVisitedZone, out var encounterState))
                            {
                                encounterState = TileDto.InitiateNonCombatEncounterState(newVisitedZone);
                                gameState.nonCombatTileState[newVisitedZone] = encounterState;


                                await _seasonDb.getCollection<DbGameState>().UpdateOneAsync(session, c => c.HeroId == heroId,
                                        Builders<DbGameState>.Update.Set(g => g.nonCombatTileState, gameState.nonCombatTileState));
                            }
                        }

                        if (consumeQuest)
                        {
                            await _seasonDb.getCollection<DbGameState>().UpdateOneAsync(session, c => c.HeroId == heroId,
                                    Builders<DbGameState>.Update.Inc(g => g.Hero.dailyQuestsUsed, 1));
                        }

                        // We have to update the location ONLY after everything is taken care of for updating the location. 
                        // Otherwise the CurrentTile in gamestate would be inaccurate.
                        await _seasonDb.getCollection<DbGameState>().UpdateOneAsync(session, c => c.HeroId == heroId,
                            Builders<DbGameState>.Update.Set(g => g.CurrentTile, newLocation));
                    }

                }

                await session.CommitTransactionAsync();
            }

            return new GameStateDto(await gameStateCollection.Find(g => g.HeroId == heroId).SingleAsync(), _combatEngine);
        }


        private void EnsureLocationAllowed(DbGameState gameState, DcxTiles newLocation)
        {
            var currentZone = TileDto.ZoneFromTile(gameState.CurrentTile);
            var newZone = TileDto.ZoneFromTile(newLocation);

            bool canHeroGoBackToTown = false;

            var encounters = gameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

            var combat = encounters?.Where(e => e.Monster != null).FirstOrDefault();

            // When hero died from a combat, hero will be allowed to move back to town
            if (combat != null && combat.isHeroDead)
            {
                canHeroGoBackToTown = true;
            }

            // Hero can only move to the zone next by. When hero dies, hero is allowed to move back to aedos
            if (newZone != currentZone && !(canHeroGoBackToTown && newLocation == DcxTiles.aedos)) {

                //NearByZone[] nearbyZones;

                if (!ZoneDto.MapNearByZones.TryGetValue(currentZone, out var nearbyZones))
                {
                    throw new Exception($"zone {currentZone} missing in MapNearByZones");
                }

                var isNewZoneNear = nearbyZones.Where(z => z.zone == newZone).FirstOrDefault();

                if (null == isNewZoneNear)
                {
                    throw new Exception($"Move prohibited. NOT nearbyzone, currentZone: {currentZone} -> newZone: {newZone}");
                }
            }

            // Hero cannot jump from a tile in Zone A to another tile in Zone C
            // If the tile we are not trying to move to are in different zones and the new location is not a zone tile, return error. 
            if (currentZone != newZone &&
                !Enum.TryParse<DcxZones>(newLocation.ToString(), true, out var zone))
            {
                throw new Exception($"Move prohibited. failed to parse. newLocation {newLocation}  ");
            }

            if (currentZone != DcxZones.aedos)
            {
                if (gameState.nonCombatTileState.TryGetValue(currentZone, out var encounterState))
                {
                    var nonDiscoverednorActiveTiles = gameState.nonCombatTileState[TileDto.ZoneFromTile(gameState.CurrentTile)]
                        .tilesToDiscoverState
                        .Where(t => t.Value.isDiscovered == false || !t.Value.isActive(gameState))
                        .Select(t => t.Key)
                        .ToArray();

                    // Within the same zone, if the tile has not been discovered, prohibit the move.
                    if (nonDiscoverednorActiveTiles.Contains(newLocation))
                    {
                        throw new Exception($"Move prohibited. newLocation {newLocation} has not been discovered nor active.");
                    }
                }
                else
                {
                    throw new Exception("failed to find encousetr state");
                }
            }

            if(gameState.nonCombatTileState.TryGetValue(currentZone, out var nonCombatState))
            {
                var undisCoveredCount = nonCombatState.tilesToDiscoverState
                    .Where(t => t.Value.isDiscovered == false && t.Key == newLocation)
                    .Count();

                if (undisCoveredCount > 0)
                {
                    throw new Exception($"tile {newLocation} for hero {gameState.HeroId} is undiscovered");
                }

            }


            
            if (gameState.nonCombatTileState.ContainsKey(currentZone))
            {
                //This will be a list of bossTiles in that zone
                var bossTilesInCurrentZone = gameState.nonCombatTileState[currentZone].tilesToDiscoverState.Keys.Where(k => TileDto.BossTiles.Contains(k));
                // If bossTilesInCurrentZone contains any boss tile that is not discovered
                // or discovered but not defeated tile(meaning, active), then isAllCurrentZoneBossesFoundAndDefeated is false
                bool isAllCurrentZoneBossesFoundAndDefeated =
                    !bossTilesInCurrentZone.Any(t =>
                    gameState.nonCombatTileState[currentZone].tilesToDiscoverState[t].isDiscovered == false ||
                    (gameState.nonCombatTileState[currentZone].tilesToDiscoverState[t].isDiscovered == true &&
                    gameState.nonCombatTileState[currentZone].tilesToDiscoverState[t].isActive(gameState)));
                // Hero cannot move to the next zone before the current zone boss is defeated.
                bool headingToWondrousThicket = newZone == DcxZones.treacherousPeaks && currentZone == DcxZones.wondrousThicket;
                bool headingToDarkTower = newZone == DcxZones.darkTower && currentZone == DcxZones.fallenTemples;
                if (((newZone - currentZone) == 1 || headingToDarkTower ||headingToWondrousThicket) && !isAllCurrentZoneBossesFoundAndDefeated)
                {
                    throw new ExceptionWithCode("Move prohibited. Zone boss not defeated.");
                }

                var isSlaverRowBeaten = false;

                isSlaverRowBeaten = (currentZone == DcxZones.darkTower &&
                    (gameState.nonCombatTileState[currentZone].tilesToDiscoverState[DcxTiles.slaversRow].isDiscovered == true &&
                    !gameState.nonCombatTileState[currentZone].tilesToDiscoverState[DcxTiles.slaversRow].isActive(gameState)));

                // To move location to final boss - library of the archmage, need to defeat slavers row first.
                if (newLocation == DcxTiles.libraryOfTheArchmage && !isSlaverRowBeaten)
                {
                    throw new ExceptionWithCode("Defeat Slavers Row first");
                }
            }        
        }

        /// <summary>
        /// Get the current gamestate for the given hero. This will always return a valid
        /// gamestate. This endpoint only needs to be called after the player chooses a hero to play with.
        /// </summary>
        /// <param name="heroId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpGet]
        public async Task<GameStateDto> Get(int heroId)
        {
            if (heroId == 0)
                throw new ArgumentException(nameof(heroId));

            var collection = _seasonDb.getCollection<DbGameState>();
            var gameState = await collection.Find(g => g.HeroId == heroId).SingleOrDefaultAsync();

            if (null == gameState)
            {
                await collection.InsertOneAsync(new DbGameState
                {
                    HeroId = heroId
                });

                return await Get(heroId);
            }


            return new GameStateDto(gameState, _combatEngine);
        }
    }
}
