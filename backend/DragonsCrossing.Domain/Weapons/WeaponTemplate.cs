using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Weapons
{
    /// <summary>
    /// Weapon templates will be pre-built. There are a fixed amount that will be manually entered into the db (about 20-30 weapons total)
    /// </summary>
    public class WeaponTemplate : GearTemplate
    {
        /// <summary>
        /// Combat Stat: The stat used to compare against the hero's stat. If they're the same the hero gets an added damage bonus.
        /// This is required.
        /// </summary>
        public CharacterClass HeroClass { get; set; }

        /// <summary>
        /// The slot this weapon can use. This is mostly used to determine if the weapon is 2 handed.
        /// For either hand weapon, this will be set to PrimaryHand. For just secondary hand weapon it will be set
        /// to secondary hand.
        /// </summary>
        public WeaponSlotType SlotType { get; set; }

        /// <summary>
        /// The base damage range this weapon has. The actual base damage is calculated each turn during combat
        /// This is going to be a dice roll so only positive whole numbers starting at 1. 
        /// It is only used during combat.
        /// </summary>
        public RangeInt BaseDamage { get; set; }

        /// <summary>
        /// The bonus damage a weapon can have when dropped.
        /// Calculated when item is generated.
        /// </summary>
        public RangeDouble? BonusDamage { get; set; }

        /// <summary>
        /// The parry percent a weapon can have when dropped
        /// </summary>
        public RangeDouble? Parry { get; set; }

        /// <summary>
        /// The dodge percent a weapon can have when dropped
        /// </summary>
        public RangeDouble? Dodge { get; set; }

        /// <summary>
        /// The critical hit percent a weapon can have when dropped
        /// </summary>
        public RangeDouble? CriticalHit { get; set; }

        public RangeDouble? DoubleHit { get; set; }        

        /// <summary>
        /// flavor text
        /// </summary>
        public WeaponType WeaponType { get; set; }
    }
}
