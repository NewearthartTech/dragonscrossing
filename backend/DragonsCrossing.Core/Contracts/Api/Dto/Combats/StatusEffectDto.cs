using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum CombatantType { Monster, Hero, Unknown}

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum EffectedbySpecialAbilityStat {
    Q3_DifficultyToHit, 
    Q3_ChanceToHit,
    Q4_ChanceToDodge,
    Q5_Parry,
    Q7_Crit,
    Q8_ArmorMitigation,

    /// <summary>
    /// Bonus damage should be positive number when increasing damage and negative number when lowering damage.
    /// For example, to decrease hero damage by 20%, we do -200. Which is equivalent to 80% of the original hero damage.
    /// </summary>
    Q8_BonusDamage,
    Q10_ArmorMitigationAmount
}

// Potentially rename this to SpecialAbilityStatusEffect
public class StatusEffectDto
{
    [Required]
    public CombatantType AffectType { get; set; }

    /// <summary>
    /// What prop in Monster or hero is effected by this
    /// We use the EffectedbySpecialAbilityStat enum to mark the prop in Hero Or Monster DTO
    /// it's a bad idea to compare strings
    /// </summary>
    [Required]
    public EffectedbySpecialAbilityStat StatName { get; set; }

    [Required]
    public string FriendlyStatName { get; set; } = "";

    [Required]
    public string Description { get; set; } = "";

    /// <summary>
    /// This will be negative for bad effects
    /// </summary>
    [Required]
    public int AffectAmount { get; set; }

    /// <summary>
    /// How many turns the asffect stays in action.
    /// We maek this optional cause the duration could be optional
    /// </summary>
    public int? Duration { get; set; }

    public bool IsPostiveForHero()
    {
        if (AffectType == CombatantType.Hero)
        {
            if (AffectAmount >= 0)
            {
                return true;
            }
            else return false;
        }

        else
        {
            if (AffectAmount >= 0)
            {
                return false;
            }
            else return true;
        }
    }


    public bool IsAffectActive(int currentTurn, int? WhichRoundMonsterUseSpecialAbility)
    {
        if(null == Duration && WhichRoundMonsterUseSpecialAbility.HasValue)
        {
            //once activated this ability continues till end of combat
            return true;
        }

        if (!WhichRoundMonsterUseSpecialAbility.HasValue)
        {
            //monster has not used special ability
            return false;
        }

        return currentTurn < (WhichRoundMonsterUseSpecialAbility + Duration);
    }
}
