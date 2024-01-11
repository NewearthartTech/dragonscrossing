using DragonsCrossing.Core.Contracts.Api.Dto.GameStates;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using static DragonsCrossing.Core.Helper.DataHelper;
using DragonsCrossing.Domain.GameStates;
using Org.BouncyCastle.Ocsp;
using System;
using DragonsCrossing.Domain.Zones;
using Common.Logging;
using System.Text.Json;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using static DragonsCrossing.Core.Common.ExtensionMethods;
using static DragonsCrossing.Core.Contracts.Api.Dto.Zones.TileDto;
using System.ComponentModel.DataAnnotations;

namespace DragonsCrossing.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "SeasonJoined")]
    public class NonCombatEncounterController : ControllerBase
    {
        readonly ISeasonsDbService _db;
        readonly ILogger _logger;
        readonly ICombatEngine _gameEngine;
        readonly IDiceService _dice;

        public NonCombatEncounterController(ISeasonsDbService db,
            IDiceService dice,
            ICombatEngine combatEngine,
            ILogger<GameStatesController> logger)
        {
            _dice = dice;
            _db = db;
            _logger = logger;
            _gameEngine = combatEngine;
        }

        /// <summary>
        /// This route will return the lore answer and also remove the lore encounter from CurrentEncounters in gamestate
        /// </summary>
        /// <param name="questionSlug">QuestionSlug is the slug of the question selected</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoreEncounter/{encounterId}/{questionSlug}")]
        public async Task<string> LoreEncounterAnswer(string encounterId, string questionSlug)
        {
            var heroId = this.GetHeroId();

            var gameState = await _db.getCollection<NewCombatLogic.DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            if (null == gameState.CurrentEncounters)
            {
                throw new InvalidDataException("CurrentEncounters should not be null here");
            }

            var loreEncounter = gameState.CurrentEncounters.Where(e => e.id == encounterId).Single() as LoreEncounter;

            if (null == loreEncounter)
            {
                throw new InvalidDataException($"encounterId {encounterId} is not type of lore encounter");
            }

            var tile = new TileDto();

            if (tile.MapLoreEncounterFinish.ContainsKey(TileDto.ZoneFromTile(gameState.CurrentTile)))
            {
                await (tile.MapLoreEncounterFinish[TileDto.ZoneFromTile(gameState.CurrentTile)](_logger, _db, heroId, encounterId, TileDto.ZoneFromTile(gameState.CurrentTile)));
            }
            else
            {
                _logger.LogError($"No lore encounter found for {gameState.CurrentTile}");
            }

            return GetLoreEncounterAnswer(questionSlug);
        }

        /// <summary>
        /// This route will return the lore answer and also remove the lore encounter from CurrentEncounters in gamestate
        /// </summary>
        /// <param name="questionSlug">QuestionSlug is the slug of the question selected</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BossEncounter/{encounterId}")]
        public async Task BossEncounterFinish(string encounterId)
        {
            var heroId = this.GetHeroId();

            var gameState = await _db.getCollection<NewCombatLogic.DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            if (null == gameState.CurrentEncounters)
            {
                throw new InvalidDataException("CurrentEncounters should not be null here");
            }

            var bossEncounter = gameState.CurrentEncounters.Where(e => e.id == encounterId).Single() as BossEncounter;

            if (null == bossEncounter)
            {
                throw new InvalidDataException($"encounterId {encounterId} is not type of boss encounter");
            }

            var tile = new TileDto();

            if (tile.MapLoreEncounterFinish.ContainsKey(TileDto.ZoneFromTile(gameState.CurrentTile)))
            {
                await (tile.MapLoreEncounterFinish[TileDto.ZoneFromTile(gameState.CurrentTile)](_logger, _db, heroId, encounterId, TileDto.ZoneFromTile(gameState.CurrentTile)));
            }
            else
            {
                _logger.LogError($"No lore encounter found for {gameState.CurrentTile}");
            }
        }

        /// <summary>
        /// This route will remove the location encounter from the CurrentEncounter in gamestate and also mark the tile as discovered in nonCombatTileState
        /// </summary>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        [HttpPut("LocationEncounterFinish/{encounterId}")]
        public async Task LocationEncounterFinish(string encounterId)
        {
            var heroId = this.GetHeroId();
            var collection = _db.getCollection<DbGameState>();
            var gameState = await _db.getCollection<NewCombatLogic.DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            if (null == gameState.CurrentEncounters)
            {
                throw new InvalidDataException("CurrentEncounters should not be null here");
            }

            var locationEncounter = gameState.CurrentEncounters.Where(e => e.id == encounterId).Single() as LocationEncounter;

            if (null == locationEncounter)
            {
                throw new InvalidDataException($"encounterId {encounterId} is not type of lore encounter");
            }

            var tile = new TileDto();

            if (tile.MapLocationEncounterFinish.ContainsKey(TileDto.ZoneFromTile(gameState.CurrentTile)))
            {
                await (tile.MapLocationEncounterFinish[TileDto.ZoneFromTile(gameState.CurrentTile)](_logger, _db, heroId, encounterId, TileDto.ZoneFromTile(gameState.CurrentTile), locationEncounter));
            }
            else
            {
                _logger.LogError("No lore encounter found");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encounterId"></param>
        /// <param name="choiceSlug"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        [HttpGet("ChanceEncounter/{encounterId}/{choiceSlug}")]
        public async Task<ChanceEncounterDto> ChanceEncounterResult(string encounterId, string choiceSlug)
        {
            var heroId = this.GetHeroId();
            var collection = _db.getCollection<DbGameState>();
            var gameState = await _db.getCollection<NewCombatLogic.DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            #region Making sure game state has correct data for the encounter we are looking for

            if (null == gameState.CurrentEncounters)
            {
                throw new InvalidDataException("CurrentEncounters should not be null here");
            }

            var chanceEncounter = gameState.CurrentEncounters.Where(e => e.id == encounterId).Single() as ChanceEncounter;
             
            if (null == chanceEncounter)
            {
                throw new InvalidDataException($"encounterId {encounterId} is not type of chance encounter");
            }
            #endregion

            #region Ensuring chance encounter data is correct and matching the requested choice

            var chacneEncounterData = GetChanceEncounterTemplateData(true);

            if (null == chacneEncounterData)
            {
                throw new InvalidDataException($"Unable to load chance encounter data");
            }

            // When front-end pass in choiceSlug as true or false, that means they are playing gambler or not 
            var parseResult = bool.TryParse(choiceSlug, out var playerChoice);

            if (parseResult)
            {
                if (playerChoice)
                {
                    // choice slug 11 to 13 are gambler choices
                    choiceSlug = GetRandomWithSeed().Next(11, 14).ToString();
                }
                else
                {
                    // slug 14 is dodge option for gambler
                    choiceSlug = "14";
                } 
            }

            var encounter = chacneEncounterData.Where(e => e.Choices.Where(c => c.choiceSlug == choiceSlug).Any()).Single();

            if (null == encounter)
            {
                throw new InvalidDataException($"Invalid chance encounter choice. ChoiceSlug {choiceSlug}");
            }

            var choice = encounter.Choices.Where(c => c.choiceSlug == choiceSlug).Single();

            if (null == choice)
            {
                throw new InvalidDataException($"Invalid chance encounter choice. ChoiceSlug {choiceSlug}");
            }

            #endregion

            // Prepare setter to remove encounter from collection
            gameState.CurrentEncounters = gameState.CurrentEncounters.Where(e => e.id != encounterId).ToArray();
            var setter = Builders<DbGameState>.Update.Set(g => g.CurrentEncounters, gameState.CurrentEncounters);

            //todo: fix this to read from template, for now we have way too may if else
            int? usesDiceWithSides = null;
            bool sendDiceResult = false;

            if (encounter.ChanceEncounterType == ChanceEncounterEnum.gambler)
            {
                usesDiceWithSides = 6;

                //not sure if we need this. But doing this to note change what we are sending back for Gambler
                sendDiceResult = parseResult && playerChoice;
            }
            else if (encounter.ChanceEncounterType == ChanceEncounterEnum.wonderingWizard)
            {
                usesDiceWithSides = 20;
            }


            int rollResult = 0;

            if (usesDiceWithSides.HasValue)
            {
                // For gambler encounters, we roll a 6 sided dice. 1 - 3 are good results, 4-6 are bad
                rollResult = _dice.Roll(usesDiceWithSides.Value);
            }
            else 
            {
                rollResult = _dice.Roll(100, DiceRollReason.ChanceEncounterGoodRoll);
            }

            bool isGoodOutcome = choice.goodOutcomeChance >= rollResult;

            _logger.LogDebug($"isGoodOutcome is {isGoodOutcome}");

            TileDto tile = new TileDto();

            string encounterResponceSlug = string.Empty;

            // Execute the logic for the result of the chance encounter. Update hero and remove encounter from collection
            if (tile.MapChanceEncounterResult.ContainsKey(encounter.ChanceEncounterType))
            {
                var result= await (tile.MapChanceEncounterResult[encounter.ChanceEncounterType](_logger, _db, heroId, choiceSlug, isGoodOutcome, setter));

                if(null != result.setter)
                {
                    setter = result.setter;
                }

                if (result.sendDiceResult.HasValue)
                {
                    sendDiceResult = result.sendDiceResult.Value;
                }
                

                encounterResponceSlug = result.encounterResponceSlug??String.Empty;

                await collection.UpdateOneAsync(g => g.HeroId == heroId, setter);
            }
            else
            {
                _logger.LogError("No chance encounter found");
            }

            return new ChanceEncounterDto
            {
                encounterResponceSlug= encounterResponceSlug,
                isGoodOutcome = isGoodOutcome,
                OutComeText = isGoodOutcome ? choice.goodOutcomeText : choice.badOutcomeText,
                DiceResult = (usesDiceWithSides.HasValue && sendDiceResult)
                ? rollResult : null // or do we prefer 0 orver null?
            }; 
        }

        public class ChanceEncounterDto
        {
            public int? DiceResult { get; set; }

            public string OutComeText { get; set; } = "";

            [Required]
            public bool isGoodOutcome { get; set; }

            /// <summary>
            /// An encounter specific response slug that is passed to the frontend
            /// </summary>
            public string? encounterResponceSlug { get; set; }

        }
    }
}
