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


/// <summary>
/// zones are also included in DiscoveredTiles case Heros can go there
/// </summary>
public enum DcxTiles
{
    // zone wildPrairie (1)
    wildPrairie, enchantedFields, aedos,

    // zone aedos
    blacksmith, adventuringGuild, sharedStash, herbalist_aedos,

    // zone mysteriousForest (2)
    mysteriousForest, sylvanWoodlands, pilgrimsClearing, herbalist_mysteriousForest,

    // zone foulWastes (3)
    foulWastes, odorousBog, ancientBattlefield, terrorswamp, herbalist_foulWastes,

    // zone treacherousPeaks (4)
    treacherousPeaks, mountainFortress, griffonsNest, summonersSummit, herbalist_treacherousPeaks,

    // zone darkTower (5)
    darkTower, labyrinthianDungeon, barracks, slaversRow, herbalist_darkTower,

    //Zone darkTower final boss
    libraryOfTheArchmage,

    //added later
    camp_mysteriousForest, camp_treacherousPeaks,

    //more foulwaste
    adventuringGuild_foulWastes,


    //zone wondrousThicket (foulwaste_a) (3.1)
    //               odorousBog,  ancientBattlefield, terrorswamp,    herbalist_foulWastes,      adventuringGuild_foulWastes
    wondrousThicket, feyClearing, shatteredStable,    forebodingDale, herbalist_wondrousThicket, adventuringGuild_wondrousThicket,


    //zone fallenTemples (treacherousPeaks_a) (4.1)
    //                                       mountainFortress, griffonsNest, summonersSummit, herbalist_treacherousPeaks,
    fallenTemples, blacksmith_fallenTemples, pillaredRuins,    acropolis,    destroyedPantheon,

    unknown = 1000,

}
