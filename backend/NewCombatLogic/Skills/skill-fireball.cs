using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_fireball()
    {
        //Deals 2D4 + (Wisdom / 7) Damage and an additional 1D4 + (Charisma / 9) damage over 4 rounds

        var diceRoll1 = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var diceRoll2 = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var heroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var wisdomImpact = heroWisdom / 7.0;

        var totalDamage = diceRoll1 + diceRoll2 + (int)Math.Round(wisdomImpact);

        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= totalDamage) ? Combat.Monster.HitPoints - totalDamage : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_fireball));

        for (var i = 1; i < 5; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Fireball",
                InRound = Combat.CurrentRound + i,
                ActionFunctionName = nameof(lingering_skill_fireball),
                Description = "Deals an additional 1D4 + (Charisma / 9) damage",
                RemainingRounds = 5 - (i + 1),
                RoundStarted = Combat.CurrentRound
            });
        }

    }

    public void lingering_skill_fireball()
    {

        var heroCharisma = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).charisma;
        var charismaImpact = heroCharisma / 9.0;

        var diceRoll = _dice.Roll(4, DiceRollReason.SkillLingeringDamage, Combat.heroAttackResult.Dice, (int)Math.Round(charismaImpact));

        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= diceRoll) ? Combat.Monster.HitPoints - diceRoll : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;
    }
}
