using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Monsters;
using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Combats
{
    /// <summary>
    /// This is the combat state that is saved to the db so we can reload it
    /// if the user quits in the middle of combat. 
    /// There should only be one of these ever per hero.
    /// </summary>
    /// <remarks>Only create properties that are needed to re-load the state or that are store</remarks>
    public class Combat
    {
        public int Id { get; set; }

        /// <summary>
        /// The tile this combat took place in.
        /// NOTE: changing this to tileId may remove the FK
        /// </summary>
        public int TileId { get; set; }

        public Tile Tile { get; set; }

        /// <summary>
        /// This is the randomly generated Monster.
        /// </summary>
        public Monster Monster { get; set; }

        /// <summary>
        /// Keeps track of users hitpoints as well as what stats changed during combat
        /// </summary>
        public Hero Hero { get; set; }

        /// <summary>
        /// The items and gold dropped after killing a monster.
        /// TODO: Confirm with Eric, but there's a chance no loot will be found at the end of combat.
        /// </summary>
        public CombatLoot? CombatLoot { get; set; }

        /// <summary>
        /// The states and calculated values of each phase of combat
        /// </summary>
        public List<CombatDetail> CombatDetails { get; set; }

        public List<HeroSkillCasted>? HeroSkillsCasted { get; set; }

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

        public OpportunityResultType? CombatOpportunityResult { get; set; }

        public OpportunityResultType? CharismaOpportunityResult { get; set; }

        /// <summary>
        /// The CombatResult must have a value for this to be set.
        /// True if the user saw the loot they earned after combat.
        /// If this is false, and CombatResult != null then load loot screen.
        /// </summary>
        public bool UserConfirmedCombatEnd { get; set; }

        /// <summary>
        /// The turn we are on
        /// </summary>
        public int Round { get; set; } = 1;
        public bool IsHeroDead { get; set; }
        public bool IsMonsterDead { get; set; }
    }
}
