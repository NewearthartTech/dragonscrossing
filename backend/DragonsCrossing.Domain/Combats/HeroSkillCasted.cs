using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Combats
{
    /// <summary>
    /// This class is only used during combat. It's main purpose is to keep track
    /// of which turn the effects of the skill becomes inactive.
    /// </summary>
    public class HeroSkillCasted
    {
        public int Id { get; set; }
        public HeroSkill SkillCasted { get; set; }

        /// <summary>
        /// The skill is active up to but not including the specified turn.
        /// ie. The turn the skill is no longer active on.
        /// This is exclusive.
        /// </summary>
        public int ActiveUntilTurn { get; set; }
    }
}
