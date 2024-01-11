using DragonsCrossing.Core.Combats;
using DragonsCrossing.Core.Combats.StateMachine;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Monsters
{
    /// <summary>
    /// Stores the calculated values of the monster based on 2 types of adjustments (bonuses and curses):
    /// 1. Hero Skill curses
    /// 2. Monster special ability bonuses
    /// All methods in this class should return the current adjusted value after the 2 types of adjustments are applied.
    /// Notice a monster only exists inside combat and so we don't need to track bonuses/curses outside combat.
    /// NOTE: If a hero skill, or monster special ability can adjust a monster property, it should go in this class.
    /// </summary>
    public class MonsterCombatStatsCalculated
    {
        private readonly Combat combat;

        public MonsterCombatStatsCalculated(Combat combat)
        {
            this.combat = combat;
        }

        /// <summary>
        /// The skills casted by the hero
        /// Note: We can't add this as a constructor param since the list is modified later than when the 
        /// constructor is called. So instead make a private property to be used by this class only.
        /// </summary>
        private List<HeroSkillCasted>? HeroSkillsCasted { get { return combat.HeroSkillsCasted; } }

        /// <summary>
        /// The monster used in combat.
        /// Note: We can't add this as a constructor param since the monster is modified later than when the 
        /// constructor is called. So instead make a private property to be used by this class only.
        /// </summary>
        private Monster Monster { get { return combat.Monster; } }

        /// <summary>            
        /// calculate monster damage including bonus damage and special ability damage
        /// </summary>
        /// <returns></returns>
        public RangeDouble GetBaseDamage()
        {
            var baseDamage = Monster.MonsterTemplateOld.Damage;
            var statBonusDamage = Monster.Power / 3;            
            var totalDamage = new RangeDouble(baseDamage.Min + statBonusDamage, baseDamage.Max + statBonusDamage);

            // special ability bonus
            double? specialAbilityBonusRate = Monster.SpecialAbilitiesCasted
            ?.Where(ability => ability.Effect.Type == MonsterSpecialAbilityEffectType.DamageRate && ability.Effect.EffectWho == MonsterSpecialAbilityEffectWho.Monster)
            .Sum(ability => ability.Effect.Amount / 100d);

            // don't want to reset the damage to 0. Ratio must be greater than 0.
            // For negative effects the ratio needs to be less than 1 but greater than 0.
            // For positive effects the ratio needs to be greater than 1.
            // For no effect, the ratio needs to be 1.
            if (specialAbilityBonusRate != null && specialAbilityBonusRate > 0)
            {
                totalDamage.Min *= specialAbilityBonusRate.Value;
                totalDamage.Max *= specialAbilityBonusRate.Value;
            }            
            
            return totalDamage;
        }

        /// <summary>
        /// determine which monster defense type to use based on the type of hero it's fighting
        /// </summary>
        /// <param name="heroClass"></param>
        /// <returns></returns>
        public RangeDouble GetBaseDefense(CharacterClass heroClass)
        {
            var baseDefense = new RangeDouble();
            switch (heroClass)
            {
                case CharacterClass.Warrior:
                    baseDefense.Min = Monster.MonsterTemplateOld.MitigationMelee.Min;
                    baseDefense.Max = Monster.MonsterTemplateOld.MitigationMelee.Max;
                    break;
                case CharacterClass.Ranger:
                    baseDefense.Min = Monster.MonsterTemplateOld.MitigationRange.Min;
                    baseDefense.Max = Monster.MonsterTemplateOld.MitigationRange.Max;                    
                    break;
                case CharacterClass.Mage:
                    baseDefense.Min = Monster.MonsterTemplateOld.MitigationMagic.Min;
                    baseDefense.Max = Monster.MonsterTemplateOld.MitigationMagic.Max;
                    break;
            }
            // TODO: include special ability bonus
            return baseDefense;
        }

        public double GetDodgeRate()
        {
            var specialAbilityBonus = Monster.SpecialAbilitiesCasted
                ?.Where(ability => ability.Effect.Type == MonsterSpecialAbilityEffectType.DodgeRate && ability.Effect.EffectWho == MonsterSpecialAbilityEffectWho.Monster)
                .Sum(e => e.Effect.Amount);
            return Monster.DodgeRate + (specialAbilityBonus ?? 0);
        }     
        
        public double GetParryRate()
        {
            // TODO: include special ability bonus
            return Monster.ParryChance;
        }

        public double GetCriticalHitRate()
        {
            // calculate positive effects from monster abilities
            var specialAbilityBonus = Monster.SpecialAbilitiesCasted
                    ?.Where(ability => ability.Effect.Type == MonsterSpecialAbilityEffectType.CriticalHitRate && ability.Effect.EffectWho == MonsterSpecialAbilityEffectWho.Monster)
                    .Sum(e => e.Effect.Amount);

            // calculate negative effects from hero casted spells/skills
            specialAbilityBonus += HeroSkillsCasted
                ?.SelectMany(skill => skill.SkillCasted.SkillTemplate.Effects
                .Where(effect => effect.Type == HeroSkillEffectType.CriticalHitRate && effect.EffectWho == HeroSkillEffectWho.Monster))
                .Sum(e => e.Amount) ?? 0;

            return Monster.CriticalHitChance + (specialAbilityBonus ?? 0);
        }        
    }
}
