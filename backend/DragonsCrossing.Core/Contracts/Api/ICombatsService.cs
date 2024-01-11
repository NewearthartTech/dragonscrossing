using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api
{
    public interface ICombatsService
    {
        Task<ActionResponseDto> StartCombat(int heroId, string slug);
        Task<ActionResponseDto> Flee(int heroId);
        Task<ActionResponseDto> Attack(int heroId);
        Task<ActionResponseDto> UseSkill(int heroId, int value);

        /// <summary>
        /// Intimidate/CharismaOpportunity
        /// The player can potentially avoid combat altogether, while still obtaining an orb
        /// </summary>
        /// <returns></returns>
        Task<ActionResponseDto> UseCharismaOpportunity(int heroId);

        /// <summary>
        /// Persuade/CombatOpportunity. A random dice roll that will either lower, raise, or do nothing to a monsters stats.
        /// </summary>
        /// <returns></returns>
        Task<ActionResponseDto> UseCombatOpportunity(int heroId);
    }
}
