using DragonsCrossing.Core.Contracts.Api.Dto.Combats;

namespace DragonsCrossing.NewCombatLogic;


public enum LogicReponses { yes, no, next, terminal, HeroIsDead, MonsterIsDead };
public class LogicFlow : Dictionary<LogicReponses, Func<LogicReponses>> { }


public partial class CombatEngine
{
    LogicReponses StartOfAttack()
    {
        Combat.isCombatActionTaken = true;

        _logger.LogDebug($"Start of attack");

        return LogicReponses.next;
    }

    LogicReponses SkillsApplied()
    {
        Combat.isCombatActionTaken = true;

        if (null != Combat && null != Combat.Monster && Combat.Monster.HitPoints <= 0)
        {
            _logger.LogDebug($"SkillsApplied. Monster is dead.");
            return LogicReponses.MonsterIsDead;
        }
        else if (GameState.Hero.remainingHitPoints <= 0)
        {
            _logger.LogDebug($"SkillsApplied. Hero is dead.");
            return LogicReponses.HeroIsDead;
        }
        else
            return LogicReponses.no;
    }

    LogicReponses MonsterFreeHit()
    {
        _logger.LogDebug($"MonsterFreeHit. Monster gets a free hit at round {Combat.CurrentRound}");

        // When hero failed to flee, the monster gets to hit the hero so set is2ndTurn to false because hero won't be able to attack back.
        Combat.CurrentRound += 1;
        Combat.is2ndTurn = false;
        Combat.isHerosTurn = false;

        return LogicReponses.next;
    }

    LogicReponses AC3_FullMissCombatTurnisOver()
    {
        return LogicReponses.terminal;
    }
    
    LogicReponses AC8_HeroDied()
    {
        Combat.isHeroDead = true;

        _logger.LogDebug("AC8_HeroDied. Hero died.");

        return LogicReponses.terminal;
    }

    LogicReponses AC9_CombatTurnOver()
    {

        _logger.LogDebug($"AC9_CombatTurnOver. End of turn");

        return LogicReponses.terminal;
    }

    LogicReponses AllDoneWithSkills()
    {
        _logger.LogDebug("AllDoneWithSkills. Skill applied.");

        return LogicReponses.terminal;
    }
}


