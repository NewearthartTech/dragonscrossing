using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Heroes
{
    public class HeroStatModifierResultDtoDto
    {
        [Required]
        public AffectedHeroStatTypeDto HeroStatType { get; set; }

        [Required]
        public int EffectAmount { get; set; }

    }
}
