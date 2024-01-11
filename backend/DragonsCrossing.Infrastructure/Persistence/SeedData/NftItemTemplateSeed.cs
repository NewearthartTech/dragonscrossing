using DragonsCrossing.Domain.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    public class NftItemTemplateSeed
    {
        public NftItemTemplateSeed(ModelBuilder builder)
        {
            builder.Entity<NftItemTemplate>().HasData(
                new NftItemTemplate()
                {
                    Id = 1,
                    Name = "Hero Egg",
                    GoldCostToOpen = 200,
                    DcxCostToOpen = 1,
                    Description = "Shiny golden egg",
                    ImageBaseUrl = @"/images/items/nft/hero_egg",
                    Type = NftItemType.Shard,
                },
                new NftItemTemplate()
                {
                    Id = 2,
                    Name = "Skill Book",
                    GoldCostToOpen = 200,
                    DcxCostToOpen = 1,
                    Description = "Book of mystic arts",
                    ImageBaseUrl = @"/images/items/nft/skill_book",
                    Type = NftItemType.Skillbook,
                });
        }
    }
}
