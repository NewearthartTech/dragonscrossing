using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Monsters
{
    public class MonsterSpecialAbilityCasted
    {
        public int Id { get; set; }
        public int MonsterId { get; set; }        
        public MonsterSpecialAbilityEffect Effect { get; set; }
        
        /// <summary>
        /// The special ability is active up to but not including the specified round.
        /// ie. The round the ability is no longer active on.
        /// This is exclusive.
        /// </summary>
        public int ActiveUntilRound { get; set; }
    }
}
