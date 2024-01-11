using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.GameStates;
using MassTransit.Contracts;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using Newtonsoft.Json;
using static DragonsCrossing.Core.Helper.DataHelper;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones;

public partial class TileDto
{
    
    

    public static NoncombatEncounterState InitiateNonCombatEncounterState(DcxZones newLocation)
    {
        switch (newLocation)
        {
            case DcxZones.wildPrairie:
                {
                    return new WildPraireState();
                }
            case DcxZones.mysteriousForest:
                {
                    return new MysteriousForestState();
                }
            case DcxZones.foulWastes:
                {
                    return new FoulWastesState();
                }
            case DcxZones.treacherousPeaks:
                {
                    return new TreacherousPeaksState();
                }
            case DcxZones.darkTower:
                {
                    return new DarkTowerState();
                }
            case DcxZones.wondrousThicket:
                return new WondrousThicketState();
            case DcxZones.fallenTemples:
                return new FallenTemplesState();
            default:
                break;
        }

        throw new Exception($"NonCombat encounter state not found for {newLocation}");
    }

    

    public static void ApplyCombatOpportunityStat(MonsterDto monster)
    {
        switch (monster.PersonalityType)
        {
            case MonsterPersonalityTypeDto.Sickly:
                monster.Charisma = CalculateAndRound(monster.Charisma, 0.7);
                break;
            case MonsterPersonalityTypeDto.Inspired:
                monster.Charisma = CalculateAndRound(monster.Charisma, 1.3);
                break;
            case MonsterPersonalityTypeDto.Fatigued:
                monster.ChanceToHit = CalculateAndRound(monster.ChanceToHit, 0.7);
                break;
            case MonsterPersonalityTypeDto.Brutal:
                monster.ChanceToHit = CalculateAndRound(monster.ChanceToHit, 1.3);
                break;
            case MonsterPersonalityTypeDto.Lazy:
                monster.Quickness = CalculateAndRound(monster.Quickness, 0.7);
                break;
            case MonsterPersonalityTypeDto.Lean:
                monster.Quickness = CalculateAndRound(monster.Quickness, 1.3);
                break;
            case MonsterPersonalityTypeDto.Stupid:
                monster.ChanceToDodge = CalculateAndRound(monster.ChanceToDodge, 0.7);
                break;
            case MonsterPersonalityTypeDto.Arcane:
                monster.ChanceToDodge = CalculateAndRound(monster.ChanceToDodge, 1.3);
                break;
            case MonsterPersonalityTypeDto.Impotent:
                monster.CritChance = CalculateAndRound(monster.CritChance, 0.7);
                break;
            case MonsterPersonalityTypeDto.Reckless:
                monster.CritChance = CalculateAndRound(monster.CritChance, 1.3);
                break;
            case MonsterPersonalityTypeDto.Deadly:
                monster.Charisma = CalculateAndRound(monster.Charisma, 1.3);
                monster.ChanceToHit = CalculateAndRound(monster.ChanceToHit, 1.3);
                monster.Quickness = CalculateAndRound(monster.Quickness, 1.3);
                monster.ChanceToDodge = CalculateAndRound(monster.ChanceToDodge, 1.3);
                monster.CritChance = CalculateAndRound(monster.CritChance, 1.3);
                break;
            default:
                break;
        }
    }

    public static MonsterTemplate[] loadMonsterTemplates()
    {
        var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        string folderName = string.Format("{0}.templates.monsters", executingAssembly.GetName().Name);
        var h = executingAssembly
            .GetManifestResourceNames();

        var monsterTemplates = h
            .Where(r => r.StartsWith(folderName) && r.EndsWith(".json"))
            .Select(r => LoadAMonsterTemplate(r))
            .ToArray();

        return monsterTemplates;
    }

    static MonsterTemplate LoadAMonsterTemplate(string templateName)
    {
        var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        using (var stream = executingAssembly.GetManifestResourceStream(templateName))
        {
            if (null == stream)
            {
                throw new Exception($"template {templateName} not found");
            }

            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();

                var ret = JsonConvert.DeserializeObject<MonsterTemplate>(result);

                if (null == ret)
                {
                    throw new Exception($"failed to de-serialize template {templateName}");
                }

                return ret;

            }
        }
    }

    
}

public class DiscovereableTileState
{
    public DcxTiles thisTile { get; set; } = DcxTiles.unknown;

    public bool isDiscovered { get; set; }

    /// <summary>
    /// This is for us to keep track if we can enter the tile for the day
    /// </summary>
    public bool isActive(DbGameState gameState)
    {
        if (TileDto.BossTiles.Contains(thisTile))
        {
            return !gameState.inactiveBossTiles.Contains(thisTile);
        }
        else if (TileDto.DailyQuestTiles.Contains(thisTile))
        {
            return !gameState.inactiveDailyTiles.Contains(thisTile);
        }

        // everything else should be active.
        return true;
    }
}

public enum LoreEnum
{
    unknown,
    lore1,
    lore2,
    // Lore 3 and lore 4 are for library of the archmage. When you defeat the final boss, you will get that.
    lore3,
    lore4
}