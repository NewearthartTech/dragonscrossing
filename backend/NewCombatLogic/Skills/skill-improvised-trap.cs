using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{

    public void apply_skill_improvised_trap()
    {
        // Reduces Monster armor mitigation amount by 20% + (Strength / 1.5) until end of combat, and deals 1D4 + (Agility / 10) damage per round for 2 rounds.
        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_improvised_trap));
        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_improvised_trap_health));

        Combat.LingeringSkillEffects.Add(new LingeringSkillAction
        {
            SkillName = "Improvised Trap",
            InRound = Combat.CurrentRound,
            ActionFunctionName = nameof(lingering_skill_improvised_trap),
            Description = "Reduces monster armor mitigation by 20% + (Strength / 1.5)%",
            RemainingRounds = -1,
            RoundStarted = Combat.CurrentRound
        });

        for (var i = 1; i < 3; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Improvised Trap - Damage",
                InRound = Combat.CurrentRound + i,
                ActionFunctionName = nameof(lingering_skill_improvised_trap_health),
                Description = "Deals 1D4 + (Agility / 10) damage per round",
                RemainingRounds = 3 - (i + 1),
                RoundStarted = Combat.CurrentRound
            });
        }        
    }

    public void lingering_skill_improvised_trap()
    {
        var currentMonsterArmorMitigation = Combat.Monster.GenerateCalculatedMonsterStats(Combat, true).ArmorMitigationAmount;
        var heroStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).strength;
        var baselineArmorBenefit = 0.2;
        var armorImpact = (heroStrength / 1.5 / 100) + baselineArmorBenefit;

        Combat.monsterAttackResult.monsteralteredStats = Combat.monsterAttackResult.monsteralteredStats.Concat(new AlteredMonsterCalculatedStats[]
            {
                new AlteredMonsterCalculatedStats
                {
                    reason = "Hero skill - Improvised Trap",
                    stats = new CalculatedMonsterStats { ArmorMitigationAmount = (int)Math.Round(-currentMonsterArmorMitigation * armorImpact) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }

    public void lingering_skill_improvised_trap_health()
    {
        var heroAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;
        var diceRoll = _dice.Roll(4, DiceRollReason.SkillLingeringDamage, Combat.heroAttackResult.Dice, (int)Math.Round(heroAgility / 10.0));
        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= diceRoll) ? Combat.Monster.HitPoints - diceRoll : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;
    }
}