using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Domain.Heroes;
using static DragonsCrossing.Core.Helper.DataHelper;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    public int CalculateChangceToDodge()
    {
        var chanceToDodge = 0;

        if (Combat.isHerosTurn)
        {
            chanceToDodge = CalculateSpecialAbilityIfApplicable(
                CombatantType.Monster,
                EffectedbySpecialAbilityStat.Q4_ChanceToDodge,
                Combat.Monster.GenerateCalculatedMonsterStats(Combat).ChanceToDodge
             );

        }
        else
        {
            // Monster's turn and we need to calculate the hero's chance to dodge
            // Function GenerateCalculatedCharacterStats already considers the following
            // 1. Hero chance to dodge level schedule
            // 2. Equipped item effects

            chanceToDodge = CalculateSpecialAbilityIfApplicable(
                CombatantType.Hero,
                EffectedbySpecialAbilityStat.Q4_ChanceToDodge,
                GameState.Hero.GenerateCalculatedCharacterStats(GameState).chanceToDodge);
        }

        return chanceToDodge;
    }

    LogicReponses Q4_DidOppositionDodge()
    {

        var chanceToDodge = CalculateChangceToDodge();

        var diceRoll = _dice.Roll(10000,
            Combat.isHerosTurn ? DiceRollReason.MonsterChanceToDodge : DiceRollReason.HeroChanceToDodge,
            Combat.isHerosTurn ? Combat.heroAttackResult.Dice: Combat.monsterAttackResult.Dice
            ); //reviewed

        // If it is hero's turn and monster didn't dodge, then hero hit the monster
        if (Combat.isHerosTurn)
        {
            Combat.heroAttackResult.IsHit = diceRoll > chanceToDodge;
        }
        else
        {
            Combat.monsterAttackResult.IsHit = diceRoll > chanceToDodge;
        }

        return diceRoll <= chanceToDodge ? LogicReponses.yes : LogicReponses.no;
    }
}


