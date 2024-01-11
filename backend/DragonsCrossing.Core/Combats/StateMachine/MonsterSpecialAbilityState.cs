using DragonsCrossing.Core.Common;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Combats.StateMachine
{
    public class MonsterSpecialAbilityState
    {
        public MonsterSpecialAbilityState(CombatStateMachine stateMachine)
        {
            var combatDetail = stateMachine.SetCombatDetail(CombatDetailType.MonsterSpecialAbility);

            var doesMonsterUseSpecialAbility = stateMachine.Combat.Monster.SpecialAbilityCastChance.IsSuccessfulRoll();
            if (doesMonsterUseSpecialAbility)
            {
                combatDetail.IsSuccess = true;
                UseMonsterSpecialAbility(stateMachine);
            }
            else
                new TryToHitState(stateMachine);
        }

        private void UseMonsterSpecialAbility(CombatStateMachine stateMachine)
        {
            // right now we only support 1 special ability for a monster, so get the single
            var monsterSpecialAbility = stateMachine.Combat.Monster.MonsterTemplateOld.SpecialAbilities.Single();

            // save the effects to be used later in different calculations
            stateMachine.Combat.Monster.SpecialAbilitiesCasted.AddRange(monsterSpecialAbility.Effects
                .Select(effect => new MonsterSpecialAbilityCasted()
                {
                    Effect = effect,
                    MonsterId = stateMachine.Combat.Monster.Id,
                    ActiveUntilRound = CalculateAbilityDuration(stateMachine, effect),
                }));
            if (monsterSpecialAbility.UsesTurn)
            {
                if (monsterSpecialAbility.IsGuaranteedDamage)
                {
                    new ArmorMitigationState(stateMachine);
                }
                else
                {
                    new TryToHitState(stateMachine);
                }
            }
            else
            {
                new TryToHitState(stateMachine);
            }
        }

        /// <summary>
        /// Returns the round the special ability will be finished.
        /// Will add +1 round if the monster went second and it was an ability that
        /// only affects when the hero attacks.
        /// </summary>
        /// <param name="stateMachine"></param>
        /// <param name="effect"></param>
        /// <returns></returns>
        private int CalculateAbilityDuration(CombatStateMachine stateMachine, MonsterSpecialAbilityEffect effect)
        {
            var activeUntilRound = stateMachine.Combat.Round + effect.TurnDuration;
            if (stateMachine.DidHeroGoFirst)
            {
                if (effect.Type == MonsterSpecialAbilityEffectType.DodgeRate
                    || effect.Type == MonsterSpecialAbilityEffectType.Parry
                    || effect.Type == MonsterSpecialAbilityEffectType.Armor)
                {
                    activeUntilRound++;
                }
            }

            return activeUntilRound;
        }

        /// <summary>
        /// Will increase damage or change monster stats based on the ability casted.        
        /// NOTE: Not needed now that we have MonsterCombatStatsCalculated class.
        /// </summary>
        /// <param name="stateMachine"></param>
        /// <param name="combatDetail"></param>
        private void CalculateSpecialAbilityEffect(CombatStateMachine stateMachine, CombatDetail combatDetail)
        {
            var monster = stateMachine.Combat.Monster;
            var hero = stateMachine.Combat.Hero;
            foreach (var specialAbilityCasted in monster.SpecialAbilitiesCasted)
            {
                var effect = specialAbilityCasted.Effect;
                switch (effect.Type)
                {
                    case MonsterSpecialAbilityEffectType.MultipleAttack:
                        break;                    
                    case MonsterSpecialAbilityEffectType.Escape:
                        break;                    
                }
            }
        }
    }
}
