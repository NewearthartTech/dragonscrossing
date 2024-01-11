using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;
using DragonsCrossing.Core.Helper;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Heroes
{
    // This will never be saved in the DB, these are calculated always from the hero and may or may not include affects from Special Abilities and Skills
    // Eric is leaning towards no
    public class CalculatedCharacterStats 
    {
        // Base Strength + item strength 
        [Required]
        public int strength { get; set; }

        [Required]
        public int agility { get; set; }

        [Required]
        public int wisdom { get; set; }

        [Required]
        public int quickness { get; set; }

        [Required]
        public int charisma { get; set; }

        [Required]
        public int chanceToCrit { get; set; }

        [Required]
        public int chanceToParry { get; set; }

        [Required]
        public int chanceToHit { get; set; }

        [Required]
        public int difficultyToHit { get; set; }

        [Required]
        public int chanceToDodge { get; set; }

        /// <summary>
        /// This is the armor mitigation chance
        /// </summary>
        [Required]
        public int armorMitigation { get; set; }

        [Required]
        public int armorMitigationAmount { get; set; }

        // We have two different bonus damages 
        [Required]
        public int itemBonusDamage { get; set; }

        // Use calculated strength(Warrior)/agility(Ranger)/wisdom(Mage) and refer to schedule
        [Required]
        public int statsBonusDamage { get; set; }

        // This is just for Branden. Will come back later for this
        [Required]
        public Range damageRange { get; set; } = new Range();
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender { Male, Female}

    public class HeroBaseStats
    {
        [Required]
        public int strength { get; set; }
        [Required]
        public int agility { get; set; }
        [Required]
        public int wisdom { get; set; }
        [Required]
        public int quickness { get; set; }
        [Required]
        public int charisma { get; set; }
    }

    [BsonIgnoreExtraElements]
    [MongoCollection("perpetualHeros")]
    public class HeroDto: HeroBaseStats
    {
        // this needs to be an int bc this is the same as the nft token ID (which has to be int)
        [Required]
        [BsonId]
        public int id { get; set; }

        /// <summary>
        /// The last knows player that owns this 
        /// </summary>
        [Required]
        public string playerId { get; set; } = String.Empty;

        /// <summary>
        /// What Season is the Hero signed up to
        /// 0 means it is not signed up to any
        /// </summary>
        [Required]
        public int seasonId { get; set; }

        // dynamically, randomly generated from template files (male or female)
        [Required]
        public string name { get; set; } = String.Empty;

        // dynamically, randomly generated at mint from an enum
        [Required]
        public CharacterClassDto heroClass { get; set; }

        // dynamically, randomly generated at mint from an enum
        [Required]
        public Gender gender { get; set; }


        /// <summary>
        /// RemainingHitPointsPercentage is a number from 0 - 100.0
        /// we reset this daily
        /// </summary>
        [Required]
        public double remainingHitPointsPercentage { get; set; }

        [Required]
        [BsonIgnore]
        public double totalHitPoints 
        { 
            get 
            { 
                return baseHitPoints + 
                    (int)equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.HitPoints) ? i.affectedAttributes[AffectedHeroStatTypeDto.HitPoints] : 0); 
            } 
            set { } 
        }

        [Required]
        [BsonIgnore]
        public double remainingHitPoints
        { 
            get 
            {
                return Math.Round(remainingHitPointsPercentage / 100 * totalHitPoints, 1);
            }
            set 
            { } 
        }

        /// <summary>
        /// this is set at 12 at mint, when you level up may be this can be increased
        /// </summary>
        [Required]
        public int baseSkillPoints { get; set; } = 12;

        /// <summary>
        /// skill points that have been used up so far
        /// </summary>
        [Required]
        public int usedUpSkillPoints { get; set; }

        /// <summary>
        /// Did the hero pass the module?
        /// </summary>
        [Required]
        public bool isAscended { get; set; } = false;

        [Required]
        public HeroRarityDto rarity { get; set; } = HeroRarityDto.Common;

        /// <summary>
        /// This is how many times the hero has passed the module
        /// </summary>
        [Required]
        public int prestigeLevel { get; set; }

        /// <summary>
        /// This is the level of the HERO. As the hero beats monsters, the hero goes to higher levels
        /// related to "Experience" Tab from Table reference Guide (https://docs.google.com/spreadsheets/d/1bdaWQnmdI6Hyooa9ELxl8uTd0n-OC159L5wwc0VO2lg/edit#gid=735024991)
        /// We need a State machine to handle leve questions.
        /// </summary>
        [Required]
        public int level { get; set; } = 1;

        [Required]
        public int experiencePoints { get; set; } = 0;

        #region Quest control
        /// <summary>
        /// aka, stamina. The total quests available per day to do. Usually is 15.
        /// This is a HERO prop, that doesn't change during game play, unless the Hero is really leveling up or something
        /// </summary>
        [Required]
        public int maxDailyQuests { get; set; } = 15;

        /// <summary>
        /// dailyQuestsUsed is the real quest use. We DONOT manipulate this expcet when ACTUALLY
        /// consuming quests. At any point of time, if we look at this value, it contains the REAL quets consumed at that time
        /// </summary>
        [Required]
        public int dailyQuestsUsed { get; set; } = 0;

        /// <summary>
        /// this prop is used to keep track of Temporary increases or decreases in MaxDailyQuests
        /// caused by skill, spells etc.
        /// </summary>
        [Required]
        public int extraDailyQuestGiven { get; set; } = 0;

        /// <summary>
        /// setting this only changes extraDailyQuestGiven
        /// getting it returns calculated values
        /// </summary>
        [Required]
        [BsonIgnore]
        public int remainingQuests {
            get { return maxDailyQuests + extraDailyQuestGiven - dailyQuestsUsed; }
            set {

                //According to Eric remaining quests should never be more then maxDailyQuests;
                if(value> maxDailyQuests)
                {
                    throw new Exception($"trying to set remainingQuests to {value} which is more then maxDailyQuests {maxDailyQuests}");
                }

                extraDailyQuestGiven = value - maxDailyQuests + dailyQuestsUsed;
            }
        }

        /// <summary>
        /// This function is used when we go through camping or when hero dies and we need to make their available quest count to become 0. When this is being called, all quests lost during camping or death penalty will be included into the totalQuestsUsed in the leader board. 
        /// </summary>
        public void AvailableQuestSetToZero()
        {        
            dailyQuestsUsed = (maxDailyQuests + extraDailyQuestGiven);
            extraDailyQuestGiven = 0;
        }

        #endregion

        //dynamically generated, depending on class
        [Required]
        public LearnedHeroSkill[] skills { get; set; } = new LearnedHeroSkill[]{};

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        public DbOnlyHeroProps dbHeroProps { get; set; } = new DbOnlyHeroProps();

        [Required]
        public ZoneBackgroundTypeDto zoneBackgroundType { get; set; }

        /// <summary>
        /// optional special image for the hero
        /// </summary>
        public string? image { get; set; }

        /// <summary>
        /// optional special background for the hero
        /// </summary>
        public string? backgroundImage { get; set; }

        [Required]
        public UnsecurdDcx unsecuredDCXValue { get; set; } = new UnsecurdDcx();

        /// <summary>
        /// We are not suer what this does
        /// is this used to define a Genesis Hero. A Genesis Hero can find other Heros,
        /// They find stones that can be used to summon other heros.
        /// </summary>
        [Required]
        public int generation { get; set; } = 0;

        /// <summary>        
        /// this is base chance to hit
        /// aka: CTH
        /// These are percentages represented as whole integers (2000 = 20%)
        /// </summary>
        [Required]
        public int baseChanceToHit { get; set; } = 0;

        [Required]
        public int baseHitPoints { get; set; }

        /// <summary>
        /// If true, it allows the player to teleport from wherever they are in the game, into town.
        /// This should probably be a controller method, not a property here
        /// </summary>
        [Required]
        public bool isHearthstoneAvailable { get; set; } = true;

        [Required]
        public int maxExperiencePoints { get; set; } = 7;


        /// <summary>
        /// This changes based on if player purchased more or something.
        /// </summary>
        [Required]
        public int learnedSkillCapacity { get; set; } = 6;

        /// <summary>
        /// Hero base difficultToHit is 2000(20%)
        /// </summary>
        [Required]
        public int difficultyToHit { get; set; } = 2500;

        /// <summary>
        /// This is unequipped items
        /// </summary>
        [Required]
        public ItemDto[] inventory { get; set; } = new ItemDto[] { };

        public static int TotalInventorySlots = 60;

        [Required]
        public ItemDto[] equippedItems { get; set; } = new ItemDto[] { };

        [Required]
        [BsonIgnore]
        public CalculatedCharacterStats calculatedStats { get { return this.GenerateCalculatedCharacterStats(null); } set { } }

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, LevelUpProps> heroLevelUpProps { get; set; } = new Dictionary<int, LevelUpProps>();


        /// <summary>
        /// The last time Daily quests was reset
        /// Once this is set we keep reseting every 24 hours
        /// </summary>
        [Required]
        public DateTime? lastDailyResetAt { get; set; } = DateTime.MinValue;

        [BsonIgnore]
        public TimeSpan timeTillNextReset
        {
            get { return (null == lastDailyResetAt || isResetNeeded()) ? TimeSpan.Zero : (lastDailyResetAt.Value.AddHours(23) - DateTime.Now); }
            set { }
        }

        public bool isResetNeeded()
        {
            return lastDailyResetAt.HasValue ? (DateTime.Now > lastDailyResetAt.Value.AddHours(23)) : true;
        }

        [Required]
        public decimal dcxRewards { get; set; }


        public LoanedHero? isLoanedHero { get; set; }

        /// <summary>
        /// Ids Over this value belongs to DFK
        /// </summary>
        static public readonly int _maxArbHeroIdVal = 100000;

        static public int LOANERHEROID = 999999991;

        /*
        999999991
        1 000 000 000
        2 147 483 647
        */


        public static bool isDefaultChainFromId(int tokenId)
        {
            return LOANERHEROID == tokenId || tokenId <= _maxArbHeroIdVal;
        }

        [BsonIgnore]
        [Required]
        public bool isDefaultChain {
            get { return isDefaultChainFromId(id); }
            set { }
        }

        [MongoIndex]
        public static void CreateIndexes(IMongoCollection<HeroDto> collection)
        {
            //disallow two redemptions with the same redeem count
            collection.Indexes.CreateOne(
                new CreateIndexModel<HeroDto>(
                new IndexKeysDefinitionBuilder<HeroDto>()
                    .Ascending(f => f.isLoanedHero!.loanedToUserId)
                    , new CreateIndexOptions<HeroDto>
                    {
                        PartialFilterExpression = Builders<HeroDto>.Filter.Exists(o => o.isLoanedHero)
                    }
                ));
        }

    }

    public enum LoanerHeroType {Demo,DFK,ClaimDFK }

    public class LoanedHero
    {
        public string loanedToUserId { get; set; } = string.Empty;
        public LoanerHeroType loanerType { get; set; } = LoanerHeroType.Demo;
    }

    public class UnsecurdDcx
    {
        [Required]
        public decimal amount { get; set; }

        /// <summary>
        /// The order that is used to wtithdraw, used to ensure we cannot double withdraw
        /// </summary>
        public string? withdrawlOrderId { get; set; }
    }

    public class LevelUpProps : HeroBaseStats
    {
        // The property names here need to match the prop names in HeroDto so that when we use reflection to update hero stats, it would succeed

        public int baseHitPoints { get; set; }
        public int baseSkillPoints { get; set; }
        public int maxDailyQuests { get; set; }
    }


    public class AlteredCalculatedStats
    {
        public string reason { get; set; } = "";

        public int round { get; set; }

        public CalculatedCharacterStats stats { get; set; } = new CalculatedCharacterStats();
    }

    public class HeroMintingAttributes
    {
        [Required]
        public Range strength { get; set; } = new Range{lower = 4, upper = 10};

        [Required] 
        public Range agility { get; set; } = new Range{ lower = 4, upper = 10};

        [Required]
        public Range wisdom { get; set; } = new Range{ lower = 4, upper = 10};

        [Required]
        public string[] equippedItemsTemplates { get; set; } = new string[] { };

        [Required]
        public string[] learnedSkillTemplates { get; set; } = new string[] { };
    }

    public class HeroTemplate
    {
        // Wen to finish up hero minting attributes
        public Dictionary<CharacterClassDto, HeroMintingAttributes> HeroMintingAttributes { get; set; }
                    = new Dictionary<CharacterClassDto, HeroMintingAttributes>
                    {
                        
                        { CharacterClassDto.Mage, new Heroes.HeroMintingAttributes{
                            wisdom = new Range{lower = 10, upper = 16},

                            equippedItemsTemplates = new string[]
                            {
                                "item_wand-of-force.json",
                                "item_hand-me-down-leathers.json",
                                "item_ill-fitted-sandals.json",
                                "item_unironically-hideous-cap.json"
                            },
                            learnedSkillTemplates = new string[]
                            {
                                "skill-firebolt.json",
                                "skill-ray-of-frost.json",
                            },

                        } },
                        { CharacterClassDto.Ranger, new Heroes.HeroMintingAttributes{
                            agility = new Range{lower = 10, upper = 16},

                            equippedItemsTemplates = new string[]
                            {
                                "item_simple-shortsword.json",
                                "item_hand-me-down-leathers.json",
                                "item_ill-fitted-sandals.json",
                                "item_unironically-hideous-cap.json"
                            },
                            learnedSkillTemplates = new string[]
                            {
                                "skill-barbed-projectile.json",
                                "skill-herbal-knowledge.json"   
                            },
                        } },
                        { CharacterClassDto.Warrior, new Heroes.HeroMintingAttributes{
                            strength = new Range{lower = 10, upper = 16},

                            equippedItemsTemplates = new string[]
                            {
                                "item_runed-axe.json",
                                "item_hand-me-down-leathers.json",
                                "item_ill-fitted-sandals.json",
                                "item_unironically-hideous-cap.json"
                            },
                            learnedSkillTemplates = new string[]
                            {
                                "skill-overhand-chop.json",
                                "skill-grapple.json"
                            },
                        } }
                        
                    };  

        [Required]
        public Range baseHitPoints { get; set; } = new Range{ lower = 75, upper = 75};

        [Required]
        public Range quickness { get; set; }  = new Range{ lower = 4, upper = 10};

        [Required]
        public Range charisma { get; set; }  = new Range{ lower = 4, upper = 10};

    }
}

/// <summary>
/// hero props saved in db for not needed in the front end
/// </summary>
public class DbOnlyHeroProps
{
    /// <summary>
    /// the seasonIds for which the skills have been transfered over to the perpetual Hero
    /// </summary>
    public int[] skillsTransferedFromSeasons { get; set; } = new int[] { };
}

