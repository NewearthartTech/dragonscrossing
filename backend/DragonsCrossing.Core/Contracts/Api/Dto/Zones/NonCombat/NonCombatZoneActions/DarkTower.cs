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
        //TODO Fix this
        public UpdateDefinition<DbGameState> CreateDarkTowerNonCombatEncounter(DbGameState gamestate, DcxTiles newLocation, UpdateDefinition<DbGameState> setter)
        {
            var zone = DcxZones.darkTower;

            if (!gamestate.nonCombatTileState.TryGetValue(TileDto.ZoneFromTile(newLocation), out var state))
            {
                state = new DarkTowerState();
            }

            var darkTowerState = state as DarkTowerState;
            if (null == darkTowerState)
                throw new InvalidOperationException("we shold have darkTowerState here");

            var nonCombatencounters = new List<NonCombatEncounter>();

            // location encounter plus lore 1

            bool lore1Found = darkTowerState.loresPlayed.TryGetValue(LoreEnum.lore1, out var lore1isDone);
            bool lore2Found = darkTowerState.loresPlayed.TryGetValue(LoreEnum.lore2, out var lore2isDone);

            bool herbalistStateFound = darkTowerState.tilesToDiscoverState.TryGetValue(DcxTiles.herbalist_darkTower, out var herbalistTileState);

            bool barracksStateFound = darkTowerState.tilesToDiscoverState.TryGetValue(DcxTiles.barracks, out var barracksTileState);

            bool slaversRowStateFound = darkTowerState.tilesToDiscoverState.TryGetValue(DcxTiles.slaversRow, out var slaversRowTileState);

            bool libraryOfTheArchmageStateFound = darkTowerState.tilesToDiscoverState.TryGetValue(DcxTiles.libraryOfTheArchmage, out var libraryOfTheArchmageTileState);

            if (herbalistStateFound && null == herbalistTileState)
            {
                throw new InvalidDataException($"DarkTower herbalistTileState is not found");
            }

            if (barracksStateFound && null == barracksTileState)
            {
                throw new InvalidDataException($"DarkTower barracksTileState is not found");
            }

            if (slaversRowStateFound && null == slaversRowTileState)
            {
                throw new InvalidDataException($"DarkTower slaversRowTileState is not found");
            }

            if (libraryOfTheArchmageStateFound && null == libraryOfTheArchmageTileState)
            {
                throw new InvalidDataException($"DarkTower libraryOfTheArchmageTileState is not found");
            }

            if (null != herbalistTileState && !herbalistTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "darkTower",
                    newLocations = new[] { DcxTiles.herbalist_darkTower },
                    IntroText = "",
                    NarratedText = ""
                });

                if (lore1Found && !lore1isDone)
                {
                    nonCombatencounters
                    .Add(GetDarkTowerLoreEncounter("A familiar face slides out from behind yet another dark cell. It is Acheron, here again to reward you with the answer to one question.", LoreEnum.lore1));
                }
                else
                {
                    nonCombatencounters.Add(GetRandomChanceEncounter());
                }
            }
            else if (null != barracksTileState && !barracksTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "darkTower",
                    newLocations = new[] { DcxTiles.barracks },
                    IntroText = "",
                    NarratedText = ""
                });

                if (lore2Found && !lore2isDone)
                {
                    nonCombatencounters
                    .Add(GetDarkTowerLoreEncounter("Acheron's distinctive yellow cloak stands out starkly in the Dark Tower. He looks somber, but hopeful. \"Another question, perhaps?\"", LoreEnum.lore2));
                }
                else
                {
                    nonCombatencounters.Add(GetRandomChanceEncounter());
                }
            }

            else if (null != slaversRowTileState && !slaversRowTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "darkTower",
                    newLocations = new[] { DcxTiles.slaversRow },
                    IntroText = "",
                    NarratedText = ""
                });

                nonCombatencounters.Add(GetRandomChanceEncounter());
            }
            else if (null != libraryOfTheArchmageTileState && !libraryOfTheArchmageTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "darkTower",
                    newLocations = new[] { DcxTiles.libraryOfTheArchmage },
                    IntroText = "",
                    NarratedText = ""
                });

                nonCombatencounters.Add(GetRandomChanceEncounter());
            }
            else
            {
                nonCombatencounters.Add(GetRandomChanceEncounter());
            }

            //This line is important as this adds the state to the db if it doesn't exit yet. 
            gamestate.nonCombatTileState[zone] = darkTowerState;

            //update the noncombatstate
            setter = setter.Set(g => g.CurrentEncounters, nonCombatencounters.ToArray())
                .Set(g => g.nonCombatTileState, gamestate.nonCombatTileState);

            return setter;
        }

        public LoreEncounter GetDarkTowerLoreEncounter(string narratedText, LoreEnum loreNumber)
        {
            return new LoreEncounter
            {
                Slug = "darkTower",
                NarratedText = narratedText,
                Dialogues = new LoreDialog[]
                {
                    new LoreDialog
                    {
                        Slug = "17",
                        QuestionText = "I cannot help but feel you are following me, Acheron. Why are you here?"
                    },
                    new LoreDialog
                    {
                        Slug = "18",
                        QuestionText = "Why does anyone serve Abaddon?"
                    },
                    new LoreDialog
                    {
                        Slug = "19",
                        QuestionText = "Must I resist Abaddon?"
                    },
                },
                LoreNumber = loreNumber
            };
        }
    }
}