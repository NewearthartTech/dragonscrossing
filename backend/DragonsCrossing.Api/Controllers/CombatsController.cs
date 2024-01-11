using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.GameStates;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Domain.GameStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.Combats;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers
{
    /// <summary>
    /// This is a child of heroes and shouldn't have a root url.
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "SeasonJoined")]
    public class CombatsController : ControllerBase
    {
        readonly ICombatEngine _gameEngine;
        readonly ISeasonsDbService _db;
        readonly ILogger _logger;

        public CombatsController(ISeasonsDbService db,
            ILogger<CombatsController> logger,
            ICombatEngine gameEngine)
        {
            _gameEngine = gameEngine;
            _db = db;
            _logger = logger;
        }

        [HttpGet("Persuade")]
        public async Task<ActionResponseDto> Persuade()
        {
            var heroId = this.GetHeroId();
            return await _gameEngine.persuade(heroId);
        }

        [HttpGet("Intimidate")]
        public async Task<ActionResponseDto> Intimidate()
        {
            throw new NotImplementedException();
        }

        [HttpGet("Flee")]
        public async Task<ActionResponseDto> Flee()
        {
            var heroId = this.GetHeroId();
            return await _gameEngine.flee(heroId);
        }


        [HttpGet("Attack")]
        public async Task<ActionResponseDto> Attack()
        {
            _logger.LogDebug("Attack called");
            var heroId = this.GetHeroId();
            return await _gameEngine.attack(heroId);
        }

        [HttpGet("ApplySkill/{Slug}")]
        public async Task<ActionResponseDto?> ApplySkill(string Slug)
        {
            var heroId = this.GetHeroId();
            var gameStateCollection = _db.getCollection<DbGameState>();
            _gameEngine.GameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            var encounters = _gameEngine.GameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

            var combat = encounters?.Where(e => e.Monster != null).Single();

            if (null == combat)
                throw new ExceptionWithCode("Hero is not in combat, cannot apply skill.");

            if (combat.isHeroAbleToFlee)
            {
                throw new ExceptionWithCode("Hero fled, cannot apply skill.");
            }

            //ensure no skill have been used so far
            if (combat.skillsUsedInRounds.Contains(combat.CurrentRound))
            {
                throw new ExceptionWithCode("Skill has already been used in this round");
            }

            // Reset the charisma opportunityResultType
            if (combat.charismaOpportunityResultType != null)
            {
                combat.charismaOpportunityResultType = null;
            }

            var skill = _gameEngine.GameState.Hero.skills.FirstOrDefault(s => s.slug.ToLower() == Slug.ToLower());

            if (null == skill)
                throw new Exception($"Skill {Slug} is not found on heroId {heroId}");

            if (_gameEngine.GameState.Hero.level < skill.levelRequirement)
            {
                throw new ExceptionWithCode($"Skill level requirement not met.");
            }

            var skillInstance = skill.skillUseInstance.Where(s => !s.alreadyUsed).FirstOrDefault();

            if (null == skillInstance)
                throw new ExceptionWithCode("no more skill use left");


            var methodInfo = _gameEngine.GetType().GetMethod(skill.methodName);

            if (null == methodInfo)
                throw new InvalidOperationException($"Skill method {skill.methodName} not found");

            var t = methodInfo.Invoke(_gameEngine, null) as Task;

            //Since applying skill is always before an attack and the round would always be 1 round behind. To make it easier to understand the round skill is used, we add 1 to the current round.
            _logger.LogDebug($"Hero used skill {skill.name} at round {combat.CurrentRound + 1}.");

            combat.skillsUsedInRounds = combat.skillsUsedInRounds.Concat(new[] { combat.CurrentRound }).ToArray();

            await _gameEngine.applySkill(heroId, _gameEngine.GameState);

            skillInstance.alreadyUsed = true;

            await gameStateCollection.UpdateOneAsync(c => c.HeroId == heroId,
                Builders<DbGameState>.Update
                .Set(c => c.Hero.skills, _gameEngine.GameState.Hero.skills)
                );
            
            return _gameEngine.actionReponse;
        }

        [HttpGet("StartRound")]
        public async Task<ActionResponseDto?> StartRound()
        {
            var heroId = this.GetHeroId();
            var collection = _db.getCollection<DbGameState>();

            _gameEngine.GameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            var encounters = _gameEngine.GameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

            var combat = encounters?.Where(e => e.Monster != null).Single();

            if (null == combat)
                throw new Exception("There is no combat to initialize");

            var setter = Builders<DbGameState>.Update.Set(g => g.CurrentEncounters, _gameEngine.GameState.CurrentEncounters);

            // If monster or hero is dead, combat over, skip and continue
            if (!combat.roundsStarted.Contains(combat.CurrentRound) && 
                !combat.isMonsterDead &&
                !combat.isHeroDead)
            {
                // If game hasn't been initialized, apply skill lingering effect
                setter = await _gameEngine.applyLingeringEffects(setter, heroId, _gameEngine.GameState);

                // Reset the charismaOpportunityResultType at the beginning of the round 
                combat.charismaOpportunityResultType = null;

                combat.roundsStarted = combat.roundsStarted.Concat(new[] { combat.CurrentRound }).ToArray();

                await collection.UpdateOneAsync(c => c.HeroId == heroId,
                    setter);

            }      
            
            return _gameEngine.actionReponse;
        }

        [HttpGet("chooseOneItemOnDeath/{itemId}")]
        public async Task<HeroDto> ChooseOneItemOnDeath(string itemId)
        {
            var heroId = this.GetHeroId();
            var gameStateCollection = _db.getCollection<DbGameState>();
            _gameEngine.GameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            if (!_gameEngine.isHeroDead)
                throw new Exception("Hero is not dead");

            var itemToSave = _gameEngine.GameState.Hero.inventory.Where(i => i.id == itemId).FirstOrDefault();

            if(null == itemToSave)
            {
                itemToSave = _gameEngine.GameState.Hero.equippedItems.Where(i => i.id == itemId).First();
            }

            _ = await gameStateCollection.UpdateOneAsync(c => c.HeroId == heroId,
                Builders<DbGameState>.Update
                    .Set(c => c.Hero.equippedItems, new ItemDto[] { })
                    .Set(c => c.Hero.inventory, new ItemDto[] { itemToSave })
                    );

            _logger.LogDebug($"Hero was defeated. Hero chose {itemToSave.name} for the one item to save.");

            return await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId)
                .Project(g=>g.Hero)
                .SingleAsync();

        }
    }
}
