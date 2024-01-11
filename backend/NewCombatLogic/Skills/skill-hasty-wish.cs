using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Helper;
using System;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    public void apply_skill_hasty_wish()
    {
        Combat.LingeringSkillEffects.RemoveAll(s => s.ActionFunctionName == nameof(lingering_skill_hasty_wish));

        // Increase Player Parry, Dodge, Armor Mitigation Chance, Crit by 25%,  and Chance to Hit by a flat 15%
        Combat.LingeringSkillEffects.Add(new LingeringSkillAction
        {
            SkillName = "Hasty Wish",
            InRound = Combat.CurrentRound,
            ActionFunctionName = nameof(lingering_skill_hasty_wish),
            Description = "Increases hero parry, dodge, armor mitigation chance, and crit by 25%. Increases hero chance to hit by a flat 25%",
            RemainingRounds = -1,
            RoundStarted = Combat.CurrentRound
        });

    }

    public void lingering_skill_hasty_wish()
    {
        var currentParry = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToParry;
        var currentDodge = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToDodge;
        var currentArmorMitigation = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).armorMitigation;
        var currentCrit = GameState.Hero.GenerateCalculatedCharacterStats(GameState, true).chanceToCrit;
        var increasedCTH = 1500;

        Combat.heroAttackResult.heroAlteredStats = Combat.heroAttackResult.heroAlteredStats.Concat(new AlteredCalculatedStats[]
            {
                new AlteredCalculatedStats
                {
                    reason = "Hero skill - Haste",
                    stats = new CalculatedCharacterStats 
                    { 
                        chanceToParry = (int)Math.Round(currentParry * .25),
                        chanceToDodge = (int)Math.Round(currentDodge * .25),
                        armorMitigation = (int)Math.Round(currentArmorMitigation * .25),
                        chanceToCrit = (int)Math.Round(currentCrit * .25),
                        chanceToHit = increasedCTH,
                    },

                    round = Combat.CurrentRound + 1
                }
            }).ToArray();
    }
}
