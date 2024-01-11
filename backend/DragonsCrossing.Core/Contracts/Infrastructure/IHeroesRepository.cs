using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Infrastructure
{
    public interface IHeroesRepository
    {
        /// <summary>
        /// Get all heroes for the authenticated user. This will be used at the
        /// start of the game to allow the user to select a Hero to play with.
        /// </summary>
        /// <returns></returns>
        Task<List<Hero>> GetAllHeroes(params Hero.ChildIncludes[] includes);
        Task<Hero> GetHero(int heroId);
        Task<List<Hero>> GetHeroes(List<int> heroIds, params Hero.ChildIncludes[] includes);
        Task<int> SaveHero(Hero hero);
        Task<HeroTemplate> GetHeroTemplate();
        Task<List<HeroName>> GetAllHeroNames();
        Task<HeroLevel> GetHeroLevel(int number);
        Task DeleteHero(Hero existingHero);
    }
}
