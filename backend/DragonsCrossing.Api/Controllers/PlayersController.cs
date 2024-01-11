using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        readonly ILogger _logger;
        readonly IGameStateService _gameStateService;

        public PlayersController(
            IGameStateService gameStateService,
            ILogger<PlayersController> logger
            )
        {
            _gameStateService = gameStateService;
            _logger = logger;
        }

        /// <summary>
        /// Get player ID associated with my authenticated userId
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [Authorize]
        [HttpGet("me")]
        public async Task<PlayerDto> me()
        {
            var playerId = this.GetUserId();

            

            if (null == playerId)
                throw new ArgumentNullException(nameof(playerId));

            return await _gameStateService.EnsurePlayerInSeason(playerId);

            

        }

    }
}
