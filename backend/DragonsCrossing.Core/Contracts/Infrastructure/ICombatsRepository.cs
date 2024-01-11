using DragonsCrossing.Domain.Combats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Infrastructure
{
    public interface ICombatsRepository
    {
        Task<Combat> GetCombat(int combatId);
        Task<int> UpsertCombat(Combat combat);       
        Task DeleteCombat(Combat combat);
    }
}
