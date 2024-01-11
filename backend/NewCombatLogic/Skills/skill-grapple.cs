using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.Weapons;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_grapple()
    {
        //Deals 1D6 damage + ((strength + agility) / 13) and reduces parry for both monster and hero by 80% for 2 rounds

        var heroStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).strength;
        var heroAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;
        var strengthAndAgilityImpact = (heroStrength + heroAgility) / 13.0;

        var diceRoll = _dice.Roll(6, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice) + (int)Math.Round(strengthAndAgilityImpact);
        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= diceRoll) ? Combat.Monster.HitPoints - diceRoll : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;


        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_grapple));

        for (var i = 1; i < 3; i++)
        {
            // Hero parry reduced by 80% for 2 rounds
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Grapple",
                InRound = Combat.CurrentRound + i,
                Description = "Reduces chance to parry for both monster and hero by 80%",
                RemainingRounds = 3 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_grapple)
            });     
        }
    }

    public void lingering_skill_grapple()
    {
        var currentChanceToParry = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToParry;
        var currentMonsterChanceToParry = Combat.Monster.GenerateCalculatedMonsterStats(Combat, true).ParryChance;

        // Hero parry reduced by 80%
        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Grapple",
                    stats = new CalculatedCharacterStats { chanceToParry = (int)Math.Round(-currentChanceToParry * .8) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();

        // Monster parry reduced by 80%
        Combat.monsterAttackResult.monsteralteredStats = Combat.monsterAttackResult.monsteralteredStats.Concat(new AlteredMonsterCalculatedStats[]
        {
                new AlteredMonsterCalculatedStats
                {
                    reason = "Hero skill - Grapple",
                    stats = new CalculatedMonsterStats { ParryChance = (int)Math.Round(-currentMonsterChanceToParry * .8) },

                    round = Combat.CurrentRound + 1
                }
        }).ToArray();
    }
}
