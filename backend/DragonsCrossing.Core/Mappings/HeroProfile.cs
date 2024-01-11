using AutoMapper;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Heroes;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Mappings
{
    public class HeroProfile : Profile
    {
        public const string COMBAT_KEY = "combat";
        public HeroProfile()
        {
            CreateMap<Hero, HeroDto>()
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.HeroName.Name))
                .ForMember(dest => dest.level, opt => opt.MapFrom(src => src.Level.Number))
                .ForMember(dest => dest.maxExperiencePoints, opt => opt.MapFrom(src => src.Level.MaxExperiencePoints))
                .ForMember(dest => dest.experiencePoints, opt => opt.MapFrom(src => src.CombatStats.ExperiencePoints))
                .BeforeMap((src, dest, context) => context.Mapper.Map(src.CombatStats, dest)) // map combat stats to hero. Use BeforeMap instead of AfterMap to preserve the Hero.Id.
                ;

            CreateMap<HeroCombatStats, HeroDto>()
                .ForMember(dest => dest.wisdom, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items[COMBAT_KEY] != null
                    ? new HeroCombatStatsCalculated((Combat)context.Items[COMBAT_KEY]).GetMaxWisdom()
                    : src.MaxWisdom))
                .ForMember(dest => dest.charisma, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items[COMBAT_KEY] != null
                    ? new HeroCombatStatsCalculated((Combat)context.Items[COMBAT_KEY]).GetMaxCharisma()
                    : src.MaxCharisma))
                .ForMember(dest => dest.quickness, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items[COMBAT_KEY] != null
                    ? new HeroCombatStatsCalculated((Combat)context.Items[COMBAT_KEY]).GetMaxQuickness()
                    : src.MaxQuickness))
                .ForMember(dest => dest.strength, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items[COMBAT_KEY] != null
                    ? new HeroCombatStatsCalculated((Combat)context.Items[COMBAT_KEY]).GetStrength()
                    : src.MaxStrength))
                .ForMember(dest => dest.agility, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items[COMBAT_KEY] != null
                    ? new HeroCombatStatsCalculated((Combat)context.Items[COMBAT_KEY]).GetAgility()
                    : src.MaxAgility))
                ;
        }
    }
}
