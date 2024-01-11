//using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// There are only 2 nft items: shard/hero egg, skill book.
    /// These items are always unsecure.
    /// Once a player camps, the nft items are all removed from the db once the blockchain transaction completes.
    /// </summary>
    public class NftItemTemplate : ChangeTracking
    {
        public int Id { get; set; }        
        public string Name { get; set; }
        public string Description { get; set; }
        public NftItemType Type { get; set; }
        public string ImageBaseUrl { get; set; }
        
        /// <summary>
        /// The cost in gold to open this nft item
        /// aka: UnlockPriceGold
        /// </summary>
        public int GoldCostToOpen { get; set; }

        /// <summary>
        /// The cost in DCX to open this nft item
        /// aka: UnlockPriceDcx
        /// </summary>
        public decimal DcxCostToOpen { get; set; }
    }
}
