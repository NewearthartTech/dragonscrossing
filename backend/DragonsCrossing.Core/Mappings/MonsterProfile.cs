using AutoMapper;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Mappings
{
    public class MonsterProfile : Profile
    {
        public MonsterProfile()
        {
            //CreateMap<MonsterTemplateOld, MonsterDto>();

            CreateMap<Monster, MonsterDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.MonsterTemplateOld.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.MonsterTemplateOld.Description))
                //.ForMember(dest => dest.ArmorDescription, opt => opt.MapFrom(src => src.MonsterTemplateOld.ArmorDescription))
                //.ForMember(dest => dest.WeaponDescription, opt => opt.MapFrom(src => src.MonsterTemplateOld.WeaponDescription))
                //.ForMember(dest => dest.SpecialAbilitiesUsed, opt => opt.MapFrom(src => src.SpecialAbilityEffects))
                .ForMember(dest => dest.PersonalityType, opt => opt.MapFrom(src => src.Personality.PersonalityType))
                //.ConstructUsing((src, context) => context.Mapper.Map<MonsterDto>(src.SpecialAbilityEffects))
                .ForMember(dest => dest.SpecialAbility, opt => opt.MapFrom(src => MapSpecialAbility(src)))
                .ForMember(dest => dest.ChanceToDodge, opt => opt.MapFrom(src => src.DodgeRate))
                .ForMember(dest => dest.ChanceToHit, opt => opt.MapFrom(src => src.HitRate))
                //.ForMember(dest => dest.sp, opt => opt.MapFrom(src => src.Personality.PersonalityType))
                //.ForMember(dest => dest.SpecialAbilitiesUsed, opt => opt.MapFrom(src => src.MonsterTemplateOld.SpecialAbilities))
                ;

            //CreateMap<MonsterSpecialAbilityEffect, MonsterSpecialAbilityDto>();
            //CreateMap<MonsterSpecialAbility, MonsterSpecialAbilityDto>()
            //    .ForMember(dest => dest.AudioUrl, opt => opt.MapFrom(src => src.???))
            //    .ForMember(dest => dest.IsAttackModifier, opt => opt.MapFrom(src => src.???))
            //    .ForMember(dest => dest.Damage, opt => opt.MapFrom(src => src.???))
            //    .ForMember(dest => dest.Effects, opt => opt.MapFrom(src => src.???))

            //CreateMap<???, MonsterItemDto>(); // What do we map this to???

        }

        private List<MonsterSpecialAbilityDto> MapSpecialAbility(Monster src)
        {
            var dtos = new List<MonsterSpecialAbilityDto>();
            if (src.SpecialAbilitiesCasted == null)
            {
                return dtos;
            }

            foreach (var specialAbility in src.SpecialAbilitiesCasted)
            {
                //dtos.Add(new MonsterSpecialAbilityDto()
                //{
                //    // We need to remap this object. Need to talk to Chad and Brandon and see how we can map this for the new MonsterSpecialAbility properties
                //    Amount = specialAbility.Effect.Amount,
                //    EffectWho = specialAbility.Effect.EffectWho,
                //    TurnDuration = specialAbility.Effect.TurnDuration,
                //    Type = specialAbility.Effect.Type,
                //    Name = src.MonsterTemplateOld.SpecialAbilities.Single(s => s.Effects.Contains(specialAbility.Effect)).Name,
                //    Description = src.MonsterTemplateOld.SpecialAbilities.Single(s => s.Effects.Contains(specialAbility.Effect)).Name,
                //});
            }
            return dtos;
        }
    }
}
