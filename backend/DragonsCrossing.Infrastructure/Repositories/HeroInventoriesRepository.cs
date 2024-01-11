using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Weapons;
using DragonsCrossing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Repositories
{
    public class HeroInventoriesRepository : IHeroInventoriesRepository
    {
        private readonly ApplicationDbContext dbContext;

        public HeroInventoriesRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<HeroInventory> GetHeroInventory(int heroId)
        {
            return await dbContext.HeroInventories.Include(h => h.Hero).FirstOrDefaultAsync(h => h.HeroId == heroId);
        }

        /// <summary>
        /// Currently we only support 1 starting armor. If that changes, we'll need to return
        /// a list of armor templates.
        /// </summary>
        /// <param name="heroClass"></param>
        /// <param name="isStartingGear"></param>
        /// <returns></returns>
        public async Task<ArmorTemplate> GetArmorTemplate(bool? isStartingGear = false)
        {
            return await dbContext.ArmorTemplates
                .FirstOrDefaultAsync(w => w.IsStartingGear == isStartingGear);
        }

        public async Task<int> CreateArmor(Armor armor)
        {
            await dbContext.Armors.AddAsync(armor);
            await dbContext.SaveChangesAsync();
            return armor.Id;
        }

        /// <summary>
        /// Currently we only support 1 starting weapon per hero class. If that changes, we'll need to return
        /// a list of weapon templates here.
        /// </summary>
        /// <param name="heroClass"></param>
        /// <param name="isStartingGear"></param>
        /// <returns></returns>
        public async Task<WeaponTemplate> GetWeaponTemplate(CharacterClass heroClass, bool? isStartingGear = false)
        {
            return await dbContext.WeaponTemplates
                .FirstOrDefaultAsync(w => w.HeroClass == heroClass && w.IsStartingGear == isStartingGear);
        }

        public async Task<int> CreateWeapon(Weapon weapon)
        {
            await dbContext.Weapons.AddAsync(weapon);
            //var state = dbContext.Entry(weapon).State;
            await dbContext.SaveChangesAsync();
            return weapon.Id;
        }

        public async Task<Weapon> GetWeapon(int weaponId)
        {
            return await dbContext.Weapons
                .SingleOrDefaultAsync(w => w.Id == weaponId);
        }

        public async Task<List<GearAffixTemplate>> GetGearAffixTemplates()
        {
            return await dbContext.GearAffixTemplates.ToListAsync();
        }
    }
}
