using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.Common;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    public bool DidHeroHit(int diceRoll) 
    {
        var heroChanceToHit = CalculateSpecialAbilityIfApplicable(
                CombatantType.Hero,
                EffectedbySpecialAbilityStat.Q3_ChanceToHit,
                GameState.Hero.GenerateCalculatedCharacterStats(GameState).chanceToHit);

        var monsterDifficultyToHit = CalculateSpecialAbilityIfApplicable(
                CombatantType.Monster,
                EffectedbySpecialAbilityStat.Q3_DifficultyToHit,
                Combat.Monster.GenerateCalculatedMonsterStats(Combat).DifficultyToHit);


        return (diceRoll + heroChanceToHit) > monsterDifficultyToHit;
    }

    public bool DidMonsterHit(int diceRoll)
    {
        var monsterChanceToHit = CalculateSpecialAbilityIfApplicable(
                CombatantType.Monster,
                EffectedbySpecialAbilityStat.Q3_ChanceToHit,
                Combat.Monster.GenerateCalculatedMonsterStats(Combat).ChanceToHit);

        var heroDifficultyToHit = CalculateSpecialAbilityIfApplicable(
                CombatantType.Hero,
                EffectedbySpecialAbilityStat.Q3_DifficultyToHit,
                GameState.Hero.GenerateCalculatedCharacterStats(GameState).difficultyToHit);

        // if ChanceToHit is a 5.3%, we store it as 530. So the die would be a 10000 side die
        return (diceRoll + monsterChanceToHit) > heroDifficultyToHit;
    }

    LogicReponses Q3_RollToSeeIfAttackisHit()
    {

        if(Combat.isHerosTurn)
        {
            var diceRoll = _dice.Roll(10000,
                DiceRollReason.DidHeroHit,
                Combat.heroAttackResult.Dice
                );
            Combat.heroAttackResult.IsHit = DidHeroHit(diceRoll);
            return Combat.heroAttackResult.IsHit ? LogicReponses.yes : LogicReponses.no;
        }
        else
        {
            var diceRoll = _dice.Roll(10000,
                DiceRollReason.DidMonsterHit,
                Combat.monsterAttackResult.Dice
                );
            Combat.monsterAttackResult.IsHit = DidMonsterHit(diceRoll);
            return Combat.monsterAttackResult.IsHit ? LogicReponses.yes : LogicReponses.no;
        }
    }
}


