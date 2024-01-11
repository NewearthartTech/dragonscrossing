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
        public UpdateDefinition<DbGameState> CreateFoulWastesNonCombatEncounter(DbGameState gamestate, DcxTiles newLocation, UpdateDefinition<DbGameState> setter)
        {
            var zone = DcxZones.foulWastes;

            if (!gamestate.nonCombatTileState.TryGetValue(TileDto.ZoneFromTile(newLocation), out var state))
            {
                state = new FoulWastesState();
            }

            var foulWastesState = state as FoulWastesState;
            if (null == foulWastesState)
                throw new InvalidOperationException("we shold have foulWastesState here");

            var nonCombatencounters = new List<NonCombatEncounter>();

            // location encounter plus lore 1

            bool lore1Found = foulWastesState.loresPlayed.TryGetValue(LoreEnum.lore1, out var lore1isDone);
            bool lore2Found = foulWastesState.loresPlayed.TryGetValue(LoreEnum.lore2, out var lore2isDone);

            bool herbalistStateFound = foulWastesState.tilesToDiscoverState.TryGetValue(DcxTiles.herbalist_foulWastes, out var herbalistTileState);

            bool ancientBattlefieldStateFound = foulWastesState.tilesToDiscoverState.TryGetValue(DcxTiles.ancientBattlefield, out var ancientBattlefieldTileState);

            bool terrorswampStateFound = foulWastesState.tilesToDiscoverState.TryGetValue(DcxTiles.terrorswamp, out var terrorswampTileState);

            bool adventuringGuildStateFound = foulWastesState.tilesToDiscoverState.TryGetValue(DcxTiles.adventuringGuild_foulWastes, out var adventuringGuildTileState);

            bool treacherousPeaksStateFound = foulWastesState.tilesToDiscoverState.TryGetValue(DcxTiles.treacherousPeaks, out var treacherousPeaksTileState);

            if (herbalistStateFound && null == herbalistTileState)
            {
                throw new InvalidDataException($"FoulWastes herbalistTileState is not found");
            }

            if (ancientBattlefieldStateFound && null == ancientBattlefieldTileState)
            {
                throw new InvalidDataException($"FoulWastes ancientBattlefieldTileState is not found");
            }

            if (terrorswampStateFound && null == terrorswampTileState)
            {
                throw new InvalidDataException($"FoulWastes terrorswampTileState is not found");
            }

            if (adventuringGuildStateFound && null == adventuringGuildTileState)
            {
                throw new InvalidDataException($"FoulWastes adventuringGuildTileState is not found");
            }

            if (treacherousPeaksStateFound && null == treacherousPeaksTileState)
            {
                throw new InvalidDataException($"FoulWastes treacherousPeaksTileState is not found");
            }

            if (null != herbalistTileState && !herbalistTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "foulWastes",
                    newLocations = new[] { DcxTiles.herbalist_foulWastes },
                    IntroText = "",
                    NarratedText = ""
                });

                if (lore1Found && !lore1isDone)
                {
                    nonCombatencounters
                    .Add(GetFoulWastesLoreEncounter("You are staring at your fire watching the flames lick at the logs when you feel someone watching you. Turning around you see Acheron sitting quietly with tears streaming down his face. \"Oh how Horizon has fallen! This used to be a place full of life and light. Now foul creatures prowl these swamps. Abbadon used to speak about the restoration of these lands. Of course, that was when he went by a different name...\"", LoreEnum.lore1));
                }
                else
                {
                    nonCombatencounters.Add(GetRandomChanceEncounter());
                }
            }
            else if (null != ancientBattlefieldTileState && !ancientBattlefieldTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "foulWastes",
                    newLocations = new[] { DcxTiles.ancientBattlefield },
                    IntroText = "",
                    NarratedText = ""
                });

                if (lore2Found && !lore2isDone)
                {
                    nonCombatencounters
                    .Add(GetFoulWastesLoreEncounter("After a long day of adventuring, you reach in your bag to find that your rations have fallen out along the way. Resigning yourself to a hard night you begin to set up camp when you smell something delicious in the air. Following the scent you find Acheron sitting by a fire roasting squirrels. \"Hungry?\" he asks, grinning at you.", LoreEnum.lore2));
                }
                else
                {
                    nonCombatencounters.Add(GetRandomChanceEncounter());
                }
            }

            else if (null != terrorswampTileState && !terrorswampTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "foulWastes",
                    newLocations = new[] { DcxTiles.terrorswamp },
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
                    newLocations = new[] { DcxTiles.adventuringGuild_foulWastes },
                    IntroText = "",
                    NarratedText = ""
                }); ;

                nonCombatencounters.Add(GetRandomChanceEncounter());
            }
            else if (null != treacherousPeaksTileState && !treacherousPeaksTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "foulWastes",
                    newLocations = new[] { DcxTiles.treacherousPeaks, DcxTiles.fallenTemples },
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
            gamestate.nonCombatTileState[zone] = foulWastesState;

            //update the noncombatstate
            setter = setter.Set(g => g.CurrentEncounters, nonCombatencounters.ToArray())
                .Set(g => g.nonCombatTileState, gamestate.nonCombatTileState);

            return setter;
        }

        public LoreEncounter GetFoulWastesLoreEncounter(string narratedText, LoreEnum loreNumber)
        {
            return new LoreEncounter
            {
                Slug = "foulWastes",
                NarratedText = narratedText,
                Dialogues = new LoreDialog[]
                {
                    new LoreDialog
                    {
                        Slug = "11",
                        QuestionText = "These foul wastes hold some ancient story, surely?"
                    },
                    new LoreDialog
                    {
                        Slug = "12",
                        QuestionText = "How are you able to wander around dangerous areas without a weapon, or care?"
                    },
                    new LoreDialog
                    {
                        Slug = "13",
                        QuestionText = "What is Abaddon's purpose?"
                    },
                },
                LoreNumber = loreNumber
            };
        }
    }
}