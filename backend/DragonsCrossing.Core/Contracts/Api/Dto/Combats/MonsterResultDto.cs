using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public class MonsterResultDto
    {
        [Required]
        public MonsterDto monster { get; set; } = new MonsterDto();

        [Required]
        public bool isDead { get; set; }

        // The attact result of the monster and hero needs to be seperated. Right now seems like only the final result is in the combat object.
        [Required]
        public MonsterAttackResultDto attackResult { get; set; } = new MonsterAttackResultDto();

        /// <summary>
        /// Loot will only exist after Hero won
        /// </summary>
        public MonsterLootDto? loot { get; set; }
    }
}
