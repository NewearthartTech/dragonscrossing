using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    public class GearAffixTier
    {
        public int Id { get; set; }
        public int GearAffixTemplateId { get; set; }
        public Zone Zone { get; set; }
        public int ZoneId { get; set; }
        public GearAffixTierType Type { get; set; }
        public double Amount { get; set; }
    }
}
