using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Infrastructure
{
    /// <summary>
    /// Repo for zones, tiles, and discoveredTiles.
    /// </summary>
    public interface IZonesRepository
    {
        Task<List<Zone>> GetZones();
        Task<Zone> GetZone(int zoneId);
        Task<Tile> GetTile(int tileId);
        Task<Tile> GetTile(int zoneOrderNumber, int tileOrderNumber);
        Task<List<Tile>> GetTilesByZone(int zoneId);
        Task<Zone> GetZoneByOrder(int orderNumber);
        Task<List<DiscoveredTile>> GetDiscoveredTiles(int heroId);
        Task<int> CreateDiscoveredTile(DiscoveredTile tile);
        Task DeleteDiscoveredTile(int disoveredTileId);
    }
}
