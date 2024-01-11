namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    Dictionary<Func<LogicReponses>, LogicFlow> Configure()
    {
        return new Dictionary<Func<LogicReponses>, LogicFlow>
        {
            {StartOfAttack, new LogicFlow {
                {LogicReponses.next,  AC1_RollToCheckWhoGoesFirst}
            }},

            {SkillsApplied, new LogicFlow {
                {LogicReponses.no,  AllDoneWithSkills},
                {LogicReponses.HeroIsDead,  AC8_HeroDied},
                {LogicReponses.MonsterIsDead,  AC7_MonsterDied},
            }},

            {MonsterFreeHit, new LogicFlow {
                {LogicReponses.next,  Q2_RollToSeeIfMonsterUsedSpecialAbility}
            }},

            {AC0_ApplyLingeringSkillEffects, new LogicFlow {
                {LogicReponses.terminal,  AllDoneWithSkills},
                {LogicReponses.MonsterIsDead,  AC7_MonsterDied},
                {LogicReponses.HeroIsDead,  AC8_HeroDied},
            }},

            {AC1_RollToCheckWhoGoesFirst, new LogicFlow {
                {LogicReponses.next,  Q1_isHerosTurn}
            }},


            {AC2_ADD_SpecialAbilityBonusDamage, new LogicFlow {
                {LogicReponses.next,  Q8_CheckForDeath}
            }},

            {AC3_FullMissCombatTurnisOver, new LogicFlow {
                //Terminal
            }},

            {AC4_ParryDmgFromOpposition, new LogicFlow {
                {LogicReponses.next,  Q10_DidOppositionMitigate}
            }},

            {AC12_ArmorMitigation, new LogicFlow {
                {LogicReponses.next,  Q7_WasHitACriticalStrike}
            }},

            {AC5_ADD_1_5_Damage, new LogicFlow {
                {LogicReponses.next,  Q6_DoesSpecialAbilityEffectDmg}
            }},

            //{AC6_RollForDamage_IncludeAdditionalAbility, new LogicFlow {
            //    {LogicReponses.next,  Q8_CheckForDeath}
            //}},

            {AllDoneWithSkills, new LogicFlow {
                //Terminal 
            }},

            {AC7_MonsterDied, new LogicFlow {
                {LogicReponses.next,  AC10_GenerateLoot},
            }},

            {AC8_HeroDied, new LogicFlow {
                //Terminal
            }},


            {AC9_CombatTurnOver, new LogicFlow {
                //Terminal
            }},

            {AC10_GenerateLoot, new LogicFlow {
                {LogicReponses.next,  AC11_HeroGainExperience},
            }},

            {AC11_HeroGainExperience, new LogicFlow {
                //Terminal
            }},


            {Q1_isHerosTurn, new LogicFlow {
                {LogicReponses.no,  Q2_RollToSeeIfMonsterUsedSpecialAbility},
                {LogicReponses.yes,  Q3_RollToSeeIfAttackisHit},
            }},

            {Q2_RollToSeeIfMonsterUsedSpecialAbility, new LogicFlow {
                {LogicReponses.next,  Q3_RollToSeeIfAttackisHit}
            }},

            {Q3_RollToSeeIfAttackisHit, new LogicFlow {
                {LogicReponses.no,  Q9_IsTurn2},
                {LogicReponses.yes,  Q4_DidOppositionDodge},
            }},

            {Q4_DidOppositionDodge, new LogicFlow {
                {LogicReponses.no,  Q5_DidOppositionParry},
                {LogicReponses.yes,  Q9_IsTurn2},
            }},

            {Q5_DidOppositionParry, new LogicFlow {
                {LogicReponses.no,  Q10_DidOppositionMitigate},
                {LogicReponses.yes,  AC4_ParryDmgFromOpposition},
            }},

            {Q10_DidOppositionMitigate, new LogicFlow {
                {LogicReponses.no,  Q7_WasHitACriticalStrike},
                {LogicReponses.yes,  AC12_ArmorMitigation},
            }},

            {Q6_DoesSpecialAbilityEffectDmg, new LogicFlow {
                {LogicReponses.no,  Q8_CheckForDeath},
                {LogicReponses.yes,  AC2_ADD_SpecialAbilityBonusDamage},
            }},

            {Q7_WasHitACriticalStrike, new LogicFlow {
                {LogicReponses.no,  Q6_DoesSpecialAbilityEffectDmg},
                {LogicReponses.yes,  AC5_ADD_1_5_Damage},
            }},

            {Q8_CheckForDeath, new LogicFlow {
                {LogicReponses.no,  Q9_IsTurn2},
                {LogicReponses.HeroIsDead,  AC8_HeroDied},
                {LogicReponses.MonsterIsDead,  AC7_MonsterDied},
            }},

            {Q9_IsTurn2, new LogicFlow {
                {LogicReponses.no,  AC9_CombatTurnOver},
                {LogicReponses.yes,  Q1_isHerosTurn },
            }},
        };
    }
    
}


