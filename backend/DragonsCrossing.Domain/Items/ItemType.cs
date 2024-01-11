using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// We need this to differentiate between the items stored in the inventory.
    /// We will have a ReferenceId that can be the id of a skill, shard, or gear.
    /// </summary>
    public enum ItemType
    {
        Skill,
        shard,
        Gear
    }
}
