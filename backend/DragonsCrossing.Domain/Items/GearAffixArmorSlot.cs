using DragonsCrossing.Domain.Armors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    public class GearAffixArmorSlot
    {
        public int Id { get; set; }
        public int GearAffixTemplateId { get; set; }
        public ArmorSlotType ArmorSlotType { get; set; }
    }
}
