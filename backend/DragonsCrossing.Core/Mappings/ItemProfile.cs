using AutoMapper;
using DragonsCrossing.Core.Contracts.Api.Dto.Armors;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Mappings
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<Weapon, ItemDto>()
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.WeaponTemplate.Name))
                //.ForMember(dest => dest.ItemClass, opt => opt.MapFrom(src => src.WeaponTemplate.HeroClass))
                ;
            //.ForMember(dest => dest.RequiredLevel, opt => opt.MapFrom(src => src.??))
            //.ForMember(dest => dest.PurchasePrice, opt => opt.MapFrom(src => src.??))
            //.ForMember(dest => dest.SellPrice, opt => opt.MapFrom(src => src.??))


            CreateMap<Armor, ItemDto>()
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.ArmorTemplate.Name))
                //.ForMember(dest => dest.ItemClass, opt => opt.MapFrom(src => src.ArmorTemplate.HeroClass))
                ;

            CreateMap<NftItem, NftItemDto>().ReverseMap();
            CreateMap<PlayerBackpack, PlayerBackpackDto>().ReverseMap();
            //.ForMember(dest => dest.Monster, opt => opt.MapFrom(src => src.Monster));
        }
    }
}
