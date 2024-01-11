using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    // For a 150% damage we will have a value of 150
    public int MonsterToHeroBonusDamagePercentage_SpecialAbility { get; set; }

    public int HeroToMonsterBonusDamagePercentage_SpecialAbility { get; set; }
    LogicReponses Q6_DoesSpecialAbilityEffectDmg()
    {
        if (!Combat.Monster.WhichRoundMonsterUsedSpecialAbility.HasValue)
        {
            //monster has not used special ability
            return LogicReponses.no;
        }

        //monster has used special ability, find the prop this will effect

        var monsterAffects = (from e in Combat.Monster.SpecialAbility.Affects
                                where (e.AffectType == CombatantType.Monster)
                                && e.StatName == EffectedbySpecialAbilityStat.Q8_BonusDamage
                                && e.IsAffectActive(Combat.CurrentRound, Combat.Monster.WhichRoundMonsterUsedSpecialAbility)
                                select e).ToArray();

        var heroAffects = (from e in Combat.Monster.SpecialAbility.Affects
                            where (e.AffectType == CombatantType.Hero)
                            && e.StatName == EffectedbySpecialAbilityStat.Q8_BonusDamage
                            && e.IsAffectActive(Combat.CurrentRound, Combat.Monster.WhichRoundMonsterUsedSpecialAbility)
                            select e).ToArray();

        MonsterToHeroBonusDamagePercentage_SpecialAbility = monsterAffects.Count() > 0 ? monsterAffects.First().AffectAmount : 0;
        HeroToMonsterBonusDamagePercentage_SpecialAbility = heroAffects.Count() > 0 ? heroAffects.First().AffectAmount : 0;

        if (Combat.isHerosTurn)
        {
            return heroAffects.Count() > 0 ? LogicReponses.yes : LogicReponses.no;
        }

        return monsterAffects.Count() > 0 ? LogicReponses.yes:LogicReponses.no;
    }
}


