using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Heroes
{
    /// <summary>
    /// Probalby not going to use. I opted to have a hero hold a list of equipped weapons and armor instead.
    /// </summary>
    public class HeroSlot
    {
        public Armor Head { get; set; }
        public Armor Chest { get; set; }
        public Armor Feet { get; set; }
        public Armor Ring { get; set; }
        public Weapon SecondaryHand { get; set; }
        public Weapon PrimaryHand { get; set; }        
    }
}
