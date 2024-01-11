using System;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Domain.GameStates;
using static DragonsCrossing.Core.Helper.DataHelper;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{

    LogicReponses AC11_HeroGainExperience()
    {      
        // If hero is 3 or less than 3 levels above the monster, give xp. Otherwise, no xp
        if (is_hero_Eligible_NFT_And_Exp )
            GameState.Hero.experiencePoints = GameState.Hero.experiencePoints + 1 >= GameState.Hero.maxExperiencePoints ? GameState.Hero.maxExperiencePoints : GameState.Hero.experiencePoints + 1;
        else 
        {
            _logger.LogDebug($"HeroId {GameState.HeroId} didn't get experience because hero level is {GameState.Hero.level - Combat.Monster!.Level} higher than monster.");
        }
        return LogicReponses.terminal;
    }

    public bool is_hero_Eligible_NFT_And_Exp
    {
        get { return (GameState.Hero.level - Combat.Monster!.Level) <= 3; }
    }
}

