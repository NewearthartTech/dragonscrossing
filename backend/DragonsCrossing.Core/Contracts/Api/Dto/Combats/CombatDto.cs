using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public class CombatDto
    {  
        public int Id { get; set; }

        /// <summary>
        /// This is the randomly generated Monster
        /// </summary>  
        public MonsterDto Monster { get; set; }

        /// <summary>
        /// Keeps track of users hitpoints as well as what stats changed during combat
        /// </summary>
        public HeroDto Hero { get; set; }

        /// <summary>
        /// The items and gold dropped after killing a monster
        /// </summary>
        public CombatLootDto? CombatLoot { get; set; }

        [Required]
        public int TileId { get; set; }
        
        public List<CombatDetailDto> CombatDetails { get; set; }

        /// <summary>
        /// This should get set at the end of combat.
        /// If this is true at the start of combat then this combat record will get deleted.
        /// </summary>
        public bool IsCombatOver { get; set; }

        /// <summary>
        /// Used to determine whether to raise or lower a monsters stats
        /// </summary>  
        public bool IsCombatOpportunityAvailable { get; set; }

        /// <summary>
        /// Used to determine whether hero gets an automatic win,
        /// or if monster gets a free hit.
        /// </summary>
        public bool IsCharismaOpportunityAvailable { get; set; }

        public OpportunityResultTypeEnum? CombatOpportunityResult { get; set; }

        public OpportunityResultTypeEnum? CharismaOpportunityResult { get; set; }

        /// <summary>
        /// The CombatResult must have a value for this to be set.
        /// True if the user saw the loot they earned after combat.
        /// If this is false, and CombatResult != null then load loot screen.
        /// </summary>    
        public bool UserConfirmedCombatEnd { get; set; }

        /// <summary>
        /// The round we are on
        /// </summary> 
        public int Round { get; set; } = 1;
        
        public bool IsHeroDead { get; set; }
       
        public bool IsMonsterDead { get; set; }       
    }
}
