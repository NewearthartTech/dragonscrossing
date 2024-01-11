using AutoMapper;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Domain.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Mappings
{
    public class PlayerProfile : Profile
    {
        public PlayerProfile()
        {
            CreateMap<Player, PlayerDto>().ReverseMap();
            //CreateMap<Player.ChildIncludes, PlayerDto.ChildIncludes>();
                //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src..Name));
        }
    }
}
