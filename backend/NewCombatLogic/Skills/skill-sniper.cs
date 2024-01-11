using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_sniper()
    {
        // Increase chance to crit by (Agility * 8)% for 2 rounds
        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_sniper));
        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_sniper_cth));

        Combat.LingeringSkillEffects.Add(new LingeringSkillAction
        {
            SkillName = "Sniper - CTH",
            InRound = Combat.CurrentRound,
            ActionFunctionName = nameof(lingering_skill_sniper_cth),
            Description = "Increases CTH by flat 10% until end of combat",
            RemainingRounds = -1,
            RoundStarted = Combat.CurrentRound
        });

        for (var i = 1; i < 3; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Sniper - Crit Increase",
                InRound = Combat.CurrentRound + i,
                Description = "Increase chance to crit by (Agility * 8)% for 2 rounds",
                RemainingRounds = 3 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_sniper)
            });
        }


    }

    public void lingering_skill_sniper()
    {
        var currentChanceToCrit = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToCrit;
        var heroAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;


        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Sniper Crit",
                    stats = new CalculatedCharacterStats { chanceToCrit = (int)Math.Round(currentChanceToCrit * ((heroAgility * 8)/100.0)) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
    public void lingering_skill_sniper_cth()
    {
        var flatChanceToHit = 1000;


        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Sniper CTH",
                    stats = new CalculatedCharacterStats { chanceToCrit = flatChanceToHit },
                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
}
