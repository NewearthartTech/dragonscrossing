using DragonsCrossing.Core.Common;
using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Monsters;
using DragonsCrossing.Domain.Weapons;
using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Items
{
    public class GearGenerator
    {
        private readonly HeroInventory heroInventory;        

        public GearGenerator(HeroInventory heroInventory)
        {
            this.heroInventory = heroInventory;
        }
        
        /// <summary>
        /// Creates a random weapon based on the template, but doesn't equip it.
        /// </summary>
        /// <param name="weaponTemplate"></param>
        /// <returns></returns>
        public Weapon CreateNewWeapon(WeaponTemplate weaponTemplate, List<GearAffixTemplate>? allGearAffixTemplates = null, int? zoneId = null)
        {
            var weapon = new Weapon()
            {
                BonusDamage = weaponTemplate.BonusDamage?.GetRandom(),
                CriticalHitRate = weaponTemplate.CriticalHit?.GetRandom(),
                DoubleHitRate = weaponTemplate.DoubleHit?.GetRandom(),
                DodgeRate = weaponTemplate.Dodge?.GetRandom(),
                ParryRate = weaponTemplate.Parry?.GetRandom(),
                WeaponTemplate = weaponTemplate,
                IsEquipped = false,
                HeroInventoryId = heroInventory.Id,
                SlotNumber = null,
                Rarity = GetGearRarity(),
            };

            // assign affixes
            var weaponAffixTemplates = allGearAffixTemplates
                ?.Where(gearAffixTemplate => gearAffixTemplate.WeaponSlots != null && gearAffixTemplate.WeaponSlots
                .Select(weaponSlot => weaponSlot.WeaponSlotType)
                .Where(weaponSlot => weaponSlot != WeaponSlotType.SecondaryHand)
                .Contains(weaponTemplate.SlotType))
                .ToList();

            if (weaponAffixTemplates != null && zoneId != null)
                weapon.Affixes = new AffixGenerator<WeaponAffix>().GetGearAffixes(weapon.Rarity, weaponAffixTemplates, zoneId.Value);

            return weapon;
        }

        /// <summary>
        /// Creates a random armor based on the template, but doesn't equip it.
        /// </summary>
        /// <param name="armorTemplate"></param>
        /// <param name="allGearAffixTemplates"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public Armor CreateNewArmor(ArmorTemplate armorTemplate, List<GearAffixTemplate>? allGearAffixTemplates = null, int? zoneId = null)
        {
            var armor = new Armor()
            {
                Defense = armorTemplate.Defense.GetRandom(),
                CriticalHitRate = armorTemplate.CriticalHit?.GetRandom(),
                ArmorTemplate = armorTemplate,
                IsEquipped = false,
                HeroInventoryId = heroInventory.Id,
                SlotNumber = null,
                Rarity = GetGearRarity(),
            };

            // assign affixes
            var armorAffixTemplates = allGearAffixTemplates
                ?.Where(gearAffixTemplate => gearAffixTemplate.ArmorSlots != null && gearAffixTemplate.ArmorSlots
                .Select(armorSlot => armorSlot.ArmorSlotType)
                .Contains(armorTemplate.SlotType))
                .ToList();

            if (armorAffixTemplates != null && zoneId != null)
                armor.Affixes = new AffixGenerator<ArmorAffix>().GetGearAffixes(armor.Rarity, armorAffixTemplates, zoneId.Value);

            return armor;
        }        

        /// <summary>
        /// Equips a weapon that's in the hero inventory
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="weapon"></param>
        /// <param name="weaponSlot"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>No need to adjust hero stats here since they're calculated in HeroCombatStatsCalculated</remarks>
        public void EquipWeapon(Weapon weapon)
        {
            var existingWeapon = heroInventory.Weapons.SingleOrDefault(w => w.Id == weapon.Id);
            
            // add the gear to inventory if it's new (this occurs if picking up loot or buying a weapon)
            if (existingWeapon == null)
                heroInventory.Weapons.Add(weapon);
            else if (existingWeapon.IsEquipped)
                UnequipWeapon(existingWeapon);
                        
            weapon.IsEquipped = true;
        }

        /// <summary>
        /// Equips an armor that's in the hero inventory
        /// </summary>
        /// <param name="armor"></param>
        /// <remarks>No need to adjust hero stats here since they're calculated in HeroCombatStatsCalculated</remarks>
        public void EquipArmor(Armor armor)
        {
            var existingArmor = heroInventory.Armors.SingleOrDefault(a => a.Id == armor.Id);

            // add the gear to inventory if it's new from loot or the shop
            if (existingArmor == null)
                heroInventory.Armors.Add(armor);
            else
                UnequipArmor(existingArmor);

            armor.IsEquipped = true;
        }

        public void DropWeapon(Weapon weapon)
        {
            if (weapon == null || weapon.Id == 0)
                return;

            UnequipWeapon(weapon);

            if (heroInventory.Weapons.Contains(weapon))
                heroInventory.Weapons.Remove(weapon);
        }

        public void DropArmor(Armor armor)
        {
            if (armor == null || armor.Id == 0)
                return;

            UnequipArmor(armor);

            if (heroInventory.Armors.Contains(armor))
                heroInventory.Armors.Remove(armor);
        }

        /// <summary>
        /// Unequips the weapon but doesn't remove it from inventory.
        /// </summary>
        /// <param name="weapon"></param>
        /// <remarks>No need to adjust hero stats here since they're calculated in HeroCombatStatsCalculated</remarks>
        public void UnequipWeapon(Weapon weapon)
        {
            if (weapon == null || !weapon.IsEquipped)
                return;

            weapon.IsEquipped = false;
        }                

        /// <summary>
        /// Unequips the armor but doesn't remove it from inventory.
        /// </summary>
        /// <param name="armor"></param>
        /// <remarks>No need to adjust hero stats here since they're calculated in HeroCombatStatsCalculated</remarks>
        public void UnequipArmor(Armor? armor)
        {
            if (armor == null)
                return;

            armor.IsEquipped = false;
        }

        private GearRarity GetGearRarity()
        {
            int randomNum = new Random().Next(0, 100);
            // 5% chance
            if (randomNum < 5) 
                return GearRarity.Legendary;
            // 15% chance
            if (randomNum < 20)
                return GearRarity.Epic;
            // 20% chance
            if (randomNum < 40)
                return GearRarity.Rare;
            // 25% chance
            if (randomNum < 65)
                return GearRarity.Uncommon;
            // 35% chance
            return GearRarity.Common;
        }
    }
}
