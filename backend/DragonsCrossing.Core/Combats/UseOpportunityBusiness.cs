using DragonsCrossing.Core.Heroes;
using DragonsCrossing.Domain.Combats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Combats
{
    public class UseOpportunityBusiness
    {
        private readonly Combat combat;
        private readonly HeroCombatStatsCalculated heroCombatStatsCalculated;

        public UseOpportunityBusiness(Combat combat)
        {
            this.combat = combat;
            heroCombatStatsCalculated = new HeroCombatStatsCalculated(combat);
        }

        public void UseCombatOpportunity()
        {
            var randomNum = new Random().Next(1, 21); // 1D20            
            var heroScore = randomNum + GetHeroStatValue();
            var monsterScore = randomNum + combat.Monster.Power;
            if (heroScore <= monsterScore)
            {
                // check for loss or tie
                if (monsterScore - heroScore >= 5)
                {
                    // loss. So give monster +10% to all stats
                    combat.CombatOpportunityResult = OpportunityResultType.Loss;
                    AdjustMonsterStats(10);
                }
                else
                {
                    combat.CombatOpportunityResult = OpportunityResultType.Tie;
                    // it was a tie so do nothing
                }
            }
            else
            {
                // check for win or tie
                if (heroScore - monsterScore >= 5)
                {
                    combat.CombatOpportunityResult = OpportunityResultType.Win;
                    AdjustMonsterStats(-15);
                }
                else
                {
                    combat.CombatOpportunityResult = OpportunityResultType.Tie;
                    // it was a tie so do nothing
                }
            }
        }

        private void AdjustMonsterStats(int v)
        {
            throw new NotImplementedException();
        }

        private int GetHeroStatValue()
        {
            if (combat.Monster.MonsterTemplateOld.MonsterClass == null)
                return 0;

            switch (combat.Monster.MonsterTemplateOld.MonsterClass)
            {
                case Domain.Heroes.CharacterClass.Warrior:
                    return heroCombatStatsCalculated.GetStrength();
                case Domain.Heroes.CharacterClass.Ranger:
                    return heroCombatStatsCalculated.GetAgility();
                case Domain.Heroes.CharacterClass.Mage:
                    return heroCombatStatsCalculated.GetMaxWisdom();
                default:
                    throw new Exception("Unable to calculate hero stat value. Contact support.");
            }
        }

        internal void UseCharismaOpportunity()
        {
            throw new NotImplementedException();
        }
    }
}
