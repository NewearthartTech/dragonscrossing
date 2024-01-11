using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public class SkillResultDto
    {
        public bool? IsDamage { get; set; }

        public bool? IsStatusEffect { get; set; }

        public List<StatusEffectDto>? StatusEffects { get; set; }

        public int? TotalDamage { get; set; }

        public int? BonusDamage { get; set; }

        public int? StatusEffectBonus { get; set; }

        public int? StatusEffectMitigation { get; set; }

        public DieResultDto[] Roll { get; set; }
    }
}
