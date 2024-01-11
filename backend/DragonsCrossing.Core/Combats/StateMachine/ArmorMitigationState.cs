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
    /// Reduces the currentDamage based on the hero's or monsters defense
    /// </summary>
    public class ArmorMitigationState
    {
        public ArmorMitigationState(CombatStateMachine stateMachine)
        {
            var combatDetail = stateMachine.SetCombatDetail(CombatDetailType.ArmorMitigation, true);

            if (stateMachine.IsHerosTurn)
            {
                // determine which monster defense type to use based on the type of hero it's fighting
                double monsterDefense = stateMachine.MonsterCombatStatsCalculated.GetBaseDefense(stateMachine.Combat.Hero.HeroClass).GetRandom();
                stateMachine.CurrentDamage = CalculateDamageAfterArmorMitigation(stateMachine.CurrentDamage, monsterDefense, combatDetail);
            }
            else
            {
                var heroDefense = stateMachine.HeroCombatStatsCalculated.GetBaseDefense().GetRandom();
                stateMachine.CurrentDamage = CalculateDamageAfterArmorMitigation(stateMachine.CurrentDamage, heroDefense, combatDetail);
            }
            new EndTurnState(stateMachine);
        }

        /// <summary>
        /// Determines final damage based on current damage and defense of opponent.
        /// </summary>
        /// <param name="currentDamage">the current running damage of whoever's turn it is</param>
        /// <param name="defensePercentage">a defense percentage from 0 to 100 (not 0 to 1) of the opponent</param>
        /// <param name="combatDetail"> combat detail </param>
        /// <returns></returns>
        private double CalculateDamageAfterArmorMitigation(double currentDamage, double defensePercentage, CombatDetail combatDetail)
        {
            double armorMitigation = defensePercentage / 100 * currentDamage;
            combatDetail.Amount = armorMitigation;
            return currentDamage - armorMitigation;
        }
    }
}
