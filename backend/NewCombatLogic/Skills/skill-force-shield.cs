using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_force_shield()
    {
        // Increases Player Armor Mitigation Amount by 30% + ((str + charisma) /2) + 1 for 3 rounds.
        // Increased Player Armor Mitigation Chance by ((agility + wisdom) / 2)% for 2 rounds.
        Combat.LingeringSkillEffects.RemoveAll(s => s.SkillName == "Force shield - Armor Mitigation");
        Combat.LingeringSkillEffects.RemoveAll(s => s.SkillName == "Force shield");


        for (var i = 1; i < 4; i++)
        {
            // Increases Player Armor Mitigation Amount by 25% + 1 for 3 rounds
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Force shield - Armor Mitigation",
                InRound = Combat.CurrentRound + i,
                Description = "Increases hero Armor Mitigation Amount by 30% + ((str + charisma) /2) + 1",
                RemainingRounds = 4 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_force_shield)
            });
        }

        for (var i = 1; i < 3; i++)
        {
            // Increased Player Armor Mitigation Chance by 20% for 2 rounds.
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Force shield",
                InRound = Combat.CurrentRound + i,
                Description = "Armor Mitigation Chance by ((agility + wisdom) / 2)%",
                RemainingRounds = 3 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_force_shield_AMC)
            });
        }
    }

    public void lingering_skill_force_shield()
    {
        var currentArmorMitigationAmount = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).armorMitigationAmount;
        var heroStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).strength;
        var heroCharisma = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).charisma;
        var strAndCharismaImpact = ((heroCharisma + heroStrength) / 2.0 /100);

        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Force Shield",
                    stats = new CalculatedCharacterStats { armorMitigationAmount = (int)Math.Round(currentArmorMitigationAmount * (.3 + strAndCharismaImpact)) + 1 },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }

    public void lingering_skill_force_shield_AMC()
    {
        var currentArmorMitigation = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).armorMitigation;
        var heroAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;
        var heroWisdom= GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var agilityAndWisdomImpact = ((heroAgility + heroWisdom) / 2.0 / 100);


        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Force Shield",
                    stats = new CalculatedCharacterStats { armorMitigation = (int)Math.Round(currentArmorMitigation * agilityAndWisdomImpact) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
}
