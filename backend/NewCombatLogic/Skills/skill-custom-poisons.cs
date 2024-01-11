using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_custom_poisons()
    {
        //Deals 2D6 + (Agility / 7) and deals an additional 2D4 + (Agility / 10) damage per round for 2 rounds.

        var diceRoll1 = _dice.Roll(6, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var diceRoll2 = _dice.Roll(6, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var currentAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;
        var initialAgilityImpact = currentAgility / 7.0;

        var damageTotal = diceRoll1 + diceRoll2 + (int)Math.Round(initialAgilityImpact);
        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= damageTotal) ? Combat.Monster.HitPoints - damageTotal : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_custom_poisons));

        for (var i = 1; i < 3; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Custom Poisons",
                InRound = Combat.CurrentRound + i,
                ActionFunctionName = nameof(lingering_skill_custom_poisons),
                Description = "Deals additional 2D4 + (Agility / 10)",
                RemainingRounds = 3 - (i + 1),
                RoundStarted = Combat.CurrentRound
            });
        }

    }
    public void lingering_skill_custom_poisons()

    {
        var currentAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;
        var lingeringAgilityImpact = currentAgility / 10.0;

        var diceRoll1 = _dice.Roll(4, DiceRollReason.SkillLingeringDamage, Combat.heroAttackResult.Dice);
        var diceRoll2 = _dice.Roll(4, DiceRollReason.SkillLingeringDamage, Combat.heroAttackResult.Dice, (int)Math.Round(lingeringAgilityImpact));

        var totalDamage = diceRoll1 + diceRoll2;
        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= totalDamage) ? Combat.Monster.HitPoints - totalDamage : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;
    }

}
