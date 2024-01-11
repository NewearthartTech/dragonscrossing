using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_rending_blow()
    {
        //Deals 1D10 + ((Wisdom + Agility ) / 13) damage, and reduces monster mitigation chance by 15% until end of combat
        var heroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var heroAgility = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).agility;
        var wisdomAgilityImpact = ((heroAgility + heroWisdom) / 13.0);
        var diceRoll = _dice.Roll(10, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice) + (int)Math.Round(wisdomAgilityImpact);

        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= diceRoll) ? Combat.Monster.HitPoints - diceRoll : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_rending_blow));


        Combat.LingeringSkillEffects.Add(new LingeringSkillAction
        {
            SkillName = "Rending Blow",
            InRound = Combat.CurrentRound,
            ActionFunctionName = nameof(lingering_skill_rending_blow),
            Description = "Reduces monster armor mitigation by 15%",
            // Set RemainingRounds to -1 when it is a permanent stat change
            RemainingRounds = -1,
            RoundStarted = Combat.CurrentRound
        });
    }

    void lingering_skill_rending_blow()
    {
        var currentMonsterArmorMitigation = Combat.Monster.GenerateCalculatedMonsterStats(Combat, true).ArmorMitigation;

        Combat.monsterAttackResult.monsteralteredStats = Combat.monsterAttackResult.monsteralteredStats.Concat(new AlteredMonsterCalculatedStats[]
            {
                new AlteredMonsterCalculatedStats
                {
                    reason = "Hero skill - Rending Blow",
                    stats = new CalculatedMonsterStats { ArmorMitigation = (int)Math.Round(-currentMonsterArmorMitigation * .15) },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
}
