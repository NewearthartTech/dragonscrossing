using DragonsCrossing.Domain.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api
{
    public interface IAuthorizationService
    {
        int PlayerId { get; set; }
        Task<Player> GetCurrentPlayer();
    }
}
