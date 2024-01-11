using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public class MonsterLootDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public ItemDto[] Items { get; set; } = new ItemDto[] { };

        [Required]
        public int Dcx { get; set; }
    }
}
