using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using static DragonsCrossing.Core.Helper.DataHelper;


namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    public int CalculateChanceToCritHit()
    {
        var critChances = 0;

        if (Combat.isHerosTurn)
        {
            // Hero's turn and we need to calculate the hero's chance to crit
            // Function GenerateCalculatedCharacterStats already considers the following
            // 1. Hero chance to crit hit level schedule
            // 2. Equipped item effects

            critChances = CalculateSpecialAbilityIfApplicable(
                CombatantType.Hero,
                EffectedbySpecialAbilityStat.Q7_Crit,
                GameState.Hero.GenerateCalculatedCharacterStats(GameState).chanceToCrit);
        }
        else
        {
            //Monster's turn 

            critChances = CalculateSpecialAbilityIfApplicable(
               CombatantType.Monster,
               EffectedbySpecialAbilityStat.Q7_Crit,
               Combat.Monster.GenerateCalculatedMonsterStats(Combat).CritChance);
        }

        return critChances;
    }

    LogicReponses Q7_WasHitACriticalStrike()
    {
        var critChances = CalculateChanceToCritHit();

        var diceRoll = _dice.Roll(10000,
        Combat.isHerosTurn ? DiceRollReason.DidHeroCrit : DiceRollReason.DidMonsterCrit,
        Combat.isHerosTurn ? Combat.heroAttackResult.Dice : Combat.monsterAttackResult.Dice
        ); //reviewed

        return diceRoll <= critChances ? LogicReponses.yes : LogicReponses.no;
    }
}


