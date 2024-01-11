using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api
{
    public interface IInventoriesService
    {
        Task<bool> EquipWeapon(int heroId, int weaponId);
        Task<bool> EquipArmor(int heroId, int armorId);
        Task SaveEquippedGear(int heroId, List<ItemDto> gear);
    }
}
