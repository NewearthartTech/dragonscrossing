using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Heroes
{
    public class EquipItemRequestDto
    {
        [Required]
        public string ItemId { get; set; } = String.Empty;

    }

    public class UnequipItemRequestDto : EquipItemRequestDto
    {
        // When an item is dragged from equippedItems to the inventory place the item at this index in the inventory
        [Required]
        public int TargetInventoryIndex { get; set; }

    }
}
