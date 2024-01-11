using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_drowse()
    {
        // Reduces Monster Dodge by wisdom + quicknes / 1.25 and Parry by wisdom + strength / 1.25 for the next 3 rounds
        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_drowse));

        for (var i = 1; i < 4; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Drowse",
                InRound = Combat.CurrentRound + i,
                Description = "Reduces monster chance to dodge by ((wisdom + quickness) / 1.25)% AND reduces monster chance to parry by ((wisdom + strength) / 1.25)% ",
                RemainingRounds = 4 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_drowse)
            });
        }
    }
    public void lingering_skill_drowse()
    {
        var currentChanceToDodge = Combat.Monster.GenerateCalculatedMonsterStats(Combat, true).ChanceToDodge;
        var currentChanceToParry = Combat.Monster.GenerateCalculatedMonsterStats(Combat, true).ParryChance;

        var heroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var heroQuickness = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).quickness;
        var heroStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).strength;

        var dodgeImpact = ((heroWisdom + heroQuickness) / 1.25 / 100);
        var parryImpact = ((heroWisdom + heroStrength) / 1.25 / 100);

        Combat.monsterAttackResult.monsteralteredStats = Combat.monsterAttackResult.monsteralteredStats.Concat(new AlteredMonsterCalculatedStats[]
        {
            new AlteredMonsterCalculatedStats
            {
                reason = "Hero skill - Drowse",
                stats = new CalculatedMonsterStats { 
                    ChanceToDodge = (int)Math.Round(-currentChanceToDodge * dodgeImpact), ParryChance = (int)Math.Round(-currentChanceToParry * parryImpact) },

                    round = Combat.CurrentRound + 1
            }
        }).ToArray();
    }
}
