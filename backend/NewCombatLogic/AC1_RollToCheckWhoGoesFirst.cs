using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Helper;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    // This is only called by unit test
    public bool IsHerosTurn(int playerRoll, int monsterRoll)
    {
        // If tie, monster goes first
        return (playerRoll + GameState.Hero.GenerateCalculatedCharacterStats(GameState).quickness / 4) 
            > (monsterRoll + Combat.Monster.GenerateCalculatedMonsterStats(Combat).Quickness / 4);
    }
    LogicReponses AC1_RollToCheckWhoGoesFirst()
    {
        var dice = _dice.Roll(20,
            DiceRollReason.MonsterGoesFirst,
            Combat.monsterAttackResult.Dice); //reviewed
        var monsterRoll = dice;

        dice = _dice.Roll(20,
            DiceRollReason.HeroGoesFirst,
            Combat.heroAttackResult.Dice); //reviewed
        var playerRoll = dice;

        var heroQuicknessBonus = GameState.Hero.GenerateCalculatedCharacterStats(GameState).quickness / 4;
        var monsterQuicknesslBonus = Combat.Monster.GenerateCalculatedMonsterStats(Combat).Quickness / 4;

        bool isHerosTurn = (playerRoll + heroQuicknessBonus) > (monsterRoll + monsterQuicknesslBonus);

        _logger.LogDebug($"Q1_playerGoesFirst: monsterRoll ={monsterRoll}:{heroQuicknessBonus}, playerRoll={playerRoll}:{monsterQuicknesslBonus}. IsHerosturn = {isHerosTurn}");

        //If Hero Goes first, we set the turn to hero
        Combat.isHerosTurn = isHerosTurn;

        Combat.Initiative = Combat.isHerosTurn ? CombatantType.Hero : CombatantType.Monster;

        return LogicReponses.next;
    }
}


