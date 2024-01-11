using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Armors
{
    /// <summary>
    /// The randomly generated armor from loot or the shop.
    /// This armor is generated from an ArmorTemplate.
    /// </summary>
    public class Armor : ChangeTracking, IItem
    {
        public int Id { get; set; }
        public int HeroInventoryId { get; set; }
        public bool IsEquipped { get; set; }
        public GearRarity Rarity { get; set; }
        public List<ArmorAffix>? Affixes { get; set; }

        /// <summary>
        /// The amount that is added/removed from certain hero stats when this weapon is equipped
        /// </summary>
        public List<HeroStatModifierResult>? HeroStatsModifierResults { get; set; }
        public ArmorTemplate ArmorTemplate { get; set; }
        
        
        /// <summary>
        /// The permanent defense that is given to the armor when generated
        /// </summary>
        public double Defense { get; set; }
        public double? CriticalHitRate { get; set; }
        public int? SlotNumber { get; set; }
    }
}
