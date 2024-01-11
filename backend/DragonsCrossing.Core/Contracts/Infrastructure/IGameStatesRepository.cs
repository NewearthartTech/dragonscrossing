using DragonsCrossing.Domain.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Infrastructure
{
    public interface IGameStatesRepository
    {
        Task<GameState> GetGameState(int gameStateId);
        Task<GameState> GetGameStateByHero(int heroId);
        Task<int> SaveGameState(GameState gameState);
    }
}
