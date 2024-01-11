using AutoMapper;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Mappings
{
    public class ZoneProfile : Profile
    {
        public ZoneProfile()
        {
            CreateMap<Zone, ZoneDto>();
            //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.HeroName.Name));

            //CreateMap<Tile, TileDto>()
            //    .ForMember(dest => dest.RequiredMinLevel, opt => opt.MapFrom(src => src.HeroLevelRequired.Min))
            //    .ForMember(dest => dest.RequiredMaxLevel, opt => opt.MapFrom(src => src.HeroLevelRequired.Max));
            //.ForMember(dest => dest.IsActive, opt => opt.MapFrom(???))
        }
    }
}
