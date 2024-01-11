using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_riposte()
    {
        // Increases Player Crit by 65% + (Agility / 2)% for 2 rounds. Increases Player Parry by 50% + (Strength / 2) for 2 rounds.

        Combat.LingeringSkillEffects.RemoveAll(s => s.SkillName == "Riposte");

        for (var i = 1; i < 4; i++)
        {
            // Increased Player Crit by 75% for 2 rounds
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Riposte",
                InRound = Combat.CurrentRound + i,
                Description = "Increases hero crit chance by 65% + (Agility / 2)% and parry by 50% + (strength / 2)",
                RemainingRounds = 4 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_riposte)
            });
        }
    }

    public void lingering_skill_riposte()
    {
        var currentCrit = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToCrit;
        var currentParry = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToParry;

        var heroAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;
        var heroStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).strength;

        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Riposte",
                    stats = new CalculatedCharacterStats {
                        chanceToCrit = (int)Math.Round(currentCrit * ((heroAgility / 2 + 65) / 100.0)),
                        chanceToParry = (int)Math.Round(currentParry * ((heroAgility / 2 + 65) / 100.0))
                    },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
}
