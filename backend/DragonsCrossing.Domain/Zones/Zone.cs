using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Zones
{
    /// <summary>
    /// The Parent land/adventure tile
    /// </summary>
    public class Zone
    {
        public int Id { get; set; }
        /// <summary>
        /// NOTE: currently not being used by UI
        /// </summary>
        public string Name { get; set; }
        public List<Tile> Tiles { get; set; }
        /// <summary>
        /// The order the zones can be explored in
        /// NOTE: currently not being used by UI
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The minimum number of lore encounters required to enter this zone
        /// NOTE: currently not being used by UI
        /// </summary>
        public int LoreEncountersRequired { get; set; }

        /// <summary>
        /// The HeroSkills that can be found in this zone.
        /// NOTE: currently not being used by UI
        /// </summary>
        public List<HeroSkillTemplateZone> HeroSkillsAllowed { get; set; }
    }
}
