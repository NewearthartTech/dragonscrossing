using System.Linq.Expressions;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using static System.Net.Mime.MediaTypeNames;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    public bool IsHeroSpecialAbilityBonusDamage { get; set; }

    public bool IsMonsterSpecialAbilityBonusDamage { get; set; }

    LogicReponses AC2_ADD_SpecialAbilityBonusDamage()
    {
        if (Combat.isHerosTurn)
        {
            IsHeroSpecialAbilityBonusDamage = true;
        }

        else
        {
            IsMonsterSpecialAbilityBonusDamage = true;
        }


        return LogicReponses.next;
    }

    public int CalculateSpecialAbilityIfApplicable(CombatantType affectType, EffectedbySpecialAbilityStat property, int value )
    {

        if (!Combat.Monster.WhichRoundMonsterUsedSpecialAbility.HasValue)
        {
            //monster has not used special ability
            return value;
        }

        //monster has used special ability, find the prop this will effect

        var Affects = (from e in Combat.Monster.SpecialAbility.Affects
                 where e.AffectType == affectType
                 && e.StatName == property
                 && e.IsAffectActive(Combat.CurrentRound, Combat.Monster.WhichRoundMonsterUsedSpecialAbility)
                 select e).ToArray();

        foreach(var e in Affects)
        {    
            // Specail ability last 2 rounds
            // Round 1 casted -20%
            // Round 2 casted -20%
            // Round 2 value should be 80% of the original value
            // Round 3 value should be 64% of the original value
            // Round 4 value should be back to 80% of the orignal value again.
            // Round 5 value should be 100% of the original value
            value = (int)Math.Round(value * (10000 + e.AffectAmount) / 10000.0);
        }

        return value;
    }


}

public class Nameof<T>
{
    public static string Property<TProp>(Expression<Func<T, TProp>> expression)
    {
        var s = expression.Body.ToString();
        var p = s.Remove(0, s.IndexOf('.') + 1);
        return p;
    }

}

