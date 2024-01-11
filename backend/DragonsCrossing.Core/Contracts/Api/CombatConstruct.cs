using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Options;
using static DragonsCrossing.Core.Contracts.Api.Dto.Zones.TileDto;

namespace DragonsCrossing.NewCombatLogic;   

public class LingeringSkillAction
{
    /// <summary>
    /// Whihc round this action will take place
    /// </summary>
    public int InRound { get; set; }

    /// <summary>
    /// ActionFunctionName is optional. When not set, we don't trigger any function.
    /// Which Function to call.
    /// </summary>
    public string ActionFunctionName { get; set; } = "";

    public string Description { get; set; } = "";

    public int RemainingRounds { get; set; }

    /// <summary>
    /// This may not be used anywhere
    /// </summary>
    public int RoundStarted { get; set; }

    public string SkillName { get; set; } = "";
}

[BsonIgnoreExtraElements]
[BsonDiscriminator(Required = true, RootClass = true)]
[BsonKnownTypes(typeof(CombatEncounter), typeof(LocationEncounter), typeof(LoreEncounter), typeof(ChanceEncounter), typeof(BossEncounter))]
[System.Text.Json.Serialization.JsonConverter(typeof(PolymorphicConverter<Encounter>))]
public abstract class Encounter : PolymorphicBase<Encounter>
{
    public Encounter()
    {
        id = Guid.NewGuid().ToString();
    }

    // For non-combat slug, that will be the zone tile name
    public string? Slug { get; set; }

    [Required]
    public string id { get; set; }
}

public class PolymorphicConverter<T> : System.Text.Json.Serialization.JsonConverter<T> where T:class
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var t = reader.GetString();
        if (null == t)
            return null;

        var h = Newtonsoft.Json.JsonConvert.DeserializeObject(t) as T;

        return h;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var t = Newtonsoft.Json.JsonConvert.SerializeObject(value, new Newtonsoft.Json.JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            Converters = new List<Newtonsoft.Json.JsonConverter>(new[] {
            new Newtonsoft.Json.Converters.StringEnumConverter()
            })
        });
        writer.WriteRawValue(t);
    }
}


/*
 * 
 * bloaterpunch — Yesterday at 4:47 PM
@Developer hierarchy of discovery for non combat encounters:
- Healer
- Daily
- Boss
- Next Tile - Can't progress until zone boss is beaten - only applies to zones 3 - 5,
as Zones 1 - 2 do not have bosses
 * 
 * 
 */



public abstract class NonCombatEncounter : Encounter
{
    [Required]
    public string IntroText { get; set; } = string.Empty;
}

[BsonIgnoreExtraElements]
public class LocationEncounter : NonCombatEncounter
{
    /// <summary>
    /// // ie. This will be the new zone name for location encounter. Mysterious Forest, Foul Wastes, Foreign Berries, Gambler, etc.
    /// </summary>
    [Required]
    public DcxTiles[] newLocations { get; set; } = new DcxTiles[] { };

    [Required]
    public string NarratedText { get; set; } = string.Empty;
}

[BsonIgnoreExtraElements]
public class BossEncounter : NonCombatEncounter
{
    [Required]
    public string NarratedText { get; set; } = string.Empty;
}

[BsonIgnoreExtraElements]
public class LoreEncounter : NonCombatEncounter
{
    [Required]
    public string NarratedText { get; set; } = string.Empty;

    [Required]
    public LoreDialog[] Dialogues { get; set; } = new LoreDialog[] { };

    [Required]
    public LoreEnum LoreNumber { get; set; } = LoreEnum.unknown;
}

[BsonIgnoreExtraElements]
public class ChanceEncounter : NonCombatEncounter
{
    [Required]
    public string EncounterName { get; set; } = String.Empty;

    [Required]
    public EncounterChoice[] Choices { get; set; } = new EncounterChoice[] { };

    [Required]
    public ChanceEncounterEnum ChanceEncounterType { get; set; } = ChanceEncounterEnum.unknown;
}

[BsonIgnoreExtraElements]
public class ChanceEncounterTemplate : NonCombatEncounter
{
    [Required]
    public string EncounterName { get; set; } = String.Empty;

    [Required]
    public EncounterChoiceTemplate[] Choices { get; set; } = new EncounterChoiceTemplate[] { };

    [Required]
    public ChanceEncounterEnum ChanceEncounterType { get; set; } = ChanceEncounterEnum.unknown;
}

public class LoreDialog
{
    [Required]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public string QuestionText { get; set; } = string.Empty;
}

public class EncounterChoice
{
    /// <summary>
    /// This slug is for us to recognize the player's choice
    /// </summary>
    [Required]
    public string choiceSlug { get; set; } = string.Empty;

    [Required]
    public string choiceText { get; set; } = string.Empty;
  
    [Required]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [BsonIgnore]
    public int goodOutcomeChance { get; set; }

    [Required]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [BsonIgnore]
    public string goodOutcomeText { get; set; } = string.Empty;

    [Required]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [BsonIgnore]
    public string badOutcomeText { get; set; } = string.Empty;
}

public class EncounterChoiceTemplate
{
    /// <summary>
    /// This slug is for us to recognize the player's choice
    /// </summary>
    [Required]
    public string choiceSlug { get; set; } = string.Empty;

    [Required]
    public string choiceText { get; set; } = string.Empty;

    [Required]
    [BsonIgnore]
    public int goodOutcomeChance { get; set; }

    [Required]
    [BsonIgnore]
    public string goodOutcomeText { get; set; } = string.Empty;

    [Required]
    [BsonIgnore]
    public string badOutcomeText { get; set; } = string.Empty;
}

[BsonIgnoreExtraElements]
public class CombatEncounter : Encounter
{
    public MonsterDto? Monster { get; set; }

    public int CurrentRound { get; set; }
    //public string CurrentState { get; set; }

    /// <summary>
    /// are we in 2nd parties turn
    /// </summary>
    public bool is2ndTurn { get; set; } = false;
    public bool isHerosTurn { get; set; } = false;

    public bool isHeroAbleToFlee { get; set; } = false;

    public bool isCharismaOpportunityAvailable { get; set; } = false;

    public CharismaOpportunityResultType? charismaOpportunityResultType { get; set; }

    // When a hero uses a skill, attack, or flee, this will be marked as true until end of game.
    // This is used to help determine if isCharismaOpportunityAvailable could be available.
    [JsonIgnore]
    public bool isCombatActionTaken { get; set; } = false;

    public HeroAttackResultDto heroAttackResult { get; set; } = new HeroAttackResultDto();

    public MonsterAttackResultDto monsterAttackResult { get; set; } = new MonsterAttackResultDto();

    public bool isMonsterDead { get; set; }
    public bool isHeroDead { get; set; }

    public List<LingeringSkillAction> LingeringSkillEffects { get; set; } = new List<LingeringSkillAction>();

    public CombatantType Initiative { get; set; }

    public MonsterLootDto? loot { get; set; } = null;

    /// <summary>
    /// Keeps track 
    /// </summary>
    public int[] skillsUsedInRounds { get; set; } = new int[] { };
    
    /// <summary>
    /// This contains a list of round numbers that has been initialized. 
    /// </summary>
    public int[] roundsStarted { get; set; } = new int[] { };

}

/// <summary>
/// This is the primary "table" that is store in the Database
/// Essensially all game operations update this state "only"
/// </summary>
[MongoCollection("GameStates")]
[BsonIgnoreExtraElements]
public class DbGameState
{
    /// <summary>
    /// We use hero as the ID for gameState
    /// </summary>
    [BsonId]
    public int HeroId {
        get { return Hero.id; }
        set { }
    }

    /// <summary>
    /// We use this to ensure we don't allow too fast actions
    /// </summary>
    public DateTime lastCombatActionAt { get; set; } = DateTime.MinValue;

    /// <summary>
    /// The location where the Hero is. All minted heros NEED to start from aedos
    /// </summary>
    [Required]
    public DcxTiles CurrentTile { get; set; } = DcxTiles.aedos;

    /// <summary>
    /// gameState is always for a Hero
    /// </summary>
    public HeroDto Hero { get; set; } = new HeroDto();

    public Encounter[] CurrentEncounters { get; set; } = new Encounter[] { };

    public DcxZones HighestZoneVisited { get; set; } = DcxZones.wildPrairie;

    /// <summary>
    /// Since we don't have transactions we use this as a place holder while the Qs actually move items
    /// </summary>
    public ItemDto[] itemsToMoveToStash { get; set; } = new ItemDto[] { };

    /// <summary>
    /// These items have been moved from inventory and are waiting to be secured as NFTS
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public ItemDto[] nftClaims { get; set; } = new ItemDto[] { };


    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
    public Dictionary<DcxZones, NoncombatEncounterState> nonCombatTileState { get; set; } = new Dictionary<DcxZones, NoncombatEncounterState>();

    public DcxTiles[] inactiveDailyTiles { get; set; } = new DcxTiles[] { };

    public DcxTiles[] inactiveBossTiles { get; set; } = new DcxTiles[] { };

    [Required]
    public LeaderStatus leaderStatus { get; set; } = new LeaderStatus();

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public int guaranteedNonCombatCounter { get; set; }


    public TileDto[] getDiscoveredTiles()
    {
        return this.nonCombatTileState.TryGetValue(TileDto.ZoneFromTile(this.CurrentTile), out var encounterState)
                ? this.nonCombatTileState[TileDto.ZoneFromTile(this.CurrentTile)]
                    .tilesToDiscoverState
                    .Where(t => t.Value.isDiscovered == true)
                    .Select(t => new TileDto() { Slug = t.Key, IsActive = t.Value.isActive(this), TileType = TileDto.TileTypeFromTile(t.Key) })
                    .ToArray()
                : new TileDto[] { };
    }
}

public class LeaderStatus
{
    /// <summary>
    /// the actualt zone discovered
    /// </summary>
    [Required]
    public DcxZones farthestZoneDiscovered { get; set; } = DcxZones.aedos;

    /// <summary>
    /// We can't guarentee that the zone enums are in Order so we made new orders
    /// </summary>
    [Required]
    public int farthestZoneDiscoveredOrder { get; set; }

    [Required]
    public int totalQuestsUsed { get; set; }

    [Required]
    public bool isFinalBossDefeated { get; set; }

    [Required]
    public int benchmarkUsedQuests { get; set; }

}


