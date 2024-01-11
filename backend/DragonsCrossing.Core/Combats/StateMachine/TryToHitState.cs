using DragonsCrossing.Domain.Combats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Combats.StateMachine
{
    public class TryToHitState
    {
        public TryToHitState(CombatStateMachine stateMachine)
        {
            var combatDetail = stateMachine.SetCombatDetail(CombatDetailType.TryToHit);
            // TODO: Need to add any hero skill bonuses and monster special ability bonuses to hit rate and dodge rate
            if (stateMachine.IsHerosTurn)
            {
                if (IsSuccessfulHit(stateMachine.HeroCombatStatsCalculated.GetHitRate(), stateMachine.MonsterCombatStatsCalculated.GetDodgeRate(), combatDetail))
                {
                    combatDetail.IsSuccess = true;
                    new OpponentDodgeState(stateMachine);
                }
                else
                {
                    //stateMachine.CurrentDamage = 0;
                    new EndTurnState(stateMachine);
                }
            }
            else
            {
                if (IsSuccessfulHit(stateMachine.Combat.Monster.HitRate, stateMachine.HeroCombatStatsCalculated.GetDodgeRate(), combatDetail))
                {
                    combatDetail.IsSuccess = true;
                    new OpponentDodgeState(stateMachine);
                }
                else
                {
                    //stateMachine.CurrentDamage = 0;
                    new EndTurnState(stateMachine);
                }
            }
        }





        private bool IsSuccessfulHit(int chanceToHit, double opponentDodgeRate, CombatDetail combatDetail)
        {
            var cth = new Random().Next(chanceToHit, 100);
            combatDetail.Amount = cth;
            return cth > opponentDodgeRate;
        }
    }
}
