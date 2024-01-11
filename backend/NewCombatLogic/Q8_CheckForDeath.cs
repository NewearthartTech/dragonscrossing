using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Domain.Heroes;
using static DragonsCrossing.Core.Helper.DataHelper;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    LogicReponses Q8_CheckForDeath()
    {

        if (Combat.isHerosTurn)
        {
            int damage = 0;
            var bonusDamage = 0;
            // 1. If no item equipped, do a 1D4 damage and set bonus damage to -1
            if (GameState.Hero.equippedItems == null ||
                (GameState.Hero.equippedItems != null && 
                !GameState.Hero.equippedItems.Any(i => i.dieDamage != null && i.dieDamage.Count() > 0)))
            {
                damage = _dice.Roll(4, DiceRollReason.HeroDieDamage, Combat.heroAttackResult.Dice);
                bonusDamage = -1;
            }
            //  If item equipped, calculate item die damage total
            // For a 2D4 result we need to show 1 + 3 or 2 + 2.
            else
            {
                damage = GameState.Hero.equippedItems.Sum(d => d.dieDamage.Sum(dice => _dice.Roll(
                dice.Sides, DiceRollReason.EquipmentDamage, Combat.heroAttackResult.Dice)));
            }
            
            // 2. Apply bonus damage when monster casts special ability that does less or extra damage to hero.
            if (IsHeroSpecialAbilityBonusDamage)
            {
                var damageBeforeSpecialAbilityBonusDamage = damage;
                damage = (int)Math.Round(damage * (10000 + HeroToMonsterBonusDamagePercentage_SpecialAbility) / 10000.0);

                var StatusEffectAmount = damage - damageBeforeSpecialAbilityBonusDamage;

                if (StatusEffectAmount > 0)
                {
                    Combat.heroAttackResult.StatusEffectBonus = StatusEffectAmount;
                }
                else
                {
                    Combat.heroAttackResult.StatusEffectMitigation = StatusEffectAmount;
                }    
            }

            // 3. Add ItemBonusDamage + StatsBonusDamage( Need to check the hero's class to find the right statsbonus dmg)
            bonusDamage = bonusDamage + GameState.Hero.GenerateCalculatedCharacterStats(GameState).itemBonusDamage +
                GameState.Hero.GenerateCalculatedCharacterStats(GameState).statsBonusDamage;

            damage += bonusDamage;

            Combat.heroAttackResult.BonusDamage = bonusDamage;

            // 4. Add crit damage sum 1 through 3 and * 1.5
            if (heroCritAttack)
            {
                Combat.heroAttackResult.CritDamage = (int)Math.Round(damage / 2.0);
                damage += Combat.heroAttackResult.CritDamage;
            }

            // 5. Reduce damage for parry => mutiply step 4 by 0.5
            if (monsterParryDmgFromHero)
            {
                Combat.heroAttackResult.ParryMitigation = (int)Math.Round(damage / 2.0);
                damage -= Combat.heroAttackResult.ParryMitigation;
            }

            #region Armor mitigation

            if (monsterMitigateDmgFromHero)
            {
               Combat.heroAttackResult.ArmorMitigation = CalculateSpecialAbilityIfApplicable(
               CombatantType.Monster,
               EffectedbySpecialAbilityStat.Q10_ArmorMitigationAmount,
               Combat.Monster.GenerateCalculatedMonsterStats(Combat).ArmorMitigationAmount);
            }

            damage -= Combat.heroAttackResult.ArmorMitigation;

            #endregion

            // If damage is ever below 0, make it a 0
            damage = damage < 0 ? 0 : damage;

            // Total damage has a floor of 0
            Combat.heroAttackResult.TotalDamage = damage;

            if (damage >= Combat.Monster.HitPoints)
            {
                Combat.Monster.HitPoints = 0;
                return LogicReponses.MonsterIsDead;
            }          

            Combat.Monster.HitPoints -= damage;

            return LogicReponses.no;
        }
        else
        {
            int damage = 0;

            // 1. Roll for dmg. We need to know individual die's roll number and add them together for this part.
            // For a 2D4 result we need to show 1 + 3 or 2 + 2. Grab the die from Monster
            damage = Combat.Monster.DieDamage.Sum(d => _dice.Roll(
                d.Sides, DiceRollReason.MonsterDieDamage, Combat.monsterAttackResult.Dice));

            // 2. Apply bonus damage when monster casts special ability that does less or extra damage to hero.
            if (IsMonsterSpecialAbilityBonusDamage)
            {
                var damageBeforeSpecialAbilityBonusDamage = damage;
                damage = (int)Math.Round(damage * (10000 + MonsterToHeroBonusDamagePercentage_SpecialAbility) / 10000.0);

                var StatusEffectAmount = damage - damageBeforeSpecialAbilityBonusDamage;

                if (StatusEffectAmount >= 0)
                {
                    Combat.monsterAttackResult.StatusEffectBonus = StatusEffectAmount;
                }
                else
                {
                    Combat.monsterAttackResult.StatusEffectMitigation = StatusEffectAmount;
                }
            }

            // 3. Add monster's bonus damage
            // 4. Add damage affected by monster's power which needs to refer to the schedule 
            Combat.monsterAttackResult.BonusDamage = Combat.Monster.GenerateCalculatedMonsterStats(Combat).BonusDamage;

            damage += Combat.monsterAttackResult.BonusDamage;

            // 5. Add crit damage sum 1 through 3 and * 1.5
            if (monsterCritAttack)
            {
                Combat.monsterAttackResult.CritDamage = (int)Math.Round(damage / 2.0);
                damage += Combat.monsterAttackResult.CritDamage;
            }

            // 6. Reduce damage for parry => mutiply step 4 by 0.5
            if (heroParryDmgFromMonster)
            {
                // The parry mitigation on the monster attact result represents the damage blocked by the hero
                Combat.monsterAttackResult.ParryMitigation = (int)Math.Round(damage / 2.0);
                damage -= Combat.monsterAttackResult.ParryMitigation;
            }

            #region Armor mitigation

            if (heroMitigateDmgFromMonster)
            {
                Combat.monsterAttackResult.ArmorMitigation = CalculateSpecialAbilityIfApplicable(
                CombatantType.Hero,
                EffectedbySpecialAbilityStat.Q10_ArmorMitigationAmount,
                GameState.Hero.GenerateCalculatedCharacterStats(GameState).armorMitigationAmount);
            }

            damage -= Combat.monsterAttackResult.ArmorMitigation;

            #endregion

            // If damage ever go negative, make it a 0
            damage = damage < 0 ? 0 : damage;

            // Total damage has a floor of 0
            Combat.monsterAttackResult.TotalDamage = damage;

            if (damage >= GameState.Hero.remainingHitPoints)
            {
                GameState.Hero.remainingHitPointsPercentage = 0;
                return LogicReponses.HeroIsDead;
            }           

            GameState.Hero.remainingHitPointsPercentage -= damage * 1.0 / GameState.Hero.totalHitPoints * 100;

            return LogicReponses.no;
        }
    }
}

