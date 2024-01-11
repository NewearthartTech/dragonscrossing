using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Domain.Players;
using DragonsCrossing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Repositories
{
    public class PlayersRepository : IPlayersRepository
    {
        private readonly ApplicationDbContext dbContext;

        public PlayersRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Player>> GetAllPlayers(params Player.ChildIncludes[] includes)
        {
            var players = ProcessIncludes(includes);
            return await players.ToListAsync();
        }

        public async Task<Player?> GetPlayer(int playerId, params Player.ChildIncludes[] includes)
        {
            var players = ProcessIncludes(includes);
            return  await players
                .Where(player => player.Id == playerId)
                .SingleOrDefaultAsync();
        }

        public async Task<Player> ensurePlayer(string blockChainWalletAddress)
        {
            var existing =  await dbContext.Players
                .Where(player => player.BlockchainPublicAddress == blockChainWalletAddress)
                .SingleOrDefaultAsync();

            if (null != existing)
                return existing;

            var newPlayer = new Player
            {
                //dee-todo: are there any defaults a player needs
                BlockchainPublicAddress = blockChainWalletAddress,

                Name = "player",
                CreatedBy = "walletAuthenticated",
                DateCreated = DateTime.Now,
                SignedSignature = "NOT USED",
                GameSettingId = 1
            };

            await dbContext.Players.AddAsync(newPlayer);

            await dbContext.SaveChangesAsync();

            return newPlayer;

        }

        public async Task<int> SavePlayer(Player player)
        {
            if (player.Id <= 0)
                await dbContext.Players.AddAsync(player);
            else
                dbContext.Players.Update(player);

            await dbContext.SaveChangesAsync();
            return player.Id;
        }

        private IQueryable<Player>? ProcessIncludes(Player.ChildIncludes[] includes)
        {
            var players = dbContext.Players.AsQueryable();
            if (includes != null && includes.Any())
            {
                if (includes.Contains(Player.ChildIncludes.GameSetting))
                    players = players.Include(p => p.GameSetting);

                if (includes.Contains(Player.ChildIncludes.Backpack))
                    players = players.Include(p => p.Backpack);

                if (includes.Contains(Player.ChildIncludes.Heroes))
                {
                    players = players.Include(p => p.Heroes);
                    if (includes.Contains(Player.ChildIncludes.Heroes_CombatStats))
                        players = players.Include(p => p.Heroes).ThenInclude(h => h.CombatStats);
                }
            }
            return players;
        }
    }
}
