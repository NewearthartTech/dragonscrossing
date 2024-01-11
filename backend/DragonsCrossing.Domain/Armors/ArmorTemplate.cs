using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Armors
{
    /// <summary>
    /// Armor templates will be pre-built. There are a fixed amount that will be manually entered into the db.
    /// </summary>
    public class ArmorTemplate : GearTemplate
    {
        /// <summary>
        /// Combat Stat: The stat used to compare against the hero's stat. If they're the same the hero gets an added defense bonus.
        /// This is optional
        /// </summary>
        public CharacterClass? HeroClass { get; set; }

        public RangeDouble Defense { get; set; }

        /// <summary>
        /// The critical hit percent armor can have when dropped
        /// </summary>
        public RangeDouble? CriticalHit { get; set; }

        /// <summary>
        /// flavor text
        /// </summary>
        public ArmorType ArmorType { get; set; }

        /// <summary>
        /// The type of slot this armor is allowed to use
        /// </summary>
        public ArmorSlotType SlotType { get; set; }
    }
}
