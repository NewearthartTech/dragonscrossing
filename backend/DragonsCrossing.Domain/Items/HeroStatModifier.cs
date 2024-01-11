using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// aka: Attribute_Effected and Attributes_Effect
    /// </summary>
    public class HeroStatModifier
    {
        public int Id { get; set; }

        /// <summary>
        /// The types of stats that are effected by the weapon or armor
        /// </summary>
        public HeroStatType StatType { get; set; }

        /// <summary>
        /// The start amount for this stat
        /// </summary>
        public int EffectAmount { get; set; }

        /// <summary>
        /// The amount to add/remove from the EffectAmount.
        /// This offset applies to all the effected attributes the same. So in theory it could be part of the weapon template
        /// but it makes more sense to put here.
        /// Ex: if the result of this random range was +3. All StatType's would have the same +3 added to them
        /// </summary>
        public RangeDouble EffectAmountOffsetRange { get; set; }
    }
}
