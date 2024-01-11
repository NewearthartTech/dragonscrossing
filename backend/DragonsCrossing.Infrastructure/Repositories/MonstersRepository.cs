using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Monsters;
using DragonsCrossing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Repositories
{
    public class MonstersRepository : IMonstersRepository
    {
        private readonly ApplicationDbContext dbContext;

        public MonstersRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<MonsterPersonality>> GetAllMonsterPersonalities()
        {
            return await dbContext.MonsterPersonalities
                .Include(m => m.AttributesAffected)
                .ToListAsync();
        }

        public async Task<Monster> GetMonster(int monsterId)
        {
            return await dbContext.Monsters.FindAsync(monsterId);
        }

        public async Task<List<MonsterItemLoot>> GetMonsterItemsLoot(int MonsterTemplateId)
        {
            return await dbContext.MonsterItemsLoot
                .Where(m => m.MonsterTemplateId == MonsterTemplateId)
                .ToListAsync();
        }

        public async Task<MonsterTemplateOld> GetMonsterTemplate(int MonsterTemplateId)
        {
            return await dbContext.MonsterTemplates
                .SingleOrDefaultAsync(template => template.Id == MonsterTemplateId);
        }

        public async Task<List<MonsterTemplateOld>> GetMonsterTemplatesByTile(int tileId)
        {
            return await dbContext.MonsterTemplates
                .Where(template => template.TileId == tileId)
                .ToListAsync();
        }

        public async Task<int> SaveMonster(Monster monster)
        {
            if (monster.Id <= 0)
                await dbContext.Monsters.AddAsync(monster);
            else
                dbContext.Update(monster);

            await dbContext.SaveChangesAsync();
            return monster.Id;
        }

        private IQueryable<MonsterTemplateOld>? ProcessIncludes(MonsterTemplateOld.ChildIncludes[] includes)
        {
            var MonsterTemplates = dbContext.MonsterTemplates.AsQueryable();
            if (includes != null && includes.Any())
            {
                if (includes.Contains(MonsterTemplateOld.ChildIncludes.SpecialAbilities))
                {
                    MonsterTemplates = MonsterTemplates
                        .Include(h => h.SpecialAbilities);
                }

                if (includes.Contains(MonsterTemplateOld.ChildIncludes.ItemsLoot))
                {
                    MonsterTemplates = MonsterTemplates
                        .Include(h => h.ItemsLoot);
                }
            }
            return MonsterTemplates;
        }
    }
}
