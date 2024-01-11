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
        public UpdateDefinition<DbGameState> CreateMysteriousForestNonCombatEncounter(DbGameState gamestate, DcxTiles newLocation, UpdateDefinition<DbGameState> setter)
        {
            var zone = DcxZones.mysteriousForest;

            if (!gamestate.nonCombatTileState.TryGetValue(TileDto.ZoneFromTile(newLocation), out var state))
            {
                state = new MysteriousForestState();
            }

            var mysteriousForestState = state as MysteriousForestState;
            if (null == mysteriousForestState)
                throw new InvalidOperationException("we shold have mysteriousForestState here");

            var nonCombatencounters = new List<NonCombatEncounter>();

            // location encounter plus lore 1

            bool lore1Found = mysteriousForestState.loresPlayed.TryGetValue(LoreEnum.lore1, out var lore1isDone);
            bool lore2Found = mysteriousForestState.loresPlayed.TryGetValue(LoreEnum.lore2, out var lore2isDone);

            bool campStateFound = mysteriousForestState.tilesToDiscoverState.TryGetValue(DcxTiles.camp_mysteriousForest, out var campTileState);

            bool pilgrimsClearingStateFound = mysteriousForestState.tilesToDiscoverState.TryGetValue(DcxTiles.pilgrimsClearing, out var pilgrimsClearingTileState);

            bool foulWastesStateFound = mysteriousForestState.tilesToDiscoverState.TryGetValue(DcxTiles.foulWastes, out var foulWastesTileState);

            if (campStateFound && null == campTileState)
            {
                throw new InvalidDataException($"MysteriousForest campTileState is not found");
            }

            if (pilgrimsClearingStateFound && null == pilgrimsClearingTileState)
            {
                throw new InvalidDataException($"MysteriousForest pilgrimsClearingTileState is not found");
            }

            if (foulWastesStateFound && null == foulWastesTileState)
            {
                throw new InvalidDataException($"MysteriousForest foulWastesTileState is not found");
            }

            if (null != campTileState && !campTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "camp",
                    newLocations = new[] { DcxTiles.camp_mysteriousForest },
                    IntroText = "",
                    NarratedText = ""
                });

                if (lore1Found && !lore1isDone)
                {
                    nonCombatencounters
                    .Add(GetMysteriousForestLoreEncounter("Acheron seems to get around, because you think you've spotted his distinctive yellow cloak a few times from the corner of your eye as you trek through the forest. He seems to be resting his eyes, but they snap open as you get closer. \"Still alive, then?\" he asks good naturedly. \"The forest air does wonders for my old lungs. Does it restore you as well?\" He chuckles to himself. \"But, asking you questions isn't the game. Come, ask me another.\"", LoreEnum.lore1));
                }
                else
                {
                    nonCombatencounters.Add(GetRandomChanceEncounter());
                }
            }
            else if (null != pilgrimsClearingTileState && !pilgrimsClearingTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "mysteriousForest",
                    newLocations = new[] { DcxTiles.pilgrimsClearing },
                    IntroText = "",
                    NarratedText = ""
                });

                if (lore2Found && !lore2isDone)
                {
                    nonCombatencounters
                    .Add(GetMysteriousForestLoreEncounter("You're sifting through your bag as you walk when Acheron appears silently at your side, startling you. \"The forest holds many wonders, for the curious, as does Horizon itself. We walk only a small part of it. Other adventurers, Travelers, and evildoers walk beside us, whether we know it or not.\" He seems to be talking to himself, mostly. You raise an eyebrow, and he smiles, but the smile does not reach his eyes. \"Another question?\"", LoreEnum.lore2));
                }
                else
                {
                    nonCombatencounters.Add(GetRandomChanceEncounter());
                }
            }

            else if (null != foulWastesTileState && !foulWastesTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "mysteriousForest",
                    newLocations = new[] { DcxTiles.foulWastes, DcxTiles.wondrousThicket },
                    IntroText = "A dark elven city-state was transported to the swamp at least a century ago, but it sank into the swamp, and with it, the hope of the dark elves forced to live on Horizon.",
                    NarratedText = "A noxious gas emanates from the ground, staining your clothes and hair with its scent. You will doubtless get used to it, but that thought is not comforting. Something has gone very wrong here."
                });

                nonCombatencounters.Add(GetRandomChanceEncounter());
            }
            else
            {
                nonCombatencounters.Add(GetRandomChanceEncounter());
            }

            //This line is important as this adds the state to the db if it doesn't exit yet. 
            gamestate.nonCombatTileState[zone] = mysteriousForestState;

            //update the noncombatstate
            setter = setter.Set(g => g.CurrentEncounters, nonCombatencounters.ToArray())
                .Set(g => g.nonCombatTileState, gamestate.nonCombatTileState);

            return setter;
        }

        public LoreEncounter GetMysteriousForestLoreEncounter(string narratedText, LoreEnum loreNumber)
        {
            return new LoreEncounter
            {
                Slug = "mysteriousForest",
                NarratedText = narratedText,
                Dialogues = new LoreDialog[]
                {
                    new LoreDialog
                    {
                        Slug = "8",
                        QuestionText = "What are Travelers?"
                    },
                    new LoreDialog
                    {
                        Slug = "9",
                        QuestionText = "Who is responsible for Aedos being transported to Horizon?"
                    },
                    new LoreDialog
                    {
                        Slug = "10",
                        QuestionText = "What is the history of these prairies and forests near Aedos?"
                    },
                },
                LoreNumber = loreNumber
            };
        }
    }
}