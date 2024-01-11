using DragonsCrossing.Domain.Armors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    public class ArmorAffix : GearAffix
    {
        public ArmorSlotType SlotType { get; set; }
    }
}
