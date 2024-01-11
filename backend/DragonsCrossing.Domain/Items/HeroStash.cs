using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// This stash is only accessible in town and to a specific hero. It stores the hero's items permanently.
    /// TODO: This isn't need at the moment though maybe later on. I suggested we need a hero stash or the shared stash
    /// would get filled up quickly if someone had multiple heroes.
    /// </summary>
    public class HeroStash : InventoryBase
    {
        public int HeroId { get; set; }
    }
}
