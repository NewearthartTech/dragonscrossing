using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// Holds all items the hero found or bought.
    /// </summary>
    public abstract class InventoryBase : ChangeTracking
    {
        public int Id { get; set; }
        /// <summary>
        /// The hero doesn't have to have a weapon equipped anymore
        /// </summary>
        public List<Weapon> Weapons { get; set; } = new List<Weapon>();

        /// <summary>
        /// The hero doesn't have to have armor equipped anymore
        /// </summary>
        public List<Armor> Armors { get; set; } = new List<Armor>();

        /// <summary>
        /// How much gold the hero currently has.
        /// This is only stored in the db and never goes to the blockchain. 
        /// This changes as a hero finds gold or spends it.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// This is always unsecured. Once it becomes secured (by camping) it's removed from the game/db and 
        /// the blockchain becomes the owner of it.
        /// </summary>
        public double Dcx { get; set; } = 0;

        /// <summary>
        /// The number of slots in this inventory including equipped weapons and armor.
        /// 20 extra slots + 6 equipped slots = 26.
        /// </summary>
        public int MaxAvailableSlots { get; set; } = 26;

        /// <summary>
        /// The order the items are displayed in the inventory. This excludes 
        /// equipped armor and weapons since they don't count towards the max available slots.
        /// </summary>
        public virtual List<IItem>? OrderedItems 
        { 
            get
            {
                var items = new List<IItem>();
                items.AddRange(Weapons);
                if (Armors != null)
                    items.AddRange(Armors);
                //if (NftItems != null)
                //    items.AddRange(NftItems);
                return items.OrderBy(x => x.SlotNumber).ToList();
            }
        }
    }
}
