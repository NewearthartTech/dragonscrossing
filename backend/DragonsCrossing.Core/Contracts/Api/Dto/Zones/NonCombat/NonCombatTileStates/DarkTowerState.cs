using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones
{
    public class DarkTowerState : NoncombatEncounterState
    {
        public DarkTowerState()
        {
            tilesToDiscoverState = new Dictionary<DcxTiles, DiscovereableTileState>
            {
                { DcxTiles.barracks, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.herbalist_darkTower, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.libraryOfTheArchmage, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.slaversRow, new DiscovereableTileState{ isDiscovered = false }}
            };

            loresPlayed = new Dictionary<LoreEnum, bool>
            {
                { LoreEnum.lore1, false},
                { LoreEnum.lore2, false},
                //Lore 3 and lore 4 are for library of the archmage. When you defeat the final boss, you will get that.
                { LoreEnum.lore3, false},
                { LoreEnum.lore4, false}
            };
        }
    }
}
