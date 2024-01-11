using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;
namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    public void apply_skill_apply_poultice()
    {
        // Heals 1D10% + (wisdom / 8)%

        var diceRoll = _dice.Roll(10, DiceRollReason.SkillHealing, Combat.heroAttackResult.Dice);
        var currentHerowisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var fullRollImpact = (diceRoll + (currentHerowisdom / 8));

        GameState.Hero.remainingHitPointsPercentage = (GameState.Hero.remainingHitPointsPercentage + fullRollImpact) >= 100.0
            ? 100.0
            : (GameState.Hero.remainingHitPointsPercentage + fullRollImpact);

    }
}
