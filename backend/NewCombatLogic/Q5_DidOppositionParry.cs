using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using static DragonsCrossing.Core.Helper.DataHelper;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    public int CalculateChanceToParry()
    {
        var parryChances = 0;

        if (Combat.isHerosTurn)
        {
            parryChances = CalculateSpecialAbilityIfApplicable(
               CombatantType.Monster,
               EffectedbySpecialAbilityStat.Q5_Parry,
               Combat.Monster.GenerateCalculatedMonsterStats(Combat).ParryChance);
        }
        else
        {
            // Monster's turn and we need to calculate the hero's chance to parry
            // Function GenerateCalculatedCharacterStats already considers the following
            // 1. hero chance to parry level schedule
            // 2. Equipped item effects

            parryChances = CalculateSpecialAbilityIfApplicable(
                CombatantType.Hero,
                EffectedbySpecialAbilityStat.Q5_Parry,
                GameState.Hero.GenerateCalculatedCharacterStats(GameState).chanceToParry);

        }

        return parryChances;
    }

    LogicReponses Q5_DidOppositionParry()
    {
        var parryChances = CalculateChanceToParry();

        var diceRoll = _dice.Roll(10000,
            Combat.isHerosTurn ? DiceRollReason.MonsterParryDmgFromHero : DiceRollReason.HeroParryDmgFromMonster,
            Combat.isHerosTurn ? Combat.heroAttackResult.Dice: Combat.monsterAttackResult.Dice
            ); //reviewed

        return diceRoll <= parryChances ? LogicReponses.yes : LogicReponses.no;
    }

}


