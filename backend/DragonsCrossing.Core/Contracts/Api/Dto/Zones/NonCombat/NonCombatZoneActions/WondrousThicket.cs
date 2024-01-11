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
        public UpdateDefinition<DbGameState> CreateWondrousThicketNonCombatEncounter(DbGameState gamestate, DcxTiles newLocation, UpdateDefinition<DbGameState> setter)
        {
            var tile_slug = "wondrousThicket";
            var herbalistTile = DcxTiles.herbalist_wondrousThicket;
            var dailyQuestTile = DcxTiles.shatteredStable;
            var bossTile = DcxTiles.forebodingDale;
            var adventuringGuildTile = DcxTiles.adventuringGuild_wondrousThicket;

            var zone = DcxZones.wondrousThicket;

            if (!gamestate.nonCombatTileState.TryGetValue(TileDto.ZoneFromTile(newLocation), out var state))
            {
                state = new WondrousThicketState();
            }

            var zoneState = state as WondrousThicketState;
            if (null == zoneState)
                throw new InvalidOperationException("we should have WondrousThicketState here");

            var nonCombatencounters = new List<NonCombatEncounter>();

            // location encounter plus lore 1

            bool lore1Found = zoneState.loresPlayed.TryGetValue(LoreEnum.lore1, out var lore1isDone);
            bool lore2Found = zoneState.loresPlayed.TryGetValue(LoreEnum.lore2, out var lore2isDone);

            bool herbalistStateFound = zoneState.tilesToDiscoverState.TryGetValue(herbalistTile, out var herbalistTileState);

            bool dailyQuestTileStateFound = zoneState.tilesToDiscoverState.TryGetValue(dailyQuestTile, out var dailyQuestTileState);

            bool bossQuestStateFound = zoneState.tilesToDiscoverState.TryGetValue(bossTile, out var bossQuestTileState);

            bool adventuringGuildStateFound = zoneState.tilesToDiscoverState.TryGetValue(adventuringGuildTile, out var adventuringGuildTileState);

            bool nextTravelTilesStateFound = zoneState.tilesToDiscoverState.TryGetValue(DcxTiles.treacherousPeaks, out var nextTravelTilesState);

            if (herbalistStateFound && null == herbalistTileState)
            {
                throw new InvalidDataException($"herbalistTileState is not found");
            }

            if (dailyQuestTileStateFound && null == dailyQuestTileState)
            {
                throw new InvalidDataException($"dailyQuestTileState is not found");
            }

            if (bossQuestStateFound && null == bossQuestTileState)
            {
                throw new InvalidDataException($"bossQuestTileState is not found");
            }

            if (adventuringGuildStateFound && null == adventuringGuildTileState)
            {
                throw new InvalidDataException($"adventuringGuildTileState is not found");
            }

            if (nextTravelTilesStateFound && null == nextTravelTilesState)
            {
                throw new InvalidDataException($"nextTravelTilesState is not found");
            }

            if (null != herbalistTileState && !herbalistTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = tile_slug,
                    newLocations = new[] { herbalistTile },
                    IntroText = "",
                    NarratedText = ""
                });

                if (lore1Found && !lore1isDone)
                {
                    nonCombatencounters
                    .Add(GetWoundrousThicketThicketLoreEncounter("\"A little off the beaten path?\" Acheron asks warmly. \"Be wary, adventurer. This place contains creatures that may teach you more about getting stronger, but I fear the magical energy of Horizon is warped here. When questing here, if your health falls too low [a fifth of your total], my brother Cocytus will attempt to bring you safely back to Aedos.\"", LoreEnum.lore1));
                }
                else
                {
                    nonCombatencounters.Add(GetRandomChanceEncounter());
                }
            }
            else if (null != dailyQuestTileState && !dailyQuestTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = tile_slug,
                    newLocations = new[] { dailyQuestTile },
                    IntroText = "",
                    NarratedText = ""
                });

                if (lore2Found && !lore2isDone)
                {
                    nonCombatencounters
                    .Add(GetWoundrousThicketThicketLoreEncounter("Acheron spots you from a distance as you struggle through another weirdly growing set of vines. \"Hello and well met again, adventurer! Wandering this way will still take you to your ultimate goal, I imagine, but it can be more dangerous. Perhaps my brother has whisked you back to Aedos when your health was low? He can be a busybody.\"", LoreEnum.lore2));
                }
                else
                {
                    nonCombatencounters.Add(GetRandomChanceEncounter());
                }
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

                nonCombatencounters.Add(GetRandomChanceEncounter());
            }
            else if (null != adventuringGuildTileState && !adventuringGuildTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "adventuringGuild",
                    newLocations = new[] { adventuringGuildTile },
                    IntroText = "",
                    NarratedText = ""
                }); ;

                nonCombatencounters.Add(GetRandomChanceEncounter());
            }
            else if (null != nextTravelTilesState && !nextTravelTilesState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = tile_slug,
                    newLocations = new[] { DcxTiles.fallenTemples },
                    IntroText = "Abaddon raised these peaks himself as a test of power and will. He can bend the land itself to his will, a feat totally unheard of in any world. The creatures that dwell here suffer under the unnatural bend of nature.",
                    NarratedText = "Bafflingly, these peaks feel new. You feel a tug at your spirit indicating that you should not be here. The air itself seems to taste wrong."
                });

                nonCombatencounters.Add(GetRandomChanceEncounter());
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

        public LoreEncounter GetWoundrousThicketThicketLoreEncounter(string narratedText, LoreEnum loreNumber)
        {
            return new LoreEncounter
            {
                Slug = "wondrousThicket",
                NarratedText = narratedText,
                Dialogues = new LoreDialog[]
                {
                    new LoreDialog
                    {
                        Slug = "20",
                        QuestionText = "Why would a brother of yours bring me to Aedos?"
                    },
                    new LoreDialog
                    {
                        Slug = "21",
                        QuestionText = "What is this place?"
                    },
                    new LoreDialog
                    {
                        Slug = "22",
                        QuestionText = "I have heard tale of other adventurers, and even seen a few on my travels. Are they also heading toward Abaddon?"
                    },
                },
                LoreNumber = loreNumber
            };
        }
    }
}