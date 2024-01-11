using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;

namespace DragonsCrossing.NewCombatLogic;




public partial class CombatEngine
{
    LogicReponses AC6_RollForDamage_IncludeAdditionalAbility()
    {
        //Re-evaluate this flow and maybe remove it
        return LogicReponses.next;
    }
}


