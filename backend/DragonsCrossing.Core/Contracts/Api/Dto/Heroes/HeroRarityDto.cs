using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Heroes
{
    public enum HeroRarityDto
    {
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        Legendary = 4,
        Mythic = 5,

        /// <summary>
        /// dee-eric-review we should NOT have Unknown
        /// </summary>
        Unknown = 6,
    }
}
