using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.Common;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_thirst_for_blood()
    {
        //Deals 2D6 + (Stregnth / 6). If a monster is killed with this attack, heal 2D6 + (Wisdom / 6) Health

        // TODO: Check if we can put the extra 3 only on one of the die and how it would look at frontend.
        var heroStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).strength;
        var diceRoll1 = _dice.Roll(6, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var diceRoll2 = _dice.Roll(6, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice, (int)Math.Round(heroStrength/ 6.0));

        var totalDmg = diceRoll1 + diceRoll2; 

        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= totalDmg) ? Combat.Monster.HitPoints - totalDmg : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

        if (Combat.isMonsterDead)
        {
            // TODO: Check if we can put the extra 3 only on one of the die and how it would look at frontend.

            var heroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
            var heal1 = _dice.Roll(6, DiceRollReason.SkillHealing, Combat.heroAttackResult.Dice);
            var heal2 = _dice.Roll(6, DiceRollReason.SkillHealing, Combat.heroAttackResult.Dice, (int)Math.Round(heroWisdom / 6.0));



            var totalHeal = heal1 + heal2;

            GameState.Hero.remainingHitPointsPercentage = (GameState.Hero.remainingHitPointsPercentage + totalHeal / GameState.Hero.totalHitPoints * 100) >= 100.0
                ? 100.0
                : (GameState.Hero.remainingHitPointsPercentage + totalHeal / GameState.Hero.totalHitPoints * 100);
        }
    }
}
