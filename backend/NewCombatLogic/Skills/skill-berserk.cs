using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_berserk()
    {
        // Increase Player Bonus Damage by 75% + 1 for 3 rounds. Decrease Player Armor Mitigation Chance by 30% - (quickness / 5)% for 2 rounds

        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_berserk));

        for (var i = 1; i < 4; i++)
        {
            // Increase Player Bonus Damage by 75% + 1 for 3 rounds.
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Berserk - Bonus Damage",
                InRound = Combat.CurrentRound + i,
                Description = "Increase Player Bonus Damage by 75% + 1",
                RemainingRounds = 4 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_berserk)
            });
        }

        for (var i = 1; i < 3; i++)
        {
            // Decrease Player Armor Mitigation Chance by 30% - (quizkness / 5)% for 2 rounds.
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Berserk - Armor Mitigation",
                InRound = Combat.CurrentRound + i,
                Description = "Decrease Player Armor Mitigation Chance by 30% - (quickness / 5)%",
                RemainingRounds = 3 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_berserk_AMC)
            });
        }
    }

    public void lingering_skill_berserk()
    {
        _logger.LogDebug($"applying lingering_skill_berserk hero ID: {GameState.HeroId}, Round: {Combat.CurrentRound}");
        var currentBonusDamage = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).statsBonusDamage + GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).itemBonusDamage;

        // We put all the extra bonus damages from the skill on statsBonusDamage because we can only choose one to add the bonus dmg on.
        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Berserk",
                    stats = new CalculatedCharacterStats { statsBonusDamage = (int)Math.Round(currentBonusDamage * .75) + 1 },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }

    public void lingering_skill_berserk_AMC()
    {
        _logger.LogDebug($"applying lingering_skill_berserk_AMC hero ID: {GameState.HeroId}, Round: {Combat.CurrentRound}"); 
        var currentArmorMitigation = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).armorMitigation;
        var currentQuickness = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).quickness;
        var quicknessImpact = currentQuickness / 5;

        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Berserk",
                    stats = new CalculatedCharacterStats { armorMitigation = (int)Math.Round(-currentArmorMitigation * ((30 - quicknessImpact) /100.0)) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
}
