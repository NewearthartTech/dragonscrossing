using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;

namespace DragonsCrossing.NewCombatLogic.tests.ContextSeed
{
    public class ItemContextSeed
    {
        public static ItemTemplate ItemTemplateSeed
        {
            get
            {
                return new ItemTemplate
                {
                    name = "Basic Leather Armor",
                    slot = ItemSlotTypeDto.chest,
                    dieDamage = new DieDto[]{ new DieDto { Sides = 5 }, new DieDto { Sides = 5 }, new DieDto { Sides = 5 } },
                    affectedAttributes = ReturnAffectedAttributesTemplate()
                };
            }
        }

        public static Dictionary<AffectedHeroStatTypeDto, Range> ReturnAffectedAttributesTemplate()
        {
            Dictionary<AffectedHeroStatTypeDto, Range> dic = new Dictionary<AffectedHeroStatTypeDto, Range>();

            dic.Add(AffectedHeroStatTypeDto.Agility, new Range { lower = 4, upper = 8 });
            dic.Add(AffectedHeroStatTypeDto.Wisdom, new Range { lower = 2, upper = 4 });
            dic.Add(AffectedHeroStatTypeDto.ChanceToHit, new Range { lower = 1, upper = 5 });

            return dic;
        }


        public static List<ItemDto> ItemsSeedData
        {
            get
            {
                return new List<ItemDto>
                {
                    new ItemDto
                    {
                        id = "1",
                        
                        name = "Basic Leather Armor",
                        slot = ItemSlotTypeDto.chest,
                        dieDamage = new DieDto[]{ new DieDto { Sides = 5 }, new DieDto { Sides = 5 } },
                        itemIndex = 7,  
                        rarity = ItemRarityDto.Mythic,
                        bonusDamage = 3
                    },
                    new ItemDto
                    {
                        id = "2",
                        
                        name = "Advanced Leather Armor",
                        slot = ItemSlotTypeDto.chest,
                        dieDamage = new DieDto[]{ new DieDto { Sides = 5 } },
                        itemIndex = 7,
                        rarity = ItemRarityDto.Rare,
                        bonusDamage = 3
                    }
                };
            }
        }     
    }
}
