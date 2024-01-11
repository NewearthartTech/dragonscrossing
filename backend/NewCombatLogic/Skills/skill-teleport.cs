using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_teleport()
    {
        //Guaranteed Flee from combat.

        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_guarentedtoFlee));

        for (var i = 1; i < 60; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                InRound = Combat.CurrentRound + i,
                ActionFunctionName = nameof(lingering_guarentedtoFlee),
                Description = "Teleport",
            });
        }

    }
}
