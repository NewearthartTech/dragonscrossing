using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_ice_tomb()
    {
        //Deals 3D4 + 3 damage and lowers monster armor mitigation chance by (Wisdom /1.75)% until end of combat

        var diceRoll1 = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var diceRoll2 = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        var diceRoll3 = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice, 3);

        var totalDmg = diceRoll1 + diceRoll2 + diceRoll3;
        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= totalDmg) ? Combat.Monster.HitPoints - totalDmg : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_ice_tomb));

        Combat.LingeringSkillEffects.Add(new LingeringSkillAction
        {
            SkillName = "Ice Tomb",
            InRound = Combat.CurrentRound,
            Description = "Lowers monster armor mitigation chance by (Wisdom /1.75)% until end of combat",
            RemainingRounds = -1,
            RoundStarted = Combat.CurrentRound,
            ActionFunctionName = nameof(lingering_skill_ice_tomb)
        });
    }

    public void lingering_skill_ice_tomb()
    {
        var currentMonsterArmorMitigation = Combat.Monster.GenerateCalculatedMonsterStats(Combat, true).ArmorMitigation;
        var heroWisdom = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).wisdom;
        var wisdomImpact = (heroWisdom / 1.75 / 100);

        Combat.monsterAttackResult.monsteralteredStats = Combat.monsterAttackResult.monsteralteredStats.Concat(new AlteredMonsterCalculatedStats[]
        {
            new AlteredMonsterCalculatedStats
            {
                reason = "Hero skill - Ice Tomb",
                stats = new CalculatedMonsterStats { ArmorMitigation = (int)Math.Round(-currentMonsterArmorMitigation * wisdomImpact) },

                    round = Combat.CurrentRound + 1
            }
        }).ToArray();
    }
}
