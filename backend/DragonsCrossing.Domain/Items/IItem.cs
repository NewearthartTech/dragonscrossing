using DragonsCrossing.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// Common properties between weapons, armor, and NFT items.
    /// </summary>
    public interface IItem
    {
        int Id { get; set; }

        /// <summary>
        /// Order is important for items in the inventory to allow players to arrange them how they want.
        /// 0 based. So 0 is first slot, and 1 is second slot, etc...
        /// No 2 items should have the same slot order.
        /// Equipped items have a SlotOrder of null.
        /// </summary>
        int? SlotNumber { get; set; }
    }
}
