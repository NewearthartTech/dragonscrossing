using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_firebolt()
    {

        //Deals 1D4 + (wisdom / 10) damage, and deals an additional 1 + (Charisma / 9) damage per round for the next 2 rounds.

        var heroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var wisdomImpact = heroWisdom / 10.0;

        var diceRoll = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice, (int)Math.Round(wisdomImpact));
        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= diceRoll) ? Combat.Monster.HitPoints - diceRoll : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_firebolt));

        for (var i = 1; i < 3; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Firebolt",
                InRound = Combat.CurrentRound + i,
                ActionFunctionName = nameof(lingering_skill_firebolt),
                Description = "Deals additional 1D4 - 1 damage",
                RemainingRounds = 3 - (i + 1),
                RoundStarted = Combat.CurrentRound
            });
        }

    }

    public void lingering_skill_firebolt()
    {
        // 1D4-1 damage
        var heroCharisma = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).charisma;
        var charismaImpact = heroCharisma / 9.0;
        var diceRoll = _dice.Roll(1, DiceRollReason.SkillLingeringDamage, Combat.heroAttackResult.Dice, (int)Math.Round(charismaImpact));
        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= diceRoll) ? Combat.Monster.HitPoints - diceRoll : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;
    }
}
