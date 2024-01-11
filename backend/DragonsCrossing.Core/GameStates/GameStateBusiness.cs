using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Domain.GameStates;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.GameStates
{
    public class GameStateBusiness
    {
        private GameState gameState;

        public GameStateBusiness()
        {
        }

        public GameStateBusiness(GameState gameState)
        {
            this.gameState = gameState;
        }

        public GameState CreateNewGameState(Hero hero, Zone zone)
        {
            return new GameState()
            {
                Hero = hero,
                Combat = null,
                CurrentZone = zone,
                CurrentZoneId = zone.Id,                
            };
        }

        public bool DoesPreviousCombatExistAndHasEnded()
        {
            return gameState.Combat != null && gameState.Combat.IsCombatOver;
        }


        public bool IsValidGameState()
        {
            return gameState != null &&
                gameState.Hero != null &&
                gameState.CurrentZoneId > 0;
        }

        public bool IsCombatStarted()
        {
            return gameState.Combat != null && 
                !gameState.Combat.IsCombatOver && 
                gameState.Combat.Monster != null &&
                gameState.Hero.Id == gameState.Combat.Hero.Id;
        }
    }
}
