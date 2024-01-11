using DragonsCrossing.Core.Common;
using DragonsCrossing.Domain.Combats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Combats.StateMachine
{
    /// <summary>
    /// Subtract the currentDamage from the monsters or hero's hitpoints
    /// and determine who's dead.
    /// </summary>
    public class EndTurnState
    {
        public EndTurnState(CombatStateMachine stateMachine)
        {
            var combatDetail = stateMachine.SetCombatDetail(CombatDetailType.EndTurn, true);

            // subtract hitpoints
            if (stateMachine.IsHerosTurn)
            {
                stateMachine.CurrentDamage = stateMachine.CurrentDamage.RoundToNearestInt();
                stateMachine.Combat.Monster.HitPoints -= (int)stateMachine.CurrentDamage;
                combatDetail.Amount = stateMachine.CurrentDamage;
            }
            else
            {
                stateMachine.CurrentDamage = stateMachine.CurrentDamage.RoundToNearestInt();
                stateMachine.Combat.Hero.CombatStats.HitPoints -= (int)stateMachine.CurrentDamage;
                combatDetail.Amount = stateMachine.CurrentDamage;
            }

            // determine who's dead
            if (stateMachine.Combat.Hero.CombatStats.HitPoints <= 0)
            {
                stateMachine.Combat.Hero.CombatStats.HitPoints = 0;
                stateMachine.Combat.IsHeroDead = true;
                stateMachine.Combat.IsCombatOver = true;
            }

            if (stateMachine.Combat.Monster.HitPoints <= 0)
            {
                stateMachine.Combat.Monster.HitPoints = 0;
                stateMachine.Combat.IsMonsterDead = true;
                stateMachine.Combat.IsCombatOver = true;
            }
        }
    }
}
