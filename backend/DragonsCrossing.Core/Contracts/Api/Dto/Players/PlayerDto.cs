using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Players
{
    [MongoCollection("Players")]
    [BsonIgnoreExtraElements]
    public class PlayerDto
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string Id { get; set; } = "";

        [Required]
        public ItemDto[] SharedStash { get; set; } = new ItemDto[] { };

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        /// <summary>
        /// Since we don't have transactions we use this as a place holder while the Qs actually move items
        /// </summary>
        public ItemDto[] itemsToMoveFromStash { get; set; } = new ItemDto[] { };


        /// <summary>
        /// Blockchain public address
        /// </summary>
        public string? BlockchainPublicAddress { get; set; } = "";

        /// <summary>
        /// The last time Daily quests was reset
        /// Once this is set we keep reseting every 24 hours
        /// </summary>
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

        //we reset every 23 hours
        public static readonly int RESET_HOURS = 23;

        [MongoIndex]
        public static void CreateIndexes(IMongoCollection<PlayerDto> collection)
        {
            collection.Indexes.CreateOne(
                new CreateIndexModel<PlayerDto>(
                new IndexKeysDefinitionBuilder<PlayerDto>()
                    .Ascending(f => f.BlockchainPublicAddress)
                    , new CreateIndexOptions<PlayerDto>
                    {
                        Unique = true
                    }
                ));
        }

    }
}
