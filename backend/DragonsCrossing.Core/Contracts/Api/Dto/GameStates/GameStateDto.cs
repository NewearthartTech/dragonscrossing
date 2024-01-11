using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.GameStates;
using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Zones;
using MongoDB.Bson.Serialization.Attributes;
using DragonsCrossing.NewCombatLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace DragonsCrossing.Core.Contracts.Api.Dto.GameStates
{
    public interface ICombatEngine
    {
        Task<ActionResponseDto> attack(int heroId);

        Task<ActionResponseDto> applySkill(int heroId, DbGameState gameState);

        Task<ActionResponseDto> flee(int HeroId);

        Task<ActionResponseDto> persuade(int HeroId);

        Task<UpdateDefinition<DbGameState>> applyLingeringEffects(UpdateDefinition<DbGameState> setter, int heroId, DbGameState gameState);

        public ActionResponseDto? actionReponse { get; }
        DbGameState GameState { get; set; }

        bool isCombatOver { get; }

        bool isHeroDead { get; }
    }


    /// <summary>
    /// The current gamestate. Used only at the beginning of the game.
    /// </summary>
    public class GameStateDto
    {
        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        protected GameStateDto(){ }

        public GameStateDto(DbGameState gameState, ICombatEngine _combatEngine)
        {
            var encounters = gameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

            var combat = encounters?.Where(e => e.Monster != null).FirstOrDefault();

            if (null != combat)
            {
                _combatEngine.GameState = gameState;
            }

            Slug = gameState.CurrentTile;

            var discoveredTiles = gameState.getDiscoveredTiles();

            var undiscoveredTileCount =
                gameState.nonCombatTileState.TryGetValue(TileDto.ZoneFromTile(gameState.CurrentTile), out var states)
                ? gameState.nonCombatTileState[TileDto.ZoneFromTile(gameState.CurrentTile)]
                    .tilesToDiscoverState
                    .Where(t => t.Value.isDiscovered == false)
                    .Count()
                : 0;

            Zone = new ZoneDto
            {
                Slug = TileDto.ZoneFromTile(Slug),
                DiscoveredTiles = OrderDiscoverTile(discoveredTiles),
                UndiscoveredTileCount = undiscoveredTileCount
            };

            if (null != combat)
            {
                Encounters = null != _combatEngine.actionReponse ? new[] { _combatEngine.actionReponse } : new Encounter[] { };
            }
            else if (null != gameState.CurrentEncounters)
            {
                Encounters = gameState.CurrentEncounters;
            }


            CalculatedCharacterStats = gameState.Hero.GenerateCalculatedCharacterStats(gameState);

        }

        private static TileDto[] OrderDiscoverTile(TileDto[] tiles)
        {
            var dailyQuestTiles = tiles.Where(t => TileDto.DailyQuestTiles.Contains(t.Slug)).ToArray();

            var dailyQuestTilesSlugs = dailyQuestTiles.Select(t => t.Slug).ToArray();

            tiles = tiles.Where(t => !dailyQuestTilesSlugs.Contains(t.Slug)).ToArray();

            var bossTiles = tiles.Where(t => TileDto.BossTiles.Contains(t.Slug)).OrderBy(t => t.Slug).ToArray();

            var bossTilesSlugs = bossTiles.Select(t => t.Slug).ToArray();

            tiles = tiles.Where(t => !bossTilesSlugs.Contains(t.Slug)).ToArray();

            var campTiles = tiles.Where(t => t.Slug.ToString().Contains("camp")).ToArray();

            var campTilesSlugs = campTiles.Select(t => t.Slug).ToArray();

            tiles = tiles.Where(t => !campTilesSlugs.Contains(t.Slug)).ToArray();

            var adventuringGuildTiles = tiles.Where(t => t.Slug.ToString().Contains("adventuringGuild")).ToArray();

            var adventuringTilesSlugs = adventuringGuildTiles.Select(t => t.Slug).ToArray();

            tiles = tiles.Where(t => !adventuringTilesSlugs.Contains(t.Slug)).ToArray();

            var herbalistTiles = tiles.Where(t => t.Slug.ToString().Contains("herbalist")).ToArray();

            var herbalistTileSlugs = herbalistTiles.Select(t => t.Slug).ToArray();

            tiles = tiles.Where(t => !herbalistTileSlugs.Contains(t.Slug)).ToArray();

            var zoneTiles = tiles.Where(t => Enum.TryParse<DcxZones>(t.Slug.ToString(), out var zoneTile)).ToArray();

            var zoneTilesSlugs = zoneTiles.Select(t => t.Slug).ToArray();

            tiles = tiles.Where(t => !zoneTilesSlugs.Contains(t.Slug)).ToArray();

            if (tiles.Length > 0)
            {
                throw new Exception($"error missing tile type {string.Join(',',tiles.Select(t=>t.Slug))}");
            }

            return dailyQuestTiles.Concat(herbalistTiles).Concat(adventuringGuildTiles).Concat(campTiles).Concat(bossTiles).Concat(zoneTiles).ToArray();
        }

        /// <summary>
        /// The location where the Hero is
        /// </summary>
        [Required]
        public DcxTiles Slug { get; set; }

        [Required]
        public ZoneDto Zone { get; set; } = new ZoneDto();

        [BsonIgnore]
        public Encounter[] Encounters { get; set; } = new Encounter[] { };

        public CalculatedCharacterStats CalculatedCharacterStats { get; set; } = new CalculatedCharacterStats { };

    }
}
