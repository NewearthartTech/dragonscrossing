using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Monsters
{
    /// <summary>
    /// The hero attribute this monster affects
    /// </summary>
    /// <remarks>EF doesn't support list of enum, so have to wrap it in this class so the table can get created with a PK</remarks>
    public class MonsterAttribute
    {
        public int Id { get; set; }
        public int MonsterPersonalityId { get; set; }
        public MonsterAttributeType Type { get; set; }
    }
}
