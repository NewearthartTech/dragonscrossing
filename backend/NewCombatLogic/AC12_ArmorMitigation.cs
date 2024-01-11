using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;

namespace DragonsCrossing.NewCombatLogic;


public partial class CombatEngine
{
    public bool monsterMitigateDmgFromHero { get; set; }
    public bool heroMitigateDmgFromMonster { get; set; }

    LogicReponses AC12_ArmorMitigation()
    {
        if (Combat.isHerosTurn)
        {
            monsterMitigateDmgFromHero = true;
        }
        else
        {
            heroMitigateDmgFromMonster = true;
        }

        return LogicReponses.next;
    }
}


