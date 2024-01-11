using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Domain.Zones;
using DragonsCrossing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Repositories
{
    public class ZonesRepository : IZonesRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ZonesRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<Zone>> GetZones()
        {
            return await dbContext.Zones
                .Include(z => z.Tiles)
                .ToListAsync();
        }

        public async Task<Tile> GetTile(int tileId)
        {
            return await dbContext.Tiles.FindAsync(tileId);
        }

        /// <summary>
        /// Gets a tile by the zone order and tile order.
        /// Ex: Get the first tile in the first zone.
        /// </summary>
        /// <param name="zoneOrderNumber"></param>
        /// <param name="tileOrderNumber"></param>
        /// <returns></returns>
        public async Task<Tile> GetTile(int zoneOrderNumber, int tileOrderNumber)
        {
            var firstZone = await dbContext.Zones
                .Include(z => z.Tiles)
                .FirstOrDefaultAsync(z => z.Order == zoneOrderNumber);
            return firstZone.Tiles.FirstOrDefault(t => t.Order == tileOrderNumber);
        }

        public async Task<List<Tile>> GetTilesByZone(int zoneId)
        {
            return await dbContext.Tiles.Where(x => x.ZoneId == zoneId).ToListAsync();
        }

        public async Task<Zone> GetZone(int zoneId)
        {
            return await dbContext.Zones
                .Where(z => z.Id == zoneId)
                .Include(z => z.Tiles)
                .SingleOrDefaultAsync();
        }

        public async Task<Zone> GetZoneByOrder(int orderNumber)
        {
            return await dbContext.Zones.SingleAsync(z => z.Order == orderNumber);
        }

        public async Task<List<DiscoveredTile>> GetDiscoveredTiles(int heroId)
        {
            return await dbContext.DiscoveredTiles
                .Where(t => t.HeroId == heroId)
                .ToListAsync();
        }

        public async Task<int> CreateDiscoveredTile(DiscoveredTile tile)
        {
            if (tile.Id != 0)
            {
                throw new Exception($"Hero: {tile.HeroId} already discovered this tile: {tile.TileId}");
            }
            await dbContext.DiscoveredTiles.AddAsync(tile);
            await dbContext.SaveChangesAsync();
            return tile.Id;
        }

        public async Task DeleteDiscoveredTile(int disoveredTileId)
        {
            var tileToDelete = await dbContext.DiscoveredTiles.FirstOrDefaultAsync(t => t.Id == disoveredTileId);
            if (tileToDelete == null)
                throw new Exception($"Unable to delete discovered tile: {disoveredTileId}. Tile does not exist.");
            dbContext.DiscoveredTiles.Remove(tileToDelete);
            await dbContext.SaveChangesAsync();
        }
    }
}
