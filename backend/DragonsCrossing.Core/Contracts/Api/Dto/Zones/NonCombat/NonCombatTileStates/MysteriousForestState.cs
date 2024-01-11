using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones
{
    public class MysteriousForestState : NoncombatEncounterState
    {
        public MysteriousForestState()
        {
            tilesToDiscoverState = new Dictionary<DcxTiles, DiscovereableTileState>
            {
                { DcxTiles.camp_mysteriousForest, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.pilgrimsClearing, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.foulWastes, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.wondrousThicket, new DiscovereableTileState{ isDiscovered = false }},
            };

            loresPlayed = new Dictionary<LoreEnum, bool>
            {
                { LoreEnum.lore1, false},
                { LoreEnum.lore2, false}
            };
        }
    }
}
