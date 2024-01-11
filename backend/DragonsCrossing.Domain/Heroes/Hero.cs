using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Items;

namespace DragonsCrossing.Domain.Heroes
{
    /// <summary>
    /// The character the Player plays with. This is an NFT item bought and sold on the blockchain outside the game.
    /// These heroes are randomly created in 2 ways:
    /// 1. Gen0 Hero: 4050 of them created before the game launches. They are bought on harmony outside the game.
    /// 2. Gen1 Hero: X of them are created during the game - they are swapped for a hero egg after the player pays (to harmony outside the game) to open the egg.
    /// </summary>
    public class Hero : ChangeTracking
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public virtual HeroName HeroName { get; set; }
        public string Name { get { return HeroName.Name; } }
        public virtual HeroTemplate HeroTemplate { get; set; }
        public virtual HeroCombatStats CombatStats { get; set; }
        public virtual HeroLevel Level { get; set; }

        /// <summary>
        /// Short term storage for the Hero. If he dies, he loses everything in this inventory except for maybe equipped gear.
        /// Contains items, and both equipped and unequipped weapons and armor for the Hero.                
        /// </summary>
        public virtual HeroInventory Inventory { get; set; }

        public int Generation { get; set; }
        public virtual CharacterClass HeroClass { get; set; }
        public virtual HeroRarity Rarity { get; set; }

        public virtual HeroGender Gender { get; set; }

        /// <summary>
        /// The relative path to the base hero images. This is not a url to a specific image. 
        /// The UI will append to this url the different images needed by the UI.
        /// We will only have 1 of these for all images related to the hero.
        /// The path is determined by the heros Gender and HeroClass: ex: /img/api/heroes/male_mage_gen_0
        /// </summary>
        public string ImageBaseUrl { get; set; }
        
        public int SkillPoints { get; set; } = 0;
        public int UnusedSkillPoints { get; set; } = 0;
        
        /// <summary>
        /// These are any skills the hero has learned, unlearned, or unidentified
        /// </summary>
        public virtual List<HeroSkill> Skills { get; set; }        
        
        /// <summary>
        /// Did the hero pass the game?
        /// </summary>
        public bool IsAscended { get; set; } = false;        

        /// <summary>
        /// The number of quests the hero has left today
        /// This value is set to 0 at midnight every day by a seperate process.
        /// Quests == Combats
        /// </summary>
        public int UnusedDailyQuests { get; set; }        

        /// <summary>
        /// If true, it allows the player to teleport from wherever they are in the game, into town.
        /// This should probably be a controller method, not a property here
        /// </summary>
        public bool IsHearthstoneAvailable { get; set; } = false;        

        public enum ChildIncludes
        {
            Skills,
            Inventory,
            CombatStats,
        }           
    }
}
