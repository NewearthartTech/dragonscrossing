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
    public class OpponentParryState
    {
        public OpponentParryState(CombatStateMachine stateMachine)
        {
            var combatDetail = stateMachine.SetCombatDetail(CombatDetailType.OpponentParry);
            if (stateMachine.IsHerosTurn)
            {
                // did the monster parry
                if (DidOpponentParry(stateMachine.Combat.Monster.ParryChance))
                {
                    combatDetail.IsSuccess = true;
                    stateMachine.CurrentDamage = CalculateParryDamage(stateMachine.CurrentDamage);
                }
            }
            else
            {
                // did the hero parry
                double parryChance = stateMachine.HeroCombatStatsCalculated.GetParryRate();
                //GameState.Hero.EquippedWeapons[0].HeroStatsModifierResults[0].StatType == Domain.Heroes.HeroStatType
                //GameState.Hero.EquippedArmor[0].HeroStatsModifierResults[0]
                if (DidOpponentParry(parryChance))
                {
                    combatDetail.IsSuccess = true;
                    stateMachine.CurrentDamage = CalculateParryDamage(stateMachine.CurrentDamage);
                }
            }
            new CriticalHitState(stateMachine);
        }

        private int CalculateParryDamage(double currentDamage)
        {
            return (int)MathRound.Down(currentDamage * .5d);
        }

        private bool DidOpponentParry(double parryRate)
        {
            return parryRate.IsSuccessfulRoll();
        }
    }
}
