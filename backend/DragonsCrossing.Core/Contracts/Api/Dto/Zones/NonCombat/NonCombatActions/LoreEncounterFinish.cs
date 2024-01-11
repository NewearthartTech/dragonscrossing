using System;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Domain.GameStates;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones;

public partial class TileDto
{
    async Task LoreEncounterFinish<T>(ILogger logger,
        ISeasonsDbService _db,
        int heroId,
        string encounterId,
        DcxZones zone
        ) where T : NoncombatEncounterState
    {
        var collection = _db.getCollection<DbGameState>();
        var gameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        var nonCombatTileState = gameState.nonCombatTileState[zone] as T;

        if (null == nonCombatTileState)
        {
            throw new InvalidDataException($" {nameof(nonCombatTileState)} {typeof(T)} not found");
        }

        bool lore1Found = nonCombatTileState.loresPlayed.TryGetValue(LoreEnum.lore1, out var lore1Played);

        bool lore2Found = nonCombatTileState.loresPlayed.TryGetValue(LoreEnum.lore2, out var lore2Played);

        bool lore3Found = nonCombatTileState.loresPlayed.TryGetValue(LoreEnum.lore3, out var lore3Played);

        bool lore4Found = nonCombatTileState.loresPlayed.TryGetValue(LoreEnum.lore4, out var lore4Played);

        // Change the dictionary value
        if (lore1Found && !lore1Played)
        {
            nonCombatTileState.loresPlayed[LoreEnum.lore1] = true;
        }
        else if (lore2Found && !lore2Played)
        {
            nonCombatTileState.loresPlayed[LoreEnum.lore2] = true;
        }
        else if (lore3Found && !lore3Played)
        {
            nonCombatTileState.loresPlayed[LoreEnum.lore3] = true;
        }
        else if (lore4Found && !lore4Played)
        {
            nonCombatTileState.loresPlayed[LoreEnum.lore4] = true;
        }
        else
        {
            throw new InvalidDataException("All lores have been played");
        }

        if (null == gameState.CurrentEncounters)
        {
            throw new InvalidDataException("CurrentEncounters should not be null here");
        }

        //Remove the encounter from CurrentEncounters by id
        gameState.CurrentEncounters = gameState.CurrentEncounters.Where(e => e.id != encounterId).ToArray();

        //Update the nonCombatTileState
        gameState.nonCombatTileState[zone] = nonCombatTileState;

        var setter = Builders<DbGameState>.Update.Set(g => g.CurrentEncounters, gameState.CurrentEncounters)
            .Set(g => g.nonCombatTileState, gameState.nonCombatTileState);

        await collection.UpdateOneAsync(g => g.HeroId == heroId, setter);
    }
}