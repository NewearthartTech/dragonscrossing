using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_sly_evasion()
    {
        // Add 5% + (Wisdom / 1.25)% chance to dodge
        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_sly_evasion));

        for (var i = 1; i < 4; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Sly Evasion",
                InRound = Combat.CurrentRound + i,
                Description = "Adds 5% + (Wisdom / 1.25)% chance to dodge",
                RemainingRounds = 4 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_sly_evasion)
            });          
        }
    }

    public void lingering_skill_sly_evasion()
    {
        var additionalChanceToDodge = 500;
        var heroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;

        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
        {
            new AlteredCalculatedStats
            {
                reason = "Hero skill - Sly Evasion",
                stats = new CalculatedCharacterStats { chanceToDodge = additionalChanceToDodge + ((int)Math.Round(heroWisdom / 1.25) * 100)  },

                round = Combat.CurrentRound + 1
            }
        }).ToArray();
    }
}
