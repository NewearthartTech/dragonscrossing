using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_bladeform_parry()
    {
        // Incease chance to parry by ((strength + quickness) * 4)% for 3 rounds
        Combat.LingeringSkillEffects.RemoveAll(s => s.SkillName == "Bladeform Parry");

        for (var i = 1; i < 4; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Bladeform Parry",
                InRound = Combat.CurrentRound + i,
                Description = "Inceases hero chance to parry by ((str + quickness) * 4)% for 3 rounds",
                RemainingRounds = 4 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_bladeform_parry)
            });
        }
    }

    public void lingering_skill_bladeform_parry()
    {
        var currentChanceToParry = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToParry;
        var currentHeroStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).strength;
        var currentHeroQuickness = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).quickness;
        var strAndAgilityImpact = ((currentHeroStrength + currentHeroQuickness) * 4.0);

        // Incease chance to parry ((strength + quickness) * 4)% for 3 rounds
        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Sniper",
                    stats = new CalculatedCharacterStats { chanceToParry = (int)Math.Round(currentChanceToParry * (strAndAgilityImpact / 100)) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
}
