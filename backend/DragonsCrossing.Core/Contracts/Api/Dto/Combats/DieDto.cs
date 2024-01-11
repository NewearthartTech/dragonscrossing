using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public enum DiceRollReason
    {
        HeroGoesFirst, MonsterGoesFirst,

        DidHeroHit, DidMonsterHit,

        HeroParryDmgFromMonster, MonsterParryDmgFromHero,

        DidHeroCrit, DidMonsterCrit,

        EquipmentDamage, MonsterDieDamage,

        SpecialAbility, SkillDamageRightAway, SkillLingeringDamage, SkillHealing,

        ChooseMonster,

        MonsterUsedSpecialAbility,

        HeroChanceToDodge, MonsterChanceToDodge,

        HeroFleeRoll,

        General,

        levelUpHitPoints, levelUpStats,

        chanceEncounter,

        

        RollForCharismaOpportunity,

        HeroCharismaRoll, MonsterCharismaRoll,

        MonsterPersonality,

        MonsterArmorMitigationRoll, HeroArmorMitigationRoll, HeroDieDamage,

        MintHeroGender, MintHeroName, MintHeroClass, ItemRarityRoll, HeroRarityRoll, ZoneBackgroundRoll,

        MonsterFleeRoll, ItemLootRoll,

        GenerateRandomChanceEncounter, ChanceEncounterGoodRoll,

        NCEAfterCombat, BonusXPAfterCocytus2Tile,

        SkillLootRoll, ShardLootRoll, MandrakeLootRoll
    }


    // This returns the result of a die roll
    public class DieResultDto : DieDto
    {
        [Required]
        public int Result { get; set; }

        /// <summary>
        /// Why we rolled the die
        /// </summary>
        public DiceRollReason RollFor { get; set; } 

        /// <summary>
        /// Sometimes we have damage rolls like 1D4 - 1, the modifier in this case would be the -1
        /// </summary>
        public int Modifier { get; set; }

        /// <summary>
        /// Final result added this die roll result plus any additional modifier (could be positive or negative number). This is for front end
        /// </summary>
        public int FinalResult { get { return this.Result + this.Modifier; } }
    }

    public class DieDto
    {
        [Required]
        public int Sides { get; set; }
    }
}
