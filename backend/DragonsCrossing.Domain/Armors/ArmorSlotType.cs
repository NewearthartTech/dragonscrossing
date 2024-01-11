using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Armors
{
    /// <summary>
    /// NOTE: A shield is considered a weapon
    /// </summary>
    public enum ArmorSlotType
    {
        Head,
        Chest,
        Feet,
        Ring,
        //OffHand, I think I can consider a shield as weapon. If so, this slot type goes away.
    }
}
