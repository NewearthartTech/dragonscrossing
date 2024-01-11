using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Items
{
    public class DisplayItemDetailsDto
    {
        [Required]
        public ItemDto Item { get; set; }

        [Required]
        public bool Open { get; set; }
    }
}
