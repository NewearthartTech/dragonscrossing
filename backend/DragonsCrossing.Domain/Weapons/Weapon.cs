using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Weapons
{
    /// <summary>
    /// The randomly generated weapon from loot or the shop. These are tied to a hero or player.
    /// Hero Item: it must be an equipped weapon or armor.
    /// Player Item: it must be an unequipped weapon or armor or an nft item and stored in the players inventory
    /// Items are the only things that give a hero buffs outside of combat.
    /// </summary>
    public class Weapon : ChangeTracking, IItem
    {
        public int Id { get; set; }
        //public int PlayerId { get; set; }
        public int HeroInventoryId { get; set; }
        public bool IsEquipped { get; set; }
        public GearRarity Rarity { get; set; }
        public List<WeaponAffix>? Affixes { get; set; }

        /// <summary>
        /// The amount that is added/removed from certain hero stats when this weapon is equipped
        /// </summary>
        public List<HeroStatModifierResult>? HeroStatsModifierResults { get; set; }

        public WeaponTemplate WeaponTemplate { get; set; }   
        /// <summary>
        /// The stat used to compare against the hero's stat. If they're the same the hero gets an added damage bonus.
        /// TODO: possibly not used. If so, update ItemGenerator with the correct value
        /// </summary>
        public HeroStatType CombatStat { get; set; }
        public double? ParryRate { get; set; }
        public double? DodgeRate { get; set; }
        public double? CriticalHitRate { get; set; }
        public double? DoubleHitRate { get; set; }

        /// <summary>
        /// The permanent damage given to a weapon when it's generated.
        /// </summary>
        public double? BonusDamage { get; set; }

        /// <summary>
        /// Equipped weapons and armor don't have a slot order
        /// </summary>
        public int? SlotNumber { get; set; }
    }
}
