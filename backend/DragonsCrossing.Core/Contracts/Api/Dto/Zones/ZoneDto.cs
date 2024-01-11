using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.Zones;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using Newtonsoft.Json.Converters;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones;

// https://lucid.app/lucidchart/4c5fe5fe-ef7e-46ad-9256-0fee54170554/edit?invitationId=inv_b6bfbf38-40ad-40c0-8a2f-3ad891311a92&page=0_0#
// D:\Development\DragonsCrossingWeb\src\pages\enchanted-fields\index.tsx This is when the user clicks on the combat tile: line 28

/// <summary>
/// These zones need to be IN ORDER
/// </summary>
[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
public enum DcxZones
{
    aedos = 0,

    wildPrairie = 1,
    mysteriousForest = 2,
    foulWastes = 3, 
    treacherousPeaks = 4, 
    darkTower = 5,

    wondrousThicket = 6,
    fallenTemples = 7,

    unknown = 1000
}

public class NearByZone
{
    public DcxZones zone { get; set; }

    /// <summary>
    /// if true it's the next higher zone. if false then this is previous zone
    /// </summary>
    public bool isNext { get; set; }
}



/// <summary>
/// The zone Where her is in
/// </summary>
public class ZoneDto
{
    [Required]
    public DcxZones Slug { get; set; } = DcxZones.wildPrairie;

    /// <summary>
    /// This is all the tiles that has been discovered for the zone
    /// todo: talk to Eric about the logic here
    /// </summary>
    [BsonIgnore]
    [Required]
    public TileDto[] DiscoveredTiles { get; set; } = new TileDto[] { };

    public static readonly IDictionary<DcxZones, int> MapZoneOrders = new Dictionary<DcxZones, int> {
        { DcxZones.aedos,  10},
        { DcxZones.wildPrairie, 20 },
        { DcxZones.mysteriousForest,  30},

        { DcxZones.foulWastes,  40},
        { DcxZones.wondrousThicket,  45},


        { DcxZones.treacherousPeaks,  50},
        { DcxZones.fallenTemples,  55},


        { DcxZones.darkTower,  60},
        
    };

    public readonly static IDictionary<DcxZones, DcxTiles[]> MapZonetoTiles = new Dictionary<DcxZones, DcxTiles[]>
    {
        { DcxZones.wildPrairie, new []{ DcxTiles.enchantedFields} },
        { DcxZones.aedos, new []{ DcxTiles.blacksmith, DcxTiles.adventuringGuild, DcxTiles.sharedStash, DcxTiles.herbalist_aedos}},
        { DcxZones.mysteriousForest, new []{ DcxTiles.sylvanWoodlands, DcxTiles.pilgrimsClearing}},


        { DcxZones.foulWastes, new []{ DcxTiles.odorousBog, DcxTiles.ancientBattlefield, DcxTiles.terrorswamp, DcxTiles.herbalist_foulWastes, DcxTiles.adventuringGuild_foulWastes }},
        { DcxZones.treacherousPeaks, new []{ DcxTiles.mountainFortress, DcxTiles.griffonsNest, DcxTiles.summonersSummit }},


        { DcxZones.wondrousThicket, new []{ DcxTiles.feyClearing, DcxTiles.shatteredStable, DcxTiles.forebodingDale, DcxTiles.herbalist_wondrousThicket, DcxTiles.adventuringGuild_wondrousThicket }},
        { DcxZones.fallenTemples, new []{ DcxTiles.blacksmith_fallenTemples, DcxTiles.pillaredRuins, DcxTiles.acropolis, DcxTiles.destroyedPantheon }},


        { DcxZones.darkTower, new []{ DcxTiles.labyrinthianDungeon, DcxTiles.barracks, DcxTiles.slaversRow, DcxTiles.libraryOfTheArchmage, DcxTiles.herbalist_darkTower }}
    };

    public readonly static IDictionary<DcxZones, NearByZone[]> MapNearByZones = new Dictionary<DcxZones, NearByZone[]>{

        {
            DcxZones.aedos, new []{
                new NearByZone{ zone = DcxZones.wildPrairie, isNext = true}
        } },

        {
            DcxZones.wildPrairie, new []{
                new NearByZone{ zone = DcxZones.aedos, isNext = false},
                new NearByZone{ zone = DcxZones.mysteriousForest, isNext = true}
        } },

        {
            DcxZones.mysteriousForest, new []{
                new NearByZone{ zone = DcxZones.wildPrairie, isNext = false},
                new NearByZone{ zone = DcxZones.foulWastes, isNext = true},
                new NearByZone{ zone = DcxZones.wondrousThicket, isNext = true},
        } },

        

        {
            DcxZones.foulWastes, new []{
                new NearByZone{ zone = DcxZones.mysteriousForest, isNext = false},
                new NearByZone{ zone = DcxZones.treacherousPeaks, isNext = true},
        } },

        {
            DcxZones.wondrousThicket, new []{
                new NearByZone{ zone = DcxZones.mysteriousForest, isNext = false},
                new NearByZone{ zone = DcxZones.fallenTemples, isNext = true}
        } },


        {
            DcxZones.treacherousPeaks, new []{
                new NearByZone{ zone = DcxZones.foulWastes, isNext = false},
                new NearByZone{ zone = DcxZones.darkTower, isNext = true}
        } },


        {
            DcxZones.fallenTemples, new []{
                new NearByZone{ zone = DcxZones.wondrousThicket, isNext = false},
                new NearByZone{ zone = DcxZones.darkTower, isNext = true}
        } },


        {
            DcxZones.darkTower, new []{
                new NearByZone{ zone = DcxZones.treacherousPeaks, isNext = false},
                new NearByZone{ zone = DcxZones.fallenTemples, isNext = false}
        } }
    };

    /// <summary>
    /// This is the number of undiscovered tiles for the zone.
    /// </summary>
    [BsonIgnore]
    [Required]
    public int UndiscoveredTileCount { get; set; }
}


public enum TileType
{
    Vendor,
    Daily,
    Boss,
    AdventuringGuild,
    Camp
}

public class ChanceEncounterResultProps
{
    public UpdateDefinition<DbGameState>?  setter { get; set; }
    public string? encounterResponceSlug { get; set; }
    public bool? sendDiceResult { get; set; }
}

public partial class TileDto
{
    [Required]
    public DcxTiles Slug { get; set; }

    [Required]
    public bool IsActive { get; set; }

    public TileType? TileType { get; set; }
    
    
    public readonly IDictionary<DcxZones, Func<ILogger, ISeasonsDbService, int, string, DcxZones, Task>> MapLoreEncounterFinish;
    public readonly IDictionary<DcxZones, Func<ILogger, ISeasonsDbService, int, string, DcxZones, LocationEncounter, Task>> MapLocationEncounterFinish;
    public readonly IDictionary<ChanceEncounterEnum, Func<ILogger, ISeasonsDbService, int, string, bool, UpdateDefinition<DbGameState>, Task<ChanceEncounterResultProps>>> MapChanceEncounterResult;

    /// <summary>
    /// Gets the zone given a tile
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public static DcxZones ZoneFromTile(DcxTiles tile)
    {
        DcxZones zone;
        if (!Enum.TryParse<DcxZones>(tile.ToString(), out zone))
        {
            //we are in a tile
            zone = ZoneDto.MapZonetoTiles
                .Where(kv => kv.Value.Contains(tile))
                .First().Key;
        }

        return zone;
    }

    public static TileType? TileTypeFromTile(DcxTiles tile)
    {
        if(BossTiles.Contains(tile))
        {
            return Zones.TileType.Boss;
        }
        else if(DailyQuestTiles.Contains(tile))
        {
            return Zones.TileType.Daily;
        }
        else if(VendorTiles.Contains(tile))
        {
            return Zones.TileType.Vendor;
        }
        else if (AdventuringGuildTiles.Contains(tile))
        {
            return Zones.TileType.AdventuringGuild;
        }
        else if (CampTiles.Contains(tile))
        {
            return Zones.TileType.Camp;
        }

        return null;
    }

    public static readonly DcxTiles[] QuestTiles = new[] 
    { 
        DcxTiles.enchantedFields, 
        DcxTiles.sylvanWoodlands, 
        DcxTiles.odorousBog ,  
        DcxTiles.mountainFortress,
        DcxTiles.labyrinthianDungeon,

        DcxTiles.feyClearing,
        DcxTiles.pillaredRuins,
    };

    public static readonly DcxTiles[] Cocytus2Tiles = new[]
    {
        DcxTiles.feyClearing,
        DcxTiles.pillaredRuins,
    };

    public static readonly DcxTiles[] BossTiles = new[]
    {
        DcxTiles.terrorswamp,
        DcxTiles.summonersSummit,
        DcxTiles.slaversRow,


        DcxTiles.forebodingDale,
        DcxTiles.destroyedPantheon,

        // Final boss
        DcxTiles.libraryOfTheArchmage
    };

    public static readonly DcxTiles[] DailyQuestTiles = new[]
    {
        DcxTiles.pilgrimsClearing,
        DcxTiles.ancientBattlefield,
        DcxTiles.griffonsNest,
        DcxTiles.barracks,

        DcxTiles.shatteredStable,
        DcxTiles.acropolis
    };

    public static readonly DcxTiles[] VendorTiles = new[]
    {
        DcxTiles.herbalist_aedos,
        DcxTiles.herbalist_darkTower,
        DcxTiles.herbalist_foulWastes,
    };

    public static readonly DcxTiles[] AdventuringGuildTiles = new[]
    {
        DcxTiles.adventuringGuild,
        DcxTiles.adventuringGuild_foulWastes
    };

    public static readonly DcxTiles[] CampTiles = new[]
   {
        DcxTiles.camp_mysteriousForest,
        DcxTiles.camp_treacherousPeaks
    };

    public TileDto()
    {

        MapLoreEncounterFinish =
            new Dictionary<DcxZones, Func<ILogger, ISeasonsDbService, int, string, DcxZones, Task>> {
                { DcxZones.wildPrairie, LoreEncounterFinish<WildPraireState>},
                { DcxZones.mysteriousForest, LoreEncounterFinish<MysteriousForestState>},
                { DcxZones.foulWastes, LoreEncounterFinish<FoulWastesState>},
                { DcxZones.treacherousPeaks, LoreEncounterFinish<TreacherousPeaksState>},
                { DcxZones.darkTower, LoreEncounterFinish<DarkTowerState>},
                { DcxZones.fallenTemples, LoreEncounterFinish<FallenTemplesState>},
                { DcxZones.wondrousThicket, LoreEncounterFinish<WondrousThicketState>},
            };

        MapLocationEncounterFinish =
            new Dictionary<DcxZones, Func<ILogger, ISeasonsDbService, int, string, DcxZones, LocationEncounter, Task>> {
                { DcxZones.wildPrairie, LocationEncounterFinish<WildPraireState>},
                { DcxZones.mysteriousForest, LocationEncounterFinish<MysteriousForestState>},
                { DcxZones.foulWastes, LocationEncounterFinish<FoulWastesState>},
                { DcxZones.treacherousPeaks, LocationEncounterFinish<TreacherousPeaksState>},
                { DcxZones.darkTower, LocationEncounterFinish<DarkTowerState>},
                { DcxZones.fallenTemples, LocationEncounterFinish<FallenTemplesState>},
                { DcxZones.wondrousThicket, LocationEncounterFinish<WondrousThicketState>},
            };

        MapChanceEncounterResult =
            new Dictionary<ChanceEncounterEnum, Func<ILogger, ISeasonsDbService, int, string, bool, UpdateDefinition<DbGameState>, Task<ChanceEncounterResultProps>>> {
                { ChanceEncounterEnum.foreignBerries, foreignBerriesChanceEncounterFinish},
                { ChanceEncounterEnum.freshwaterOrb, freshwaterOrbChanceEncounterFinish},
                { ChanceEncounterEnum.gambler, gamblerChanceEncounterFinish},
                { ChanceEncounterEnum.wonderingWizard, wonderingWizardChanceEncounterFinish},
                { ChanceEncounterEnum.rustingArandomWeapon, rustingArandomWeaponChanceEncounterFinish},
                { ChanceEncounterEnum.riddler, riddlerChanceEncounterFinish},
                { ChanceEncounterEnum.lovecraftianMonster, lovecraftianMonsterChanceEncounterFinish}
            };

    } 
}
