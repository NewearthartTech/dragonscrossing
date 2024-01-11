using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{

    public void apply_skill_barbed_projectile()
    {
        //Deals 1D4 damage + (Agility / 9) and deals an additional 1D4 damage per round for 3 rounds.

        var diceRoll = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var currentAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;
        var finalRollWithAgility = diceRoll + (int)Math.Round(currentAgility / 9.0);
     
        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= finalRollWithAgility) ? Combat.Monster.HitPoints - finalRollWithAgility : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

        Combat.LingeringSkillEffects.RemoveAll(s => s.SkillName == "Barbed Projectile");

        for (var i=1; i<4; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Barbed Projectile",
                InRound = Combat.CurrentRound + i,
                ActionFunctionName = nameof(lingering_skill_barbed_projectile),
                Description = "Deals additional 1D4 damage",
                RemainingRounds = 4 - (i + 1),
                RoundStarted = Combat.CurrentRound
            });
        }
    }

    public void lingering_skill_barbed_projectile()
    {
        var diceRoll = _dice.Roll(4, DiceRollReason.SkillLingeringDamage, Combat.heroAttackResult.Dice);
        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= diceRoll) ? Combat.Monster.HitPoints - diceRoll : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;
    }
}
