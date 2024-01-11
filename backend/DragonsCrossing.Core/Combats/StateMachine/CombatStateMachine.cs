using DragonsCrossing.Core.Heroes;
using DragonsCrossing.Core.Monsters;
using DragonsCrossing.Domain.Combats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Combats.StateMachine
{
    /// <summary>
    /// Holds the state for the current turn/round (both monster and hero attacks)
    /// Note: this is not persisted to the db.
    /// </summary>
    public class CombatStateMachine
    {
        private int combatDetailOrder = 1;
        private readonly bool didHeroGoFirst;
        private bool isHerosTurn;

        public CombatStateMachine(Combat combat, HeroCombatStatsCalculated heroCombatStatsCalculated, MonsterCombatStatsCalculated monsterCombatStatsCalculated, bool doesHeroGoFirst)
        {
            // remove previous combat details since we don't need it
            combat.CombatDetails = new List<CombatDetail>();
            Combat = combat;
            HeroCombatStatsCalculated = heroCombatStatsCalculated;
            MonsterCombatStatsCalculated = monsterCombatStatsCalculated;
            didHeroGoFirst = doesHeroGoFirst;
            isHerosTurn = doesHeroGoFirst;
        }

        public HeroCombatStatsCalculated HeroCombatStatsCalculated { get; private set; }

        public MonsterCombatStatsCalculated MonsterCombatStatsCalculated { get; private set; }

        /// <summary>
        /// The current running damage. It can get updated in certain states.
        /// The end state will subtract the current damage from the hit points
        /// </summary>
        public double CurrentDamage { get; set; }

        public Combat Combat { get; private set; }

        /// <summary>
        /// Who's turn is it currently?
        /// true: if it's currently the heros turn.
        /// This value changes based on who's turn we are currently on
        /// </summary>
        public bool IsHerosTurn { get { return isHerosTurn; } }

        /// <summary>
        /// Who had the intitiative?
        /// true: if the hero had the initiative
        /// This value will never change once set
        /// </summary>
        public bool DidHeroGoFirst { get { return didHeroGoFirst; } }

        /// <summary>
        /// Creates a new CombatDetail and returns it. Will also add it to the combatDetails list.
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="isSuccess"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public CombatDetail SetCombatDetail(CombatDetailType currentState, bool isSuccess = false, double amount = 0)
        {
            var currentCombatDetail = new CombatDetail(currentState, combatDetailOrder++, isSuccess, amount);
            Combat.CombatDetails.Add(currentCombatDetail);
            return currentCombatDetail;
        }

        /// <summary>
        /// RunPreRoundCalculations()
        /// ResetAbilities()
        /// BeginningPhaseCleanup()
        /// RunPreRoundLogic()
        /// Reset damage, monster special abilities and hero skills.
        /// Don't reset the CombatDetails or order, they need to persist across turns.
        /// </summary>
        public void RunPreRoundLogic()
        {
            // remove monster special abilities that are expired
            if (Combat.Monster.SpecialAbilitiesCasted != null)
            {
                Combat.Monster.SpecialAbilitiesCasted = Combat.Monster.SpecialAbilitiesCasted
                    .Where(s => s.ActiveUntilRound > Combat.Round).ToList();
            }

            // TODO: reset hero skills casted that are expired
        }

        /// <summary>
        /// Change turns between hero and monster
        /// </summary>
        internal void ChangeTurns()
        {
            isHerosTurn = !isHerosTurn;
        }
    }
}
