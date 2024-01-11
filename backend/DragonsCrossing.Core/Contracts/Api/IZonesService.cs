using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api
{
    public interface IZonesService
    {
        Task<ZoneDto> GetZone(int zoneId);
        Task<List<ZoneDto>> GetZones();
        Task<List<ZoneDto>> GetDiscoveredZonesAndTiles(int heroId);
    }
}
