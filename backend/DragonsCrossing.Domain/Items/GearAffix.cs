using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// </summary>
    public abstract class GearAffix
    {
        public int Id { get; set; }

        /// <summary>
        /// contains default values needed to generate this gear
        /// </summary>
        public GearAffixTemplate GearAffixTemplate { get; set; }
        
        /// <summary>
        /// The randomly assigned tier to this affix
        /// </summary>
        public GearAffixTier Tier { get; set; }
    }
}
