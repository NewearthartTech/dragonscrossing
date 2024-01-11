using System.ComponentModel.DataAnnotations;
using System.Text;
using DragonsCrossing.Api.Utilities;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Services;
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
public class MintsController : ControllerBase
{
    readonly ILogger _logger;
    readonly Web3Config _web3Config;
    readonly HeroMintConfig _whiteListConfig;
    readonly IPerpetualDbService _perpetualDb;
    readonly IBlockchainService _blockChain;

    public MintsController(
        IPerpetualDbService perpetualDb,
        IBlockchainService blockChain,
        IConfiguration config,
        ILogger<HeroesController> logger)
    {
        _logger = logger;
        _blockChain = blockChain;
        _perpetualDb = perpetualDb;
        _web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
        _whiteListConfig = config.GetSection("whitelist").Get<HeroMintConfig>() ?? new HeroMintConfig();
    }


    static readonly string REWARDID = "reward";

    string GetValueFromQueryParam(string? queryParams, string key)
    {
        if (string.IsNullOrWhiteSpace(queryParams))
        {
            return string.Empty;
        }

        var qParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(queryParams);

        if (qParams!.TryGetValue(REWARDID, out var qValue))
        {
            if (!string.IsNullOrWhiteSpace(qValue))
            {
                return qValue.ToLower();
            }
        }

        return string.Empty;
    }


    [HttpPost("price")]
    public  HeroMintPrice getHeroMintPrice([FromBody] HeroMintPriceReq req)
    {
        if (!_web3Config.devMode)
            throw new ExceptionWithCode("no more mints");


        return new HeroMintPrice();
    }

    [HttpPost("order")]
    public async Task<HeroMintOrder> MintOrder([FromBody] HeroMintOrderReq order)
    {
        if (!_web3Config.devMode)
            throw new ExceptionWithCode("no more mints");


        var lowerWalletAddress = order.walletAddress.ToLower();
        var rewardId = GetValueFromQueryParam(order.queryParams, REWARDID);

        var rewardId_wallet = string.Empty;

        if (!string.IsNullOrWhiteSpace(rewardId))
        {
            if (string.IsNullOrWhiteSpace(lowerWalletAddress))
                throw new Exception("wallet address is required for reward");


            rewardId_wallet = $"{rewardId}_{lowerWalletAddress}";

            var existing = await _perpetualDb.getCollection<HeroMintOrder>()
                .Find(w => w.rewardId_wallet == rewardId_wallet).FirstOrDefaultAsync();

            if (null != existing)
            {
                _logger.LogInformation($"mint order already exists for rewardId_wallet {rewardId_wallet}");
                return existing;
            }

        }

        var ret = new HeroMintOrder
        {
            walletAddress = lowerWalletAddress,
            paymentTokenAddress = _web3Config.deployedContracts(_web3Config.defaultChainId)!.dcxTokenContract,
            mintProps = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                c = order.heroClass.HasValue ? order.heroClass.ToString() : "",
                g = order.gender.HasValue ? order.gender.ToString() : "",
            }),
            rewardId_wallet = string.IsNullOrWhiteSpace(rewardId_wallet)?null: rewardId_wallet

        };

        if (0 == order.quantity)
            throw new Exception("Quantity is 0");

        if ( order.quantity > 1)
            throw new ExceptionWithCode("Quantity is more then max Quantity");


        if (!string.IsNullOrWhiteSpace(order.queryParams))
        {
            var qParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(order.queryParams);

            if(qParams!.TryGetValue(nameof(HeroMintOrder.affiliateId), out var affiliateId))
            {
                if(!string.IsNullOrWhiteSpace(affiliateId))
                {
                    ret.affiliateId = affiliateId.ToLower();
                }
            }
        }

        var mintPrice = new HeroMintPrice();

        ret.totalPrice = (decimal)
            (((order.heroClass.HasValue || order.gender.HasValue) ?
                mintPrice.boostedPrice : mintPrice.basePrice) * order.quantity);

        byte[] orderHash = new Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes(ret.orderHash));

        ret.authorization = Web3Utils.generateSignature(_web3Config!,
            new ABIValue("bytes32", orderHash),
            new ABIValue("uint256", order.quantity),
            new ABIValue("address", ret.paymentTokenAddress),
            new ABIValue("uint256", await Web3Utils.weiValueForDcxToken(_web3Config!, ret.totalPrice, _web3Config!.defaultChainId)),
            new ABIValue("bytes", Encoding.UTF8.GetBytes(ret.mintProps)),
            new ABIValue("address", order.walletAddress)
            );
        
        await _perpetualDb.getCollection<HeroMintOrder>().InsertOneAsync(ret);
        

        return ret;
    }


}

/// <summary>
/// almost an anti pattern, we store system configs that can be changed by API
/// </summary>
[MongoCollection("sysConfig")]
public class SystemConfig
{
    [BsonId]
    public string configName { get; set; } = string.Empty;

    /// <summary>
    /// we store the configs as JSON strings
    /// </summary>
    public string configStr { get; set; } = string.Empty;

}

public class MintWhiteList
{
    public long maxHeros { get; set; }

    /// <summary>
    /// each wallet is allowed only 1
    /// </summary>
    public bool onlyOne { get; set; }

    public string[] whiteList { get; set; } = new string[] { };

    public HeroMintPrice mintPrice { get; set; } = new HeroMintPrice();

    //any free or reduced priced mints
    public RewardMint[] rewardMints { get; set; } = new RewardMint[] { };
}

public class RewardMint
{
    public string wallet { get; set; } = string.Empty;

    public decimal price { get; set; }

    public string id { get; set; } = string.Empty;
}


public class HeroMintConfig
{
    /// <summary>
    /// The file we are using for seasons
    /// </summary>
    public string whiteListFolder { get; set; } = "whitelist";

    public bool mintAllowed { get; set; } = false;

    /// <summary>
    /// The url preefix. actual season is picked up from urlprefix + fileName
    /// </summary>
    public string urlPrefix { get; set; } = "https://raw.githubusercontent.com/deeNewearth/dcxgamestates/main";

}

public class HeroMintOrderReq
{
    [Required]
    public int quantity { get; set; }
    
    public Gender? gender { get; set; }

    public CharacterClassDto? heroClass { get; set; }

    [Required]
    public string walletAddress { get; set; } = "";

    public string? queryParams { get; set; }


}

[MongoCollection("mintOrders")]
public class HeroMintOrder
{
    [BsonId]
    [Required]
    public string orderHash { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string walletAddress { get; set; } = "";

    [Required]
    public string mintProps { get; set; } = string.Empty;

    [Required]
    public string paymentTokenAddress { get; set; } = string.Empty;

    [Required]
    public decimal totalPrice { get; set; }

    [Required]
    public string authorization { get; set; } = "";

    public string? affiliateId { get; set; }

    /// <summary>
    /// we are making this a composite. if this is not null there can only be ONE rewrd per wallet id
    /// </summary>
    [BsonIgnoreIfDefault]
    public string? rewardId_wallet { get; set; }


    
    [MongoIndex]
    public static void CreateIndexes(IMongoCollection<HeroMintOrder> collection)
    {
        //var caseInsensitiveCollation = new Collation("en", strength: CollationStrength.Primary);


        collection.Indexes.CreateOne(
            new CreateIndexModel<HeroMintOrder>(
            new IndexKeysDefinitionBuilder<HeroMintOrder>()
                .Ascending(f => f.rewardId_wallet), new CreateIndexOptions
                {
                    Unique = true,
                    Sparse = true
                }
            ));

        /* ONly One is gone
        collection.Indexes.CreateOne(
            new CreateIndexModel<HeroMintOrder>(
            new IndexKeysDefinitionBuilder<HeroMintOrder>()
                .Ascending(f => f.walletAddress), new CreateIndexOptions
                {
                    //Collation = caseInsensitiveCollation,
                    Unique = true
                }
            ));
        */

    }
    
}

public class HeroMintPriceReq
{
    [Required]
    public string walletAddress { get; set; } = "";

    public string? queryParams { get; set; }

}

public class HeroMintPrice
{
    [Required]
    public decimal basePrice { get; set; } = 0.05m;

    [Required]
    public decimal boostedPrice { get; set; } = 0.07m;

    [Required]
    public int maxQuantity { get; set; } = 5;

}

