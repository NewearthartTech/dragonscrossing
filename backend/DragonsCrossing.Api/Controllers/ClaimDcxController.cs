using System.ComponentModel.DataAnnotations;
using System.Text;
using DragonsCrossing.Api.Utilities;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Core.Sagas;
using DragonsCrossing.Core.Services;
using DragonsCrossing.NewCombatLogic;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Nethereum.ABI;
using Nethereum.Util;
using Nethereum.Web3;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ClaimDcxController : ControllerBase
{
    readonly ILogger _logger;
    readonly Web3Config _web3Config;
    readonly RewardsConfig _config;
    readonly IPerpetualDbService _perpetualDb;
    readonly IBlockchainService _blockChain;
    readonly IDataHelperService _dataService;
    readonly IDiceService _dice;
    readonly IPublishEndpoint _publishEp;

    public ClaimDcxController(
        IPerpetualDbService perpetualDb,
        IDataHelperService dataService,
        IBlockchainService blockChain,
        IConfiguration config,
        IDiceService dice,
        IPublishEndpoint publishEp,
        ILogger<HeroesController> logger)
    {
        _publishEp = publishEp;
        _dice = dice;
        _logger = logger;
        _dataService = dataService;
        _blockChain = blockChain;
        _perpetualDb = perpetualDb;
        _web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
        _config = config.GetSection("rewards").Get<RewardsConfig>() ?? new RewardsConfig();
    }

    [HttpGet("dcxRewards")]
    public async Task<DcxReward[]> GetDcxRewards([FromQuery] string? include = null)
    {
        var filter = Builders<DcxReward>.Filter.Where(r => true);
        switch ((include ?? "").ToLower())
        {
            case "claimed":
                filter = Builders<DcxReward>.Filter.Where(r => !string.IsNullOrEmpty(r.claimedHash));
                break;
            case "unclaimed":
                filter = Builders<DcxReward>.Filter.Where(r => string.IsNullOrEmpty(r.claimedHash));
                break;
        }

        var rewards = (await _perpetualDb.getCollection<RewardsBase>()
            .OfType<DcxReward>()
            .Find(filter).ToListAsync())
            .Cast<DcxReward>()
            .ToArray();

        return rewards;

    }

    [HttpGet("itemRewards")]
    public async Task<ItemsReward[]> GetItemRewards([FromQuery] string? include = null)
    {
        var filter = Builders<ItemsReward>.Filter.Where(r => true);
        switch ((include??"").ToLower())
        {
            case "claimed":
                filter = Builders<ItemsReward>.Filter.Where(r => !string.IsNullOrEmpty(r.claimedHash));
                break;
            case "unclaimed":
                filter = Builders<ItemsReward>.Filter.Where(r => string.IsNullOrEmpty(r.claimedHash));
                break;
        }

        var rewards = (await _perpetualDb.getCollection<RewardsBase>()
            .OfType<ItemsReward>()
            .Find(filter).ToListAsync())
            .Cast<ItemsReward>()
            .ToArray();

        return rewards;

    }


    [HttpGet("loadDcxRewards/{fileName}")]
    public async Task<DcxReward[]> LoadDcxRewards(string fileName)
    {
        var url = $"{_config.urlPrefix}/{_config.dcxRewardsFolder}/{fileName}";

        _logger.LogInformation($"Loading dcxRewards from {url}");

        try
        {
            var dcxRewards = await ExtentionMethods.LoadObjectFromURL<DcxReward[]>(url);


            var toRun = dcxRewards.Select(reward =>
            {
                var lowerAddress = reward.address.ToLower();
                if (string.IsNullOrWhiteSpace(lowerAddress))
                    throw new Exception("empty address found");

                return new UpdateOneModel<DcxReward>(
                Builders<DcxReward>.Filter.Where(r => r.rewardId == reward.rewardId && r.address == lowerAddress),
                Builders<DcxReward>.Update
                    .Set(r=>r.amount, reward.amount)
                    .SetOnInsert(r=>r.address, lowerAddress)
                    .SetOnInsert(r => r.rewardId, reward.rewardId)
                    .SetOnInsert(r=>r.id, Guid.NewGuid().ToString())
                )
                {
                    IsUpsert = true
                }; ;
            }).ToArray();


            if (toRun.Length > 0)
            {
                var done = await _perpetualDb.getCollection< RewardsBase > ()
                .OfType<DcxReward>().BulkWriteAsync(toRun);

                _logger.LogInformation($"DCXRewards upserted : {done.Upserts.Count}, modified: {done.ModifiedCount}");
            }

            return await GetDcxRewards();

        }
        catch (Exception ex)
        {
            throw new ExceptionWithCode(ex.Message, innerException: ex);
        }

    }

    [HttpGet("loadItemsRewards/{fileName}")]
    public async Task<ItemsReward[]> LoadItemsRewards(string fileName)
    {
        var url = $"{_config.urlPrefix}/{_config.dcxRewardsFolder}/{fileName}";

        _logger.LogInformation($"Loading dcxRewards from {url}");

        try
        {
            var itemsRewards = await ExtentionMethods.LoadObjectFromURL<ItemsReward[]>(url);


            var toRun = itemsRewards.Select(reward =>
            {
                var lowerAddress = reward.address.ToLower();
                if (string.IsNullOrWhiteSpace(lowerAddress))
                    throw new Exception("empty address found");

                return new UpdateOneModel<ItemsReward>(
                Builders<ItemsReward>.Filter.Where(r => r.rewardId == reward.rewardId && r.address == lowerAddress),
                Builders<ItemsReward>.Update
                    .Set(r => r.slot, reward.slot)
                    .SetOnInsert(r => r.address, lowerAddress)
                    .SetOnInsert(r => r.rewardId, reward.rewardId)
                    .SetOnInsert(r => r.id, Guid.NewGuid().ToString())
                )
                {
                    IsUpsert = true
                }; ;
            }).ToArray();


            if (toRun.Length > 0)
            {
                var done = await _perpetualDb.getCollection<RewardsBase>()
                .OfType<ItemsReward>().BulkWriteAsync(toRun);

                _logger.LogInformation($"DCXRewards upserted : {done.Upserts.Count}, modified: {done.ModifiedCount}");
            }

            return await GetItemRewards();

        }
        catch (Exception ex)
        {
            throw new ExceptionWithCode(ex.Message, innerException: ex);
        }

    }


    static readonly string[] NOTUsedForReward = new[] { "skill-overhand-chop.json", "skill-grapple.json",
        "skill-barbed-projectile.json", "skill-herbal-knowledge.json",
        "skill-fireball.json", "skill-ray-of-frost.json"
    };

    [HttpGet("available/{walletAddress}")]
    public async Task<RewardsClaims> claims(string walletAddress)
    {
        var dcxClaim = new ClaimDcxAutorization();
        var dcxReward = await _perpetualDb.getCollection<RewardsBase>()
                .OfType<DcxReward>()
                .Find(r =>
                    r.address == walletAddress.ToLower() && string.IsNullOrEmpty(r.claimedHash)
                ).SortByDescending(r => r.amount)
                .FirstOrDefaultAsync();

        if (null != dcxReward)
        {
            dcxClaim = new ClaimDcxAutorization
            {
                orderId = dcxReward.id,
                amount = dcxReward.amount,
                authorization = await _blockChain.AuthrorizeClaimDcx(walletAddress, dcxReward.amount, dcxReward.id, _web3Config.defaultChainId)

            };
        }

        var itemRewards = (await _perpetualDb.getCollection<RewardsBase>()
        .OfType<ItemsReward>()
        .Find(r =>
            r.address == walletAddress.ToLower() && string.IsNullOrEmpty(r.claimedHash)
        )
        .ToListAsync()).ToArray();

        var itemsNeedingNFTids = await Task.WhenAll(itemRewards.Where(i => 0 == i.itemNftId)
            .Select(async item =>
            {
                item.itemNftId = await _perpetualDb.GetNextSequence<NftizedItem>();

                return new UpdateOneModel<ItemsReward>(
                    Builders<ItemsReward>.Filter.Where(r => r.id == item.id ),
                    Builders<ItemsReward>.Update
                .Set(r => r.itemNftId, item.itemNftId));
            }));

        if (itemsNeedingNFTids.Length > 0)
        {
            var done = await _perpetualDb.getCollection<RewardsBase>()
            .OfType<ItemsReward>().BulkWriteAsync(itemsNeedingNFTids);

            _logger.LogInformation($"ItemsReward upserted : {done.Upserts.Count}, modified: {done.ModifiedCount}");
        }

        var skillTemplates = _dataService.loadAllSkillTemplates();

        skillTemplates = skillTemplates.Where(t => !NOTUsedForReward.Contains(t)).ToArray();

        var nftsCreated = itemRewards.Select((reward,randI) =>
        {

            ItemDto randomTemplate;


            if (reward.slot == ItemRewardType.skill)
            {
                var rand = _dice.Roll(skillTemplates.Length);
                var templateName = skillTemplates[rand - 1];

                randomTemplate = _dataService.CreateItemFromTemplate(templateName,
                    DcxZones.aedos, HeroRarityDto.Common);
            }
            else
            {
                randomTemplate = DataHelper.CreateTypefromJsonTemplate($"templates.items.shard.json", new ItemDto());
            }

            randomTemplate.nftTokenId = reward.itemNftId;



            return new UpdateOneModel<NftizedItem>(
                Builders<NftizedItem>.Filter.Where(i => i.itemId == reward.itemNftId),
                Builders<NftizedItem>.Update
                    .SetOnInsert(i => i.item, randomTemplate)
                )
            { IsUpsert = true };
        }).ToArray();

        if (nftsCreated.Length > 0)
        {
            var done = await _perpetualDb.getCollection<NftizedItem>()
            .BulkWriteAsync(nftsCreated);

            _logger.LogInformation($"NftizedItem upserted : {done.Upserts.Count}, modified: {done.ModifiedCount}");
        }

        return new RewardsClaims
        {
            dcxClaim = dcxClaim,
            itemClaims = itemRewards.Select( ir=>new ClaimItemAutorization
            {
                orderId = ir.id,
                itemNftId = ir.itemNftId,
                slot = ir.slot,
                authorization =  _blockChain.AuthrorizeMintItem(walletAddress, ir.itemNftId)
            }).ToArray()
        };
    }

    [HttpGet("claimItemCompleted/{chainId}/{txHash}")]
    public  async Task<string> ClaimItemCompleted(long chainId, string txHash)
    {
        var minted = await _blockChain.GetTxnStatus<cbContract.Tokenomics.ContractDefinition.MintItemFunction>(txHash, chainId);
        if (null == minted)
        {
            throw new ExceptionWithCode("bad tx hash");
        }

        var mintedTokeId = (int)minted.TokenId;

        var done = await _perpetualDb.getCollection<RewardsBase>()
            .OfType<ItemsReward>()
            .UpdateOneAsync(
                r => r.itemNftId == mintedTokeId,
                Builders<ItemsReward>.Update.Set(r => r.claimedHash, txHash)
            );


        var itemReward = await _perpetualDb.getCollection<RewardsBase>()
        .OfType<ItemsReward>()
        .Find(r =>
            r.itemNftId == mintedTokeId
        ).SingleOrDefaultAsync();

        if(null != itemReward)
        {
            await _publishEp.Publish(new UpdateNFtOwner
            {
                contractAddress =  _web3Config.deployedContracts(chainId)!.DcxItemsContract.ToLower(),
                tokenId = itemReward.itemNftId,
                chainId = chainId
            });

        }

        return $"updated {done.ModifiedCount} records";
    }


    [HttpGet("claimDCXCompleted/{chainId}/{txHash}")]
    public async Task<string> ClaimDCXCompleted(long chainId, string txHash)
    {
        var claimed = await _blockChain.GetTxnStatus<cbContract.Tokenomics.ContractDefinition.ClaimDCXFunction>(txHash, chainId);
        if (null == claimed)
        {
            throw new ExceptionWithCode("bad tx hash");
        }


        var done = await _perpetualDb.getCollection<RewardsBase>()
            .OfType<DcxReward>()
            .UpdateOneAsync(
                r => r.id == claimed.OrderId,
                Builders<DcxReward>.Update.Set(r => r.claimedHash, txHash)
            );

        return $"updated {done.ModifiedCount} records";

    }

}

public class RewardsClaims
{
    public ClaimDcxAutorization dcxClaim { get; set; } = new ClaimDcxAutorization();

    public ClaimItemAutorization[] itemClaims { get; set; } = new ClaimItemAutorization[] { };
}

public class ClaimDcxAutorization
{
    [Required]
    public decimal amount { get; set; }

    [Required]
    public string orderId { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string authorization { get; set; } = string.Empty;
}

public class ClaimItemAutorization
{
    [Required]
    public string orderId { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public int itemNftId { get; set; }

    [Required]
    public string authorization { get; set; } = string.Empty;

    [Required]
    public ItemRewardType slot { get; set; }
}




public class RewardsConfig
{
    
    public string dcxRewardsFolder { get; set; } = "rewards";
    public string itemsRewardsFolder { get; set; } = "rewards";

    /// <summary>
    /// The url preefix. actual season is picked up from urlprefix + fileName
    /// </summary>
    public string urlPrefix { get; set; } = "https://raw.githubusercontent.com/deeNewearth/dcxgamestates/main";

}

[MongoCollection("seasonRewards")]
[BsonDiscriminator(Required = true, RootClass = true)]
[BsonKnownTypes(typeof(DcxReward), typeof(ItemsReward))]

public class RewardsBase
{
    [BsonId]
    public string id { get; set; } = Guid.NewGuid().ToString();

    public string rewardId { get; set; } = string.Empty;
    public string address { get; set; } = string.Empty;

    public string? claimedHash { get; set; }

    [MongoIndex]
    public static void CreateIndexes(IMongoCollection<RewardsBase> collection)
    {
        collection.Indexes.CreateOne(
            new CreateIndexModel<RewardsBase>(
            new IndexKeysDefinitionBuilder<RewardsBase>()
                .Ascending(f => f.address)
                .Ascending(f => f.rewardId)
                , new CreateIndexOptions<RewardsBase>
                {
                    Unique = true,
                }
            ));
    }

}

public class DcxReward: RewardsBase
{
    public decimal amount { get; set; }
}



public class ItemsReward: RewardsBase
{
    public ItemRewardType slot { get; set; }

    public int itemNftId { get; set; }

}