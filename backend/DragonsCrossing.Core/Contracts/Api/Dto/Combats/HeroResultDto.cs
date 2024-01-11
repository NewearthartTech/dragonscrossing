using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public class HeroResultDto
    {
        [Required]
        public HeroDto hero { get; set; }

        [Required]
        public bool isDead { get; set; }

        // To be calculated later
        [Required]
        public int LevelLoss { get; set; } = 1;

        [Required]
        public HeroAttackResultDto attackResult { get; set; } = new HeroAttackResultDto();

        // Future Epic
        public SkillResultDto? SkillResult { get; set; }

        // Future Epic
        public OpportunityResultTypeDto? CombatOpportunityResult { get; set; }

        // Future Epic
        public OpportunityResultTypeDto? CharismaOpportunityResult { get; set; }
    }
}
