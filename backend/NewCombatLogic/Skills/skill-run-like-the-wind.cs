using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{

    public void apply_skill_run_like_the_wind()
    {
        //Guaranteed Flee from combat.

        Combat.LingeringSkillEffects.RemoveAll(s => s.Description == "Run like the Wind");

        for (var i = 1; i < 60; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                InRound = Combat.CurrentRound +i,
                ActionFunctionName = nameof(lingering_guarentedtoFlee),
                Description = "Run like the Wind",
            });
        }
    }
}
