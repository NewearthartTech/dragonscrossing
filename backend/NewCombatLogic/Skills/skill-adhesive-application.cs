 using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    //Reduces Monster Difficulty to Hit by (strength + agility) / 1.5) % for 2 rounds and Reduces monster dodage by (wisdom * 2.0)% for 3 rounds

    public void apply_skill_adhesive_application()
    {
        Combat.LingeringSkillEffects.RemoveAll(s => s.SkillName == "Adhesive Application - DTH");
        Combat.LingeringSkillEffects.RemoveAll(s => s.SkillName == "Adhesive Application - Dodge");

        for (var i = 1; i < 3; i++)
        {
            //Reduces Monster Difficulty to Hit by (strength + agility) / 1.5) % for 2 rounds
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Adhesive Application - DTH",
                InRound = Combat.CurrentRound + i,
                Description = "Reduces monster difficulty to hit by ((strength + agility) / 1.5)%",
                RemainingRounds = 3 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_adhesive_application_DTH)
                
            });
            
        }

        for (var i = 1; i < 4; i++)
        {
            // Reduces monster dodage by (wisdom * 2)% for 3 rounds
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Adhesive Application - Dodge",
                InRound = Combat.CurrentRound + i,
                Description = "Reduces monster dodage by (wisdom * 2.0)%",
                RemainingRounds = 4 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_adhesive_application_dodge)
                
            });
        }
        
    }

    public void lingering_skill_adhesive_application_DTH()
    {
        var currentMonsterDTH = Combat.Monster.GenerateCalculatedMonsterStats(Combat, true).DifficultyToHit;
        var currentHeroStrength = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).strength;
        var currentHeroAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;
        var heroStrengthAgilityImpact = ((currentHeroAgility + currentHeroStrength) / 1.5 / 100);

        Combat.monsterAttackResult.monsteralteredStats = Combat.monsterAttackResult.monsteralteredStats.Concat(new AlteredMonsterCalculatedStats[]
            {
                new AlteredMonsterCalculatedStats
                {
                    reason = "Hero skill - Adhesive Application",

                 
                    
                    stats = new CalculatedMonsterStats { DifficultyToHit = (int)Math.Round((-currentMonsterDTH * heroStrengthAgilityImpact)) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }

    public void lingering_skill_adhesive_application_dodge()
    {
        var currentMonsterDodge = Combat.Monster.GenerateCalculatedMonsterStats(Combat, true).ChanceToDodge;
        var currentHeroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var heroWisdomImpact = currentHeroWisdom * 2.0 / 100;

        Combat.monsterAttackResult.monsteralteredStats = Combat.monsterAttackResult.monsteralteredStats.Concat(new AlteredMonsterCalculatedStats[]
            {
                new AlteredMonsterCalculatedStats
                {
                    reason = "Hero skill - Adhesive Application",
                    stats = new CalculatedMonsterStats { ChanceToDodge = (int)Math.Round(-currentMonsterDodge * heroWisdomImpact) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }

}
