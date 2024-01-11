using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    public enum GearAffixEffect
    {
        // PrimaryHand and TwoHand weapons
        Draining,
        Truthful,
        Disarming,
        Savage,
        Stunning,

        // Chest Armor
        Quick,
        Thick,
        Reflective,
        Recovering,

        // Ring Armor
        Deft,
        Doubling,
        Fetching,

        // Any slot type but SecondaryHand weapon
        Strong,
        Agile,
        Wise,
        Vital,
    }
}
