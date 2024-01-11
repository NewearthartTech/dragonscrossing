using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using static DragonsCrossing.Core.Helper.DataHelper;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    public int CalculateChanceToMitigate()
    {
        var mitigateChances = 0;

        if (Combat.isHerosTurn)
        {
            mitigateChances = CalculateSpecialAbilityIfApplicable(
               CombatantType.Monster,
               EffectedbySpecialAbilityStat.Q8_ArmorMitigation,
               Combat.Monster.GenerateCalculatedMonsterStats(Combat).ArmorMitigation);
        }
        else
        {
            // Monster's turn and we need to calculate the hero's chance to mitigate
            // Function GenerateCalculatedCharacterStats already considers the following
            // 1. hero chance to mitigate level schedule
            // 2. Equipped item effects

            mitigateChances = CalculateSpecialAbilityIfApplicable(
                CombatantType.Hero,
                EffectedbySpecialAbilityStat.Q8_ArmorMitigation,
                GameState.Hero.GenerateCalculatedCharacterStats(GameState).armorMitigation);

        }

        return mitigateChances;
    }

    LogicReponses Q10_DidOppositionMitigate()
    {
        var mitigateChance = CalculateChanceToMitigate();

        var diceRoll = _dice.Roll(10000,
            Combat.isHerosTurn ? DiceRollReason.MonsterArmorMitigationRoll : DiceRollReason.HeroArmorMitigationRoll,
            Combat.isHerosTurn ? Combat.heroAttackResult.Dice: Combat.monsterAttackResult.Dice
            );

        return diceRoll <= mitigateChance ? LogicReponses.yes : LogicReponses.no;
    }

}


