using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Infrastructure
{
    public interface IHeroInventoriesRepository
    {
        Task<HeroInventory> GetHeroInventory(int heroId);
        Task<ArmorTemplate> GetArmorTemplate(bool? isStartingGear);
        Task<int> CreateArmor(Armor armor);
        Task<WeaponTemplate> GetWeaponTemplate(CharacterClass heroClass, bool? isStartingGear);
        Task<int> CreateWeapon(Weapon weapon);
        Task<Weapon> GetWeapon(int weaponId);
        Task<List<GearAffixTemplate>> GetGearAffixTemplates();
    }
}
