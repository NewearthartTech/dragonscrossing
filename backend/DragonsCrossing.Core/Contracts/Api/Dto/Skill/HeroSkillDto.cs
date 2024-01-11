using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using MongoDB.Bson.Serialization.Attributes;
using DragonsCrossing.NewCombatLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Skill
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(Required = true, RootClass = true)]
    [BsonKnownTypes(typeof(UnidentifiedHeroSkill), typeof(UnlearnedHeroSkill), typeof(LearnedHeroSkill))]
    [System.Text.Json.Serialization.JsonConverter(typeof(PolymorphicConverter<HeroSkillDto>))]
    public class HeroSkillDto : PolymorphicBase<HeroSkillDto>
    {
        [Required]
        public string id { get; set; } = String.Empty;

        /// <summary>
        /// used to update skill props when the version changes
        /// </summary>
        [Required]
        public int version { get; set; }
    }


    public class UnidentifiedHeroSkill : HeroSkillDto
    {
        [Required]
        public decimal dcxToIdentify { get; set; }

        // This is used to identify which skill will be discovered
        //do not send this over wite before the skill becomes "learned"
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string slug { get; set; } = String.Empty;
    }

    public class LearnedHeroSkill : HeroSkillDto
    {
        // This is the actual method call in the game engine class
        [Required]
        public string methodName { get; set; } = String.Empty; //Ex: apply_skill_Barbed_Projectile

        [Required]
        public string slug { get; set; } = String.Empty;// Ex: Barbed-Projectile

        [Required]
        public string name { get; set; } = String.Empty;

        [Required]
        public string description { get; set; } = String.Empty;

        [Required]
        public CharacterClassDto skillClass { get; set; }

        /// <summary>
        /// static value, comes from Template
        /// </summary>
        [Required]
        public int requiredSkillPoints { get; set; }

        /// <summary>
        /// This is determined by the user in adventuring guild using UI
        /// will only change from there
        /// </summary>
        [Required]
        public int allocatedSkillPoints { get; set; }

        /// <summary>
        /// This contains an array of how may uses we get
        /// as the skill is used it is set to TRUE
        /// Lets say we allocte 10 points to fireball, and requiredSkillPoints = 5
        /// this array will be [false, false]
        /// As we use a fireball this will become [true,false]
        /// User can take away a USE that is still set to false, and reclaim the skillPoints available
        /// On reset we simply mark all the elements to false
        ///
        /// Once a skill is used, they have to wait till reset to reallocate that use
        /// </summary>
        [Required]
        public SkillUseInstance[] skillUseInstance { get; set; } = new SkillUseInstance[] { };



        //TODO: Calculate this
        [Required]
        [BsonIgnore]
        public SkillsCalculatedStats skillsCalculatedStats { get; set; } = new SkillsCalculatedStats();

        [Required]
        public int levelRequirement { get; set; }

    }

    public class SkillUseInstance
    {
        public string id { get; set; } = Guid.NewGuid().ToString();

        public bool alreadyUsed { get; set; }

    }

    public class UnlearnedHeroSkill: LearnedHeroSkill
    {
        [Required]
        public decimal dcxToLearn { get; set; }
    }

    public class SkillsCalculatedStats
    {
        [Required]
        public int TotalUses { get; set; }

        [Required]
        public int CurrentUsesRemaining { get; set; }
    }
}
