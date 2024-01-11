using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Zones
{
    public class ZoneValidator
    {
        private readonly Hero hero;

        public ZoneValidator(Hero hero)
        {
            this.hero = hero;
        }

        /// <summary>
        /// Will throw an exception if the hero can't access the tile
        /// </summary>
        /// <param name="tileToAccess"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public void VerifyHeroCanAccessTile(Tile tileToAccess, List<DiscoveredTile> discoveredTiles)
        {
            if (tileToAccess == null)
                throw new ArgumentNullException(nameof(tileToAccess));
            
            // verify hero level
            var isHeroLevelWithinLimits = hero.Level.Number >= tileToAccess.HeroLevelRequired.Min && hero.Level.Number <= tileToAccess.HeroLevelRequired.Max;
            if (!isHeroLevelWithinLimits)            
                throw new Exception($"Hero is not allowed to access tile: {tileToAccess.Id}");

            // verify the hero has discovered the tile
            if (discoveredTiles == null)
                throw new Exception("Unable to retrieve discovered tiles. Please Contact support.");
            var discoveredTile = discoveredTiles.FirstOrDefault(d => d.TileId == tileToAccess.Id);
            if (discoveredTile == null)
                throw new Exception($"Hero: {hero.Id} has not discovered the tile yet: {tileToAccess.Id}");

            // verify the tile is not complete yet (for boss and daily tiles)
            if (discoveredTile.IsComplete)
                throw new Exception($"Hero: {hero.Id} has already completed tile: {tileToAccess.Id}");

            // yes hero can access tile
            return;
        }        
    }
}
