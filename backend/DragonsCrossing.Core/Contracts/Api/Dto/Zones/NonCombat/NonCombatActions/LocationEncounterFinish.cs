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
    async Task LocationEncounterFinish<T>(ILogger logger,
       ISeasonsDbService _db,
       int heroId,
       string encounterId,
       DcxZones zone,
       LocationEncounter locationEncounter
       ) where T : NoncombatEncounterState
    {
        var collection = _db.getCollection<DbGameState>();
        var gameState = await _db.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        //var newLocation = locationEncounter.NewLocation;

        var noncombatTileState = gameState.nonCombatTileState[zone] as T;

        if (null == noncombatTileState)
        {
            throw new InvalidDataException($" {nameof(noncombatTileState)} {typeof(T)} not found");
        }

        if (null == gameState.CurrentEncounters)
        {
            throw new InvalidDataException("CurrentEncounters should not be null here");
        }

        var titlesDiscovered =false;

        foreach (var newLocation in locationEncounter.newLocations)
        {
            if (noncombatTileState.tilesToDiscoverState.ContainsKey(newLocation))
            {
                noncombatTileState.tilesToDiscoverState[newLocation].isDiscovered = true;
                noncombatTileState.tilesToDiscoverState[newLocation].thisTile = newLocation;
                titlesDiscovered = true;
            }
        }

        if (titlesDiscovered)
        {
            gameState.nonCombatTileState[zone] = noncombatTileState;

            gameState.CurrentEncounters = gameState.CurrentEncounters.Where(e => e.id != encounterId).ToArray();

            var setter = Builders<DbGameState>.Update
                .Set(g => g.CurrentEncounters, gameState.CurrentEncounters)
                .Set(g => g.nonCombatTileState, gameState.nonCombatTileState);

            await collection.UpdateOneAsync(g => g.HeroId == heroId, setter);
        }
    }
}
