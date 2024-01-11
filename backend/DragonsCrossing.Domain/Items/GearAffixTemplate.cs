using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// The weapon and armor affix default values and ranges used to generate the actual gear affix when an item is generated.
    /// </summary>
    public class GearAffixTemplate
    {
        public int Id { get; set; }
        //public int? ArmorId { get; set; }
        //public int? WeaponId { get; set; }
        //public GearAffix Affix { get; set; }
        public GearAffixEffect Effect { get; set; }
        public string EffectDescription { get; set; }
        public List<GearAffixWeaponSlot>? WeaponSlots { get; set; }
        public List<GearAffixArmorSlot>? ArmorSlots { get; set; }

        /// <summary>
        /// The available tiers for this affix
        /// </summary>
        public List<GearAffixTier> Tiers { get; set; }
    }
}
