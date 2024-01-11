using DragonsCrossing.Core.Contracts.Api.Dto.Combats;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    public bool MonsterHasUsedSpecialAblilty()
    {
        return Combat.Monster.WhichRoundMonsterUsedSpecialAbility.HasValue &&
                !Combat.Monster.SpecialAbility.CanUseSpecialAbilityMoreThanOnce;
    }

    public bool CanMonsterUseSpecailAbility(int diceRoll)
    {
        // if ChanceOfUsingSpecialAbility is a 2.3%, we store it as 230. So the die would be a 10000 side die
        if (diceRoll <= Combat.Monster.ChanceOfUsingSpecialAbility)
        {   
            // Monster will cast the special ability at the current round, but will only start taking effect next round, thus the +1
            Combat.Monster.WhichRoundMonsterUsedSpecialAbility = Combat.CurrentRound + 1;
            return true;
        }

        return false;
    }

    LogicReponses Q2_RollToSeeIfMonsterUsedSpecialAbility()
    {
        //if the Monster has already used special ability and cannot use special ability anymore we just skip to next step.
        if (MonsterHasUsedSpecialAblilty())
        {
            return LogicReponses.next;
        }

        var diceRoll = _dice.Roll(10000,
            DiceRollReason.MonsterUsedSpecialAbility,
            Combat.monsterAttackResult.Dice);

        CanMonsterUseSpecailAbility(diceRoll);

        return LogicReponses.next;
    }
}


