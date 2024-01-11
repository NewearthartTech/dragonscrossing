using DragonsCrossing.Domain.GameStates;
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
        public UpdateDefinition<DbGameState> CreateWildPrairieNonCombatEncounter(DbGameState gamestate,
            DcxTiles newLocation, UpdateDefinition<DbGameState> setter)
        {
            var zone = DcxZones.wildPrairie;

            if (!gamestate.nonCombatTileState.TryGetValue(TileDto.ZoneFromTile(newLocation), out var state))
            {
                state = new WildPraireState();
            }

            var wildPraireState = state as WildPraireState;
            if (null == wildPraireState)
                throw new InvalidOperationException("we shold have wildPraireState here");

            var nonCombatencounters = new List<NonCombatEncounter>();

            // location encounter plus lore 1

            bool lore1Found = wildPraireState.loresPlayed.TryGetValue(LoreEnum.lore1, out var lore1isDone);
            bool lore2Found = wildPraireState.loresPlayed.TryGetValue(LoreEnum.lore2, out var lore2isDone);

            bool mysteriousForestStateFound = wildPraireState.tilesToDiscoverState.TryGetValue(DcxTiles.mysteriousForest, out var mysteriousForestTileState);

            if (mysteriousForestStateFound && null == mysteriousForestTileState)
            {
                throw new InvalidDataException($"WildPraire mysteriousForestTileState is not found");
            }

            if (null != mysteriousForestTileState && !mysteriousForestTileState.isDiscovered)
            {
                nonCombatencounters.Add(new LocationEncounter
                {
                    Slug = "wildPrairie",
                    newLocations = new[] { DcxTiles.mysteriousForest },
                    IntroText = "The homeworld of the elves, orcs, and other lesser races dabbled in magic frequently. They are exiles here as much as Aedos is, cast out for their hubris.",
                    NarratedText = "The forest which rings the wild prairie is lush and wondrous. The trees look unfamiliar, and some at the edge appear to be dying. When you step in, you hear curious languages whispered in the leaves, and the wind brings the smell of adventure. The forest must have come from a world with fey and unruly creatures. You catch flashes of them wherever you step."
                });

                if (lore1Found && !lore1isDone)
                {
                    nonCombatencounters
                        .Add(GetWildPrairieLoreEncounter("The feeling you get from this traveler is strange, even for this new frontier of Horizon. He motions for you to come talk to him. \"Not from around here, are you?\" he asks, not expecting an answer. \"Call me Acheron, and ask me one question; I'll answer truthfully. Others of my order may not be so kind, but I think it only fair that I start you off on your journey with the right information.\" Something about this traveler reeks with power, and you cannot even consider attacking him.", LoreEnum.lore1));
                }
                else 
                {
                    nonCombatencounters.Add(GetRandomChanceEncounter());
                }
            }
            else if (lore1Found && !lore1isDone)
            {
                nonCombatencounters
                        .Add(GetWildPrairieLoreEncounter("The feeling you get from this traveler is strange, even for this new frontier of Horizon. He motions for you to come talk to him. \"Not from around here, are you?\" he asks, not expecting an answer. \"Call me Acheron, and ask me one question; I'll answer truthfully. Others of my order may not be so kind, but I think it only fair that I start you off on your journey with the right information.\" Something about this traveler reeks with power, and you cannot even consider attacking him.", LoreEnum.lore1));
                nonCombatencounters.Add(GetRandomChanceEncounter());

            }

            // second lore
            else if (lore2Found && !lore2isDone)
            {

                nonCombatencounters
                    .Add(GetWildPrairieLoreEncounter("You spot Acheron in the distance. You wave, and he does not wave back, but he motions you to come closer. You do. \"I am busy,\" he says, \"but not too busy to share some information with a wanderer such as yourself. I can sympathize with a person placed in a world they are not from. Ask me one question, and I will be happy to answer.\" You are intimidated by the power rolling off of him.", LoreEnum.lore2));
                nonCombatencounters.Add(GetRandomChanceEncounter());
            }

            else
            {
                nonCombatencounters.Add(GetRandomChanceEncounter());
            }

            //This line is important as this adds the state to the db if it doesn't exit yet. 
            gamestate.nonCombatTileState[zone] = wildPraireState;

            //update the noncombatstate
            setter = setter.Set(g => g.CurrentEncounters, nonCombatencounters.ToArray())
                .Set(g => g.nonCombatTileState, gamestate.nonCombatTileState);

            return setter;
        }

        public LoreEncounter GetWildPrairieLoreEncounter(string narratedText, LoreEnum loreNumber)
        {
            return new LoreEncounter
            {
                Slug = "wildPrairie",
                NarratedText = narratedText,
                Dialogues = new LoreDialog[]
                {
                    new LoreDialog
                    {
                        Slug = "5",
                        QuestionText = "What is this \"order\" you speak of and what is their purpose?"
                    },
                    new LoreDialog
                    {
                        Slug = "6",
                        QuestionText = "What do you know about the magical calamity that ripped Aedos from its home and placed it here in Horizon?"
                    },
                    new LoreDialog
                    {
                        Slug = "7",
                        QuestionText = "Is there something I should be particularly careful of while I'm off adventuring?"
                    },
                },
                LoreNumber = loreNumber
            };
        }
    }
}
