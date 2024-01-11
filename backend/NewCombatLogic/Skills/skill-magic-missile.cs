using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{

    public void apply_skill_magic_missile()
    {
        //Deal 3D4 + Wisdom / 8
        var currentWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState).wisdom;

        var diceRollFirst = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var diceRollSecond = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var diceRollThird = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice, (int)Math.Round(currentWisdom / 8.0));

        var damagTotal = diceRollFirst + diceRollSecond + diceRollThird;

        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= damagTotal) ? Combat.Monster.HitPoints - damagTotal : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

    }
}
