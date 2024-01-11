using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones
{
    public class WildPraireState : NoncombatEncounterState
    {
        public WildPraireState()
        {
            tilesToDiscoverState = new Dictionary<DcxTiles, DiscovereableTileState>
            {
                { DcxTiles.mysteriousForest, new DiscovereableTileState{ isDiscovered = false }}
            };

            loresPlayed = new Dictionary<LoreEnum, bool>
            {
                { LoreEnum.lore1, false},
                { LoreEnum.lore2, false}
            };
        }
    }
}
