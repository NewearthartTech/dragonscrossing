using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
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
    /// <summary>
    /// Used by frontend 10/20/2022
    /// This needs to be saved in GameState
    /// </summary>
    public class LevelUpResponseDto
    {
        [Required]
        public int HeroId { get; set; }

        [Required]
        public int DcxToLevel { get; set; }

        [Required]
        public int GainedHpBase { get; set; }

        [Required]
        public DieResultDto GainedHpRoll { get; set; }

        [Required]
        public int GainedStatsBase { get; set; }

        [Required]
        public DieResultDto GainedStatsRoll { get; set; }

        [Required]
        public int GainedSkillPointsBase { get; set; }
    }
}
