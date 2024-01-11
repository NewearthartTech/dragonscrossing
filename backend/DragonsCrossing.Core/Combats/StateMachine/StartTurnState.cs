using DragonsCrossing.Domain.Combats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Combats.StateMachine
{
    public class StartTurnState
    {
        public StartTurnState(CombatStateMachine stateMachine)
        {
            stateMachine.SetCombatDetail(CombatDetailType.StartTurn, true);

            // reset damage whenever we start a turn
            stateMachine.CurrentDamage = 0;

            // calculate damage then return control here. Damage needs to be calculated here before the MonsterSpecialAbilityState
            //new CalculateBaseDamageState(stateMachine);

            if (stateMachine.IsHerosTurn)
                new TryToHitState(stateMachine);
            else
                new MonsterSpecialAbilityState(stateMachine);
        }
    }
}
