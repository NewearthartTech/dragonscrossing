using System;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using static DragonsCrossing.Core.Helper.DataHelper;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    LogicReponses AC7_MonsterDied()
    {
        Combat.isMonsterDead = true;

        return LogicReponses.next;
    }

}

