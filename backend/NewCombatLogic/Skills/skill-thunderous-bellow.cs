using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.Players;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_thunderous_bellow()
    {
        // Reduce the monster's parry and dodge by ((Strength + Wisdom) / 1.5) for 2 rounds.
        // Increase the player's parry score by (charisma / 1.5)% until the end of combat
        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_thunderous_bellow));
        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_thunderous_bellow_player));

        for (var i = 1; i < 3; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Thunderous Bellow - Parry & Dodge",
                InRound = Combat.CurrentRound + i,
                Description = "Reduce the monster's parry and dodge by ((Strength + Wisdom) / 1.5)",
                RemainingRounds = 3 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_thunderous_bellow)
            });
        }

        Combat.LingeringSkillEffects.Add(new LingeringSkillAction
        {
            SkillName = "Thunderous Bellow - Chance to Parry",
            InRound = Combat.CurrentRound,
            ActionFunctionName = nameof(lingering_skill_thunderous_bellow_player),
            Description = "Increase the player's parry score by (charisma / 1.5)%",
            // Set RemainingRounds to -1 when it is a permanent stat change
            RemainingRounds = -1,
            RoundStarted = Combat.CurrentRound
        });
    }

    public void lingering_skill_thunderous_bellow()
    {
        // Reduce the monster's parry and dodge by ((Strength + Wisdom) / 1.5)

        var currentMonsterDodge = Combat.Monster.GenerateCalculatedMonsterStats(Combat, true).ChanceToDodge;

        var currentMonsterParry = Combat.Monster.GenerateCalculatedMonsterStats(Combat, true).ParryChance;

        var heroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var heroStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).strength;
        var wisdomAndStrengthImpact = ((heroStrength + heroWisdom) / 1.5 / 100.0);


        Combat.monsterAttackResult.monsteralteredStats = Combat.monsterAttackResult.monsteralteredStats.Concat(new AlteredMonsterCalculatedStats[]
            {
                new AlteredMonsterCalculatedStats
                {
                    reason = "Hero skill - Thunderous Bellow",
                    stats = new CalculatedMonsterStats { ChanceToDodge = (int)Math.Round(-currentMonsterDodge * (wisdomAndStrengthImpact)), ParryChance = (int)Math.Round(-currentMonsterParry * (wisdomAndStrengthImpact)) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }

    public void lingering_skill_thunderous_bellow_player()
    {
        // Increase the player's parry score by an amount equal to their charisma score. 
        var currentCharisma = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).charisma;
        var currentParry = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToParry;

        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Thunderous Bellow",
                    stats = new CalculatedCharacterStats { chanceToParry = (currentParry * ((int)Math.Round(currentCharisma / 1.5 / 100))) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
}
