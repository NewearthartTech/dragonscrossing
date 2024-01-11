using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public class CombatDetailDto
    {
        [Required]
        public CombatDetailTypeDto Type { get; set; }

        /// <summary>
        /// Did the combat state succeed
        /// </summary>
        [Required]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The value calculated in each state.
        /// Ex: Damage dealt, or parry percent, or special ability damage
        /// </summary>
        [Required]
        public double Amount { get; set; }

        /// <summary>
        /// The order the states flowed
        /// </summary>
        [Required]
        public int Order { get; set; }
    }
}
