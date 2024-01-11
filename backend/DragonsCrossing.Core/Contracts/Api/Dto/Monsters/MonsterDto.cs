using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Domain.Monsters;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Monsters
{
    public class Range
    {
        public int lower { get; set; }
        public int upper { get; set; }
    }

    public enum CombatAttribute
    {
        Wisdom,
        Strength, 
        Agility,
        Unknown
    }

    public class AlteredMonsterCalculatedStats
    {
        public string reason { get; set; } = "";

        public int round { get; set; }

        public CalculatedMonsterStats stats { get; set; } = new CalculatedMonsterStats();
    }

    public class MonsterLootCharacteristics
    {
        public int ChancesOfDrop { get; set; }
    }

    /// <summary>
    /// used to denote what stats are effected by level difference
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class EffectedByLevelDifferenceAttribute : System.Attribute{}

    public class CalculatedMonsterStats
    {
        [Required]
        public int ChanceToHit { get; set; }

        [Required]
        public int DifficultyToHit { get; set; }

        [Required]
        public int ArmorMitigation { get; set; }

        [Required]
        public int BonusDamage { get; set; }

        [Required]
        public int ChanceToDodge { get; set; }

        [Required]
        public int CritChance { get; set; }

        [Required]
        public int ParryChance { get; set; }

        [Required]
        public int Charisma { get; set; }

        [Required]
        public int Quickness { get; set; }

        [Required]
        public int ArmorMitigationAmount { get; set; }
    }

    [BsonIgnoreExtraElements]
    public abstract class MonsterCommon
    {

        // human readable name with spaces and all that jazz
        [Required]
        public string Name { get; set; }

        // code friendly monster name 
        //[Unique]
        [Required]
        public string MonsterSlug { get; set; }

        [Required]
        public string Description { get; set; }

        // This is just flavor
        [Required]
        public String[] MonsterItems { get; set; }

        [Required]
        public int AppearChance { get; set; }

        [Required]
        public DieDto[] DieDamage { get; set; } = new DieDto[]{};

        [Required]
        public Zones.DcxTiles LocationTile { get; set; }

        // MonsterSpecialAbility will look like this:
        //         {
        //   "id": 1,
        //   "name": "Whirly Gig",
        //   "canUseSpecialAbilityMoreThanOnce" : true,
        //   "affects": [
        //     {
        //       "affectType": "monster",
        //       "statName": "chanceToDodge",
        //       "friendlyStatName": "dodge",
        //       "affectAmount": 10,
        //       "duration": 2
        //     },
        //     {
        //       "affectType": "hero",
        //       "statName": "parry",
        //       "friendlyStatName": "parry",
        //       "affectAmount": 10,
        //       "duration": 2
        //     },
        //     {
        //       "affectType": "hero",
        //       "statName": "chanceToDodge",
        //       "friendlyStatName": "dodge",
        //       "affectAmount": -10,
        //       "duration": 2
        //     }
        //   ]
        // }
        public MonsterSpecialAbilityDto SpecialAbility { get; set; }

    }

    [BsonIgnoreExtraElements]
    public class MonsterTemplate : MonsterCommon
    {
        [Required]
        public Range MaxHitPoints { get; set; }

        [Required]
        public Range ChanceToHit { get; set; }

        [Required]
        public Range ChanceToDodge { get; set; }

        [Required]
        public Range DifficultyToHit { get; set; }

        [Required]
        public Range CritChance { get; set; }

        [Required]
        public Range ParryChance { get; set; }

        [Required]
        public Range Charisma { get; set; }

        [Required]
        public Range Quickness { get; set; }

        [Required]
        public Range Level { get; set; }

        [Required]
        public Range Power { get; set; }

        /// <summary>
        /// This is the armor mitigation chance 
        /// </summary>
        [Required]
        public Range ArmorMitigation { get; set; } 

        [Required]
        public Range ArmorMitigationAmount { get; set; } = new Range();

        [Required]
        public Range BonusDamage { get; set; }

        [Required]
        public Range ChanceOfUsingSpecialAbility { get; set; }

        /// <summary>
        /// This is a map of template name and chance to drop and anything else needed for rarity
        /// </summary>
        public Dictionary<string, MonsterLootCharacteristics> LootItemsTemplates { get; set; } = new Dictionary<string, MonsterLootCharacteristics>();
    }

    [BsonIgnoreExtraElements]
    public class  MonsterDto : MonsterCommon 
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public int HitPoints { get; set; }

        [EffectedByLevelDifference]
        [Required]
        public int MaxHitPoints { get; set; }

        [EffectedByLevelDifference]
        [Required]
        public int ChanceToHit { get; set; }

        [EffectedByLevelDifference]
        [Required]
        public int DifficultyToHit { get; set; }

        [Required]
        public int Power { get; set; }

        /// <summary>
        /// This is the armor mitigation chance 
        /// </summary>
        [EffectedByLevelDifference]
        [Required]
        public int ArmorMitigation { get; set; }

        [Required]
        public int ArmorMitigationAmount { get; set; }

        [EffectedByLevelDifference]
        [Required]
        public int BonusDamage { get; set; }

        [EffectedByLevelDifference]
        [Required]
        public int ChanceToDodge {get;set;}

        [EffectedByLevelDifference]
        [Required]
        public int CritChance { get; set; }

        [EffectedByLevelDifference]
        [Required]
        public int ParryChance { get; set; }

        [EffectedByLevelDifference]
        [Required]
        public int Charisma { get; set; }

        [Required]
        public int Quickness { get; set; }

        [Required]
        public int Level { get; set; }  

        [Required]
        public int ChanceOfUsingSpecialAbility { get; set; }

        /// <summary>
        /// TODO: Calculated personality (if there is one)
        /// effect found in Table reference Guide - Monster personality
        /// </summary>
        [Required]
        public MonsterPersonalityTypeDto PersonalityType { get; set; }

        /// <summary>
        /// Monster personality desc
        /// </summary>
        [Required]
        public string PersonalityTypeDesc
        {
            get 
            {
                switch (PersonalityType)
                {
                    case MonsterPersonalityTypeDto.Sickly:
                        return "Charisma reduced by 30%";
                        
                    case MonsterPersonalityTypeDto.Inspired:
                        return "Charisma increased by 30%";

                    case MonsterPersonalityTypeDto.Fatigued:
                        return "Chance to hit reduced by 30%";

                    case MonsterPersonalityTypeDto.Brutal:
                        return "Chance to hit increased by 30%";

                    case MonsterPersonalityTypeDto.Lazy:
                        return "Quickness reduced by 30%";

                    case MonsterPersonalityTypeDto.Lean:
                        return "Quickness increased by 30%";

                    case MonsterPersonalityTypeDto.Stupid:
                        return "Chance to dodge reduced by 30%";

                    case MonsterPersonalityTypeDto.Arcane:
                        return "Chance to dodge increased by 30%";

                    case MonsterPersonalityTypeDto.Impotent:
                        return "Chance to crit reduced by 30%";

                    case MonsterPersonalityTypeDto.Reckless:
                        return "Chance to crit increased by 30%";

                    case MonsterPersonalityTypeDto.Deadly:
                        return "Charisma, quickness, chance to hit, chance to dodge, and chance to crit increased by 30%";

                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// Optional int that defines which Monster used special ability in
        /// will be null if Monster never used the special ability
        /// </summary>
        public int? WhichRoundMonsterUsedSpecialAbility {get;set;}

    }
}
