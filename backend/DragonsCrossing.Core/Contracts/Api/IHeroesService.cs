using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Services;
using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api
{
    public interface IHeroesService
    {
        Task<HeroDto> GetHero(int heroId, bool perpetualOnly = false);
    }
}
