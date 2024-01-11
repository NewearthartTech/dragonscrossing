using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.GameStates;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Core.Sagas;
using DragonsCrossing.Core.Services;
using DragonsCrossing.Domain.GameStates;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Driver;
using Nethereum.ABI;
using Nethereum.Web3;
using DragonsCrossing.NewCombatLogic;
using Nethereum.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers;

/// <summary>
/// This is a child of heroes and shouldn't have a root url.
/// </summary>

public class SummonHeroConfig
{
    public decimal priceInDcx { get; set; } = 3.6m;

}
public class NftTxnInfo {
    public DateTime fulfillmentTxnHashTime { get; set; }
    public string fulfillmentTxnHash { get; set; } = "";
    public long chainId { get; set; }
    public int nftTokenId { get; set; }
}

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SummonHeroController: NftUSeControllerBase<
        SummonHeroOrder,
        cbContract.Tokenomics.ContractDefinition.SummonHeroFunction>
{
    readonly SummonHeroConfig _config;

    public SummonHeroController(
        IBlockchainService blockChain,
        ISeasonsDbService seasonDb,
        IPerpetualDbService perpetualDb,
        IConfiguration config,
        IPublishEndpoint publishEp) : base(blockChain, seasonDb, perpetualDb, config, publishEp) {

        _config = config.GetSection("summonHero").Get<SummonHeroConfig>() ?? new SummonHeroConfig();
    }

    
    protected override int GetDFKTokenId(SummonHeroOrder txFunction)
    {
        throw new NotImplementedException();
    }

    protected override int GetNFTTokenId(cbContract.Tokenomics.ContractDefinition.SummonHeroFunction txFunction)
    {
        return (int)txFunction.RuneStoneId;
    }


    [HttpGet("summonCost/{nftTokenId}")]
    public async Task<decimal> GetSummonCost(int nftTokenId)
    {
        throw new NotImplementedException();
    }

    protected override async Task CreateNFTUserOrder(SummonHeroOrder order)
    {
        var heroId = this.GetHeroId(throwIfNull:false);

        if(0 != heroId)
        {
            var hero = await _perpetualDb.getCollection<HeroDto>().Find(c => c.id == heroId).SingleOrDefaultAsync();

            if(null != hero.isLoanedHero)
            {
                order.heroIdToTransfer = hero.id;
            }
        }

        var itemToUse = (await _perpetualDb.getCollection<NftizedItem>().Find(i => i.itemId == order.nftTokenId)
            .Project(i => i.item)
            .SingleAsync());

        if (null == itemToUse)
            throw new Exception($"item tokenId {order.nftTokenId} is not a skill item");

        order.chainId = itemToUse.isDefaultChain ? _web3Config.defaultChainId : _web3Config.shadowChain.playChainId;
        if (0 == order.chainId)
        {
            order.chainId = _web3Config.defaultChainId;
        }


        if (itemToUse.slot != ItemSlotTypeDto.shard)
            throw new Exception($"item {order.nftTokenId} is not a shard");

        order.priceInDcx = _config.priceInDcx;

        byte[] orderHash = new Sha3Keccack().CalculateHash(System.Text.Encoding.UTF8.GetBytes(order.orderHash));

        order.authorization = Web3Utils.generateSignature(_web3Config,
            new ABIValue("bytes32", orderHash),
            new ABIValue("uint256", order.nftTokenId),
            new ABIValue("uint256", await Web3Utils.weiValueForDcxToken(_web3Config!, order.priceInDcx, order.chainId)),
            new ABIValue("bytes", System.Text.Encoding.UTF8.GetBytes(order.mintProps)),
            new ABIValue("uint256", order.heroIdToTransfer),
            new ABIValue("address", await playerWalletAddress()));
    }
    
}

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class IdentifySkillController : NftUSeControllerBase<
    IdentifySkillOrder,
    cbContract.Tokenomics.ContractDefinition.ExchangeItemFunction>
{
    readonly IDataHelperService _dataHelperService;
    public IdentifySkillController(
        IBlockchainService blockChain,
        ISeasonsDbService seasonDb,
        IPerpetualDbService perpetualDb,
        IDataHelperService dataHelperService,
        IConfiguration config,
        IPublishEndpoint publishEp) : base(blockChain, seasonDb, perpetualDb, config, publishEp)
    {
        _dataHelperService = dataHelperService;
    }

    protected override int GetNFTTokenId(cbContract.Tokenomics.ContractDefinition.ExchangeItemFunction txFunction)
    {
        return (int)txFunction.OldTokenId;
    }


    protected override int GetDFKTokenId(IdentifySkillOrder txFunction)
    {
        return txFunction.nftTokenId;
    }


    protected override async Task CreateNFTUserOrder(IdentifySkillOrder order)
    {
        if (0 == order.nftTokenId)
            throw new Exception("order.nftTokenId cannot be 0");

        var itemToUse = (await _perpetualDb.getCollection<NftizedItem>().Find(i => i.itemId == order.nftTokenId)
            .Project(i => i.item)
            .SingleAsync())
            as SkillItem;

        if (null == itemToUse)
            throw new Exception($"item tokenId {order.nftTokenId} is not a skill item");

        order.chainId = itemToUse.isDefaultChain?_web3Config.defaultChainId:_web3Config.shadowChain.playChainId;
        if(0 == order.chainId)
        {
            order.chainId = _web3Config.defaultChainId;
        }

        var unIdentofied = itemToUse.skill as UnidentifiedHeroSkill;
        if(null == unIdentofied)
            throw new Exception("skill is not UnidentifiedHeroSkill");

        order.priceInDcx = unIdentofied.dcxToIdentify;

        if(order.chainId == _web3Config.defaultChainId)
        {
            order.newItemTokenId = await _perpetualDb.GetNextSequence<NftizedItem>();

            order.authorization = Web3Utils.generateSignature(_web3Config,
                new ABIValue("uint256", order.newItemTokenId),
                new ABIValue("uint256", order.nftTokenId),
                new ABIValue("uint256", await Web3Utils.weiValueForDcxToken(_web3Config!, order.priceInDcx, order.chainId)),
                new ABIValue("address", await playerWalletAddress()));

        }
        else
        {
            order.newItemTokenId = await _perpetualDb.GetNextSequence<NftizedDFKItem>();

        }

    }

    protected override async Task PostNFTUserOrder(IdentifySkillOrder order)
    {
        var nftCollection = _perpetualDb.getCollection<NftizedItem>();

        var oldItem = await nftCollection.Find(i => i.itemId == order.nftTokenId).SingleAsync();

        var skillItem = oldItem.item as SkillItem;
        if (null == skillItem)
            throw new Exception($"item {oldItem.itemId} is not Skill Item");

        var unLearnedSkill = skillItem.skill as UnidentifiedHeroSkill;
        if (null == unLearnedSkill)
            throw new Exception($"item {oldItem.itemId} is not unLearnedSkill ");

        var item = _dataHelperService.CreateItemFromTemplate(unLearnedSkill.slug + ".json",
                TileDto.ZoneFromTile(/*gameState.CurrentTile*/DcxTiles.aedos)
                //, gameState.Hero.rarity
                )

            as SkillItem;

        if (null == item)
            throw new Exception($"item is not SkillItem");

        item.nftTokenId = order.newItemTokenId;
        item.skill = DataHelper.CreateTypefromJsonTemplate($"templates.skills.{unLearnedSkill.slug}.json", new UnlearnedHeroSkill());
        item.slot = ItemSlotTypeDto.unlearnedSkill;
        

        if (0 == order.chainId)
        {
            order.chainId = _web3Config.defaultChainId;
        }

        await nftCollection.InsertOneAsync(new NftizedItem
        {
            item = item
        });

        await _publishEp.Publish(new UpdateNFtOwner
        {
            contractAddress = _web3Config.deployedContracts(order.chainId)!.DcxItemsContract.ToLower(),
            tokenId = order.newItemTokenId,
            chainId = order.chainId
        });

    }
}

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "SeasonJoined")]
public class NftActionController : NftUSeControllerBase<
        NftActionOrder,
        cbContract.Tokenomics.ContractDefinition.ExchangeItemFunction
        >
{
    readonly IServiceProvider _sp;
    readonly IGameStateService _gameStateService;

    public NftActionController(
        IBlockchainService blockChain,
        IGameStateService gameStateService,
        IServiceProvider sp,
        ISeasonsDbService db,
        IPerpetualDbService perpetualDb,
        IConfiguration config,
        IPublishEndpoint publishEp) : base(blockChain, db, perpetualDb, config, publishEp)
    {
        _sp = sp;
        _gameStateService = gameStateService;
    }


    protected override int GetNFTTokenId(cbContract.Tokenomics.ContractDefinition.ExchangeItemFunction txFunction)
    {
        return (int)txFunction.OldTokenId;
    }

    protected override int GetDFKTokenId(NftActionOrder txFunction)
    {
        throw new NotImplementedException();
    }


    protected override async Task CreateNFTUserOrder(NftActionOrder order)
    {
        var heroId = this.GetHeroId();

        var gameState = await _seasonDb.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();


        var itemToUse = (await _perpetualDb.getCollection<NftizedItem>().Find(i => i.itemId == order.nftTokenId)
            .Project(i => i.item)
            .SingleAsync());

        order.chainId = itemToUse.isDefaultChain ? _web3Config.defaultChainId : _web3Config.shadowChain.playChainId;
        if (0 == order.chainId)
        {
            order.chainId = _web3Config.defaultChainId;
        }

        if (null == itemToUse)
            throw new Exception($"item tokenId {order.nftTokenId} is not a item");

        order.heroId = heroId;

        order.authorization = Web3Utils.generateSignature(_web3Config,
            new ABIValue("uint256", 0),
            new ABIValue("uint256", order.nftTokenId),
            new ABIValue("uint256", await Web3Utils.weiValueForDcxToken(_web3Config!, order.priceInDcx, order.chainId)),
            new ABIValue("address", await playerWalletAddress()));
    }

    protected override async Task PostNFTUserOrder(NftActionOrder order)
    {
        ISeasonsDbService seasonDb = ActivatorUtilities.CreateInstance<SeasonsDbService>(_sp, order.seasonId);

        var gameStateCollection = seasonDb.getCollection<DbGameState>();
        var gameState = await seasonDb.getCollection<DbGameState>().Find(c => c.HeroId == order.heroId).SingleAsync();

        var nftCollection = _perpetualDb.getCollection<NftizedItem>();

        var theActionItem = await nftCollection.Find(i => i.itemId == order.nftTokenId).SingleAsync();

        switch (theActionItem.item.slug)
        {
            case "quest-refresh":
                await _gameStateService.ResetQuestsIfNeeded(order.heroId, forceItNow: true);
                break;

            default:
                throw new NotImplementedException();
        }
    }
}



[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "SeasonJoined")]
public class LearnSkillController : NftUSeControllerBase<
        LearnSkillOrder,
        cbContract.Tokenomics.ContractDefinition.ExchangeItemFunction
        >
{
    readonly IServiceProvider _sp;

    public LearnSkillController(
        IBlockchainService blockChain,
        IServiceProvider sp,
        ISeasonsDbService db,
        IPerpetualDbService perpetualDb,
        IConfiguration config,
        IPublishEndpoint publishEp) : base(blockChain, db, perpetualDb, config, publishEp) {
        _sp = sp;
    }


    protected override int GetNFTTokenId(cbContract.Tokenomics.ContractDefinition.ExchangeItemFunction txFunction)
    {
        return (int)txFunction.OldTokenId;
    }

    protected override int GetDFKTokenId(LearnSkillOrder txFunction)
    {
        return txFunction.nftTokenId;
    }


    protected override async Task CreateNFTUserOrder(LearnSkillOrder order)
    {
        var heroId = this.GetHeroId();

        var gameState = await _seasonDb.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

        var itemToUse = (await _perpetualDb.getCollection<NftizedItem>().Find(i => i.itemId == order.nftTokenId)
        .Project(i => i.item)
        .SingleAsync())
        as SkillItem;

        if (null == itemToUse)
            throw new Exception($"item tokenId {order.nftTokenId} is not a skill item");

        order.chainId = itemToUse.isDefaultChain ? _web3Config.defaultChainId : _web3Config.shadowChain.playChainId;
        if (0 == order.chainId)
        {
            order.chainId = _web3Config.defaultChainId;
        }


        if (null == itemToUse)
            throw new Exception($"item tokenId {order.nftTokenId} is not a skill item");

        var unLearned = itemToUse.skill as UnlearnedHeroSkill;
        if (null == unLearned)
            throw new Exception("skill is not UnlearnedHeroSkill");

        // Prevent hero from creating nft use order if class requirement is not met when trying to learn a skill

        DataHelper.CheckIfCanLearnSkill(gameState, unLearned);

        order.priceInDcx = unLearned.dcxToLearn;
        order.levelRequirement = unLearned.levelRequirement;
        order.heroId = heroId;

        order.authorization = Web3Utils.generateSignature(_web3Config,
            new ABIValue("uint256", 0),
            new ABIValue("uint256", order.nftTokenId),
            new ABIValue("uint256", await Web3Utils.weiValueForDcxToken(_web3Config!, order.priceInDcx, order.chainId)),
            new ABIValue("address", await playerWalletAddress()));
    }

    protected override async Task PostNFTUserOrder(LearnSkillOrder order)
    {
        ISeasonsDbService seasonDb  = ActivatorUtilities.CreateInstance<SeasonsDbService>(_sp, order.seasonId);

        var gameStateCollection = seasonDb.getCollection<DbGameState>();
        var gameState = await seasonDb.getCollection<DbGameState>().Find(c => c.HeroId == order.heroId).SingleAsync();

        var nftCollection = _perpetualDb.getCollection<NftizedItem>();

        var theSkillItem = await nftCollection.Find(i => i.itemId == order.nftTokenId).SingleAsync();

        var skillItem = theSkillItem.item as SkillItem;
        if (null == skillItem)
            throw new Exception($"item {theSkillItem.itemId} is not Skill Item");

        var unLearnedSkill = skillItem.skill as UnlearnedHeroSkill;
        if (null == unLearnedSkill)
            throw new Exception($"item {theSkillItem.itemId} is not UnlearnedHeroSkill ");

        DataHelper.CheckIfCanLearnSkill(gameState, unLearnedSkill);

        var learnedSkill = unLearnedSkill.CreateSkillFromUnlearned();

        if (null == learnedSkill)
            throw new Exception("failed to create Learned skill from unlearned");

        var done = await gameStateCollection.UpdateOneAsync(g => g.HeroId == order.heroId && g.Hero.skills.Length < 6,
            Builders<DbGameState>.Update.Push(g => g.Hero.skills, learnedSkill));

        if(done.ModifiedCount != 1)
        {
            throw new Exception("failed to update hero skills");
        }

    }
}



public abstract class NftUSeControllerBase<T, T1> : ControllerBase
                    where T : NFTUseBase, new()
                    where T1 : Nethereum.Contracts.FunctionMessage, new()
{
    //protected readonly ISeasonsDbService _seasonDb;
    protected readonly IPerpetualDbService _perpetualDb;
    protected readonly IPublishEndpoint _publishEp;
    protected readonly Web3Config _web3Config;
    protected readonly ISeasonsDbService _seasonDb;
    protected readonly IBlockchainService _blockChain;


    public NftUSeControllerBase(
        IBlockchainService blockChain,
        ISeasonsDbService seasonDb,
        IPerpetualDbService perpetualDb,
        IConfiguration config,
        IPublishEndpoint publishEp)
    {
        _blockChain = blockChain;
        _seasonDb = seasonDb;
        _publishEp = publishEp;
        //_seasonDb = db;
        _perpetualDb = perpetualDb;

        _web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
    }

    protected abstract int GetNFTTokenId(T1 txFunction);

    protected abstract int GetDFKTokenId(T txFunction);

    protected abstract Task CreateNFTUserOrder(T order);

    protected virtual Task PostNFTUserOrder(T order) {
        return Task.CompletedTask;
    }

    protected async Task<string> playerWalletAddress()
    {
        var userId = this.GetUserId();

        var player = await _perpetualDb.getCollection<PlayerDto>().Find(p => p.Id == userId).SingleAsync();

        if (string.IsNullOrWhiteSpace(player.BlockchainPublicAddress))
            throw new Exception("Play wallet address is NULL");

        return player.BlockchainPublicAddress;

    }


    [AllowAnonymous]
    [HttpGet("completedRecently")]
    public async Task<List<NftTxnInfo>> completedRecently([FromQuery] int days = 1)
    {
        //list all txHash completed in the last that many days
         DateTime startDate = DateTime.UtcNow.AddDays(-days);

        var collection = _perpetualDb.getCollection<DcxOrder>().OfType<T>();

        
         var filter = Builders<T>.Filter.Gte(x => x.fulfillmentTxnHashTime, startDate); 
         var completedTransactions = await collection.Find(filter).ToListAsync();

       var filteredTransactions = completedTransactions.Select(x => new NftTxnInfo
    {
        fulfillmentTxnHashTime = x.fulfillmentTxnHashTime,
        fulfillmentTxnHash = x.fulfillmentTxnHash,
        chainId = x.chainId,
        nftTokenId = x.nftTokenId,
        
    }).ToList();

    return filteredTransactions;
    }
    [AllowAnonymous]
    [HttpGet("completedRecentlyUnknown")]
    public async Task<List<NftTxnInfo>> completedRecentlyUnknown([FromQuery] int count = 10)
    {
        var collection = _perpetualDb.getCollection<DcxOrder>().OfType<T>();


        var filter = Builders<T>.Filter.Exists(x => x.fulfillmentTxnHashTime, false);
         var completedTransactions = await collection.Find(filter).Limit(count).ToListAsync();

       var filteredTransactions = completedTransactions.Select(x => new NftTxnInfo
    {
        fulfillmentTxnHash = x.fulfillmentTxnHash,
        chainId = x.chainId,
        nftTokenId = x.nftTokenId,
        
    }).ToList();

    return filteredTransactions;
    }

    [AllowAnonymous]
    [HttpGet("completed/{chainId}/{txHash}")]
    public async Task<T> PostOrder(long chainId, string txHash)
    {
        if (string.IsNullOrWhiteSpace(txHash))
            throw new ArgumentNullException();

        int nftTokenId;
        if (chainId == _web3Config.defaultChainId)
        {
            var txDetails = await _blockChain.GetTxnStatus<T1>(txHash, chainId);
            if (null == txDetails)
                throw new ExceptionWithCode("transaction not found");
            nftTokenId = GetNFTTokenId(txDetails);

        }else if(chainId == _web3Config.shadowChain.playChainId)
        {
            var txDetails = await _blockChain.GetDFKPayDetails<T>(txHash);
            if (null == txDetails?.order)
                throw new ExceptionWithCode("transaction not found");
            nftTokenId = GetDFKTokenId(txDetails.order);
        }
        else
        {
            throw new NotImplementedException();
        }



        var collection = _perpetualDb.getCollection<DcxOrder>().OfType<T>();


        await _publishEp.Publish(new UpdateNFtOwner
        {
            contractAddress = _web3Config.deployedContracts(chainId)!.DcxItemsContract.ToLower(),
            tokenId = nftTokenId,
            chainId = chainId
        });

        var existing = await collection.Find(l => l.nftTokenId == nftTokenId && l.chainId == chainId).SingleAsync();

        if (!String.IsNullOrWhiteSpace(existing.fulfillmentTxnHash))
            throw new ExceptionWithCode("transaction already submitted");

        await PostNFTUserOrder(existing);

        await collection.UpdateOneAsync(l=>l.nftTokenId == nftTokenId,
            Builders<T>.Update
            .Set(o => o.fulfillmentTxnHash, txHash)
            .Set(o=>o.fulfillmentTxnHashTime, DateTime.UtcNow)
            );

        var updatedOrder = await GetOrder(nftTokenId);

        /*
        if (!String.IsNullOrWhiteSpace(updatedOrder.fulfillmentTxnHash) &&
            String.IsNullOrWhiteSpace(existing.fulfillmentTxnHash))
        {
            //await Core.Sagas.SummonHeroOrdered.SubmitSummonHeroOrder(updatedOrder, _publishEp);
        }
        */

        return updatedOrder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="fromLevel">what level we are trying to level up from. Can also be used to retrieve information of a completed SummonHero</param>
    /// <returns></returns>
    /// <exception cref="ExceptionWithCode"></exception>
    [HttpGet("create/{nftTokenId}")]
    public async Task<T> GetOrder(int nftTokenId)
    {
//        var heroId = this.GetHeroId();
        //var collection = _seasonDb.getCollection<DcxOrder>().OfType<T>();
        var collection = _perpetualDb.getCollection<DcxOrder>().OfType<T>();

        var userId = this.GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            //we are not authenticated no worries
            return new T
            {
                
                nftTokenId = nftTokenId,
            };
        }
            

        var existing = await collection.Find(l => l.nftTokenId == nftTokenId && l.userId == userId).SingleOrDefaultAsync();

        if (null != existing)
            return existing;


        var order = new T
        {
            userId = userId,
            nftTokenId = nftTokenId,
            forWallet = await playerWalletAddress()
        };

        if (_seasonDb.isAvailable)
        {
            order.seasonId = this.GetSeasonId();
        }

        await CreateNFTUserOrder(order);

        await collection.InsertOneAsync(order);

        return order;

    }

}

