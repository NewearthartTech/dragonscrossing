using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Items
{
    public class UpdateInventoryRequestDto
    {
        [Required]
        public int HeroId { get; set; }

        [Required]
        public List<ItemDto> Items { get; set; }
    }
}
