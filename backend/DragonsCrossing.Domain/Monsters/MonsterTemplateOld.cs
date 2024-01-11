using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Weapons;
using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Monsters
{
    /// <summary>
    /// The base monster that holds all the default values 
    /// including min/max values for calculations.
    /// There will be one of these pre-inserted into the db for each monster in the game.
    /// This class is used to hold settings used to generate the actual monster during combat.
    /// </summary>
    public class MonsterTemplateOld : ChangeTracking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// The relative directory of the actual image.
        /// This is created from the image root directory
        /// </summary>
        public string ImageFilePath { get; set; }

        /// <summary>
        /// The max hit points
        /// </summary>
        public virtual RangeInt MaxHitPoints { get; set; }

        /// <summary>
        /// A percentage of how likley this monster will appear when user clicks a combat tile
        /// </summary>
        public double AppearanceChance { get; set; }

        /// <summary>
        /// This determines how much bonus damage the monster does.
        /// See Table Ref Guide -> Combat Attribute Bonus
        /// Ex: if power is 6 then bonus damage is 2.
        /// </summary>
        public virtual RangeInt Power { get; set; }
        public virtual RangeInt Charisma { get; set; }
        /// <summary>
        /// Used to calculate LevelScaling value (see Game Mechanics)
        /// </summary>
        public virtual RangeInt Level { get; set; }
        public virtual RangeInt DodgeRate { get; set; }
        public virtual RangeInt Quickness { get; set; }
        public virtual RangeInt HitRate { get; set; }

        /// <summary>
        /// The damage the monster does in combat
        /// </summary>
        public virtual RangeInt Damage { get; set; }

        public int CharismaOpportunityChance { get; set; }

        /// <summary>
        /// A percentage represented as a whole number.
        /// The higher the value the more likely the combat opportunity will be available
        /// </summary>
        public int CombatOpportunityChance { get; set; }

        /// <summary>
        /// As a percent
        /// </summary>
        public virtual RangeDouble CriticalHitChance { get; set; }

        /// <summary>
        /// As a percent
        /// </summary>
        public virtual RangeDouble ParryChance { get; set; }

        /// <summary>
        /// amount damage is reduced from melee/strength
        /// </summary>
        public virtual RangeDouble MitigationMelee { get; set; }

        /// <summary>
        /// amount damage is reduced from melee/strength
        /// </summary>
        public virtual RangeDouble MitigationRange { get; set; }

        /// <summary>
        /// amount damage is reduced from magic
        /// </summary>
        public virtual RangeDouble MitigationMagic { get; set; }

        /// <summary>
        /// Monsters don't actually have weapons or armor. This is just flavor
        /// </summary>
        public string? WeaponDescription { get; set; }

        /// <summary>
        /// Monsters don't actually have weapons or armor. This is just flavor
        /// </summary>
        public string? ArmorDescription { get; set; }

        /// <summary>
        /// This is what stat the player will be tested against. 
        /// aka: combat opportunity stat
        /// TODO: Check to see if this can be null. According to the spreadsheet, it can.
        /// </summary>
        public virtual CharacterClass? MonsterClass { get; set; }

        /// <summary>
        /// The special abilites that CAN be cast during combat.
        /// may have multiple but will have 1 for now
        /// If a monster ability lasts 2 rounds and he recasts it even though he already had it, that's ok. Don't worry about.
        /// We need to be able to change special ability values like (double attack to triple attack, etc...) in the db.
        /// This isn't calculated, just copied over
        /// </summary>
        public virtual List<MonsterSpecialAbility>? SpecialAbilities { get; set; }

        /// <summary>
        /// The liklihood the monster will cast/use its ability.
        /// This is a percentage not decimal.
        /// </summary>
        public int SpecialAbilityCastChance { get; set; }

        /// <summary>
        /// Where this monster can be found
        /// </summary>
        public int TileId { get; set; }

        /// <summary>
        /// Where this monster can be found
        /// </summary>
        public Tile Tile { get; set; }

        #region Loot Table
        /// <summary>
        /// The possible items that can drop from loot
        /// </summary>
        public virtual List<MonsterItemLoot> ItemsLoot { get; set; }        

        /// <summary>
        /// This may not be used, waiting for calculation from Granite
        /// </summary>
        public double DcxLootChance { get; set; }
        public virtual RangeDouble DcxLootAmount { get; set; }
        public double GoldLootChance { get; set; }
        public virtual RangeInt GoldLootAmount { get; set; }

        // all monsters have the same experience points amount
        //public RangeDouble ExperiencePointsAmount { get; set; }
        #endregion

        public enum ChildIncludes
        {
            ItemsLoot,
            SpecialAbilities
        }
    }    
}
