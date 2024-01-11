using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Infrastructure
{
    public interface IMonstersRepository
    {
        Task<MonsterTemplateOld> GetMonsterTemplate(int MonsterTemplateId);
        Task<int> SaveMonster(Monster monster);
        Task<Monster> GetMonster(int monsterId);
        Task<List<MonsterTemplateOld>> GetMonsterTemplatesByTile(int tileId);
        Task<List<MonsterPersonality>> GetAllMonsterPersonalities();
        Task<List<MonsterItemLoot>> GetMonsterItemsLoot(int MonsterTemplateId);
    }
}
