using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    LogicReponses Q9_IsTurn2()
    {   
        // If is2ndTurn is true, return to Q1 and start the combat for the opposite combatant
        if (Combat.is2ndTurn)
            return LogicReponses.yes;

        return LogicReponses.no;
    }
}


