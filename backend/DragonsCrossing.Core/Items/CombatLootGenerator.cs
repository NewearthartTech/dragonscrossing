using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.GameStates;
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
    public class CombatLootGenerator
    {
        private readonly HeroInventory heroInventory;
        private readonly GearGenerator gearGenerator;
        private MonsterTemplateOld MonsterTemplateOld;

        public CombatLootGenerator(HeroInventory heroInventory, MonsterTemplateOld MonsterTemplateOld)
        {
            this.heroInventory = heroInventory;
            this.MonsterTemplateOld = MonsterTemplateOld;
            this.gearGenerator = new GearGenerator(heroInventory);
        }

        public CombatLoot GenerateCombatLoot(List<GearAffixTemplate> allGearAffixTemplates)
        {
            var combatLoot = new CombatLoot()
            {
                UnsecuredDcx = MonsterTemplateOld.DcxLootChance.IsSuccessfulRoll() 
                    ? MonsterTemplateOld.DcxLootAmount.GetRandom() 
                    : 0,
                Gold = MonsterTemplateOld.GoldLootChance.IsSuccessfulRoll() 
                    ? MonsterTemplateOld.GoldLootAmount.GetRandom()
                    : 0,
                NftItems = null, // TODO: implement this
                ExperiencePoints = GetExperiencePoints(MonsterTemplateOld.Tile.Type),              
            };

            AssignAndGenerateGearLoot(allGearAffixTemplates, combatLoot);
            return combatLoot;
        }        

        /// <summary>
        /// TODO: confirm this is correct
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private int GetExperiencePoints(TileType type)
        {
            switch (type)
            {
                case TileType.RegularCombat:
                case TileType.DailyCombat:
                    return 1;
                case TileType.Boss:
                    return 2;
                case TileType.FinalBoss:
                    return 3;
                default:
                    return 0;
            }
        }

        private void AssignAndGenerateGearLoot(List<GearAffixTemplate> allGearAffixTemplates, CombatLoot combatLoot)
        {
            if (MonsterTemplateOld.ItemsLoot == null)
            {
                // TODO: Log warning here. I think all monsters should have at least 1 gear that can drop.
                return;
            }

            var newWeapons = new List<Weapon>();
            var newArmors = new List<Armor>();
            var newNftItems = new List<NftItem>();

            foreach (var itemLoot in MonsterTemplateOld.ItemsLoot)
            {
                bool didLootDrop = itemLoot.LootDropChance.IsSuccessfulRoll();
                if (!didLootDrop)
                {
                    continue;
                }

                if (itemLoot.WeaponTemplate != null)
                {                    
                    var newWeapon = gearGenerator.CreateNewWeapon(itemLoot.WeaponTemplate, allGearAffixTemplates, MonsterTemplateOld.Tile.ZoneId);
                    if (newWeapon != null)
                    {
                        newWeapons.Add(newWeapon);
                    }
                }
                else if (itemLoot.ArmorTemplate != null)
                {                    
                    var newArmor = gearGenerator.CreateNewArmor(itemLoot.ArmorTemplate, allGearAffixTemplates, MonsterTemplateOld.Tile.ZoneId);
                    if (newArmor != null)
                    {
                        newArmors.Add(newArmor);
                    }
                }
                else if (itemLoot.NftItemTemplate != null)
                {
                    // generate a skill book or shard
                }
            }

            combatLoot.Weapons = newWeapons.Any() ? newWeapons : null;
            combatLoot.Armor = newArmors.Any() ? newArmors : null;
            combatLoot.NftItems = newNftItems.Any() ? newNftItems : null;
        }        
    }
}
