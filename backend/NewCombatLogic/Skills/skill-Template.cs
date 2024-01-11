using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{

    public void apply_skill_template()
    {
        var currentChanceToParry = GameState.Hero.GenerateCalculatedCharacterStats(GameState).chanceToParry;
        var currentMonsterChanceToParry = Combat.Monster.GenerateCalculatedMonsterStats(Combat).ParryChance;

        //Deals 1D6 damage and reduces parry for both Monster and Player by 80% for 2 rounds.

        // 1: Any direct damage down right away should go here like the following
        var diceRoll = _dice.Roll(6, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= diceRoll) ? Combat.Monster.HitPoints - diceRoll : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;

        Combat.LingeringSkillEffects.RemoveAll(s => s.SkillName == "Barbed Projectile");

        // 2. Any linger affects put in a for loop.
        // This i = 1; i < 3 means its for a 2 round linger effect
        for (var i=1; i<3; i++)
        {
            // Remember, add ANY lingering affect to the LingeringSkillEffects collection. It could be hit points or other stats
            Combat.LingeringSkillEffects.Add(new LingeringSkillAction
            {
                SkillName = "Barbed Projectile",
                // Lingering effects start on the next round. When casted in round 0 (Before any attack action), it would start at round 0 + 1 => 1 
                // For any stat lingering, we need to do the extra +1, if the lingering is immediate dmg then don't 
                InRound = Combat.CurrentRound + i + 1,
                // ActionFunctionName is optional, when not set, it means we don't need to call any funtion. 
                // It is usually set when we are calling function to reduce hitpoints
                // It is usually NOT set when we are changing stats. Changing stat requires us to perform point #3
                ActionFunctionName = nameof(lingering_skill_Template),
                Description = "Deals additional 1D4 damage",
                // Remaining rounds = look at the upper boundary of the loop which is 3 and then - i
                // If remaining rouns is set to -1, it means it is a status effect till end of game
                RemainingRounds = 3 - i,
                RoundStarted = Combat.CurrentRound
            });
        }
    }

    void lingering_skill_Template()
    {
        var diceRoll = _dice.Roll(4, DiceRollReason.SkillDamageRightAway, Combat.heroAttackResult.Dice);
        Combat.Monster.HitPoints = (Combat.Monster.HitPoints >= diceRoll) ? Combat.Monster.HitPoints - diceRoll : 0;
        Combat.isMonsterDead = Combat.Monster.HitPoints <= 0;
        // For permanent stat changes, use the following way - refer to skill_Rending_Blow
        //Combat.Monster.ArmorMitigation = (int)Math.round(Combat.Monster.ArmorMitigation * 85 / 100.0);
    }


    void lingering_skill_Template_Lingering()
    {
        var currentChanceToDodge = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToDodge;

        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
        {
            new AlteredCalculatedStats
            {
                reason = "Hero skill - test",
                stats = new CalculatedCharacterStats { chanceToDodge = (int)Math.Round(currentChanceToDodge * -80 /100.0) },
                // Remember to do + 1 here as it starts from next round of combat
                    round = Combat.CurrentRound + 1
            }
        }).ToArray();
    }
}
