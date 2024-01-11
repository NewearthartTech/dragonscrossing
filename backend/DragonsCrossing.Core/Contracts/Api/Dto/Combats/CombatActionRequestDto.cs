using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public class CombatActionRequestDto
    {
        [Required]
        public int HeroId { get; set; }

        [Required]
        public string Slug { get; set; }

        [Required]
        public CombatActionDto ActionType { get; set; }

        public HeroSkillDto? Skill { get; set; }
    }
}
