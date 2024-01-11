using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// Contains a copy of the secured DCX and NFT’s stored in the players blockchain wallet. It is used for reference only since the blockchain owns this info. 
    /// This is just a local in-memory cache of what is stored on the blockchain.
    /// For a better user experience, the backpack will need to be updated every once in a while (when the player clicks a refresh button or finishes combat, etc…) by pulling the info from the blockchain. 
    /// This data doesn’t need to be stored in the db - only in memory. 
    /// It will need to be refreshed when the player wants to open a skill book or shard to make sure the player has enough secured DCX. 
    /// Nothing in the backpack is associated with a single hero, only to the player/wallet.
    /// Gear cannot be stored in this stash, only NFT Items such as hero shards and skill books.
    /// </summary>
    public class PlayerBackpack
    {
        public int Id { get; set; }
        
        /// <summary>
        /// The player or wallet
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// Secured NFT's retrieved from the blockchain
        /// </summary>
        public List<NftItem>? NftItems { get; set; }

        /// <summary>
        /// The Total secured DCX this wallet has. It cannot be negative.
        /// The source of truth for secured Dcx is harmony.
        /// This value is only retreived from harmony.
        /// </summary>
        public decimal SecuredDcx { get; set; } = 0;
    }
}
