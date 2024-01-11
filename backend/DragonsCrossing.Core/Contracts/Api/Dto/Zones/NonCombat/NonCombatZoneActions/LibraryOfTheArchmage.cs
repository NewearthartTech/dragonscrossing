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
    public partial class TileDto
    {
        static public UpdateDefinition<DbGameState> CreateLibraryOfTheArchmageNonCombatEncounter(DbGameState gamestate, DcxTiles newLocation)
        {
            if (!gamestate.nonCombatTileState.TryGetValue(ZoneFromTile(newLocation), out var state))
            {
                throw new Exception("darkTower nonCombatTileState shouldn't be null");
            }

            var darkTowerState = state as DarkTowerState;
            if (null == darkTowerState)
                throw new InvalidOperationException("we shold have darkTowerState here");

            var nonCombatencounters = new List<NonCombatEncounter>();

            // location encounter plus lore 1

            bool finalBossLore1Found = darkTowerState.loresPlayed.TryGetValue(LoreEnum.lore3, out var finalBossLore1isDone);
            bool finalBossLore2Found = darkTowerState.loresPlayed.TryGetValue(LoreEnum.lore4, out var finalBossLore2isDone);


            if (finalBossLore1Found && !finalBossLore1isDone)
            {
                nonCombatencounters
                .Add(new BossEncounter
                {
                    Slug = "abaddon-outro",
                    NarratedText = "And thus you have destroyed me based on nothing but rumor and feeling, Aedosian. You have slain me. Me! A creature older than your race. In so-called righteous anger. Look in the mirror, Aedosian. Yes, I meddled with Aedos. But I was only permitted to because of the greed and power-hunger of the powerful humans who live in your city. Aedosians who now seek to bend this world to their will as I have. And who is that behind you? Acheron, my brother, the famous meddler. A fence-sitter. I go on to the dark in front of you, Brother. I doubt our other siblings will leave Horizon alone after what has happened here.",

                });
            }

            if (finalBossLore2Found && !finalBossLore2isDone)
            {
                nonCombatencounters
                .Add(new BossEncounter
                {
                    Slug = "acheron-outro",
                    NarratedText = "Close my brother’s eyes for him, child of Aedos. His worries are over. But the evil he has done remains. The powerful and for now unnamed of Aedos invited him in and caused your land to be ripped from its reality. Abaddon is vanquished, but I think he was right. Our family will not enjoy hearing of what has transpired on Horizon. There are fearful days ahead. Dragon’s Crossing will awaken from its slumber. \r\nYou have bested a great danger to Horizon and brought some semblance of safety to this small corner of your new home. Still, you must be wary of threats from within, and all those still left without. You have come a long way and done much good. For that alone, you shall be remembered. Go now. Rest. There will be much to do in the days to come.",

                });
            }

            return Builders<DbGameState>.Update
                .Set(g => g.CurrentEncounters, nonCombatencounters.ToArray());

        }
    }
}