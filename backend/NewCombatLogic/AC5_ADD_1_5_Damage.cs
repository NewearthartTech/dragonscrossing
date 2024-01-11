using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    public bool heroCritAttack { get; set; }
    public bool monsterCritAttack { get; set; }

    LogicReponses AC5_ADD_1_5_Damage()
    {
        if (Combat.isHerosTurn)
        {
            heroCritAttack = true;
        }
        else
        {
            monsterCritAttack = true;
        }

        return LogicReponses.next;
    }
}


