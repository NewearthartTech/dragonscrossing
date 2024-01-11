using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Heroes
{
    public class AllocateStatsRequestDto
    {
        [Required]
        public int HeroId { get; set; }

        [Required]
        public int StrengthIncrease { get; set; }

        [Required]
        public int WisdomIncrease { get; set; }

        [Required]
        public int AgilityIncrease { get; set; }

        [Required]
        public int QuicknessIncrease { get; set; }

        [Required]
        public int CharismaIncrease { get; set; }
    }
}
