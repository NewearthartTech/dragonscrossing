using System;
using System.Linq.Expressions;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Combats;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.GameStates;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Core.Services;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.GameStates;
using MongoDB.Driver;
using Newtonsoft.Json;
using static DragonsCrossing.Core.Helper.DataHelper;

namespace DragonsCrossing.NewCombatLogic;

public class CombatConfig
{
    public bool enableMendrakeRoot { get; set; } 
}

public partial class CombatEngine: ICombatEngine
{
    readonly ISeasonsDbService _db;
    readonly IBlockchainService _blockchainService;
    readonly ILogger _logger;
    readonly IDiceService _dice;
    readonly IDataHelperService _dataHelperService;
    readonly CombatConfig _combatConfig;

    public CombatEngine(
        ISeasonsDbService db,
        IConfiguration config,
        IDiceService dice,
        IBlockchainService blockchainService,
        IDataHelperService dataHelperService,
        ILogger<CombatEngine> logger)
    {
        _db = db;
        _dice = dice;
        _blockchainService = blockchainService;
        _logger = logger;
        _dataHelperService = dataHelperService;
        _combatConfig = config.GetSection("combat").Get<CombatConfig>() ?? new CombatConfig();

    }

    public DbGameState GameState { get; set; } = new DbGameState
    {

    };


    public CombatEncounter Combat
    {
        get
        {
            var encounters = GameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

            var combat = encounters?.Where(e => e.Monster != null).FirstOrDefault();
            return combat ?? new CombatEncounter();
        }
    }

    

    async Task EnsureGameState(int heroId)
    {
        var collection = _db.getCollection<DbGameState>();
        GameState = await collection.Find(c => c.HeroId == heroId).SingleOrDefaultAsync();

        //todo: use proper HTTP error code
        if (null == Combat)
            throw new ExceptionWithCode("not found");

        if (null == Combat.Monster)
        {
            throw new Exception("Combat has not been initialized");
        }

    }

    public async Task<ActionResponseDto> persuade(int heroId)
    {
        await EnsureGameState(heroId);

        var collection = _db.getCollection<DbGameState>();

        GameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        var encounters = GameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

        var combat = encounters?.Where(e => e.Monster != null).Single();

        if (null == combat )
            throw new Exception("We are not in combat");

        if (null == combat.Monster)
        {
            throw new Exception("Monster cannot be null during combat");
        }

        if (!combat.isCharismaOpportunityAvailable)
        {
            throw new Exception("Charisma opportunity not available, cannot persuade.");
        }

        var monsterCharismaResult = combat.Monster.Charisma + _dice.Roll(20, DiceRollReason.MonsterCharismaRoll, combat.monsterAttackResult.Dice);

        var heroCharismaResult = GameState.Hero.calculatedStats.charisma + _dice.Roll(20, DiceRollReason.HeroCharismaRoll, combat.heroAttackResult.Dice);

        var difference = heroCharismaResult - monsterCharismaResult;

        // Question: Does using persuade increase round number if monster is not killed? 
        // What are some of the flags need to be set for each scenario?

        // Hero should always have initiative on persuade/charisma opportunity
        Combat.Initiative = CombatantType.Hero;

        if (difference < -3)
        {
            // Loss, monster gets a free hit
            Combat.charismaOpportunityResultType = CharismaOpportunityResultType.Loss;

            RunSteps(MonsterFreeHit);
        }
        else if (difference >= -3 && difference <= 3)
        {
            // Tie, nothing happens
            Combat.charismaOpportunityResultType = CharismaOpportunityResultType.Tie;
        }
        else if (difference > 3 && difference <= 7)
        {
            // Partial win, hero gains experience but no loot.
            Combat.isMonsterDead = true;
            Combat.charismaOpportunityResultType = CharismaOpportunityResultType.PartialWin;
            Combat.loot = new MonsterLootDto() { };

            RunSteps(AC11_HeroGainExperience);
        }
        else if (difference > 7)
        {
            // Full win, hero gains experience and loot
            Combat.isMonsterDead = true;
            Combat.charismaOpportunityResultType = CharismaOpportunityResultType.Win;

            RunSteps(AC10_GenerateLoot);
        }

        Combat.isCombatActionTaken = true;

        await collection.ReplaceOneAsync(c => c.HeroId == GameState.HeroId, GameState);

        return actionReponse;
    }

    public async Task<UpdateDefinition<DbGameState>> applyLingeringEffects(UpdateDefinition<DbGameState> setter, int heroId, DbGameState gameState)
    {
        await EnsureGameState(heroId);

        GameState = gameState;

        RunSteps(this.AC0_ApplyLingeringSkillEffects);

        setter = setter.Set(g => g.CurrentEncounters, GameState.CurrentEncounters);
        setter = setter.Set(g => g.Hero.experiencePoints, GameState.Hero.experiencePoints);
        return setter;
    }
    public async Task<ActionResponseDto> flee(int heroId)
    {
        await EnsureGameState(heroId);

        var collection = _db.getCollection<DbGameState>();

        GameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        var encounters = GameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

        var combat = encounters?.Where(e => e.Monster != null).Single();

        if (null == combat)
            throw new Exception("We are not in combat");

        // Reset the charisma opportunityResultType
        if (combat.charismaOpportunityResultType != null)
        {
            combat.charismaOpportunityResultType = null;
        }

        var canFleeFromSkill = null != Combat.LingeringSkillEffects
            .Find(f =>
                f.ActionFunctionName == nameof(lingering_guarentedtoFlee) &&
                f.InRound == combat.CurrentRound
            );

        var heroFleeScore = _dice.Roll(20, DiceRollReason.HeroFleeRoll, Combat.heroAttackResult.Dice) + (GameState.Hero.GenerateCalculatedCharacterStats(GameState).quickness / 4.0);

        var monsterFleeScore = _dice.Roll(20, DiceRollReason.MonsterFleeRoll, Combat.monsterAttackResult.Dice) + (Combat.Monster.GenerateCalculatedMonsterStats(Combat).Quickness / 4.0);

        var canFleeFromCombat = ((heroFleeScore > monsterFleeScore) || canFleeFromSkill);

        Combat.Initiative = CombatantType.Hero;

        if (!canFleeFromCombat)
        { 
            RunSteps(MonsterFreeHit);
        }
        else 
        {
            Combat.isHeroAbleToFlee = true;
        }

        Combat.isCombatActionTaken = true;

        await collection.ReplaceOneAsync(c => c.HeroId == GameState.HeroId, GameState);

        return actionReponse;
    }

    public async Task<ActionResponseDto> attack(int heroId)
    {
        await EnsureGameState(heroId);
        var collection = _db.getCollection<DbGameState>();

        GameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        /* commented out the bandaid
        if(GameState.lastCombatActionAt.ToUniversalTime().AddSeconds(2) > DateTime.Now.ToUniversalTime())
        {
            throw new ExceptionWithCode("Attack not yet possible");
        }

        var combatDateFilter = GameState.lastCombatActionAt == DateTime.MinValue ?
            Builders<DbGameState>.Filter.Where(c => c.HeroId == heroId) :
            Builders<DbGameState>.Filter.Where(c => c.HeroId == heroId && c.lastCombatActionAt == GameState.lastCombatActionAt);


        var updatedCount = await collection.UpdateOneAsync(combatDateFilter,
    Builders<DbGameState>.Update
                    .Set(c => c.lastCombatActionAt, DateTime.Now));

        if(0 == updatedCount.MatchedCount)
        {
            throw new ExceptionWithCode("Concurrent attack not possible");
        }
        */


        var encounters = GameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

        var combat = encounters?.Where(e => e.Monster != null).Single();

        if (null == combat)
            throw new ExceptionWithCode("Hero is not in combat");

        if (combat.isHeroDead)
        {
            throw new ExceptionWithCode("Hero is dead, cannot attact.");
        }

        if (combat.isHeroAbleToFlee)
        {
            throw new ExceptionWithCode("Hero fled, cannot attact.");
        }

        // Reset the charisma opportunityResultType
        if (combat.charismaOpportunityResultType != null)
        {
            combat.charismaOpportunityResultType = null;
        }

        combat.CurrentRound += 1;

        await collection.UpdateOneAsync(c => c.HeroId == heroId,
            Builders<DbGameState>.Update
                .Set(c => c.CurrentEncounters, new Encounter[] { combat }));


        GameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        RunSteps(this.StartOfAttack);

        await collection.ReplaceOneAsync(c => c.HeroId == GameState.HeroId, GameState);

        return actionReponse;
    }

    public async Task<ActionResponseDto> applySkill(int heroId, DbGameState gameState)
    {
        
        var collection = _db.getCollection<DbGameState>();
        GameState = gameState;

        // After a skill is applied, we go throgh steps to make sure loot is generated if monster is dead and similar things for Hero.
        RunSteps(this.SkillsApplied);

        await collection.ReplaceOneAsync(c => c.HeroId == GameState.HeroId, GameState);

        return actionReponse;
    }

    public bool isCombatOver { get { return Combat.isHeroDead || Combat.isMonsterDead; } }

    public bool isHeroDead { get { return Combat.isHeroDead; } }

    public ActionResponseDto? actionReponse
    { 
        get
        {
            if (null == Combat)
                return null;

            var ret = new ActionResponseDto
            {
                round = Combat.CurrentRound,
                initiative = Combat.Initiative,
                // Charisma opportunities only show during the beginning of the encounter.
                //Any action taken OTHER THAN the charisma opportunity, (attacking, using a special ability, fleeing) would void the opportunity, making it disappear. 
                isCharismaOpportunityAvailable = Combat.isCombatActionTaken ? false : Combat.isCharismaOpportunityAvailable,
                
                monsterResult = null == Combat.Monster ? new MonsterResultDto() : new MonsterResultDto
                {
                    monster = Combat.Monster,
                    isDead = Combat.isMonsterDead,
                    attackResult = Combat.monsterAttackResult,
                    loot = Combat.loot
                },

                heroResult = new HeroResultDto
                {
                    hero = GameState.Hero,
                    isDead = Combat.isHeroDead,

                    attackResult = Combat.heroAttackResult
                },
                // Right now we only allow ONE special ability per monster so the list will only have one result
                monsterSpecialAbilityStatusEffects = GetActiveMonsterSpecialAbilityStatusEffects(),
                monsterCastedSpecialAbility = (Combat.Monster.WhichRoundMonsterUsedSpecialAbility.HasValue &&
                                              ((Combat.CurrentRound + 1) == Combat.Monster.WhichRoundMonsterUsedSpecialAbility.Value))
                                              ? new LingeringStatusEffects()
                                              {
                                                  Name = Combat.Monster.SpecialAbility.Name,
                                                  Description = BuildSpecialAbilityStatusEffectDesc(GetMonsterActiveStatusEffects()),
                                                  Slug = Combat.Monster.SpecialAbility.SoundSlug
                                              }
                                              : null,
                heroSkillStatusEffects = GetActiveHeroSkillStatusEffects(),
                DidHeroFlee = Combat.isHeroAbleToFlee,
                isSkillUseAvailable = !Combat.skillsUsedInRounds.Contains(Combat.CurrentRound),
                charismaOpportunityResultType = Combat.charismaOpportunityResultType
            };

            return ret;
        }
    }

    

    void RunSteps(Func<LogicReponses> step)
    {
        var steps = Configure();

        var responce = step();

        var flow = steps[step];

        if (flow.ContainsKey(responce))
        {
            var nextStep = flow[responce];
            if(null != nextStep)
            {
                RunSteps(nextStep);
            }
        }
        else
        {
            //we are terminal
        }
    }

    #region Private methods
    private LingeringStatusEffects[] GetActiveMonsterSpecialAbilityStatusEffects()
    {
        StatusEffectDto[] activeEffects = GetMonsterActiveStatusEffects();
        if (activeEffects != null && activeEffects.Count() > 0)
        {
            return new LingeringStatusEffects[]
            { 
                new LingeringStatusEffects()
                {
                    Name = Combat.Monster.SpecialAbility.Name,
                    Description = BuildSpecialAbilityStatusEffectDesc(activeEffects)
                }
            };
        }
        else 
            return null;
    }

    private StatusEffectDto[] GetMonsterActiveStatusEffects()
    {
        return Combat.Monster.SpecialAbility.Affects
                        .Where(effect => effect.IsAffectActive(Combat.CurrentRound, Combat.Monster.WhichRoundMonsterUsedSpecialAbility) == true &&
                        // Duration is null means the status effect is active until end of game.
                        // If the status affect is still active but remaining round for the status affect is 0, don't include it.
                        (effect.Duration == null || (int)(effect.Duration - (Combat.CurrentRound - Combat.Monster?.WhichRoundMonsterUsedSpecialAbility ?? 0) - 1) > 0))
                        .ToArray();
    }

    private LingeringStatusEffects[] GetActiveHeroSkillStatusEffects()
    {
        var activeSkills = Combat.LingeringSkillEffects.Where(e => e.InRound == Combat.CurrentRound || (e.RemainingRounds == -1 && e.InRound < Combat.CurrentRound))
        .OrderByDescending(s => s.RemainingRounds).DistinctBy(s => s.SkillName)
        .Select(e => new LingeringStatusEffects()
        {
           Name = e.SkillName,
           Description = BuildSkillStatusEffectDesc(e.Description, e.RemainingRounds)
        }).ToArray();

        return activeSkills;
    }

    private string BuildSkillStatusEffectDesc(string description, int remainingRounds)
    {
        string desc = "";

        string round = remainingRounds > 0 ? "rounds" : "round";

        if (remainingRounds != -1)
        {
            desc = $"{description} for the next {remainingRounds+1} {round}.";
        }
        else
        {
            desc = $"{description} until the end of combat.";
        }

        return desc;
    }

    private string BuildSpecialAbilityStatusEffectDesc(StatusEffectDto[] activeEffects)
    {
        string desc = "";

        foreach (var effect in activeEffects)
        {
            // According to affectAmount to determine increase or reduce
            string actionStr = effect.AffectAmount > 0 ? "Increase" : "Reduce";
            // This will be hero or monster.
            string affectType = effect.AffectType.ToString().ToLower();
            // This will be the action like dodge or parry.
            string action = effect.FriendlyStatName.ToLower();

            // Duration is null means the status effect is active until end of game.
            if (effect.Duration == null)
            {
                desc += $"{actionStr} {affectType} {action} by {Math.Abs(effect.AffectAmount) / 100.0}% until the end of combat. ";
            }
            else 
            {
                int remainingRounds = (int)(effect.Duration - (Combat.CurrentRound - Combat.Monster?.WhichRoundMonsterUsedSpecialAbility ?? 0) - 1);
                
                string round = remainingRounds > 1 ? "rounds" : "round";
                
                desc += $"{actionStr} {affectType} {action} by {Math.Abs(effect.AffectAmount) / 100.0}% for the next {remainingRounds} {round}. ";
            }
        }
        return desc;
    }
    #endregion Private methods
}

