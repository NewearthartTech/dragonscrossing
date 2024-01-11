using DragonsCrossing.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Zones
{
    /// <summary>
    /// It's been decided the UI will hard code everything about the tile. The only thing used  by the UI is the Tile.Id.
    /// The UI will map/hard code the tileId to the corresponding UI elements.
    /// This is probably only temporary since the UI should use everything here.
    /// </summary>
    public class Tile
    {
        public int Id { get; set; }
        
        /// <summary>
        /// NOTE: currently not being used by UI
        /// </summary>
        public string Name { get; set; }

        public Zone Zone { get; set; }

        public int ZoneId { get; set; }
        
        /// <summary>
        /// Quest, Move (to next zone whether town or another zone), Boss, Hero
        /// NOTE: currently not being used by UI
        /// </summary>
        public virtual TileType Type { get; set; }

        /// <summary>
        /// The order the tiles are in - usually in difficulty order.
        /// This is used to determine which hidden tile is made visible next.
        /// NOTE: currently not being used by UI
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The range of levels the hero must be in to enter the tile.
        /// </summary>
        public virtual RangeInt HeroLevelRequired { get; set; }
    }
}
