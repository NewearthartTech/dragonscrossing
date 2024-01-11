using System.Reflection;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Domain.GameStates;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.NewCombatLogic;
using Newtonsoft.Json;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;

namespace DragonsCrossing.Core.Helper
{
    public static partial class DataHelper
    {

        public static void UpdateMaxExperiencePoints(this HeroDto hero)
        {
            var schedule = CreateTypefromJsonTemplate($"ScheduleTableRef.Experience.json", new[] {
            new {
                level = 0,
                xpRequired = 0,
            } });

            hero.maxExperiencePoints = schedule.First(s => s.level == hero.level).xpRequired;
        }

        /// <summary>
        /// Generic method to Adjust herps props up or down depending on the adjustment class 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hero"></param>
        /// <param name="reduceStats"></param>
        /// <param name="Props"></param>
        public static void AdjustHeroProps<T>(this HeroDto hero, bool reduceStats, T toAdjust)
        {
            var heroProps = typeof(HeroDto)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(k => k.Name, v => v);


            var propsToAdjust = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in propsToAdjust)
            {
                if (!heroProps.ContainsKey(prop.Name))
                {
                    //don't assert 
                    //Debug.Assert(false, "We have template and DTO mismatch");
                    continue;
                }

                if (prop.GetType() != heroProps[prop.Name].GetType())
                {
                    throw new Exception("type mismatch");
                }

                if (heroProps[prop.Name].PropertyType != typeof(int))
                {
                    throw new Exception($"Hero prop {prop.Name} is {heroProps[prop.Name].PropertyType} Only int types are supported");
                }

                var currentValue = heroProps[prop.Name].GetValue(hero);
                if (null == currentValue)
                {
                    throw new InvalidOperationException("Int value should never be null. We have logic error");
                }

                var num = (int)currentValue;

                var toAdjustVal = prop.GetValue(toAdjust);
                if (null == toAdjustVal)
                {
                    throw new InvalidOperationException("Int value should never be null. We have logic error");
                }

                var toAdjustNum = (int)toAdjustVal;


                if (reduceStats)
                {
                    num -= toAdjustNum;
                }
                else
                {
                    num += toAdjustNum;
                }


                heroProps[prop.Name].SetValue(hero, num);


            }
        }

        /// <summary>
        /// This returns the calculated stats for a hero for a certain round. The isBaseOnly gives us option to get the calculated value for base stats only and not including lingering effects
        /// The isBaseOnly is especially used in creating lingering effeccts as we don't want to add lingering effects upon lingering effects
        /// </summary>
        /// <param name="hero"></param>
        /// <param name="gameState"></param>
        /// <param name="isBaseOnly"></param>
        /// <returns></returns>
        public static CalculatedCharacterStats GenerateCalculatedCharacterStats(this HeroDto hero, DbGameState gameState, bool isBaseOnly = false)
        {
            var encounters = gameState?.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();
            var combat = encounters?.Where(e => e.Monster != null).FirstOrDefault();
            var currentRound = combat?.CurrentRound ?? -1;

            var ret = new CalculatedCharacterStats
            {
                strength = hero.strength + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.Strength) ? i.affectedAttributes?[AffectedHeroStatTypeDto.Strength] : 0),

                agility = hero.agility + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.Agility) ? i.affectedAttributes?[AffectedHeroStatTypeDto.Agility] : 0),

                wisdom = hero.wisdom + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.Wisdom) ? i.affectedAttributes[AffectedHeroStatTypeDto.Wisdom] : 0),

                quickness = hero.quickness + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.Quickness) ? i.affectedAttributes[AffectedHeroStatTypeDto.Quickness] : 0),

                charisma = hero.charisma + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.Charisma) ? i.affectedAttributes[AffectedHeroStatTypeDto.Charisma] : 0),

                chanceToHit = hero.baseChanceToHit + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.ChanceToHit) ? i.affectedAttributes[AffectedHeroStatTypeDto.ChanceToHit] : 0),

                difficultyToHit = hero.difficultyToHit + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.DifficultyToHit) ? i.affectedAttributes[AffectedHeroStatTypeDto.DifficultyToHit] : 0),

                chanceToCrit = ReadJsonFileAndReturnStatValue("ChanceToCritHitSchedule.json", (hero.agility + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.Agility) ? i.affectedAttributes?[AffectedHeroStatTypeDto.Agility] : 0))).Result + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.CriticalHitRate) ? i.affectedAttributes[AffectedHeroStatTypeDto.CriticalHitRate] : 0),

                chanceToDodge = (int)Math.Round(hero.quickness / 4.0) * 100 + ReadJsonFileAndReturnStatValue("ChanceToDodgeSchedule.json", (hero.wisdom + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.Wisdom) ? i.affectedAttributes[AffectedHeroStatTypeDto.Wisdom] : 0))).Result + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.DodgeRate) ? i.affectedAttributes[AffectedHeroStatTypeDto.DodgeRate] : 0),

                chanceToParry = ReadJsonFileAndReturnStatValue("ChanceToParrySchedule.json", (hero.strength + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.Strength) ? i.affectedAttributes?[AffectedHeroStatTypeDto.Strength] : 0))).Result + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.ParryRate) ? i.affectedAttributes[AffectedHeroStatTypeDto.ParryRate] : 0),

                armorMitigation = (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.ArmorMitigation) ? i.affectedAttributes[AffectedHeroStatTypeDto.ArmorMitigation] : 0),

                armorMitigationAmount = (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.ArmorMitigationAmount) ? i.affectedAttributes[AffectedHeroStatTypeDto.ArmorMitigationAmount] : 0),

                itemBonusDamage = (int)hero.equippedItems?.Sum(i => i.bonusDamage),

                statsBonusDamage = GetStatsBonusDamage(hero),

                damageRange = GetDamageRange(hero)
            };

            // get all integer properties
            var statProps = typeof(CalculatedCharacterStats)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(int))
                .ToArray();

            if (null != combat && null != combat.heroAttackResult && !isBaseOnly)
            {
                foreach (var alteredStat in combat.heroAttackResult.heroAlteredStats.Where(h => h.round == currentRound))
                {
                    foreach (var prop in statProps)
                    {
                        var value = prop.GetValue(alteredStat.stats);
                        if (null == value)
                            continue;

                        var intValue = (int)value;

                        if (0 == intValue)
                            continue;

                        var orgValue = prop.GetValue(ret);
                        if (null == orgValue)
                            continue;

                        var orgIntValue = (int)orgValue;
                        prop.SetValue(ret, orgIntValue + intValue);
                    }
                }

            }
            return ret;
        }


        public static int GetStatsBonusDamage(HeroDto hero)
        {
            int bonusDamage = 0;
            switch (hero.heroClass)
            {
                case CharacterClassDto.Warrior:
                    bonusDamage = ReadJsonFileAndReturnStatValue("StrengthBonusDamageSchedule.json", (hero.strength + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.Strength) ? i.affectedAttributes?[AffectedHeroStatTypeDto.Strength] : 0))).Result;
                    break;
                case CharacterClassDto.Ranger:
                    bonusDamage = ReadJsonFileAndReturnStatValue("AgilityBonusDamageSchedule.json", (hero.agility + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.Agility) ? i.affectedAttributes?[AffectedHeroStatTypeDto.Agility] : 0))).Result;
                    break;
                case CharacterClassDto.Mage:
                    bonusDamage = ReadJsonFileAndReturnStatValue("WisdomBonusDamageSchedule.json", (hero.wisdom + (int)hero.equippedItems?.Sum(i => i.affectedAttributes.ContainsKey(AffectedHeroStatTypeDto.Wisdom) ? i.affectedAttributes[AffectedHeroStatTypeDto.Wisdom] : 0))).Result;
                    break;
                default:
                    return 0;
            }
            return bonusDamage;
        }

        public static Range GetDamageRange(HeroDto hero)
        {
            int lower = 0;
            int upper = 0;

            if (hero.equippedItems == null ||
                (hero.equippedItems != null &&
                !hero.equippedItems.Any(i => i.dieDamage != null && i.dieDamage.Count() > 0)))
            {
                // When hero doesn't any die damage from all equipped items,
                // we give the hero a base damage of 0-3 (1D4 - 1)
                lower = 0 + (int)GetStatsBonusDamage(hero);

                upper = 3 + (int)GetStatsBonusDamage(hero);
            }
            else 
            {
                lower = (int)hero.equippedItems?.Sum(i => i.dieDamage.Length)
                    + (int)hero.equippedItems?.Sum(i => i.bonusDamage)
                    + (int)GetStatsBonusDamage(hero);

                upper = (int)hero.equippedItems?.Sum(i => i.dieDamage.Sum(d => d.Sides))
                    + (int)hero.equippedItems?.Sum(i => i.bonusDamage)
                    + (int)GetStatsBonusDamage(hero);
            }

            return new Range() { lower = lower, upper = upper };
        }
    }
}
