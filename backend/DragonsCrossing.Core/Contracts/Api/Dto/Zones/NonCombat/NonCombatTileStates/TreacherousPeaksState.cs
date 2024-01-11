using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones
{
    public class TreacherousPeaksState : NoncombatEncounterState
    {
        public TreacherousPeaksState()
        {
            tilesToDiscoverState = new Dictionary<DcxTiles, DiscovereableTileState>
            {
                //mountainFortress is always available
                { DcxTiles.griffonsNest, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.summonersSummit, new DiscovereableTileState{ isDiscovered = false }},

                //herbalist_treacherousPeaks is always available

                { DcxTiles.camp_treacherousPeaks, new DiscovereableTileState{ isDiscovered = false}},

                { DcxTiles.darkTower, new DiscovereableTileState{ isDiscovered = false }},
            };

            loresPlayed = new Dictionary<LoreEnum, bool>
            {
                { LoreEnum.lore1, false},
                { LoreEnum.lore2, false}
            };
        }
    }

    public class FallenTemplesState : NoncombatEncounterState
    {
        public FallenTemplesState()
        {
            tilesToDiscoverState = new Dictionary<DcxTiles, DiscovereableTileState>
            {
                //{ DcxTiles.blacksmith_fallenTemples, new DiscovereableTileState{ isDiscovered = false }},

                //pillaredRuins is always available

                { DcxTiles.acropolis, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.destroyedPantheon, new DiscovereableTileState{ isDiscovered = false }},
                //{ DcxTiles.blacksmith_fallenTemples, new DiscovereableTileState{ isDiscovered = false }},
                { DcxTiles.darkTower, new DiscovereableTileState{ isDiscovered = false }},
            };

            loresPlayed = new Dictionary<LoreEnum, bool>
            {
                { LoreEnum.lore1, false},
                { LoreEnum.lore2, false}
            };
        }
    }
}
