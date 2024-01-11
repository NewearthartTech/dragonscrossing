using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Monsters;
using DragonsCrossing.Domain.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DragonsCrossing.Core.Heroes
{
    /// <summary>
    /// Stores the calculated values of the hero based on 3 types of adjustments (bonuses and curses):
    /// 1. Item affix bonuses
    /// 2. Hero Skill bonuses
    /// 3. Monster special ability curses
    /// All methods in this class should return the current adjusted value after the 3 types of adjustments are applied.
    /// Outside combat, only item affixes are applied.
    /// Inside combat, all 3 types of adjustments are applied
    /// NOTE: If an item, hero skill, or monster special ability can adjust a hero property, it should go in this class.
    /// </summary>
    /// <remarks>
    /// I thought about separating this class into 2: HeroStatsCalculated(Hero) and HeroCombatStatsCalculated(Combat), 
    /// but I realized I'd need a 3rd class to aggregate both of them called HeroStatsCalculatedAggregate.
    /// This is just unnecessary overhead so instead it's 1 class with 2 constructors - which makes for a cleaner design.
    /// </remarks>
    public class HeroCombatStatsCalculated
    {
        private readonly Hero hero;
        private readonly Combat? combat;

        /// <summary>
        /// If not in combat, we just need to add item affix bonuses
        /// </summary>
        /// <param name="hero"></param>
        public HeroCombatStatsCalculated(Hero hero)
        {
            this.hero = hero;
        }

        /// <summary>
        /// If in combat, we need to add hero skills bonsues and monster special abilities curses
        /// in addition to item affix bonuses
        /// </summary>
        /// <param name="combat"></param>
        public HeroCombatStatsCalculated(Combat combat)
        {
            this.hero = combat.Hero;
            this.combat = combat;
        }

        /// <summary>
        /// The skills casted by the hero
        /// Note: We can't add this as a constructor param since the list is modified later than when the 
        /// constructor is called. So instead make a private property to be used by this class only.
        /// </summary>
        private List<HeroSkillCasted>? HeroSkillsCasted { get { return combat?.HeroSkillsCasted; } }

        /// <summary>
        /// The skills casted by the hero
        /// Note: We can't add this as a constructor param since the list is modified later than when the 
        /// constructor is called. So instead make a private property to be used by this class only.
        /// </summary>
        private List<MonsterSpecialAbilityCasted>? MonsterSpecialAbilitiesCasted { get { return combat?.Monster.SpecialAbilitiesCasted; } }

        /// <summary>
        /// The hero's current/base/max damage.
        /// A read only calculated value based on equipped weapons and stats.
        /// Outside of combat, this value can only be a range. 
        /// Inside combat it can be a single value because that's when the dice is rolled to determine damage.
        /// </summary>
        public RangeDouble GetBaseDamage()
        {   
            if (hero.Inventory.EquippedWeapons == null || !hero.Inventory.EquippedWeapons.Any())
            {
                return hero.HeroTemplate.NoWeaponDamage;
            }

            var minDamage = hero.Inventory.EquippedWeapons.Sum(equippedWeapon =>
                    equippedWeapon.WeaponTemplate.BaseDamage.Min
                    + equippedWeapon.BonusDamage
                    + CalculateWeaponBonusDamageFromStat(equippedWeapon.WeaponTemplate)) ?? 0;
            var maxDamage = hero.Inventory.EquippedWeapons.Sum(equippedWeapon =>
                    equippedWeapon.WeaponTemplate.BaseDamage.Max
                    + equippedWeapon.BonusDamage
                    + CalculateWeaponBonusDamageFromStat(equippedWeapon.WeaponTemplate)) ?? 0;
            return new RangeDouble(minDamage, maxDamage);            
        }

        /// <summary>
        /// The hero's current/base/max defense
        /// This is a percentage, not a decimal
        /// </summary>
        public RangeDouble GetBaseDefense()
        {
            var minDefense = hero.Inventory.EquippedArmors.Sum(equippedArmor =>
            equippedArmor.ArmorTemplate.Defense.Min
            + equippedArmor.Defense);

            var maxDefense = hero.Inventory.EquippedArmors.Sum(equippedArmor =>
            equippedArmor.ArmorTemplate.Defense.Max
            + equippedArmor.Defense);

            return new RangeDouble(minDefense, maxDefense);
        }

        public double GetCriticalHitRate()
        {
            // combat attribute bonus (agility)
            var combatAttributeBonus = GetAgility() / 10;

            // items. Ex: +2% chance to crit hit
            var itemBonus = hero.Inventory.EquippedArmors.Sum(armor => armor.CriticalHitRate) ?? 0;

            // TODO: Calculate skills bonus
            return itemBonus + combatAttributeBonus;
        }

        public int MaxDailyQuests
        {
            get
            {
                return hero.HeroTemplate.TotalDailyQuests + hero.Level.AdditionalQuests;
            }
        }

        /// <summary>
        /// The max adjusted hit points.        
        /// </summary>
        /// <returns></returns>
        public int GetMaxHitPoints()
        {
            // TODO: adjust this based on item affixes
            return hero.CombatStats.MaxHitPoints;
        }

        /// <summary>
        /// The current adjusted hit points
        /// Notice there are 2 properties for hit points instead of 1 like the other stats.
        /// This is because the results of combat permanently change the current hit points (Hero.CombatStats.HitPoints).
        /// </summary>
        /// <returns></returns>
        public int GetHitPoints()
        {
            return hero.CombatStats.HitPoints;
        }

        /// <summary>
        /// The current/base/max adjusted strength
        /// </summary>
        public int GetStrength()
        {
            return hero.CombatStats.MaxStrength;  
        }

        /// <summary>
        /// The current/base/max adjusted agility
        public int GetAgility()
        {
            // TODO: adjust this based on item affixes
            return hero.CombatStats.MaxAgility;
        }

        /// <summary>
        /// The current/base/max adjusted wisdom
        /// </summary>
        public int GetMaxWisdom()
        {
            // TODO: adjust this based on item affixes, hero skills, and monster abilities
            return hero.CombatStats.MaxWisdom;
        }

        /// <summary>
        /// The current/base/max adjusted charisma
        /// </summary>
        public int GetMaxCharisma()
        {
            // TODO: adjust this based on item affixes
            return hero.CombatStats.MaxCharisma;
        }

        /// <summary>
        /// The current/base/max adjusted quickness
        /// </summary>
        public int GetMaxQuickness()
        {
            // TODO: adjust this based on item affixes
            return hero.CombatStats.MaxQuickness;
        }
                        
        /// <summary>        
        /// The chance the hero will hit the monster
        /// aka: CTH
        /// All monsters and players have a base CTH of 0
        /// </summary>
        public int GetHitRate()
        {
            // TODO: adjust this based on item affixes
            return 0;
        }        

        /// <summary>
        /// The dodge rate determins if the monster hits the hero.
        /// Used in the hit state. Not to be confused with the Dodge state.
        /// </summary>
        /// <returns></returns>
        public double GetDodgeRate()
        {
            // TODO: include item affix bonuses
            return 25;
        }

        public double GetParryRate()
        {
            return GetMaxWisdom() * .05 +
                    hero.Inventory.EquippedWeapons
                    .Sum(w => w.ParryRate != null ? w.ParryRate.Value : 0);
        }

        /// <summary>
        /// TODO: Don't think this belongs in this class
        /// </summary>
        public List<HeroSkill> LearnedSkills
        {
            get
            {
                return hero.Skills.Where(s => s.Status == HeroSkillStatus.Learned).ToList();
            }
        }

        /// <summary>
        /// if the weapon class is for this type of hero then give the hero a bonus to damage.
        /// The bonus to damage is a 1 to 1 ratio with the stat. 
        /// Ex1: A warrior class weapon for a warrior hero with 3 strength will give the hero 3 extra damage.
        /// Ex2: A warrior class weapon for a mage hero with 3 strength will give no extra damage.
        /// </summary>
        /// <param name="weaponTemplate"></param>
        /// <returns></returns>
        private double CalculateWeaponBonusDamageFromStat(WeaponTemplate weaponTemplate)
        {
            if (hero.HeroClass == weaponTemplate.HeroClass)
            {
                switch (hero.HeroClass)
                {
                    case CharacterClass.Warrior:
                        return MathRound.Down(GetStrength());
                    case CharacterClass.Ranger:
                        return MathRound.Down(GetAgility());
                    case CharacterClass.Mage:
                        return MathRound.Down(GetMaxWisdom());
                }
            }
            return 0;
        }
    }
}
