using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Domain.GameStates;
using DragonsCrossing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Repositories
{
    public class GameStatesRepository : IGameStatesRepository
    {
        private readonly ApplicationDbContext dbContext;

        public GameStatesRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<GameState> GetGameState(int gameStateId)
        {
            return await dbContext.GameStates
                .SingleOrDefaultAsync(g => g.Id == gameStateId);
        }

        public async Task<GameState> GetGameStateByHero(int heroId)
        {
            return await dbContext.GameStates
                .SingleOrDefaultAsync(g => g.Hero.Id == heroId);
        }

        public async Task<int> SaveGameState(GameState gameState)
        {
            if (gameState.Id <= 0)
                await dbContext.GameStates.AddAsync(gameState);
            else
                dbContext.Update(gameState);

            await dbContext.SaveChangesAsync();
            return gameState.Id;
        }
    }
}
