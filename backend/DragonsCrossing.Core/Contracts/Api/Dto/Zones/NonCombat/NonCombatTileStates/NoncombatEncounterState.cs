using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones
{
    [BsonDiscriminator(Required = true, RootClass = true)]
    [BsonKnownTypes(typeof(WildPraireState), typeof(MysteriousForestState),
        typeof(FoulWastesState), typeof(TreacherousPeaksState), typeof(DarkTowerState),
        typeof(WondrousThicketState), typeof(FallenTemplesState)
    )]
    public class NoncombatEncounterState
    {
        /// <summary>
        /// map of tiles and have they been discovered
        /// </summary>
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<DcxTiles, DiscovereableTileState> tilesToDiscoverState { get; set; } = new Dictionary<DcxTiles, DiscovereableTileState> { };

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<LoreEnum, bool> loresPlayed { get; set; } = new Dictionary<LoreEnum, bool> { };
    }
}
