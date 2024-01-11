using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Weapons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class GearAffixWeaponSlotSeed
    {
        public GearAffixWeaponSlotSeed(ModelBuilder builder)
        {
            builder.Entity<GearAffixWeaponSlot>(b =>
            {
                // Draining
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 1,
                    GearAffixTemplateId = 1,
                    WeaponSlotType = WeaponSlotType.PrimaryHand
                });
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 2,
                    GearAffixTemplateId = 1,
                    WeaponSlotType = WeaponSlotType.TwoHand
                });

                // Truthful
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 3,
                    GearAffixTemplateId = 2,
                    WeaponSlotType = WeaponSlotType.PrimaryHand
                });
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 4,
                    GearAffixTemplateId = 2,
                    WeaponSlotType = WeaponSlotType.TwoHand
                });

                // Disarming
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 5,
                    GearAffixTemplateId = 3,
                    WeaponSlotType = WeaponSlotType.PrimaryHand
                });
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 6,
                    GearAffixTemplateId = 3,
                    WeaponSlotType = WeaponSlotType.TwoHand
                });

                // Savage
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 7,
                    GearAffixTemplateId = 4,
                    WeaponSlotType = WeaponSlotType.PrimaryHand
                });
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 8,
                    GearAffixTemplateId = 4,
                    WeaponSlotType = WeaponSlotType.TwoHand
                });

                // Stunning
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 9,
                    GearAffixTemplateId = 5,
                    WeaponSlotType = WeaponSlotType.PrimaryHand
                });
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 10,
                    GearAffixTemplateId = 5,
                    WeaponSlotType = WeaponSlotType.TwoHand
                });

                // Strong
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 11,
                    GearAffixTemplateId = 13,
                    WeaponSlotType = WeaponSlotType.PrimaryHand
                });
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 12,
                    GearAffixTemplateId = 13,
                    WeaponSlotType = WeaponSlotType.TwoHand
                });

                // Agile
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 13,
                    GearAffixTemplateId = 14,
                    WeaponSlotType = WeaponSlotType.PrimaryHand
                });
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 14,
                    GearAffixTemplateId = 14,
                    WeaponSlotType = WeaponSlotType.TwoHand
                });

                // Wise
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 15,
                    GearAffixTemplateId = 15,
                    WeaponSlotType = WeaponSlotType.PrimaryHand
                });
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 16,
                    GearAffixTemplateId = 15,
                    WeaponSlotType = WeaponSlotType.TwoHand
                });

                // Vital
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 17,
                    GearAffixTemplateId = 16,
                    WeaponSlotType = WeaponSlotType.PrimaryHand
                });
                b.HasData(new GearAffixWeaponSlot()
                {
                    Id = 18,
                    GearAffixTemplateId = 16,
                    WeaponSlotType = WeaponSlotType.TwoHand
                });
            });
        }
    }
}
