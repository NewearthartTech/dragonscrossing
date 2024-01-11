using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    LogicReponses AC0_ApplyLingeringSkillEffects()
    {
        var lingering = Combat.LingeringSkillEffects
            .Where(e => e.InRound == Combat.CurrentRound || e.RemainingRounds == -1)
            .OrderByDescending(s => s.RemainingRounds)
            .Select(l =>
            {
                if (string.IsNullOrEmpty(l.ActionFunctionName))
                    return false;

                var mi = typeof(CombatEngine).GetMethod(l.ActionFunctionName);
                if (null == mi )
                {
                    _logger.LogCritical($"missing method {l.ActionFunctionName}");
                    return false;
                }

                mi.Invoke(this, null);

                _logger.LogDebug($"Hero skill {l.SkillName} lingering effect applied at round {l.InRound}, remaining {l.RemainingRounds} round(s).");

                return true;

            })
            .ToArray() ;

        // After executing hero skill, check if either monster or hero died.
        if (Combat.isMonsterDead)
        {
            return LogicReponses.MonsterIsDead;
        }
        else if (Combat.isHeroDead)
        {
            return LogicReponses.HeroIsDead;
        }

        return LogicReponses.terminal;
    }
}


