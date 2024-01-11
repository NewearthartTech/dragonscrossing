using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    public bool monsterParryDmgFromHero { get; set; }
    public bool heroParryDmgFromMonster { get; set; }

    LogicReponses AC4_ParryDmgFromOpposition()
    {
        if (Combat.isHerosTurn)
        {
            monsterParryDmgFromHero = true;
        }
        else
        {
            heroParryDmgFromMonster = true;
        }

        return LogicReponses.next;
    }
}


