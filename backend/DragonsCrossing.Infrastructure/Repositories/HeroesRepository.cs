using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Repositories
{
    public class HeroesRepository : IHeroesRepository
    {
        private readonly ApplicationDbContext dbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public HeroesRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// TODO: Make sure we won't delete a hero's name, level, or template if a hero is deleted.
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public async Task<int> SaveHero(Hero hero)
        {
            if (hero.Id <= 0)
                await dbContext.Heroes.AddAsync(hero);
            else
                dbContext.Heroes.Update(hero);            

            await dbContext.SaveChangesAsync();
            return hero.Id;
        }

        public async Task<List<Hero>> GetAllHeroes(params Hero.ChildIncludes[] includes)
        {
            var heroes = ProcessIncludes(includes);
            return await heroes.ToListAsync();            
        }

        public async Task<List<Hero>> GetHeroes(List<int> heroIds, params Hero.ChildIncludes[] includes)
        {
            var heroes = ProcessIncludes(includes);
            return await heroes
                .Where(hero => heroIds.Contains(hero.Id))
                .ToListAsync();
        }

        /// <summary>
        /// Gets a hero
        /// </summary>
        /// <param name="heroId"></param>
        /// <returns></returns>
        public async Task<Hero> GetHero(int heroId)
        {
            return await dbContext.Heroes
                .Include(h => h.Inventory.Weapons)
                .Include(h => h.Inventory.Armors).FirstOrDefaultAsync(h => h.Id == heroId);
        }

        /// <summary>
        /// Gets the active hero template. There should only ever be 1 active template.
        /// </summary>
        /// <returns></returns>
        public async Task<HeroTemplate> GetHeroTemplate()
        {
            return await dbContext.HeroTemplates
                .Where(template => template.IsActive)
                .FirstAsync();
        }

        public async Task<List<HeroName>> GetAllHeroNames()
        {
            return await dbContext.HeroNames.ToListAsync();
        }

        public async Task<HeroLevel> GetHeroLevel(int number)
        {
            return await dbContext.HeroLevels
                .SingleOrDefaultAsync(h => h.Number == number);                
        }

        public async Task DeleteHero(Hero existingHero)
        {
            dbContext.Heroes.Remove(existingHero);
            await dbContext.SaveChangesAsync();
        }

        private IQueryable<Hero>? ProcessIncludes(Hero.ChildIncludes[] includes)
        {
            var heroes = dbContext.Heroes.AsQueryable();
            if (includes != null && includes.Any())
            {
                if (includes.Contains(Hero.ChildIncludes.Skills))
                    heroes = heroes.Include(h => h.Skills);

                if (includes.Contains(Hero.ChildIncludes.Inventory))
                {
                    heroes = heroes
                        .Include(h => h.Inventory.Weapons)
                        .Include(h => h.Inventory.Armors);
                }

                if (includes.Contains(Hero.ChildIncludes.CombatStats))
                    heroes = heroes.Include(h => h.CombatStats);
            }
            return heroes;
        }        
    }
}
