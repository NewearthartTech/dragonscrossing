using DragonsCrossing.Domain.Zones;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones
{
    public partial class NonCombatHelpers
    {
        public UpdateDefinition<DbGameState> CreateFallenTemplesNonCombatEncounter(DbGameState gamestate, DcxTiles newLocation, UpdateDefinition<DbGameState> setter)
        {
            var tile_slug = "fallenTemples";
            //var herbalistTile = DcxTiles.herbalist_treacherousPeaks;
            var dailyQuestTile = DcxTiles.acropolis;
            var bossTile = DcxTiles.destroyedPantheon;
            //var adventuringGuildTile = DcxTiles.adventuringGuild_fallenTemples;

            var blackSmithTile = DcxTiles.blacksmith_fallenTemples;

            var zone = DcxZones.fallenTemples;

            if (!gamestate.nonCombatTileState.TryGetValue(TileDto.ZoneFromTile(newLocation), out var state))
            {
                state = new FallenTemplesState();
            }

            var zoneState = state as FallenTemplesState;
            if (null == zoneState)
                throw new InvalidOperationException("we should have FallenTemplesState here");

            var nonCombatencounters = new List<NonCombatEncounter>();

            // location encounter plus lore 1

            bool lore1Found = zoneState.loresPlayed.TryGetValue(LoreEnum.lore1, out var lore1isDone);
            bool lore2Found = zoneState.loresPlayed.TryGetValue(LoreEnum.lore2, out var lore2isDone);

            //bool herbalistStateFound = zoneState.tilesToDiscoverState.TryGetValue(herbalistTile, out var herbalistTileState);

            bool dailyQuestTileStateFound = zoneState.tilesToDiscoverState.TryGetValue(dailyQuestTile, out var dailyQuestTileState);

            bool bossQuestStateFound = zoneState.tilesToDiscoverState.TryGetValue(bossTile, out var bossQuestTileState);

            //bool adventuringGuildStateFound = zoneState.tilesToDiscoverState.TryGetValue(adventuringGuildTile, out var adventuringGuildTileState);

            bool blackSmithStateFound = zoneState.tilesToDiscoverState.TryGetValue(blackSmithTile, out var blackSmithTileState);

            bool nextTravelTilesStateFound = zoneState.tilesToDiscoverState.TryGetValue(DcxTiles.darkTower, out var nextTravelTilesState);

            /*
            if (herbalistStateFound && null == herbalistTileState)
            {
                throw new InvalidDataException($"herbalistTileState is not found");
            }
            */

            if (blackSmithStateFound && null == blackSmithTileState)
            {
                throw new InvalidDataException($"blackSmithStateFound is not found");
            }

            if (dailyQuestTileStateFound && null == dailyQuestTileState)
            {
                throw new InvalidDataException($"dailyQuestTileState is not found");
            }

            if (bossQuestStateFound && null == bossQuestTileState)
            {
                throw new InvalidDataException($"bossQuestTileState is not found");
            }

            /*
            if (adventuringGuildStateFound && null == adventuringGuildTileState)
            {
                throw new InvalidDataException($"adventuringGuildTileState is not found");
            }
            */

            if (nextTravelTilesStateFound && null == nextTravelTilesState)
            {
                throw new InvalidDataException($"nextTravelTilesState is not found");
            }

            /*
            if (null != herbalistTileState && !herbalistTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = tile_slug,
                    newLocations = new[] { herbalistTile },
                    IntroText = "",
                    NarratedText = ""
                });
            }
            else */


            if (null != dailyQuestTileState && !dailyQuestTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = tile_slug,
                    newLocations = new[] { dailyQuestTile },
                    IntroText = "",
                    NarratedText = ""
                });

            }

            else if (null != blackSmithTileState && !blackSmithTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = tile_slug,
                    newLocations = new[] { blackSmithTile },
                    IntroText = "",
                    NarratedText = ""
                });

            }

            else if (null != bossQuestTileState && !bossQuestTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = tile_slug,
                    newLocations = new[] { bossTile },
                    IntroText = "",
                    NarratedText = ""
                });

            }

            /*else if (null != adventuringGuildTileState && !adventuringGuildTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "adventuringGuild",
                    newLocations = new[] { adventuringGuildTile },
                    IntroText = "",
                    NarratedText = ""
                }); ;

            }*/


            else if (null != nextTravelTilesState && !nextTravelTilesState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = tile_slug,
                    newLocations = new[] { DcxTiles.darkTower},
                    IntroText = "The dark tower is made from material from Dragons Crossing itself. Bringing it to Horizon and crafting with it required an ego both monumental and capable. If Abaddon dwells anywhere, it is here.",
                    NarratedText = "You've heard some disturbing things about Abaddon, but this Dark Tower reeks of evil and control. Echoing screams issue forth from its depths intermittnetly. If Abaddon is anywhere, it is here."
                });

                
            }


            if (lore1Found && !lore1isDone)
            {
                nonCombatencounters
                .Add(GetFallenTemplesLoreEncounter("You spot Acheron leaning against a pillar, as if he owns the place. \"These temples contain echoes of deities, strange and powerful. Be very careful here! Resting after exploring here will not restore your energy,\" he says solemnly. \"If your health falls too low, Cocytus will probably push you back to Aedos, willing or not.\"", LoreEnum.lore1));
            }
            else if (lore2Found && !lore2isDone)
            {
                nonCombatencounters
                .Add( GetFallenTemplesLoreEncounter("Farewell!\" Acheron calls to someone disappearing behind a tree with a spritely giggle. You only saw a flash, but what you did see was breathtakingly beautiful. \"Ah, my favorite adventurer, how are your travels? Good, I hope?\"", LoreEnum.lore2));
            }
            else
            {
                nonCombatencounters.Add(GetRandomChanceEncounter());
            }

            //This line is important as this adds the state to the db if it doesn't exit yet. 
            gamestate.nonCombatTileState[zone] = zoneState;

            //update the noncombatstate
            setter = setter.Set(g => g.CurrentEncounters, nonCombatencounters.ToArray())
                .Set(g => g.nonCombatTileState, gamestate.nonCombatTileState);

            return setter;
        }

         public LoreEncounter GetFallenTemplesLoreEncounter(string narratedText, LoreEnum loreNumber)
        {
            return new LoreEncounter
            {
                Slug = "fallenTemples",
                NarratedText = narratedText,
                Dialogues = new LoreDialog[]
                {
                    new LoreDialog
                    {
                        Slug = "23",
                        QuestionText = "How was this place brought to Horizon?"
                    },
                    new LoreDialog
                    {
                        Slug = "24",
                        QuestionText = "Why are you always wandering around Horizon?"
                    },
                    new LoreDialog
                    {
                        Slug = "25",
                        QuestionText = "Is Abaddon truly evil?"
                    },
                },
                LoreNumber = loreNumber
            };
        }
    }
}