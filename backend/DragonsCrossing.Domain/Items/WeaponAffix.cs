using DragonsCrossing.Domain.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    public class WeaponAffix : GearAffix
    {
        public WeaponSlotType SlotType { get; set; }
    }
}
