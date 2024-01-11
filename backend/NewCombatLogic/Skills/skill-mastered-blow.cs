using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_mastered_blow()
    {
        //Deal 2D6 + Strength / 5
        var currentStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState).strength;

        var diceRoll0 = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var diceRoll1 = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var diceRoll2 = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice, (int)Math.Round(currentStrength / 5.0));

        var damagTotal = diceRoll0 + diceRoll1 + diceRoll2;

        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= damagTotal) ? Combat.Monster.HitPoints - damagTotal : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

    }
}
