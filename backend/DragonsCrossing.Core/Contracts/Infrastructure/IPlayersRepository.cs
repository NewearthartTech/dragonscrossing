using DragonsCrossing.Domain.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Infrastructure
{
    public interface IPlayersRepository
    {
        Task<Player?> GetPlayer(int playerId, params Player.ChildIncludes[] includes);
        Task<int> SavePlayer(Player player);
        Task<List<Player>> GetAllPlayers(params Player.ChildIncludes[] includes);
        Task<Player> ensurePlayer(string blockChainWalletAddress);
        
    }
}
