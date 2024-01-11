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
    /// The possible items that can drop from this monster template
    /// </summary>
    public class MonsterItemLoot
    {
        public int Id { get; set; }
        public int MonsterTemplateId { get; set; }
        public WeaponTemplate? WeaponTemplate { get; set; }
        public int? WeaponTemplateId { get; set; }
        public ArmorTemplate? ArmorTemplate { get; set; }
        public int? ArmorTemplateId { get; set; }
        public NftItemTemplate? NftItemTemplate { get; set; }
        public int? NftItemTemplateId { get; set; }
        public double LootDropChance { get; set; }
    }
}
