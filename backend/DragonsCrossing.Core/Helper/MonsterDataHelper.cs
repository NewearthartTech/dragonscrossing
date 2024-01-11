using System.Reflection;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.NewCombatLogic;
using Newtonsoft.Json;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;

namespace DragonsCrossing.Core.Helper
{
    public static partial class DataHelper
    {
        public static CalculatedMonsterStats GenerateCalculatedMonsterStats(this MonsterDto monster, CombatEncounter combat, bool isBaseOnly = false)
        {
            var currentRound = combat?.CurrentRound ?? -1;

            var ret = new CalculatedMonsterStats
            {
                Quickness = monster.Quickness,
                Charisma = monster.Charisma,
                ChanceToHit = monster.ChanceToHit,
                DifficultyToHit = monster.DifficultyToHit,
                CritChance = monster.CritChance,
                ChanceToDodge = monster.ChanceToDodge,
                ParryChance = monster.ParryChance,
                ArmorMitigation = monster.ArmorMitigation,
                BonusDamage = monster.BonusDamage,
                ArmorMitigationAmount = monster.ArmorMitigationAmount
            };

            // get all integer properties
            var statProps = typeof(CalculatedMonsterStats)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(int))
                .ToArray();

            if (null != combat && null != combat.monsterAttackResult && !isBaseOnly)
            {
                foreach (var alteredStat in combat.monsterAttackResult.monsteralteredStats.Where(h => h.round == currentRound))
                {
                    foreach (var prop in statProps)
                    {
                        var value = prop.GetValue(alteredStat.stats);
                        if (null == value)
                            continue;

                        var intValue = (int)value;

                        if (0 == intValue)
                            continue;

                        var orgValue = prop.GetValue(ret);
                        if (null == orgValue)
                            continue;

                        var orgIntValue = (int)orgValue;
                        prop.SetValue(ret, orgIntValue + intValue);
                    }
                }
            }

            return ret;
        }  
    }
}
