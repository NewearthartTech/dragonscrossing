using DragonsCrossing.Core.Items;
using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Monsters;
using DragonsCrossing.Domain.Weapons;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.UnitTests.Items
{
    [TestClass]
    public class CombatLootGeneratorTests
    {
        [TestMethod]
        public void GenerateMonsterLootTest_NewWeapon()
        {
            // Arrange entities
            const WeaponSlotType weaponSlotType = WeaponSlotType.TwoHand;
            const double lootDropChance = 100;

            var MonsterTemplateOld = new MonsterTemplateOld()
            {
                ItemsLoot = new List<MonsterItemLoot>()
                {
                    new MonsterItemLoot()
                    {
                        LootDropChance = lootDropChance,
                        WeaponTemplate = new WeaponTemplate()
                        {
                            SlotType = weaponSlotType,
                        }
                    },
                }
            };

            var allGearAffixTemplates = GetDefaultGearAffixTemplates(weaponSlotType);
            var heroInventory = new HeroInventory();
            var combatLootGenerator = new CombatLootGenerator(heroInventory, MonsterTemplateOld);

            // Act
            var combatLoot = combatLootGenerator.GenerateCombatLoot(allGearAffixTemplates);
            Assert.IsNotNull(combatLoot.Weapons);
            Assert.IsTrue(combatLoot.Weapons.Any());
        }

        private List<GearAffixTemplate> GetDefaultGearAffixTemplates(WeaponSlotType weaponSlotType)
        {
            const int tierAmount = 1;

            return new List<GearAffixTemplate>()
            {
                new GearAffixTemplate()
                {
                    Tiers = new List<GearAffixTier>()
                    {
                        new GearAffixTier()
                        {
                            Type = GearAffixTierType.Tier1,
                            Amount = tierAmount,
                        },
                        new GearAffixTier()
                        {
                            Type = GearAffixTierType.Tier2,
                            Amount = tierAmount + 1,
                        },
                        new GearAffixTier()
                        {
                            Type = GearAffixTierType.Tier3,
                            Amount = tierAmount + 2,
                        },
                        new GearAffixTier()
                        {
                            Type = GearAffixTierType.Tier4,
                            Amount = tierAmount + 3,
                        },
                    },
                    WeaponSlots = new List<GearAffixWeaponSlot>()
                    {
                        new GearAffixWeaponSlot()
                        {
                            WeaponSlotType = weaponSlotType,
                        }
                    }
                }
            };
        }
    }
}
