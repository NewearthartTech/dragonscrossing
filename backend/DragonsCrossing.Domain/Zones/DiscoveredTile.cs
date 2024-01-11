using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Zones
{
    /// <summary>
    /// The hidden tiles the hero has discovered.    
    /// This is a many to many table between tiles and heroes.
    /// A hero can discover many tiles. A tile can be discovered by many heroes.
    /// </summary>
    public class DiscoveredTile
    {
        public int Id { get; set; }
        public int TileId { get; set; }
        public Tile Tile { get; set; }
        public int HeroId { get; set; }
        public Hero Hero { get; set; }

        /// <summary>
        /// True if hero has already passed/completed the tile.
        /// If this is true, the hero can't access the tile again.
        /// If this is boss tile it marks it as complete so hero can't play him again.
        /// If this is a daily tile, this will get reset to false at midnight so hero can play it the next day.
        /// If this is a regular combat tile or navigation tile, it should always be false.
        /// </summary>
        public bool IsComplete { get; set; }        
    }
}
