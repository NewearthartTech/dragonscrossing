using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{

    public void apply_skill_ray_of_frost()
    {
        //Deals 1D4 + (wisdom / 8) and lowers monster Difficulty to hit by (Agility)% for 2 rounds
        var currentWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState).wisdom;

        var diceRoll2 = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice, (int)Math.Round(currentWisdom / 8.0));

        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= diceRoll2) ? Combat.Monster.HitPoints - diceRoll2 : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

        Combat.LingeringSkillEffects.RemoveAll(s => s.SkillName == "Ray of Frost");

        for (var i = 1; i < 3; i++)
        {
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Ray of Frost",
                InRound = Combat.CurrentRound + i,
                Description = "Lowers monster Difficulty to hit by (Agility)% for 2 rounds",
                RemainingRounds = 3 - (i + 1),
                RoundStarted = Combat.CurrentRound,
                ActionFunctionName = nameof(lingering_skill_ray_of_frost)
            });
        }
    }

    public void lingering_skill_ray_of_frost()
    {
        var currentMonsterDTH = Combat.Monster.GenerateCalculatedMonsterStats(Combat, true).DifficultyToHit;
        var heroAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;


        Combat.monsterAttackResult.monsteralteredStats = Combat.monsterAttackResult.monsteralteredStats.Concat(new AlteredMonsterCalculatedStats[]
            {
                new AlteredMonsterCalculatedStats
                {
                    reason = "Hero skill - Ray of Frost",
                    stats = new CalculatedMonsterStats { DifficultyToHit = (int)Math.Round(-currentMonsterDTH * (heroAgility / 100.0)) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
}
