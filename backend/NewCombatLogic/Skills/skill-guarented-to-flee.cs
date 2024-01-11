using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{

    public void apply_skill_guarentedtoFlee()
    {
        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_guarentedtoFlee));
        for (var i = 1; i < 60; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                InRound = Combat.CurrentRound + i,
                ActionFunctionName = nameof(lingering_guarentedtoFlee),
                Description = "Guaranteed to flee",
            });
        }
    }

    public void lingering_guarentedtoFlee()
    {
        //no action needed 
    }
}
