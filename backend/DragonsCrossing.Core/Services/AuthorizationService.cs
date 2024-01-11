using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Domain.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IPlayersRepository playersRepository;
        private int playerId = 1; // TODO: Remove this hard coded playerId once it's figured out.

        public AuthorizationService(IPlayersRepository playersRepository)
        {
            this.playersRepository = playersRepository;
        }

        //TODO: somehow get player id
        public int PlayerId 
        { 
            get
            {
                return playerId;
            }
            set
            {
                playerId = value;
            }
        }

        /// <summary>
        /// Gets the current player from the db. 
        /// Note: If you need to get the player from the blockchain then use the BlockchainService.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Player> GetCurrentPlayer()
        {
            if (PlayerId <= 0)
            {
                throw new Exception("Invalid player id");
            }

            var player = await playersRepository.GetPlayer(PlayerId);

            if (player == null)
                throw new Exception("Player doesn't exist");            

            return player;
        }
    }
}
