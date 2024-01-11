using DragonsCrossing.Core.Common;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Combats.StateMachine
{
    public class CriticalHitState
    {
        public CriticalHitState(CombatStateMachine stateMachine)
        {
            var combatDetail = stateMachine.SetCombatDetail(CombatDetailType.CriticalHit);
            double critHitChance = 0;

            if (stateMachine.IsHerosTurn)
            {
                // TODO: hero skills. Ex: guaranteed crit hit on next 2 attacks.
                var heroSkillsBonus = 0;

                // TODO: non combat encounter bonus. Ex: +10 to crit for next 2 combats
                var nonCombatEncounterBonus = 0;

                critHitChance = stateMachine.HeroCombatStatsCalculated.GetCriticalHitRate() + nonCombatEncounterBonus + heroSkillsBonus;
            }
            else
            {
                critHitChance = stateMachine.MonsterCombatStatsCalculated.GetCriticalHitRate();
            }

            combatDetail.Amount = critHitChance;
            CalculateCriticalHit(stateMachine, combatDetail, critHitChance);

            new ArmorMitigationState(stateMachine);
        }

        private void CalculateCriticalHit(CombatStateMachine stateMachine, CombatDetail combatDetail, double critHitChance)
        {
            if (critHitChance.IsSuccessfulRoll())
            {
                stateMachine.CurrentDamage *= 1.5;
                combatDetail.IsSuccess = true;
            }
        }
    }
}
