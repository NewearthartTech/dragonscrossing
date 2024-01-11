using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{

    public void apply_skill_overhand_chop()
    {
        //Deal 1D6 + Strength / 7
        var currentStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState).strength;

        var diceRoll2 = _dice.Roll(6, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice, (int)Math.Round(currentStrength / 7.0));

        var damagTotal = diceRoll2;

        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= damagTotal) ? Combat.Monster.HitPoints - damagTotal : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

    }
}
