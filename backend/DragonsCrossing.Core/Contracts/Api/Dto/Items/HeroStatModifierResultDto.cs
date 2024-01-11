using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Items
{
    public class HeroStatModifierResultDto
    {
        public int Id { get; set; }

        public int HeroStatEffectedId { get; set; }

        /// <summary>
        /// The types of stats that are effected by the weapon or armor
        /// </summary>
        public AffectedHeroStatTypeDto StatType { get; set; }

        /// <summary>
        /// The amount to add/remove from the hero's stat.
        /// </summary>
        public int EffectAmount { get; set; }
    }
}
