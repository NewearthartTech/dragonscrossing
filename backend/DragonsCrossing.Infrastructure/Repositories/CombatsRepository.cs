using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Repositories
{
    public class CombatsRepository : ICombatsRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CombatsRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// This should get called at the start of combat.
        /// </summary>
        /// <param name="combat"></param>
        /// <returns></returns>
        public async Task DeleteCombat(Combat combat)
        {
            // ensure the hero and tile are not deleted.
            combat.Hero = null;
            combat.Tile = null;
            if (combat.CombatLoot != null)
            {
                dbContext.CombatLoots.Remove(combat.CombatLoot);
            }
            dbContext.Monsters.Remove(combat.Monster);
            dbContext.Combats.Remove(combat);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Combat> GetCombat(int combatId)
        {
            return await dbContext.Combats.FindAsync(combatId);
        }

        public async Task<int> UpsertCombat(Combat combat)
        {
            if (combat.Id <= 0)
                await dbContext.AddAsync(combat);
            else
                dbContext.Update(combat);

            await dbContext.SaveChangesAsync();
            return combat.Id;
        }

        
    }
}
