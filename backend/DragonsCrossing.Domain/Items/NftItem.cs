using DragonsCrossing.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// The non-equipable item that can drop from combat
    /// </summary>
    public class NftItem : ChangeTracking, IItem
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public NftItemTemplate NftItemTemplate { get; set; }
        
        /// <summary>
        /// True: Player still owns this item but it can still be lost if hero dies
        /// False: When nft item is burned and saved to the users wallet
        /// </summary>
        public bool IsActive { get; set; } = true;        

        public int? SlotNumber { get; set; }

        /// <summary>
        /// The amount of gold this item will sell for at the shop.
        /// Can NFT items be sold at the shop before a hero camps? Or do they always go to the hero's wallet?
        /// Until I hear otherwise, I'm going to assume nft items can't be sold in game - only outside the game.
        /// </summary>
        //public int SellPriceGold { get; set; }
        //public decimal SellPriceDcx { get; set; }
    }
}
