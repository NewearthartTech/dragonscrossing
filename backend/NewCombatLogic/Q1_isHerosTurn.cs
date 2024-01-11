using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;

namespace DragonsCrossing.NewCombatLogic;


/*
 Initiative - Who goes first in combat. Initiative is re-rolled for every round of combat.
 Initiative is determined when both the player and monster roll 1D20 and add their quickness score. Highest goes first. 

 */

public partial class CombatEngine
{
    public void IsHerosTurn()
    {
        if (Combat.is2ndTurn)
        {
            //if it's 2nd turn flip the parties
            Combat.isHerosTurn = !Combat.isHerosTurn;

            //We set is2ndTurn here to false because both party will finish their attack after this point.
            Combat.is2ndTurn = false;
        }
        else
        {
            //Switch over to 2nd turn
            Combat.is2ndTurn = true;
        }
    }
    LogicReponses Q1_isHerosTurn()
    {
        IsHerosTurn();

        return Combat.isHerosTurn ? LogicReponses.yes : LogicReponses.no;
    }
}


