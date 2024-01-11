using AutoMapper;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Domain.Combats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Mappings
{
    public class CombatProfile : Profile
    {
        public CombatProfile()
        {
            CreateMap<Combat, ActionResponseDto>()
                .ForMember(dest => dest.monsterResult, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.heroResult, opt => opt.MapFrom(src => src));
            //.ForMember(dest => dest.Monster, opt => opt.MapFrom(src => src.Monster));

            CreateMap<Combat, HeroResultDto>()
               .ForMember(dest => dest.isDead, opt => opt.MapFrom(src => src.IsHeroDead))
               .ForMember(dest => dest.attackResult, opt => opt.MapFrom(src => src))
               ;
            //.ForMember(dest => dest.AttackResult, opt => opt.MapFrom(src => src.???))
            //.ForMember(dest => dest.specialAbilityResult, opt => opt.MapFrom(src => src.???));

            CreateMap<Combat, MonsterResultDto>()
                .ForMember(dest => dest.isDead, opt => opt.MapFrom(src => src.IsMonsterDead));
            //.ForMember(dest => dest.AttackResult, opt => opt.MapFrom(src => src.???))
            //.ForMember(dest => dest.specialAbilityResult, opt => opt.MapFrom(src => src.???));

            CreateMap<Combat, AttackResultDto>()
               .ForMember(dest => dest.IsHit, opt => opt.MapFrom(src => src.CombatDetails.FirstOrDefault(d => d.Type == CombatDetailType.TryToHit).IsSuccess))
               .ForMember(dest => dest.CritDamage, opt => opt.MapFrom(src => src.CombatDetails.FirstOrDefault(d => d.Type == CombatDetailType.CriticalHit && d.IsSuccess == true).Amount))
               .ForMember(dest => dest.ArmorMitigation, opt => opt.MapFrom(src => src.CombatDetails.FirstOrDefault(d => d.Type == CombatDetailType.ArmorMitigation && d.IsSuccess == true).Amount))
               ;

            CreateMap<CombatDetail, CombatDetailDto>();

            CreateMap<CombatLoot, CombatLootDto>();

        }
    }
}
