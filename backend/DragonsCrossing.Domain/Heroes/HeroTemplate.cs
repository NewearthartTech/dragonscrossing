using DragonsCrossing.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Heroes
{
    /// <summary>
    /// HeroMint. These are the rules to generate a new random hero.
    /// </summary>
    /// <remarks>
    /// We will probably only have 1 of these records in the db. We could have 1 record per rarity.
    /// If we only ever have 1 record, we could just store this info in this class and not use a table
    /// </remarks>
    public class HeroTemplate : ChangeTracking
    {
        public int Id { get; set; }

        //public List<Hero> Heroes { get; set; }

        /// <summary>
        /// Gen0 and Gen1 heroes share the same images
        /// </summary>
        public string ImageBaseUrl { get; set; }

        /// <summary>
        /// The number of stat points each stat has when a hero is created. 
        /// Right now it ranges from 4 to 10 (7 +- 3)
        /// </summary>
        public RangeInt StartingStatPointsEachStat { get; set; }

        /// <summary>
        /// This is 1D4 or 1-4
        /// </summary>
        public RangeDouble NoWeaponDamage { get; set; }

        /// <summary>
        /// The total skill points a player starts out with. Currently it's 20.
        /// </summary>
        public int TotalSkillPoints { get; set; }

        /// <summary>
        /// aka, stamina. The total quests available per day to do. Usually is 20.
        /// </summary>
        public int TotalDailyQuests { get; set; }

        ///// <summary>
        ///// TODO: We only need this property if there is 1 unique record per hero rarity
        ///// </summary>
        //public HeroRarity? Rarity { get; set; }

        ///// <summary>
        ///// TODO: This needs to be fixed if we don't have 1 hero template record per rarirty.
        ///// </summary>
        //public double? RarityRate { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// The starting hitpoints for the hero
        /// </summary>
        public int MaxHitPoints { get; set; }

        //public double DropRate { get; set; }
    }
}
