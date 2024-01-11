using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    public class HeroStatModifierResult
    {
        public int Id { get; set; }

        public int HeroStatEffectedId { get; set; }

        /// <summary>
        /// The types of stats that are effected by the weapon or armor
        /// </summary>
        public HeroStatType StatType { get; set; }

        /// <summary>
        /// The amount to add/remove from the hero's stat.
        /// </summary>
        public int EffectAmount { get; set; }
    }
}
