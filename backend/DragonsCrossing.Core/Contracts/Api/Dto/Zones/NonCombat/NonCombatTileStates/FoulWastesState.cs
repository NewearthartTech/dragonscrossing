using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones
{
    public class FoulWastesState : NoncombatEncounterState
    {
        public FoulWastesState()
        {
            tilesToDiscoverState = new Dictionary<DcxTiles, DiscovereableTileState>
            {
                //odorousBog is always available
                { DcxTiles.ancientBattlefield, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.terrorswamp, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.herbalist_foulWastes, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.adventuringGuild_foulWastes, new DiscovereableTileState{ isDiscovered = false }},

                //zones
                { DcxTiles.treacherousPeaks, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.fallenTemples, new DiscovereableTileState{ isDiscovered = false }},
            };

            loresPlayed = new Dictionary<LoreEnum, bool>
            {
                { LoreEnum.lore1, false},
                { LoreEnum.lore2, false}
            };
        }
    }

    public class WondrousThicketState : NoncombatEncounterState
    {
        public WondrousThicketState()
        {
            tilesToDiscoverState = new Dictionary<DcxTiles, DiscovereableTileState>
            {
                //feyClearing is always available
                { DcxTiles.shatteredStable, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.forebodingDale, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.herbalist_wondrousThicket, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.adventuringGuild_wondrousThicket, new DiscovereableTileState{ isDiscovered = false }},

                //zones
                { DcxTiles.treacherousPeaks, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.fallenTemples, new DiscovereableTileState{ isDiscovered = false }},
            };

            loresPlayed = new Dictionary<LoreEnum, bool>
            {
                { LoreEnum.lore1, false},
                { LoreEnum.lore2, false}
            };
        }
    }
}
