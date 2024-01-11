using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Domain.Heroes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using DragonsCrossing.NewCombatLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;
namespace DragonsCrossing.Core.Contracts.Api.Dto.Items
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(SkillItem))]
    [System.Text.Json.Serialization.JsonConverter(typeof(PolymorphicConverter<ItemDto>))]

    public class ItemDto : ItemCommon
    {
        /// <summary>
        /// This is not a slug. This is the GUID
        /// </summary>
        [Required]
        public string id { get; set; } = String.Empty;

        /// <summary>
        /// This is the nftToken Id for NFTized Items
        /// </summary>
        public int nftTokenId { get; set; }

        // This is the UI index
        public int? itemIndex { get; set; }

        // Will the item template provide Rarity? Or this is decided later?
        public ItemRarityDto rarity { get; set; } = ItemRarityDto.Unknown;

        public int? bonusDamage { get; set; }

        // Example [Strength, 4]
        // This dictionary contains the stats that the item affects directly on the hero when equipped
        // The only time this property is used is when an item is equipped or unequipped
        // Q's are using this property
        [Required]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<AffectedHeroStatTypeDto, int> affectedAttributes { get; set; } = new Dictionary<AffectedHeroStatTypeDto, int>();

        public ItemDto()
        {

            id = Guid.NewGuid().ToString();
        }

        public bool isNftAble()
        {
            return (new[]{
                        ItemSlotTypeDto.shard,
                        ItemSlotTypeDto.unidentifiedSkill,
                        ItemSlotTypeDto.unlearnedSkill,
                        ItemSlotTypeDto.nftAction
            }).Contains(this.slot);
        }

        /// <summary>
        /// Ids Over this value belongs to DFK
        /// </summary>
        static public readonly int _maxArbHeroIdVal = 100000;


        [BsonIgnore]
        [Required]
        public bool isDefaultChain
        {
            get { return nftTokenId <= _maxArbHeroIdVal; }
            set { }
        }

    }

    public class SkillItem : ItemDto
    {
        public HeroSkillDto skill { get; set; } = new UnidentifiedHeroSkill();
    }

    public class SkillItemTemplate: ItemTemplate
    {
        public UnidentifiedHeroSkill skill { get; set; } = new UnidentifiedHeroSkill();
    }

    public class ItemTemplate : ItemCommon
    {
        [Required]
        public Range bonusDamage { get; set; } = new Range();

        // This should work. This dictionary would have a list of AffectedAttributes that is a type of AffectedHeroStatTypeDto
        // and the values are in the form of Range
        [Required]
        public Dictionary<AffectedHeroStatTypeDto, Range> affectedAttributes { get; set; } = new Dictionary<AffectedHeroStatTypeDto, Range>();

        public DcxZones? blackSmithZone { get; set; }

    }

    [BsonIgnoreExtraElements]
    public abstract class ItemCommon:PolymorphicBase<ItemCommon>
    { 
        // This slug is the coding friendly name - unique
        [Required]
        public string slug { get; set; } = "";

        // Human readable name - unique
        [Required]
        public string name { get; set; } = String.Empty;

        // This is a slug that will be mapped to the item sound on the frontend at this lcoation: DragonsCrossingWeb\public\audio\sound-effects\item
        [Required]
        public string itemDropSound { get; set; } = String.Empty;

        [Required]
        public ItemSlotTypeDto slot { get; set; }

        [Required]
        public DieDto[] dieDamage { get; set; } = new DieDto[]{};

        [Required]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<HeroStatType, int> heroStatComplianceDictionary { get; set; } = new Dictionary<HeroStatType, int>();

        [Required]
        public CharacterClassDto[] allowedHeroClassList { get; set; } = new CharacterClassDto[]{};

        /// <summary>
        /// Front-end logic: If this imageSlug is null or empty, then use the regular slug for image path
        /// </summary>
        public string? imageSlug { get; set; }

        [Required]
        public int levelRequirement { get; set; }
    }
}
