using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Players
{
    public class PlayerBackpackDto
    {
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The player or wallet
        /// </summary>
        [Required]
        public int PlayerId { get; set; }

        /// <summary>
        /// Secured NFT's retrieved from the blockchain
        /// </summary>
        public List<NftItemDto>? NftItems { get; set; }

        /// <summary>
        /// The Total secured DCX this wallet has. It cannot be negative.
        /// The source of truth for secured Dcx is harmony.
        /// This value is only retreived from harmony.
        /// </summary>
        [Required]
        public decimal SecuredDcx { get; set; } = 0;
    }
}
