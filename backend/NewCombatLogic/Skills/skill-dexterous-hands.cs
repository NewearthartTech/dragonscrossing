using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_dexterous_hands()
    {
        // Increased Player Crit by 40% + (Agility /2)% for 3 rounds. Increases Player Parry by (strength)% for 3 rounds.
        Combat.LingeringSkillEffects.RemoveAll(s => s.SkillName == "Dexterous Hands - CTC");
        Combat.LingeringSkillEffects.RemoveAll(s => s.SkillName == "Dexterous Hands - CTP");

        for (var i = 1; i < 4; i++)
        {
            // Increased Player Crit by 40% for 3 rounds
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Dexterous Hands - CTC",
                InRound = Combat.CurrentRound + i,
                Description = "Increases hero chance to crit by 40 + (Agi / 2)%",
                RemainingRounds = 4 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_dexterous_hands_crit)
            });
        }

        for (var i = 1; i < 4; i++)
        {
            //   Increases Player Parry by (strength)% for 3 rounds.
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Dexterous Hands - CTP",
                InRound = Combat.CurrentRound + i,
                Description = "Increases hero chance to parry by 30% + (strength)%",
                RemainingRounds = 4 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_dexterous_hands_parry)
            });
        }

    }

    public void lingering_skill_dexterous_hands_crit()
    {
        var currentCrit = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToCrit;
        var currentAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;
        var agilityImpact = currentAgility / 2;

        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Dexterous Hands",
                    stats = new CalculatedCharacterStats { chanceToCrit = (int)Math.Round((currentCrit * (.4 + agilityImpact / 100) ))},

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }

    public void lingering_skill_dexterous_hands_parry()
    {
        var currentParry = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToParry;
        var currentStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).strength;

        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Dexterous Hands",
                    stats = new CalculatedCharacterStats { chanceToParry = (int)Math.Round(currentParry * (currentStrength / 100.0)) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
}
