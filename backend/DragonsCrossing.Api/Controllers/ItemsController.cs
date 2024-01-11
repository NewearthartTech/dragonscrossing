using DragonsCrossing.Api.Utilities;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Armors;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Core.Sagas;
using DragonsCrossing.Domain.GameStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using static DragonsCrossing.Api.Utilities.ExtentionMethods;
using static DragonsCrossing.Core.Helper.DataHelper;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "SeasonJoined")]
    public class ItemsController : ControllerBase
    {
        readonly ISeasonsDbService _seasonsDb;
        readonly IPerpetualDbService _perpetualDb;
        
        readonly static int MAX_SHARED_STASH_SPACE = 4;

        public ItemsController(ISeasonsDbService db, IPerpetualDbService perpetualDb)
        {
            _seasonsDb = db;
            _perpetualDb = perpetualDb;
        }

        /// <summary>
        /// re arranging items in Inventory
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpPost("MoveItemInInventory")]
        public async Task<HeroDto> MoveItem([FromBody] MoveItemRequestDto move)
        {
            var heroId = this.GetHeroId();

            var collection = _seasonsDb.getCollection<NewCombatLogic.DbGameState>();

            var inventory = new List<ItemDto>( await collection.Find(h => h.HeroId == heroId)
                .Project(h => h.Hero.inventory)
                .SingleAsync());

            if (move.FromIndex >= inventory.Count())
            {
                throw new InvalidOperationException("moveFrom index is greater then array inventory count");
            }

            var itemAtFromIndex = inventory[move.FromIndex];

            var itemAtToIndex = move.ToIndex >= inventory.Count() ? null : inventory[move.ToIndex];


            if (null == itemAtToIndex)
            {
                inventory.RemoveAt(move.FromIndex);
                inventory.Add(itemAtFromIndex);
            }
            else
            {
                inventory[move.ToIndex] = itemAtFromIndex;
                inventory[move.FromIndex] = itemAtToIndex;
            }


            await collection.UpdateOneAsync(h => h.HeroId == heroId,
                Builders<NewCombatLogic.DbGameState>.Update.Set(g => g.Hero.inventory, inventory.ToArray()));

            return await collection.Find(h => h.HeroId == heroId)
                .Project(g=>g.Hero)
                .SingleAsync();
        }

        /// <summary>
        /// Moving items from inventory to body slots
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="ExceptionWithCode"></exception>
        [HttpPost("EquipItem")]
        public async Task<HeroDto> EquipItem([FromBody] EquipItemRequestDto req)
        {
            var heroId = this.GetHeroId();

            var collection = _seasonsDb.getCollection<NewCombatLogic.DbGameState>();

            var filter = Builders<NewCombatLogic.DbGameState>.Filter.And(
                    Builders<NewCombatLogic.DbGameState>.Filter.Where(h => h.HeroId == heroId),
                    Builders<NewCombatLogic.DbGameState>.Filter.ElemMatch(h => h.Hero.inventory, i => i.id == req.ItemId));

            var Q = collection.Find(filter);

            var t = Q.ToString();

            var hero = await Q
                .Project(g => g.Hero)
                .SingleAsync();

            var itemToEuip = hero.inventory
                .Where(i => i.id == req.ItemId)
                .Single()
                ;

            if (!itemToEuip.allowedHeroClassList.Contains(hero.heroClass))
            {
                throw new ExceptionWithCode("Item hero class requirement not met");
            }

            if (!IsHeroStatCompliedForItem(itemToEuip, hero))
            {
                throw new ExceptionWithCode("Item stat requirement not met");
            }

            if (!IsHeroLevelCompliedForItem(itemToEuip, hero))
            {
                throw new ExceptionWithCode("Item level requirement not met");
            }

            ItemDto[] itemsToUnEquip = new ItemDto[] { };

            if (itemToEuip.slot == ItemSlotTypeDto.twoHand)
            {
                itemsToUnEquip = itemsToUnEquip.Concat(hero.equippedItems
                    .Where(i => i.slot == ItemSlotTypeDto.mainHand ||
                                i.slot == ItemSlotTypeDto.offHand).ToArray())
                    .ToArray();
            }

            if (itemToEuip.slot == ItemSlotTypeDto.mainHand ||
                itemToEuip.slot == ItemSlotTypeDto.offHand)
            {
                itemsToUnEquip = itemsToUnEquip.Concat(hero.equippedItems
                    .Where(i => i.slot == ItemSlotTypeDto.twoHand).ToArray())
                    .ToArray();              
            }

            var regularItemToUnEquip = hero.equippedItems
                    .Where(i => i.slot == itemToEuip.slot)
                    .SingleOrDefault();

            if (regularItemToUnEquip != null)
            {
                itemsToUnEquip = itemsToUnEquip.Concat(new[] { regularItemToUnEquip }).ToArray();
            }
            
            if (null != itemsToUnEquip)
            {
                foreach (var itemToUnEquip in itemsToUnEquip)
                {
                    var doneUnequip = await collection.UpdateOneAsync(g => g.HeroId == hero.id,
               Builders<NewCombatLogic.DbGameState>.Update
                   .PullFilter(h => h.Hero.equippedItems, i => i.id == itemToUnEquip.id)
                   .Push(h => h.Hero.inventory, itemToUnEquip));
                }
            }

            var done = await collection.UpdateOneAsync(filter,
                Builders<NewCombatLogic.DbGameState>.Update
                    .PullFilter(h => h.Hero.inventory, i => i.id == itemToEuip.id)
                    .Push(h => h.Hero.equippedItems, itemToEuip)
                );

            return await collection.Find(h => h.HeroId == heroId)
                .Project(g=>g.Hero)
                .SingleAsync();
        }

        /// <summary>
        /// Moving Items from body slots to Inventory
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("UnequipItem")]
        public async Task<HeroDto> UnequipItem([FromBody] EquipItemRequestDto req)
        {
            var heroId = this.GetHeroId();
            var collection = _seasonsDb.getCollection<NewCombatLogic.DbGameState>();
            var gameState = await _seasonsDb.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            var inventorySpaceLeft = HeroDto.TotalInventorySlots - gameState.Hero.inventory.Length;

            if (inventorySpaceLeft <= 0)
            {
                throw new ExceptionWithCode("Inventory is full. Unable to move item to inventory.");
            }

            var filter = Builders<NewCombatLogic.DbGameState>.Filter.And(
                    Builders<NewCombatLogic.DbGameState>.Filter.Where(h => h.HeroId == heroId),
                    Builders<NewCombatLogic.DbGameState>.Filter.ElemMatch(h => h.Hero.equippedItems, i => i.id == req.ItemId),
                    // Inventory length cannot exceed the total space  - 1
                    // Say alrady have 59 items => 59 <= 60 - 1 is OK
                    // Say already have 60 items => 60 <= 60 -1 is not OK 
                    Builders<DbGameState>.Filter.SizeLte(g => g.Hero.inventory, (HeroDto.TotalInventorySlots - 1))

                    );

            var itemToUnquip = (await collection.Find(filter).Project(g => g.Hero).SingleAsync())
                .equippedItems
                .Where(i => i.id == req.ItemId)
                .Single()
                ;

            var done = await collection.UpdateOneAsync(filter,
                Builders<NewCombatLogic.DbGameState>.Update
                    .PullFilter(h => h.Hero.equippedItems, i => i.id == req.ItemId)
                    .Push(h => h.Hero.inventory, itemToUnquip)
                );

            return await collection.Find(h => h.HeroId == heroId).Project(g => g.Hero).SingleAsync();
        }

        /// <summary>
        /// Destroy an item from inventory.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("DestroyItem")]
        public async Task<HeroDto> DestroyItem([FromBody] EquipItemRequestDto req)
        {
            var heroId = this.GetHeroId();
            var collection = _seasonsDb.getCollection<NewCombatLogic.DbGameState>();

            var filter = Builders<NewCombatLogic.DbGameState>.Filter.Where(h => h.HeroId == heroId);
            UpdateDefinition<DbGameState> setter;

            var hero = await collection.Find(filter).Project(g => g.Hero).SingleAsync();

            var itemToDestroy = new ItemDto();

            // check if the item to destroy is in inventory first, if not, try to filter from equippedItems
            if (hero.inventory.Where(i => i.id == req.ItemId).Count() > 0)
            {
                itemToDestroy = hero.inventory.Where(i => i.id == req.ItemId).Single();
                setter = Builders<NewCombatLogic.DbGameState>.Update.PullFilter(h => h.Hero.inventory, i => i.id == req.ItemId);
            }
            else if (hero.equippedItems.Where(i => i.id == req.ItemId).Count() > 0)
            {
                itemToDestroy = hero.equippedItems.Where(i => i.id == req.ItemId).Single();
                setter = Builders<NewCombatLogic.DbGameState>.Update.PullFilter(h => h.Hero.equippedItems, i => i.id == req.ItemId);
            }
            else
            {
                throw new Exception("Item not found");
            }

            // A player should not be able to destroy an item that is an nft or unsecured nft 
            if (itemToDestroy.slot == ItemSlotTypeDto.shard ||
                itemToDestroy.slot == ItemSlotTypeDto.unidentifiedSkill ||
                itemToDestroy.slot == ItemSlotTypeDto.unlearnedSkill)
            {
                throw new ExceptionWithCode("Item cannot be destroyed.");
            }

            var done = await collection.UpdateOneAsync(filter, setter);

            return await collection.Find(h => h.HeroId == heroId).Project(g => g.Hero).SingleAsync();
        }

        [HttpGet("blackSmithItems")]
        public async Task<ItemDto[]> BlackSmithItems()
        {
            var heroId = this.GetHeroId();

            var gameState = await _seasonsDb.getCollection<NewCombatLogic.DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            return GetBlackSmithItems(getBackSmithHighestZone(gameState), _seasonsDb).Result;

        }

        public static DcxZones getBackSmithHighestZone(DbGameState gameState)
        {
            var highestZoneOrder = gameState.HighestZoneVisited;
            switch (highestZoneOrder)
            {
                case DcxZones.fallenTemples:
                    highestZoneOrder = DcxZones.treacherousPeaks;
                    break;
                case DcxZones.wondrousThicket:
                    highestZoneOrder = DcxZones.foulWastes;
                    break;
            }

            return highestZoneOrder;
        }
        

        [HttpPut("buyItem/{slug}")]
        public async Task<HeroDto> BuyItem(string slug)
        {
            var heroId = this.GetHeroId();

            var collection = _seasonsDb.getCollection<NewCombatLogic.DbGameState>();
            var gameState = await _seasonsDb.getCollection<NewCombatLogic.DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            var highestZoneOrder = getBackSmithHighestZone(gameState);

            var blackSmithItem = await _seasonsDb.getCollection<BlackSmithItem>().Find(i => i.zone == highestZoneOrder).SingleAsync();

            var itemToBuy = blackSmithItem.items.Single(i => i.slug == slug);

            var questsNeeded = 1;
            var maxDailyQuestNeeded = (gameState.Hero.maxDailyQuests + gameState.Hero.extraDailyQuestGiven) - questsNeeded;

            if (gameState.Hero.dailyQuestsUsed > maxDailyQuestNeeded)
            {
                throw new ExceptionWithCode("Purchase failed. Hero doesn't have enough quests.");
            }
            else            
            {
                // This makes each item being purchased has a unique id
                itemToBuy.id = Guid.NewGuid().ToString();

                // Low re-entrancy attck vulnerability 
                var deductQuest = await collection.UpdateOneAsync(h => h.HeroId == heroId &&
                h.Hero.dailyQuestsUsed <= maxDailyQuestNeeded,
                    Builders<NewCombatLogic.DbGameState>.Update
                    .Push(h => h.Hero.inventory, itemToBuy)
                    .Inc(h => h.Hero.dailyQuestsUsed, questsNeeded)
                    );

                return await collection.Find(h => h.HeroId == heroId)
                    .Project(g => g.Hero)
                    .SingleOrDefaultAsync();
            }

            throw new ExceptionWithCode("Purchase failed. Hero doesn't have enough quests to purchase item.");
        }

        [HttpPut("smelt")]
        public async Task<HeroDto> SmeltItems([FromBody] string[] itemIds)
        {

            var heroId = this.GetHeroId();

            if (itemIds.Length != 10)
                throw new Exception("need 10 items to smelt");

            var collection = _seasonsDb.getCollection<NewCombatLogic.DbGameState>();
            var gameState = await _seasonsDb.getCollection<NewCombatLogic.DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            var hero = gameState.Hero;

            FilterDefinition<DbGameState>[] myFilter = new[]
            {
                Builders<DbGameState>.Filter.Where(g=>g.HeroId == heroId)
            }.Concat(itemIds.Select(id =>
            Builders<DbGameState>.Filter.ElemMatch(g => g.Hero.inventory,
            Builders<ItemDto>.Filter.Where(i => i.id == id)))).ToArray();

            var done = await collection.UpdateOneAsync(Builders<DbGameState>.Filter.And(myFilter),
            Builders<NewCombatLogic.DbGameState>.Update
            .PullFilter(h => h.Hero.inventory, i => itemIds.Contains(i.id))
            .Inc(g => g.Hero.extraDailyQuestGiven, 1)
            ) ;

            if (done.MatchedCount != 1)
            {
                throw new Exception();
            }

            return await collection.Find(h => h.HeroId == heroId)
                .Project(g => g.Hero)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Only works if monster is dead in combat
        /// </summary>
        /// <param name="itemIds">The item Ids to pickup NOT the slugs</param>
        /// <returns>any loot left</returns>
        [HttpPost("pickupLoot")]
        public async Task<ItemDto[]> PickupLoot(string[] itemIds)
        {
            var heroId = this.GetHeroId();
            var collection = _seasonsDb.getCollection<NewCombatLogic.DbGameState>();
            var gameState = await collection.Find(c => c.HeroId == heroId).SingleAsync();

            var encounters = gameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

            var combat = encounters?.Where(e => e.Monster != null).Single();

            if (null == combat)
                throw new Exception("We are not in combat");

            if(!combat.isMonsterDead)
                throw new Exception("Monster is not dead");

             if(null == combat.loot)
                throw new Exception("loot is null");


            var selectedLoots = combat.loot.Items.Where(i => itemIds.Contains(i.id)).ToArray();

            if (0 == selectedLoots.Length)
                throw new ExceptionWithCode("no items found for loot");

            if ((gameState.Hero.inventory.Length + selectedLoots.Length) > HeroDto.TotalInventorySlots)
                throw new ExceptionWithCode("Inventory limit exceeded");


            var lootField = $"{nameof(DbGameState.CurrentEncounters)}.0.{nameof(CombatEncounter.loot)}.{nameof(MonsterLootDto.Items)}";

            var pullfilter = Builders<ItemDto>.Filter.In(i => i.id, itemIds);

            var findFilter = Builders<DbGameState>.Filter.And(
                Builders<DbGameState>.Filter.Where(g => g.HeroId == heroId),
                Builders<DbGameState>.Filter.SizeLte(g=>g.Hero.inventory,(HeroDto.TotalInventorySlots - selectedLoots.Length))
                );

            var done = await collection.UpdateOneAsync(
                findFilter,

                Builders<DbGameState>.Update
                .PullFilter<ItemDto>(lootField, pullfilter)
                .PushEach(g=>g.Hero.inventory, selectedLoots)
                );

            gameState = await collection.Find(c => c.HeroId == heroId).SingleAsync();

            encounters = gameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

            combat = encounters?.Where(e => e.Monster != null).FirstOrDefault();

            return combat?.loot?.Items??new ItemDto[] { };

        }


        [HttpGet("moveToStash")]
        public async Task<PlayerDto> MoveToStash(string itemId)
        {
            var heroId = this.GetHeroId();
            var userId = this.GetUserId();

            var gameStateCollection = _seasonsDb.getCollection<NewCombatLogic.DbGameState>();
            var gameState = await gameStateCollection.Find(c => c.HeroId == heroId).SingleAsync();

            var itemToMove = gameState.Hero.inventory.Where(i => i.id == itemId).Single();

            var questToConsume = 2;
            var maxDailyQuestNeeded = (gameState.Hero.maxDailyQuests + gameState.Hero.extraDailyQuestGiven) - questToConsume;

            if (gameState.Hero.dailyQuestsUsed > maxDailyQuestNeeded)
            {
                throw new ExceptionWithCode($"Unable to move item to shared stash. Quests required to move item: {questToConsume}");
            }

            if (itemToMove.slot == ItemSlotTypeDto.shard || itemToMove.slot == ItemSlotTypeDto.unidentifiedSkill || itemToMove.slot == ItemSlotTypeDto.unlearnedSkill)
            {
                throw new ExceptionWithCode($"Skill books and shards cannot be stashed");
            }

            var playerCollection = _seasonsDb.getCollection<PlayerDto>();
            var player = await playerCollection.Find(p => p.Id == userId).SingleAsync();

            if (player.SharedStash.Count() >= MAX_SHARED_STASH_SPACE)
            {
                throw new ExceptionWithCode("Shared Stash full");
            }

            //DEE TODO: Might need to add transaction here

            _ = await gameStateCollection.UpdateOneAsync(
                g => g.HeroId == heroId
                &&
                g.Hero.inventory.Contains(itemToMove),

                Builders<NewCombatLogic.DbGameState>.Update
                    .PullFilter(g => g.Hero.inventory,
                        Builders<ItemDto>.Filter.Eq(i => i.id, itemToMove.id))
                    .Push(g => g.itemsToMoveToStash, itemToMove)
                    .Inc(h => h.Hero.dailyQuestsUsed, questToConsume)
                );        

            _ = await playerCollection.UpdateManyAsync(p => p.Id == userId,
                Builders<PlayerDto>.Update
                .Push(p=>p.SharedStash,itemToMove));

            return await playerCollection.Find(p => p.Id == userId).SingleAsync();

        }

        [HttpGet("moveFromStash")]
        public async Task<PlayerDto> MoveFromStash(string itemId)
        {
            var heroId = this.GetHeroId();
            var userId = this.GetUserId();

            var gameStateCollection = _seasonsDb.getCollection<NewCombatLogic.DbGameState>();
            var playerCollection = _seasonsDb.getCollection<PlayerDto>();
            var gameState = await _seasonsDb.getCollection<NewCombatLogic.DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            var player = await playerCollection.Find(p => p.Id == userId).SingleAsync();

            var itemToMove = player.SharedStash.Where(i => i.id == itemId).Single();

            var questToConsume = 2;
            var maxDailyQuestNeeded = (gameState.Hero.maxDailyQuests + gameState.Hero.extraDailyQuestGiven) - questToConsume;

            if (gameState.Hero.dailyQuestsUsed > maxDailyQuestNeeded)
            {
                throw new ExceptionWithCode($"Unable to move item to inventory. Quests required to move item: {questToConsume}.");
            }


            _ = await playerCollection.UpdateOneAsync(
                g => g.Id == userId
                &&
                g.SharedStash.Contains(itemToMove),

                Builders<PlayerDto>.Update
                    .PullFilter(g => g.SharedStash,
                        Builders<ItemDto>.Filter.Eq(i => i.id, itemToMove.id))
                    .Push(g => g.itemsToMoveFromStash, itemToMove)
                );


            _ = await gameStateCollection.UpdateManyAsync(c => c.HeroId == heroId,
                Builders<NewCombatLogic.DbGameState>.Update
                .Push(p => p.Hero.inventory, itemToMove)
                .Inc(h => h.Hero.dailyQuestsUsed, questToConsume));

            return await playerCollection.Find(p => p.Id == userId).SingleAsync();

        }

        [AllowAnonymous]
        [HttpGet("fromNft/{id}")]
        public async Task<ItemDto> getNftDetails(int id)
        {
            return await _perpetualDb.getCollection<NftizedItem>().Find(i => i.itemId == id)
                    .Project(i => i.item)
                    .SingleAsync();
        }

        [AllowAnonymous]
        [HttpGet("fromNfts/{ids}")]
        public async Task<ItemDto[]> getNftDetailsMany(string? ids)
        {
            var allIds = ExtentionMethods.ParseColonSeperatedIds(ids)
                .Distinct()
                .Select(v=> (int)v)
                .ToArray();

            if(0 == allIds.Length)
            {
                return new ItemDto[] { };
            }
            return (await _perpetualDb.getCollection<NftizedItem>().Find(i => allIds.Contains(i.itemId))
                    .Project(i => i.item)
                    .ToListAsync()).ToArray();
        }

    }
}
