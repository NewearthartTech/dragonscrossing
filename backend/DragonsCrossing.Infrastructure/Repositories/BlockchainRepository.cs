using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Core.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DragonsCrossing.Infrastructure.Repositories;

public class BlockchainRepository : IBlockchainRepository
{

    public async Task<bool> DoesPlayerExist(int playerId)
    {
        // TODO: make a web3 call to the blockchain to get the player id
        if (playerId == 1)
            return true;
        else
            throw new Exception("Only player 1 exists because it's hard coded");
    }

    public async Task<List<int>> GetHeroIds(int playerId)
    {
        // TODO: make a web3 call to the blockchain to get the hero ids for the player
        return new List<int>()
        {
            3033,3034,3035,3036
        };
    }
}

