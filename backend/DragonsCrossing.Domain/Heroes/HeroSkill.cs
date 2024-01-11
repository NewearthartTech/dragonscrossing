using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Heroes
{
    /// <summary>
    /// These are the skills the hero has learned, unlearned, or unidentified.
    /// Hero skills never take up a hero's turn. Skills are used first before attacking
    /// </summary>
    public class HeroSkill
    {
        public int Id { get; set; }
        public HeroSkillTemplate SkillTemplate { get; set; }
        public HeroSkillStatus Status { get; set; }
    }
}
