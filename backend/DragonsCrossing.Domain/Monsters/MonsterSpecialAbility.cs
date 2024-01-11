using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Monsters
{
    public class MonsterSpecialAbility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int MonsterTemplateId { get; set; }

        /// <summary>
        /// Does this ability take up a turn?
        /// </summary>
        public bool UsesTurn { get; set; }
        public bool IsGuaranteedDamage { get; set; }
        public bool CanCastAgain { get; set; }

        /// <summary>
        /// The different effects this ability has. These are effects
        /// that CAN be cast during combat (tied to a MonsterTemplateOld not monster).
        /// </summary>
        public List<MonsterSpecialAbilityEffect> Effects { get; set; }
    }
}
