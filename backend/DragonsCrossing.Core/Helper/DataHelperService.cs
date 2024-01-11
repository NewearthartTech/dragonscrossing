using System.Reflection;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using Newtonsoft.Json;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;
using static DragonsCrossing.Core.Helper.DataHelperService;
using static System.Net.Mime.MediaTypeNames;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using System.Collections;

namespace DragonsCrossing.Core.Helper;

public interface IDataHelperService
{
    ItemDto CreateItemFromTemplate(string itemTemplateName, DcxZones currentZone, HeroRarityDto? heroRarity = null );
    void FixRarity(ItemDto item, DcxZones currentZone);
    void FixRarityByHeroRarity(ItemDto item, HeroRarityDto heroRarity, DcxZones currentZone);
    void UpdateItemProps(ItemDto item, DcxZones currentZone);
    ItemAndDropChance[] GetAllMonsterLoots(DbGameState gameState, CombatEncounter combat);
    void ApplyLevelScaling(CombatEncounter combat, DbGameState gameState);
    string[] loadAllSkillTemplates();

}

public class DataHelperService : IDataHelperService
{
    readonly IDiceService _dice;
    readonly ILogger _logger;
    public DataHelperService(IDiceService dice, ILogger<DataHelperService> logger)
    {
        _dice = dice;
        _logger = logger;
    }

    /// <summary>
    ///  TODO: Write down the logic here with Eric. Refer to the excel sheet with the calculation.
    ///  
    /// </summary>
    /// <param name="item"></param>
    /// <param name="heroRarity"></param>
    /// <param name="currentZone"></param>
    public void FixRarity(ItemDto item, DcxZones currentZone)
    {
        //clone the ranges as we don't want to change the global static
        var tmpRanges = new Dictionary<ItemRarityDto, Range>(ItemRaritySchedules.itemRarityChance);

        var diceRoll = _dice.Roll(100, Contracts.Api.Dto.Combats.DiceRollReason.ItemRarityRoll);

        item.rarity = tmpRanges.Where(r => r.Value.upper >= diceRoll && r.Value.lower <= diceRoll).First().Key;

        _logger.LogDebug($"Item rarity for {item.name} - {item.id} is set to {item.rarity} because die roll was {diceRoll}");

        UpdateItemProps(item, currentZone);

    }

    /// <summary>
    ///  TODO: Write down the logic here with Eric. Refer to the excel sheet with the calculation.
    ///  
    /// </summary>
    /// <param name="item"></param>
    /// <param name="heroRarity"></param>
    /// <param name="currentZone"></param>
    public void FixRarityByHeroRarity(ItemDto item, HeroRarityDto heroRarity, DcxZones currentZone)
    {
        //this will contain 1 for Uncommon, 2 for Rare and so on
        var heroBoostPoint = ItemRaritySchedules.heroRarityBoostPoints[heroRarity];

        //clone the ranges as we don't want to change the global static
        var tmpRanges = new Dictionary<ItemRarityDto, Range>(ItemRaritySchedules.itemRarityChance);

        foreach (var range in tmpRanges)
        {
            if (range.Key != ItemRarityDto.Common)
            {
                range.Value.lower -= heroBoostPoint;
                _logger.LogDebug($"Item rarity is not Common, set the item rarity chance lower boundary to {range.Value.lower} which is lowered by heroBoostPoint:{heroBoostPoint}.");
            }

            range.Value.upper -= heroBoostPoint;
            _logger.LogDebug($"Item rarity chance upper boundary is set to {range.Value.upper} which is lowered by heroBoostPoint:{heroBoostPoint}.");
        }

        var diceSides = 100 - heroBoostPoint;

        var diceRoll = _dice.Roll(diceSides, Contracts.Api.Dto.Combats.DiceRollReason.ItemRarityRoll);

        item.rarity = tmpRanges.Where(r => r.Value.upper >= diceRoll && r.Value.lower <= diceRoll).First().Key;

        _logger.LogDebug($"The number of die's sides for item rarity determination is 100 - heroBoostPoint, which equals to {diceSides}. Die roll result is {diceRoll}. Item rarity chance is determined by: tmpRanges.Where(r => r.Value.upper >= diceRoll).First().Key. Item rarity for item id {item.id} is set to {item.rarity}");

        UpdateItemProps(item, currentZone);

    }

    public void UpdateItemProps(ItemDto item, DcxZones currentZone)
    {
        var boost = new ItemRarityBenefits();
        //we will choose one of these props to boost
        
        var propsList = new[]
        {
            // The first 7 elements is whole number points not percentages
                AffectedHeroStatTypeDto.Agility,
                AffectedHeroStatTypeDto.HitPoints,
                AffectedHeroStatTypeDto.Charisma,
                AffectedHeroStatTypeDto.Quickness,
                AffectedHeroStatTypeDto.Strength,
                AffectedHeroStatTypeDto.Wisdom,
                AffectedHeroStatTypeDto.ArmorMitigationAmount                
        };

        var propPercentageList = new[]
        {
            AffectedHeroStatTypeDto.ArmorMitigation,
            AffectedHeroStatTypeDto.ParryRate,
            AffectedHeroStatTypeDto.DodgeRate,
            AffectedHeroStatTypeDto.CriticalHitRate,
        };

        var finalList = propsList.Concat(propPercentageList).ToArray();

        var propsDiceRoll = _dice.Roll(finalList.Length);

        var propToBoost = finalList[propsDiceRoll - 1];

        if (!item.affectedAttributes.TryGetValue(propToBoost, out var currentValue))
        {
            currentValue = 0;
        }

        // The first 7 properties in the list are not chance enhancements so they are on a different schedule
        if (propsDiceRoll <= propsList.Length)
        {
            boost = ItemRaritySchedules.itemRarityBenefits[item.rarity];
        }
        else 
        {
            boost = ItemRaritySchedules.itemRarityPercentageBenefits[item.rarity];
        }

        if(!boost.zoneBoost.TryGetValue(currentZone, out var zoneBoost)){
            zoneBoost = 0;
            _logger.LogWarning($"zoneBoost for zone {currentZone} not in template");
        }

        // Only half value is given when the prop to boost is ArmorMitigationAmount
        if (propToBoost == AffectedHeroStatTypeDto.ArmorMitigationAmount)
        {
            item.affectedAttributes[propToBoost] = currentValue + (int)Math.Round(zoneBoost / 2.0);
        }
        else
        {
            item.affectedAttributes[propToBoost] = currentValue + zoneBoost;
        }
    }

    public string[] loadAllSkillTemplates()
    {
        var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        string folderName = string.Format("{0}.templates.items", executingAssembly.GetName().Name);
        var h = executingAssembly
            .GetManifestResourceNames();

        return  h
            .Where(r => r.StartsWith(folderName+".skill")
                            && r.EndsWith(".json"))
            .Select(r => {
                var g = r.Replace(folderName + ".", "");
                return g;
            })
            .ToArray();

        
    }



    /// <summary>
    /// Hero rarity is an optional param. Only pass in value when hero's rarity is affecting the item rarity.
    /// </summary>
    /// <param name="itemTemplateName"></param>
    /// <param name="currentZone"></param>
    /// <param name="heroRarity"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="Exception"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public ItemDto CreateItemFromTemplate(string itemTemplateName, DcxZones currentZone, HeroRarityDto? heroRarity = null)
    {
        var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

        if (null == executingAssembly)
            throw new InvalidOperationException("executingAssembly cannot be null");

        var templateName = $"{executingAssembly.GetName().Name}.templates.items.{itemTemplateName}";

        using (var stream = executingAssembly.GetManifestResourceStream(templateName))
        {
            if (null == stream)
            {
                throw new Exception($"template {templateName} not found");
            }
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();

                var itemTemplate = JsonConvert.DeserializeObject<ItemTemplate>(result);

                if (null == itemTemplate)
                {
                    _logger.LogCritical($"item template {templateName} could not be serialized");

                    throw new InvalidDataException($"item template {templateName} could not be serialized");
                }                 

                var item = new ItemDto
                {
                    id = Guid.NewGuid().ToString()
                };

                var fieldsToIgnore = new string[] {
                    nameof(SkillItemTemplate.skill)
                    };

                if (itemTemplate.slot == ItemSlotTypeDto.unidentifiedSkill)
                {
                    itemTemplate = JsonConvert.DeserializeObject<SkillItemTemplate>(result);

                    var skillTemplate = itemTemplate as SkillItemTemplate;
                    if (null == skillTemplate)
                    {
                        _logger.LogCritical($"item template {templateName} could not be serialized");

                        throw new InvalidDataException($"item template {templateName} could not be serialized");
                    }
                        

                    //we want to hide the item slug till the skill is learned
                    //so we store it ni the SkillItem, where the slug is json ignored
                    skillTemplate.skill.slug = itemTemplate.slug;
                    fieldsToIgnore = fieldsToIgnore.Concat(new[] { nameof(ItemTemplate.slug) }).ToArray();

                    item = new SkillItem
                    {
                        id = Guid.NewGuid().ToString(),
                        skill = skillTemplate.skill,
                    };

                }


                item.UpdateFromTemplate(itemTemplate, fieldsToIgnore);

                item.rarity = ItemRarityDto.Unknown;

                _logger.LogDebug($"Item rarity is set to {item.rarity} to begin with.");

                // Skill and shard will have a drop sound of leather-boots
                // also there is no rarity for these
                if (item.slot == ItemSlotTypeDto.unidentifiedSkill || item.slot == ItemSlotTypeDto.shard)
                {
                    item.rarity = ItemRarityDto.Unknown;
                    _logger.LogDebug($"Item is a type of {item.slot}. ItemDropSound is set to leather-boots and rarity remains {item.rarity}");
                }
                else
                {
                    // Azure issue ticket #404. Hero rarity will not affect item rarity until further notice. Keeping the FixRarityByHeroRarity for now but is not used anywhere
                    // We would only pass in heroRarity with value here when hero rarity affects item rarity.
                    if (null != heroRarity && heroRarity.HasValue)
                    {
                        FixRarityByHeroRarity(item, heroRarity.Value, currentZone);
                    }
                    else 
                    {
                        FixRarity(item, currentZone);
                    }
                }

                return item;

            }
        }
    }

    public ItemAndDropChance[] GetAllMonsterLoots(DbGameState gameState, CombatEncounter combat)
    {
        var template = TileDto.loadMonsterTemplates().Where(t => t.MonsterSlug == combat.Monster.MonsterSlug).Single();

        var allItems = template.LootItemsTemplates.Select(t =>
        {
            ItemDto item = new ItemDto();
            try
            {
                item = CreateItemFromTemplate(t.Key,
            TileDto.ZoneFromTile(gameState.CurrentTile)
            //, GameState.Hero.rarity
            );
            }
            catch (Exception ex)
            {
                // When we can't find the template for the item, set id to empty in case we accidently generates it
                item.id = "";
                _logger.LogError($"Couldn't find item template. {ex.Message}");
            }
            return new ItemAndDropChance()
            {
                item = item,
                dropChance = t.Value.ChancesOfDrop
            };
        }).ToArray();

        // When item id is null or empty, that means we couldn't find the template.
        // In theory this should never happen.
        allItems = allItems.Where(i => !string.IsNullOrEmpty(i.item.id)).ToArray();

        return allItems;
    }

    public class ItemAndDropChance 
    {
        public ItemDto item { get; set; } = new ItemDto();

        public int dropChance { get; set; }
    }

    public void ApplyLevelScaling(CombatEncounter combat, DbGameState gameState)
    {
        var levelDifference = combat.Monster.Level - gameState.Hero.level;

        int levelIncreasePercentage = GenerateLevelIncreasePercentages(levelDifference);

        if (levelIncreasePercentage == 0)
        {
            return;
        }

        var effectedProperties = typeof(MonsterDto).GetProperties()
            .Where(x => x.GetCustomAttributes(typeof(EffectedByLevelDifferenceAttribute), true).Any());

        ApplyLevelScalingToMonsterStats(effectedProperties, levelIncreasePercentage, combat.Monster, levelDifference);
    }

    public void ApplyLevelScalingToMonsterStats(IEnumerable<PropertyInfo> effectedProperties, int levelIncreasePercentage, MonsterCommon monster, int levelDifference)
    {

        foreach (var prop in effectedProperties)
        {
            var current = (int)prop.GetValue(monster);

            // When level diff is greater than 3, add an additional flat number to stats before scaling.
            if (levelDifference > 3)
            {
                // MaxHitPoints and Charisma will have additional 5 to it
                if (prop.Name == nameof(MonsterDto.MaxHitPoints) || prop.Name == nameof(MonsterDto.Charisma))
                {
                    current = current + 5;
                }
                // Everthing else except for bonus damage will always get the extra 500 before scaling.
                else if (prop.Name != nameof(MonsterDto.BonusDamage))
                {
                    current = current + 500;
                }
            }

            var newvalue = DataHelper.CalculateAndRound(current, (1.0 + levelIncreasePercentage / 100.0));

            _logger.LogDebug($"Monster prop {prop.Name} is affected by levelscaling by {levelIncreasePercentage}. Current value is {current}, and new value is {newvalue}");

            prop.SetValue(monster, newvalue);
        }
    }

    public int GenerateLevelIncreasePercentages(int levelDifference)
    {
        int levelIncreasePercentage;

        if (levelDifference <= 2)
        {
            //0 - 2 level difference will be unscaled
            _logger.LogDebug($"Monster level minus hero level is {levelDifference}. 0 - 2 level difference will be unscaled");
            levelIncreasePercentage = 0;
        }
        //dee-eric-review at 3 7 and then increase 15 for each level difference
        else if (3 == levelDifference)
        {
            //3 level difference will be a 7% increase in all monster stats
            _logger.LogDebug($"Monster level minus hero level is {levelDifference}. 3 level difference will be a 7% increase in all monster stats");
            levelIncreasePercentage = 7;
        }
        else
        {
            // For a 4 level diff, the increase percentage would be 7 + 15
            // For a 5 level diff, the increase percentage would be 7 + 15 + 15
            _logger.LogDebug($"Monster level minus hero level is {levelDifference}. For a 4 level diff, the increase percentage would be 7 + 15. For a 5 level diff, the increase percentage would be 7 + 15 + 15");
            levelIncreasePercentage = 7 + (levelDifference - 3) * 15;
        }

        return levelIncreasePercentage;
    }
}

[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum ItemRewardType { shard, skill }
