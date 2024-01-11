using DragonsCrossing.Core.Combats.StateMachine;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Heroes;
using DragonsCrossing.Core.Monsters;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Combats
{
    public class CombatGenerator
    {
        private readonly Combat combat;
        private readonly HeroCombatStatsCalculated heroCombatStatsCalculated;
        private readonly MonsterCombatStatsCalculated monsterCombatStatsCalculated;

        public CombatGenerator()
        {
        }
        public CombatGenerator(Combat combat)
        {
            this.combat = combat;
            heroCombatStatsCalculated = new HeroCombatStatsCalculated(combat);
            monsterCombatStatsCalculated = new MonsterCombatStatsCalculated(combat);
        }

        public Combat StartCombat(Hero hero, Monster monster)
        {
            return new Combat()
            {
                Hero = hero,
                Monster = monster,
                TileId = monster.MonsterTemplateOld.TileId,
                Round = 1,
                IsCombatOpportunityAvailable = IsCombatOpportunityAvailable(monster),
                IsCharismaOpportunityAvailable = IsCharismaOpportunityAvailable(monster),
                CombatLoot = null,
                UserConfirmedCombatEnd = false,       
                IsCombatOver = false,                
            };
        }

        private bool IsCombatOpportunityAvailable(Monster monster)
        {
            return monster.MonsterTemplateOld.CombatOpportunityChance.IsSuccessfulRoll();
        }

        private bool IsCharismaOpportunityAvailable(Monster monster)
        {
            return monster.MonsterTemplateOld.CharismaOpportunityChance.IsSuccessfulRoll();
        }

        public void Attack()
        {
            var doesHeroGoFirst = DoesHeroGoFirst();
            var stateMachine = new CombatStateMachine(combat, heroCombatStatsCalculated, monsterCombatStatsCalculated, doesHeroGoFirst);
            stateMachine.SetCombatDetail(CombatDetailType.DoesHeroGoFirst, doesHeroGoFirst);
            
            // first attack
            new StartTurnState(stateMachine);

            // second attack (if the opponent is still alive)
            if (!combat.IsCombatOver)
            {
                // change turns
                stateMachine.ChangeTurns();
                new StartTurnState(stateMachine);
            }

            // move to the next round if combat is still going
            if (!combat.IsCombatOver)
            {
                combat.Round++;

                // reset abilities and skills, give passive damage, etc...
                stateMachine.RunPreRoundLogic();
            }            
        }

        private bool DoesHeroGoFirst()
        {
            // 1D20 roll + quickness. Tie goes to monster
            var random = new Random();
            var randomNumHero = random.Next(1, 21) + heroCombatStatsCalculated.GetMaxQuickness();
            var randomNumMonster = random.Next(1, 21) + combat.Monster.Quickness;

            return randomNumHero > randomNumMonster;
        }        
    }
}
