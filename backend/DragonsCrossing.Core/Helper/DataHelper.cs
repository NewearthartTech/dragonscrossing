using System.Reflection;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.NewCombatLogic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;

namespace DragonsCrossing.Core.Helper
{
    [MongoCollection("GeneratedSequences")]
    [BsonIgnoreExtraElements]
    public class GeneratedSequence
    {
        [BsonId]
        public string generatedType { get; set; } = String.Empty;

        public int lastId { get; set; }

    }


    public static partial class DataHelper
    {

        public static async Task<int> GetNextSequence<T>(this IPerpetualDbService db)
        {

            var genType = typeof(T).Name;

            var generated = await db.getCollection<GeneratedSequence>()
                .FindOneAndUpdateAsync<GeneratedSequence>(g => g.generatedType == genType,
                Builders<GeneratedSequence>.Update
                .Inc(g => g.lastId, 1)
                .SetOnInsert(g => g.generatedType, genType)
                ,
                new FindOneAndUpdateOptions<GeneratedSequence>
                {
                    IsUpsert = true
                }
                );

            if(null == generated)
            {
                //we just inserted cool just call it again
                //yeah yeah our id will start from 2
                return await GetNextSequence<T>(db);
            }

            if (typeof(T) == typeof(Sagas.NftizedDFKItem))
            {
                return generated.lastId + Contracts.Api.Dto.Items.ItemDto._maxArbHeroIdVal;
            }
            else if (typeof(T) == typeof(Sagas.NftizedItem))
            {
                if (generated.lastId > Contracts.Api.Dto.Items.ItemDto._maxArbHeroIdVal)
                    throw new Exception("mox items created");
            }else if (typeof(T) == typeof(Services.DFkHeroWrapper))
            {
                return generated.lastId + HeroDto._maxArbHeroIdVal;
            }
            return generated.lastId;
        }


        public static T CreateTypefromJsonTemplate<T>(string templateName, T typeDef)
        {
            return CreateTypefromJsonTemplate<T>(templateName);
        }

        public static T CreateTypefromJsonTemplate<T>(string templateName)
        {
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            if (null == executingAssembly)
                throw new InvalidOperationException("executingAssembly not found");

            var template = $"{executingAssembly.GetName().Name}.{templateName}";

            using (var stream = executingAssembly.GetManifestResourceStream(template))
            {
                if (null == stream)
                {
                    throw new InvalidOperationException($"failed to open stream for template file {template}");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();

                    var ret = JsonConvert.DeserializeObject<T>(json);
                    if (null == ret)
                        throw new InvalidOperationException("failed to DeserializeObject");

                    return ret;
                }
            }

        }

        //WEN-TODO: This does not need to be a task
        public static Task<int> ReadJsonFileAndReturnStatValue(string templateName, int attributeValue)
        {

            int calculatedValue = 0;

            var dictionary = CreateTypefromJsonTemplate<Dictionary<int, int>>($"ScheduleTableRef.{templateName}");

            if (dictionary.ContainsKey(attributeValue))
            {
                calculatedValue += dictionary[attributeValue];
            }

            return Task.FromResult(calculatedValue);
        }

        public static void CheckIfCanLearnSkill(DbGameState gameState, UnlearnedHeroSkill unLearned)
        {
            if (gameState.Hero.heroClass != unLearned.skillClass)
            {
                throw new ExceptionWithCode("Unable to learn skill. Skill class requirement not met.");
            }

            if (gameState.Hero.skills.Where(s => s.slug == unLearned.slug).Count() > 0)
            {
                throw new ExceptionWithCode("Unable to learn skill. Skill has already been learned.");
            }

            if (gameState.Hero.skills.Length >= 6)
            {
                throw new ExceptionWithCode("Unable to learn skill. Maximum of 6 leaned skills has been reached.");
            }
        }
    }
}
