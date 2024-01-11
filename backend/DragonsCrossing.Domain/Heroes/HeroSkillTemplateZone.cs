using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Heroes
{
    /// <summary>
    /// The skill that can be found in the zone.
    /// This is the many-to-many class.
    /// </summary>
    public class HeroSkillTemplateZone
    {
        public int Id { get; set; }
        public int ZoneId { get; set; }
        public int HeroSkillTemplateId { get; set; }
    }
}
