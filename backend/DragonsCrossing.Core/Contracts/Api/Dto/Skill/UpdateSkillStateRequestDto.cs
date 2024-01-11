using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Skill
{
    public class UpdateSkillStateRequestDto
    {
        [Required]
        public int HeroId { get; set; }

        [Required]
        public string SkillId { get; set; }
    }
}
