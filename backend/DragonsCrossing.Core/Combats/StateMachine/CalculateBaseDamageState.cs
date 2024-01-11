using DragonsCrossing.Core.Common;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Common;

namespace DragonsCrossing.Core.Combats.StateMachine
{
    /// <summary>
    /// Damage is calculated eagerly - meaning apply damage modifiers in each state so it always keeps a "running total"
    /// instead of calculating modifiers at the end of the calculation.
    /// </summary>
    /// <remarks>This is the only state that doesn't take you to another state.</remarks>
    public class CalculateBaseDamageState
    {
        public CalculateBaseDamageState(CombatStateMachine stateMachine)
        {
            var combatDetail = stateMachine.SetCombatDetail(CombatDetailType.CalculateBaseDamage);
            if (stateMachine.IsHerosTurn)
            {
                stateMachine.CurrentDamage = stateMachine.HeroCombatStatsCalculated.GetBaseDamage().GetRandom();
            }
            else
            {
                stateMachine.CurrentDamage = stateMachine.MonsterCombatStatsCalculated.GetBaseDamage().GetRandom();
                //stateMachine.CurrentDamage = MathRound.Down(totalDamage);
            }

            combatDetail.IsSuccess = true;
            combatDetail.Amount = stateMachine.CurrentDamage;
            new OpponentParryState(stateMachine);
        }
    }
}
