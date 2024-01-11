using DragonsCrossing.Domain.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    public class GearAffixWeaponSlot
    {
        public int Id { get; set; }
        public int GearAffixTemplateId { get; set; }
        public WeaponSlotType WeaponSlotType { get; set; }
    }
}
