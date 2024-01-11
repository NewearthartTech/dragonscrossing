using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// The temporary storage per hero. If the hero dies all items in this inventory are lost including equipped weapons and armor.
    /// Unsecured DCX and NFT's can also be stored here.
    /// </summary>
    public class HeroInventory : InventoryBase
    {
        public int HeroId { get; set; }

        public Hero Hero { get; set; }

        public List<Armor> EquippedArmors
        {
            get
            {
                return Armors.Where(armor => armor.IsEquipped).ToList();
            }
        }

        /// <summary>
        /// A read only list of weapons that are equipped.
        /// </summary>
        public List<Weapon> EquippedWeapons
        {
            get
            {
                return Weapons.Where(weapon => weapon.IsEquipped).ToList();
            }
        }

        /// <summary>
        /// This is always unsecured. Once it becomes secured (by camping) it's removed from the game/db and 
        /// the blockchain becomes the owner of it.
        /// As far as I know, NFT items can't be stored in the shared stash or Hero stash.
        /// </summary>
        public List<NftItem>? NftItems { get; set; }

        public override List<IItem>? OrderedItems
        {
            get
            {
                var items = new List<IItem>();
                items.AddRange(Weapons);
                if (Armors != null)
                    items.AddRange(Armors);
                if (NftItems != null)
                    items.AddRange(NftItems);
                return items.OrderBy(x => x.SlotNumber).ToList();
            }
        }
    }
}
