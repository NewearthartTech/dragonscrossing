using DragonsCrossing.Domain.Items;
using Microsoft.EntityFrameworkCore;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class MonsterItemLootSeed
    {
        public MonsterItemLootSeed(ModelBuilder builder)
        {
            builder.Entity<MonsterItemLoot>(b =>
            {      
                // Wandering Goblin loot
                b.HasData(new MonsterItemLoot()
                {
                    Id = 1,
                    MonsterTemplateId = 1,     
                    LootDropChance = 50,
                    WeaponTemplateId = 4,                    
                });
                b.HasData(new MonsterItemLoot()
                {
                    Id = 2,
                    MonsterTemplateId = 1,
                    LootDropChance = 50,
                    WeaponTemplateId = 5,
                });

                // Rival Adventurer loot
                b.HasData(new MonsterItemLoot()
                {
                    Id = 3,
                    MonsterTemplateId = 2,
                    LootDropChance = 50,
                    WeaponTemplateId = 4,
                });
                b.HasData(new MonsterItemLoot()
                {
                    Id = 4,
                    MonsterTemplateId = 2,
                    LootDropChance = 50,
                    WeaponTemplateId = 5,
                });

                // Giant Wolf loot
                b.HasData(new MonsterItemLoot()
                {
                    Id = 5,
                    MonsterTemplateId = 3,
                    LootDropChance = 50,
                    WeaponTemplateId = 4,
                });
                b.HasData(new MonsterItemLoot()
                {
                    Id = 6,
                    MonsterTemplateId = 3,
                    LootDropChance = 50,
                    WeaponTemplateId = 5,
                });

                // Mighty Stag loot
                b.HasData(new MonsterItemLoot()
                {
                    Id = 7,
                    MonsterTemplateId = 4,
                    LootDropChance = 50,
                    WeaponTemplateId = 4,
                });
                b.HasData(new MonsterItemLoot()
                {
                    Id = 8,
                    MonsterTemplateId = 4,
                    LootDropChance = 50,
                    WeaponTemplateId = 5,
                });
            });
        }
    }
}
