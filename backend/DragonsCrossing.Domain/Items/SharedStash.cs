using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// Stores weapons, armor and unsecured nft items in long-term storage.
    /// Once a player camps, the nft items are all removed from the db once the blockchain transaction completes.
    /// </summary>
    public class SharedStash : InventoryBase
    {
        public int PlayerId { get; set; }
    }
}
