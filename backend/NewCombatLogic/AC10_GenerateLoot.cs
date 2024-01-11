using System;
using System.Linq;
using AutoMapper.Internal;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using static DragonsCrossing.Core.Helper.DataHelper;

namespace DragonsCrossing.NewCombatLogic;

public partial class CombatEngine
{
    LogicReponses AC10_GenerateLoot()
    {
        // TODO: This needs to be reviewed         
        // TODO: Dcx logic is still pending for discussion

        var allLootItems = new ItemDto[] { };

        if (null == Combat.Monster)
            throw new InvalidOperationException("Monster should not be null here");

        // If the loot has been generated, don't regenerate again and just skip.
        if (null != Combat.loot && Combat.loot.Items != null)
        {
            _logger.LogDebug("Loot has been generated, cannot generate again.");
            return LogicReponses.next;
        }

        #region get loots
        var allLootItemsFromMonsterTemplate = _dataHelperService.GetAllMonsterLoots(GameState, Combat);

        // We seperate the logic for generating skill books and regular items

        var skillItems = allLootItemsFromMonsterTemplate.Where(i => i.item.slot == ItemSlotTypeDto.unidentifiedSkill)
            .Where(i =>
            {
                var skillDropRoll = _dice.Roll(10000, DiceRollReason.SkillLootRoll);
                var finalDropChance = i.dropChance;

                var heroRarityBoostSkillDropRate = CreateTypefromJsonTemplate($"ScheduleTableRef.heroRarityBoostSkillDropRate.json",
                            new Dictionary<HeroRarityDto, int>());

                // Skill book drop rate is affected by the heroRarityBoostSkillDropRate schedule
                if (heroRarityBoostSkillDropRate.ContainsKey(GameState.Hero.rarity))
                {
                    finalDropChance += heroRarityBoostSkillDropRate[GameState.Hero.rarity];
                }
                else
                {
                    _logger.LogError($"heroRarityBoostSkillDropRate template doesn't contain information for hero rarity of {GameState.Hero.rarity}");
                }

                _logger.LogDebug($"Skill item loot. Skill item drop rate for {i.item.slug} is {finalDropChance}. Die roll result is {skillDropRoll}. Item dropped : itemDropRoll <= t.Value.ChancesOfDrop = {skillDropRoll <= finalDropChance}");

                return skillDropRoll <= finalDropChance;
            }).Select(i => i.item).ToArray();

        var normalItems = allLootItemsFromMonsterTemplate.Where(i => i.item.slot != ItemSlotTypeDto.unidentifiedSkill)
            .Where(i =>
            {
                var itemDropRoll = _dice.Roll(10000, DiceRollReason.ItemLootRoll);

                _logger.LogDebug($"Item loot. Item drop rate for {i.item.name} is {i.dropChance}. Die roll result is {itemDropRoll}. Item dropped : itemDropRoll <= t.Value.ChancesOfDrop = {itemDropRoll <= i.dropChance}");

                return itemDropRoll <= i.dropChance;
            }).Select(i => i.item).ToArray();

        allLootItems = allLootItems.Concat(normalItems).ToArray();

        if (TileDto.BossTiles.Contains(GameState.CurrentTile))
        {
            // Boss loot will only drop items that are matching hero's class
            // Shards(which is added later on for every monster loot) and unidentifiedskills are exceptions
            var heroClassItems = allLootItems.Where(i =>
            i.allowedHeroClassList.Contains(GameState.Hero.heroClass) ||
            i.slot == ItemSlotTypeDto.unidentifiedSkill ||
            i.slot == ItemSlotTypeDto.shard
            ).ToArray();

            allLootItems = heroClassItems;
        }

        if (is_hero_Eligible_NFT_And_Exp && GameState.Hero.isLoanedHero?.loanerType != Core.Contracts.Api.Dto.Heroes.LoanerHeroType.Demo)
        {
            allLootItems = allLootItems.Concat(skillItems).ToArray();
        }

        #endregion


        #region shard
        {
            var shardRoll = _dice.Roll(10000, DiceRollReason.ShardLootRoll);

            var heroRarityBoostShardDropRate = CreateTypefromJsonTemplate($"ScheduleTableRef.heroRarityBoostShardDropRate.json",
                        new Dictionary<HeroRarityDto, int>());

            var shardDropChance = 10;

            if (heroRarityBoostShardDropRate.ContainsKey(GameState.Hero.rarity))
            {
                shardDropChance += heroRarityBoostShardDropRate[GameState.Hero.rarity];
            }
            else
            {
                _logger.LogError($"heroRarityBoostShardDropRate template doesn't contain information for hero rarity of {GameState.Hero.rarity}");
            }

            //TODO: Make this become itemtemplate.
            var shardDto = CreateTypefromJsonTemplate($"templates.items.shard.json", new ItemDto());

            _logger.LogDebug($"Shard drop rate is {shardDropChance}, shard drop roll is {shardRoll}. Shard dropped : {shardRoll <= shardDropChance}");

            // Hero won't get skillbook or shard if hero level is 4 (including 4) levels above the monster.
            if (is_hero_Eligible_NFT_And_Exp && GameState.Hero.isLoanedHero?.loanerType != Core.Contracts.Api.Dto.Heroes.LoanerHeroType.Demo)
            {
                if ((shardRoll <= shardDropChance
                && GameState.Hero.generation == 0
                && shardDto.slot == ItemSlotTypeDto.shard))
                {
                    allLootItems = allLootItems.Concat(new ItemDto[] { shardDto }).ToArray();
                }
            }
        }

        #endregion shard


        

        #region mandrake roots
        if(_combatConfig.enableMendrakeRoot)
        {
            var lootRoll = _dice.Roll(10000, DiceRollReason.MandrakeLootRoll);

            var heroRarityBoostDropRate = CreateTypefromJsonTemplate($"ScheduleTableRef.heroRarityBoostMandrakeDropRate.json",
                        new Dictionary<HeroRarityDto, int>());

            var dropChance = 10;

            if (heroRarityBoostDropRate.ContainsKey(GameState.Hero.rarity))
            {
                dropChance += heroRarityBoostDropRate[GameState.Hero.rarity];
            }
            else
            {
                _logger.LogError($"heroRarityBoostDropRate template doesn't contain information for hero rarity of {GameState.Hero.rarity}");
            }


            var mandrakeDto = new ItemDto
            {
                slug= "quest-refresh",
                name= "Mandrake root",
                itemDropSound= "ring",
                slot=ItemSlotTypeDto.nftAction,
                allowedHeroClassList=new[] {CharacterClassDto.Warrior, CharacterClassDto.Ranger, CharacterClassDto.Mage },
                imageSlug= "mandrake-root",
                levelRequirement=1
            };

            _logger.LogDebug($"mandrake drop rate is {dropChance}, drop roll is {lootRoll}.  dropped : {lootRoll <= dropChance}");

            if (is_hero_Eligible_NFT_And_Exp && GameState.Hero.isLoanedHero?.loanerType != Core.Contracts.Api.Dto.Heroes.LoanerHeroType.Demo)
            {
                if ((lootRoll <= dropChance
                && mandrakeDto.slot == ItemSlotTypeDto.nftAction))
                {
                    allLootItems = allLootItems.Concat(new ItemDto[] { mandrakeDto }).ToArray();
                }
            }

        }

        #endregion shard


        Combat.loot = new MonsterLootDto() { Items = allLootItems };

        return LogicReponses.next;
    }

}

