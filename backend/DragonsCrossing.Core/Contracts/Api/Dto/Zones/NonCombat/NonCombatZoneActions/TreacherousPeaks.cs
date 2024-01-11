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
        public UpdateDefinition<DbGameState> CreateTreacherousPeaksNonCombatEncounter(DbGameState gamestate, DcxTiles newLocation, UpdateDefinition<DbGameState> setter)
        {
            var zone = DcxZones.treacherousPeaks;

            if (!gamestate.nonCombatTileState.TryGetValue(TileDto.ZoneFromTile(newLocation), out var state))
            {
                state = new TreacherousPeaksState();
            }

            var treacherousPeaksState = state as TreacherousPeaksState;
            if (null == treacherousPeaksState)
                throw new InvalidOperationException("we shold have treacherousPeaksState here");

            var nonCombatencounters = new List<NonCombatEncounter>();

            // location encounter plus lore 1

            bool lore1Found = treacherousPeaksState.loresPlayed.TryGetValue(LoreEnum.lore1, out var lore1isDone);
            bool lore2Found = treacherousPeaksState.loresPlayed.TryGetValue(LoreEnum.lore2, out var lore2isDone);

            bool campStateFound = treacherousPeaksState.tilesToDiscoverState.TryGetValue(DcxTiles.camp_treacherousPeaks, out var campTileState);

            bool griffonsNestStateFound = treacherousPeaksState.tilesToDiscoverState.TryGetValue(DcxTiles.griffonsNest, out var griffonsNestTileState);

            bool summonersSummitStateFound = treacherousPeaksState.tilesToDiscoverState.TryGetValue(DcxTiles.summonersSummit, out var summonersSummitTileState);

            bool darkTowerStateFound = treacherousPeaksState.tilesToDiscoverState.TryGetValue(DcxTiles.darkTower, out var darkTowerTileState);

            if (campStateFound && null == campTileState)
            {
                throw new InvalidDataException($"TreacherousPeaks campTileState is not found");
            }

            if (griffonsNestStateFound && null == griffonsNestTileState)
            {
                throw new InvalidDataException($"TreacherousPeaks griffonsNestTileState is not found");
            }

            if (summonersSummitStateFound && null == summonersSummitTileState)
            {
                throw new InvalidDataException($"TreacherousPeaks summonersSummitTileState is not found");
            }

            if (darkTowerStateFound && null == darkTowerTileState)
            {
                throw new InvalidDataException($"TreacherousPeaks darkTowerTileState is not found");
            }

            if (null != campTileState && !campTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "camp",
                    newLocations = new[] { DcxTiles.camp_treacherousPeaks },
                    IntroText = "",
                    NarratedText = ""
                });

                if (lore1Found && !lore1isDone)
                {
                    nonCombatencounters
                    .Add(GetTreacherousPeaksLoreEncounter("I can definitely climb that cliff face. That was your last concious thought before waking up in a cave with a lump on your head. Looking around you see Acheron sitting cross legged in the center of the cave with his eyes closed. \"That was quite a fall. Lucky for you the cougar that was stalking you was open to negotiation.\"", LoreEnum.lore1));
                }
                else
                {
                    nonCombatencounters.Add(GetRandomChanceEncounter());
                }
            }
            else if (null != griffonsNestTileState && !griffonsNestTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "treacherousPeaks",
                    newLocations = new[] { DcxTiles.griffonsNest },
                    IntroText = "",
                    NarratedText = ""
                });

                if (lore2Found && !lore2isDone)
                {
                    nonCombatencounters
                    .Add(GetTreacherousPeaksLoreEncounter("I can definitely climb that cliff face. That was your last concious thought before waking up in a cave with a lump on your head. Looking around you see Acheron sitting cross legged in the center of the cave with his eyes closed. \"That was quite a fall. Lucky for you the cougar that was stalking you was open to negotiation.\"", LoreEnum.lore2));
                }
                else
                {
                    nonCombatencounters.Add(GetRandomChanceEncounter());
                }
            }

            else if (null != summonersSummitTileState && !summonersSummitTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "treacherousPeaks",
                    newLocations = new[] { DcxTiles.summonersSummit },
                    IntroText = "",
                    NarratedText = ""
                });

                nonCombatencounters.Add(GetRandomChanceEncounter());
            }
            else if (null != darkTowerTileState && !darkTowerTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "treacherousPeaks",
                    newLocations = new[] { DcxTiles.darkTower },
                    IntroText = "The dark tower is made from material from Dragons Crossing itself. Bringing it to Horizon and crafting with it required an ego both monumental and capable. If Abaddon dwells anywhere, it is here.",
                    NarratedText = "You've heard some disturbing things about Abaddon, but this Dark Tower reeks of evil and control. Echoing screams issue forth from its depths intermittnetly. If Abaddon is anywhere, it is here."
                });

                nonCombatencounters.Add(GetRandomChanceEncounter());
            }
            else
            {
                nonCombatencounters.Add(GetRandomChanceEncounter());
            }

            //This line is important as this adds the state to the db if it doesn't exit yet. 
            gamestate.nonCombatTileState[zone] = treacherousPeaksState;

            //update the noncombatstate
            setter = setter.Set(g => g.CurrentEncounters, nonCombatencounters.ToArray())
                .Set(g => g.nonCombatTileState, gamestate.nonCombatTileState);

            return setter;
        }

        public LoreEncounter GetTreacherousPeaksLoreEncounter(string narratedText, LoreEnum loreNumber)
        {
            return new LoreEncounter
            {
                Slug = "treacherousPeaks",
                NarratedText = narratedText,
                Dialogues = new LoreDialog[]
                {
                    new LoreDialog
                    {
                        Slug = "14",
                        QuestionText = "As I get farther from Aedos, I feel a strange corruption growing in the air. Is this Abaddon?"
                    },
                    new LoreDialog
                    {
                        Slug = "15",
                        QuestionText = "These peaks feel new, somehow. How can this be?"
                    },
                    new LoreDialog
                    {
                        Slug = "16",
                        QuestionText = "My distance from Aedos leaves me free to wonder if any Aedosians served in our untimley transportation to Horizon. Do you know anything else about this?"
                    },
                },
                LoreNumber = loreNumber
            };
        }
    }
}