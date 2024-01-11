using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_insidious_distraction()
    {
        //Deal 2D6 + Quickness / 5
        var currentQuickness = GameState.Hero.GenerateCalculatedCharacterStats(GameState).quickness;

        var diceRoll1 = _dice.Roll(6, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);

        var diceRoll2 = _dice.Roll(6, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice, (int)Math.Round(currentQuickness / 5.0));

        var damagTotal = diceRoll1 + diceRoll2;

        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= damagTotal) ? Combat.Monster.HitPoints - damagTotal : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

    }
}
