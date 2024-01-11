using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Monsters;

public class MonsterSpecialAbilityDto
{
    [Required]
    public int Id { get; set; }

    public string SoundSlug { get; set; } = "";

    [Required]
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public bool CanUseSpecialAbilityMoreThanOnce { get; set; }

    //public bool? IsAttackModifier { get; set; }

    /// <summary>
    /// The amount this special ability did if any.
    /// Could be damage bonus, bonus to stats, etc...
    /// </summary>
    //public DieDto? Damage { get; set; }

    //public int? DamageBonus { get; set; }

    public StatusEffectDto[] Affects { get; set; } = new StatusEffectDto[] { };

    // Commented for now to match frontend, can put back if we figured what they are for.

    //[Required]
    //public MonsterSpecialAbilityEffectTypeDto Type { get; set; }

    //[Required]
    //public MonsterSpecialAbilityEffectWhoDto EffectWho { get; set; }

    //[Required]
    //public int TurnDuration { get; set; } = 1;
}
