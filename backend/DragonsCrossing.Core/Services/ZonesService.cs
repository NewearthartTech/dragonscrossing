using AutoMapper;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Core.Zones;
using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Services
{
    public class ZonesService : IZonesService
    {
        private readonly IZonesRepository zonesRepository;
        private readonly IHeroesRepository heroesRepository;
        private readonly IMapper mapper;

        public ZonesService(
            IAuthorizationService authorizationService,
            IPlayersRepository playersRepository,
            IZonesRepository zonesRepository, 
            IHeroesRepository heroesRepository, 
            IMapper mapper)            
        {
            this.zonesRepository = zonesRepository;
            this.heroesRepository = heroesRepository;
            this.mapper = mapper;
        }

        public async Task<List<ZoneDto>> GetZones()
        {
            var zones = await zonesRepository.GetZones();
            return mapper.Map<List<ZoneDto>>(zones);
        }

        public async Task<ZoneDto> GetZone(int zoneId)
        {            
            var zone = await zonesRepository.GetZone(zoneId);            
            return mapper.Map<ZoneDto>(zone);
        }

        public async Task<List<ZoneDto>> GetDiscoveredZonesAndTiles(int heroId)
        {
            var discoveredTiles = await zonesRepository.GetDiscoveredTiles(heroId);
            var discoveredZones = (await zonesRepository.GetZones())
                .Where(z => z.Tiles.Select(t => t.Id)
                .Intersect(discoveredTiles.Select(d => d.TileId))
                .Any());
            return mapper.Map<List<ZoneDto>>(discoveredZones);           
        }        
    }
}
