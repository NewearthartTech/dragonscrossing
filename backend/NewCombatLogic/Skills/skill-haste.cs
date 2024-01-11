using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_haste()
    {
        // Increase Player Chance to Hit by a flat 5 + ((Quickness + wisdom) / 4)%.

        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_berserk));

        Combat.LingeringSkillEffects.Add(new LingeringSkillAction
        {
            SkillName = "Haste - CTH",
            InRound = Combat.CurrentRound,
            ActionFunctionName = nameof(lingering_skill_haste_CTH),
            Description = "Increases hero chance to hit by a flat 5 + ((Quickness + Wisdom) / 4)%",
            RemainingRounds = -1,
            RoundStarted = Combat.CurrentRound
        });
    }

    public void lingering_skill_haste_CTH()
    {
        var baseCthBonus = 500;
        var heroQuickness = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).quickness;
        var heroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var additionalChanceToHit = baseCthBonus + (((heroQuickness + heroWisdom) / 4) * 100);

        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Haste",
                    stats = new CalculatedCharacterStats { chanceToHit = additionalChanceToHit },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
}
