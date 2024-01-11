using System.Reflection;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using Newtonsoft.Json;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;

namespace DragonsCrossing.Core.Helper
{
    public class ItemRarityBenefits
    {
        public Dictionary<DcxZones, int> zoneBoost { get; set; }
         = new Dictionary<DcxZones, int>();
    }

    public static class ItemRaritySchedules
    {
        public static Dictionary<ItemRarityDto, ItemRarityBenefits> itemRarityBenefits { get; }
        public static Dictionary<ItemRarityDto, ItemRarityBenefits> itemRarityPercentageBenefits { get; }
        public static Dictionary<ItemRarityDto, Range> itemRarityChance { get; }
        public static Dictionary<HeroRarityDto, int> heroRarityBoostPoints { get; }


        static ItemRaritySchedules()
        {
            itemRarityBenefits = DataHelper.CreateTypefromJsonTemplate($"ScheduleTableRef.itemRarityBenefits.json",
                    new Dictionary<ItemRarityDto, ItemRarityBenefits>());

            itemRarityPercentageBenefits = DataHelper.CreateTypefromJsonTemplate($"ScheduleTableRef.itemRarityPercentageBenefits.json",
                    new Dictionary<ItemRarityDto, ItemRarityBenefits>());

            itemRarityChance = DataHelper.CreateTypefromJsonTemplate($"ScheduleTableRef.itemRarityChance.json",
                    new Dictionary<ItemRarityDto, Range>());

            heroRarityBoostPoints = DataHelper.CreateTypefromJsonTemplate($"ScheduleTableRef.heroRarityBoostPoints.json",
                    new Dictionary<HeroRarityDto, int>());
        }

    }


    public static partial class DataHelper
    {

        
        

        public static UnlearnedHeroSkill CreateSkillFromTemplate(string itemTemplateName)
        {
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            if (null == executingAssembly)
                throw new InvalidOperationException("executingAssembly cannot be null");

            var templateName = $"{executingAssembly.GetName().Name}.templates.skills.{itemTemplateName}";

            using (var stream = executingAssembly.GetManifestResourceStream(templateName))
            {
                if (null == stream)
                {
                    throw new Exception($"template {templateName} not found");
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();

                    var skillFromTemplate = JsonConvert.DeserializeObject<UnlearnedHeroSkill>(result);

                    if (null == skillFromTemplate)
                        throw new InvalidDataException($"item template {templateName} could not be serialized");

                    skillFromTemplate.id = Guid.NewGuid().ToString();

                    return skillFromTemplate;
                }
            }

        }

        /// <summary>
        /// Takes UnlearnedHeroSkill object and then turn it into a LearnedHeroSkill object
        /// </summary>
        /// <param name="unlearnedHeroSkill"></param>
        /// <returns></returns>
        public static LearnedHeroSkill CreateSkillFromUnlearned(this UnlearnedHeroSkill unlearnedHeroSkill)
        {
            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(unlearnedHeroSkill);
            jsonData = jsonData.Replace(nameof(UnlearnedHeroSkill), nameof(LearnedHeroSkill));
            var learnedSkill = Newtonsoft.Json.JsonConvert.DeserializeObject<LearnedHeroSkill>(jsonData);

            if (null == learnedSkill)
            {
                throw new Exception("Failed to create learned hero skill");
            }


            var unLearnedSkill = DataHelper.CreateTypefromJsonTemplate($"templates.skills.{learnedSkill.slug}.json", new UnlearnedHeroSkill());

            learnedSkill.description = unLearnedSkill.description;
            learnedSkill.requiredSkillPoints = unLearnedSkill.requiredSkillPoints;


            if (string.IsNullOrWhiteSpace(learnedSkill.id))
            {
                learnedSkill.id = Guid.NewGuid().ToString();
            }

            return learnedSkill;
        }

        

        public static async Task<ItemDto[]> GetBlackSmithItems(DcxZones highestZoneOrder, ISeasonsDbService dbService)
        {

            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            var itemFileNames = GetAllItemTemplateNames();

            var items = itemFileNames.Select(fileName =>
            {
                //var fileName = $"{executingAssembly.GetName().Name}.templates.items.{itemFilename}";

                using (var stream = executingAssembly.GetManifestResourceStream(fileName))
                {
                    if (null == stream)
                    {
                        throw new Exception($"template {fileName} not found");
                    }
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string result = reader.ReadToEnd();

                        var itemTemplate = JsonConvert.DeserializeObject<Contracts.Api.Dto.Items.ItemTemplate>(result);

                        // highestZoneVisited determines the quality of items shown to player which is determined by BlackSmithZone in item template
                        if (itemTemplate.blackSmithZone == highestZoneOrder)
                        {
                            var item = new ItemDto();

                            //todo: we need to account for AffectedAttributes
                            item.UpdateFromTemplate(itemTemplate);

                            item.rarity = ItemRarityDto.Common;

                            return item;
                        }

                        return null;
                    }
                }
            }).Where(i => null != i).ToArray();

            var blackSmithItem = await dbService.getCollection<BlackSmithItem>().Find(i => i.zone == highestZoneOrder).SingleOrDefaultAsync();

            // If collection is empty, insert items
            if (blackSmithItem == null)
            {
                await dbService.getCollection<BlackSmithItem>().InsertOneAsync(new BlackSmithItem() { zone = highestZoneOrder, items = items });
                return items;
            }

            await dbService.getCollection<BlackSmithItem>().UpdateOneAsync(b => b.zone == highestZoneOrder, Builders<BlackSmithItem>.Update.Set(b => b.items, items));

            return items;
        }

        /// <summary>
        /// Loads all the item file names from the item folder.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string[] GetAllItemTemplateNames()
        {
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            if (null == executingAssembly)
                throw new InvalidOperationException("executingAssembly cannot be null");

            var allFileNames = executingAssembly.GetManifestResourceNames();

            var itemFileNames = allFileNames.Where(n => n.Contains(".items.item")).ToArray();

            return itemFileNames;
        }
    }

    [MongoCollection("BlackSmithItems")]
    [BsonIgnoreExtraElements]
    public class BlackSmithItem
    {
        public ItemDto[] items { get; set; } = new ItemDto[] { };

        public DcxZones zone { get; set; }
    }
}
