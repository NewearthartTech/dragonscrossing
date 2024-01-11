using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_herbal_knowledge()
    {
        // Heals 3 + (Wisdom / 8) Hit Points And increases dodge by 15% + (Wisdom / 6) for 2 rounds

        var heroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var healedAmount = (3 + heroWisdom / 8.0);

        GameState.Hero.remainingHitPointsPercentage = (GameState.Hero.remainingHitPointsPercentage + 3.0 / GameState.Hero.totalHitPoints * 100) >= 100.0
            ? 100.0
            : (GameState.Hero.remainingHitPointsPercentage + healedAmount / GameState.Hero.totalHitPoints * 100);

        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_herbal_knowledge));

        for (var i = 1; i < 3; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Herbal Knowledge",
                InRound = Combat.CurrentRound + i,
                Description = "increases dodge by 15% + (Wisdom / 6) for 2 rounds",
                RemainingRounds = 3 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_herbal_knowledge)
            });
        }
    }

    public void lingering_skill_herbal_knowledge()
    {
        var currentChanceToDodge = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToDodge;
        var heroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var baseLineDodgeBenefit = 1500;
        var totalDodgeBenefit = ((heroWisdom / 6.0) * 100) + baseLineDodgeBenefit;

        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
        {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Herbal Knowledge",
                    stats = new CalculatedCharacterStats { chanceToDodge = (int)Math.Round(currentChanceToDodge * (totalDodgeBenefit / 10000)) },

                    round = Combat.CurrentRound + 1
                }
        }).ToArray();
    }
}
