using DragonsCrossing.Core.Common;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Combats.StateMachine
{
    public class OpponentDodgeState
    {
        public OpponentDodgeState(CombatStateMachine stateMachine)
        {
            var combatDetail = stateMachine.SetCombatDetail(CombatDetailType.OpponentDodge);
            if (stateMachine.IsHerosTurn)
            {
                if (IsSuccessfulDodge(stateMachine.Combat.Monster.Quickness, 0, combatDetail))
                {
                    //stateMachine.CurrentDamage = 0;
                    new EndTurnState(stateMachine);
                }
            }
            else
            {
                var heroDodgeBonus = stateMachine.HeroCombatStatsCalculated.GetMaxWisdom() * .05;
                if (IsSuccessfulDodge(stateMachine.HeroCombatStatsCalculated.GetMaxQuickness(), heroDodgeBonus, combatDetail))
                {
                    //stateMachine.CurrentDamage = 0;
                    new EndTurnState(stateMachine);
                }
            }

            // failed to dodge so move on
            new CalculateBaseDamageState(stateMachine);
        }

        /// <summary>
        /// Dodge Rate for both monster and hero is: quickness / 4
        /// </summary>
        /// <param name="dodgeRate"></param>
        /// <param name="combatDetail"></param>
        /// <returns></returns>
        private bool IsSuccessfulDodge(double quickness, double bonus, CombatDetail combatDetail)
        {
            var dodgeRate = quickness / 4 + bonus;
            combatDetail.Amount = dodgeRate;
            var isSuccessfulDodge = dodgeRate.IsSuccessfulRoll();
            combatDetail.IsSuccess = isSuccessfulDodge;
            return isSuccessfulDodge;
        }
    }
}
