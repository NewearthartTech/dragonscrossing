using DragonsCrossing.Core.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.NewCombatLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public class ActionResponseDto: Encounter
    {
        [Required]
        public int round { get; set; }

        [Required]
        public bool isCharismaOpportunityAvailable { get; set; }

        public CharismaOpportunityResultType? charismaOpportunityResultType { get; set; }

        [Required]
        public MonsterResultDto monsterResult { get; set; } = new MonsterResultDto();

        [Required]
        public HeroResultDto heroResult { get; set; } = new HeroResultDto();

        [Required]
        public CombatantType initiative { get; set; }

        // If this is not null then we know that the monster just cast a special ability this round
        // Only for the current round. If a monster did not cast a special ability this round then it should be null
        public LingeringStatusEffects? monsterCastedSpecialAbility { get; set; }

        [Required]
        public LingeringStatusEffects[] heroSkillStatusEffects { get; set; } = new LingeringStatusEffects[] {};

        // When monster cast special ability, we will need to store them here.
        [Required]
        public LingeringStatusEffects[] monsterSpecialAbilityStatusEffects { get; set; } = new LingeringStatusEffects[] {};

        [Required]
        public bool DidHeroFlee { get; set; } = false;

        [Required]
        public bool isSkillUseAvailable { get; set; }

    }

    // Potentially reuse this for monster special ability status effects
    public class LingeringStatusEffects
    {
        // User friendly name
        [Required]
        public string Name { get; set; }

        // Used to know what sound file to play
        [Required]
        public string Slug { get; set; } = "";

        // The description will change after every round to reflect the current status (2 rounds, 1 round, etc.)
        [Required]
        public string Description { get; set; }

        //[Required]
        //public bool IsPositiveForHero { get; set; }
    }
}
