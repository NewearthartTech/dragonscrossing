using System;
using DragonsCrossing.Core.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using System.ComponentModel.DataAnnotations;

namespace DragonsCrossing.Core.Sagas;

[BsonIgnoreExtraElements]
[MongoCollection("contractWatchState")]
public class ContractWatchState
{
    [BsonId]
    public string contractAddressAndChain { get; set; } = string.Empty;

    public UpdateNFtOwner updateMessage { get; set; } = new UpdateNFtOwner();

    public DateTime lastRanAt { get; set; } = DateTime.Now;
}

public class NewOwner
{
    public string ownerAddress { get; set; } = String.Empty;
    public int tokenId { get; set; }
}

public interface IUpdateNFTOwnerService
{
    Task UpdateOwner(NewOwner[] owners, string contractAddress, long chainId);
    Task<OwnedNft[]> GetOwnedNfts(string contractAddress, string walletAddress);
}

public class OwnedNft
{
    [Required]
    public int tokenId { get; set; }

    [Required]
    public long chainId { get; set; }
}

public class UpdateNFTOwnerService: IUpdateNFTOwnerService
{
    readonly ILogger _logger;
    readonly IPerpetualDbService _perpertualDb;

    public UpdateNFTOwnerService(
    IPerpetualDbService perpertualDb,
    ILogger<OwnerCacheConsumer> logger)
    {
        _perpertualDb = perpertualDb;
        _logger = logger;
    }

    public async Task<OwnedNft[]> GetOwnedNfts(string contractAddress, string walletAddress)
    {
        var lowerOwner = walletAddress.ToLower();
        var contractLower = contractAddress.ToLower();
        var owned = await _perpertualDb.getCollection<NftOwner>()
            .Find(o => o.owner == lowerOwner && o.contractAddress == contractLower && 0 != o.chainId)
            .ToListAsync();

        return owned.Select(o => new OwnedNft
        {
            tokenId = o.tokenId,
            chainId = o.chainId
        }).DistinctBy(h=>h.tokenId)
        .ToArray();
    }


    public async Task UpdateOwner(NewOwner[] owners, string contractAddress, long chainId)
    {
        contractAddress = contractAddress.ToLower();

        var newOwners = owners.Select(t =>
        {
            var ownerAddress = t.ownerAddress.ToLower();

            return new UpdateOneModel<NftOwner>(
                Builders<NftOwner>.Filter.Where(o => o.contractAddress == contractAddress && o.tokenId == t.tokenId && o.chainId == chainId ),
                Builders<NftOwner>.Update
                .Set(o => o.owner, ownerAddress)
                .Set(o => o.lastUpdated, DateTime.Now)
                .SetOnInsert(o => o.contractAddress, contractAddress)
                .SetOnInsert(o => o.tokenId, t.tokenId)
                .SetOnInsert(o => o.chainId, chainId)

                )
            {
                IsUpsert = true
            };

        }).ToArray();

        if (newOwners.Length > 0)
        {
            var done = await _perpertualDb.getCollection<NftOwner>().BulkWriteAsync(newOwners);

            _logger.LogInformation($"UpdateAllTokens upserted : {done.Upserts.Count}, modified: {done.ModifiedCount}", new { contractAddress });
        }
    }
}

public class OwnerCacheConsumer : IConsumer<UpdateNFtOwner>
{
    readonly ILogger _logger;
    readonly Web3Config _web3Config;
    readonly IPerpetualDbService _perpertualDb;
    readonly IUpdateNFTOwnerService _updateNFTOwnerService;

    public OwnerCacheConsumer(
        IConfiguration config,
        IPerpetualDbService perpertualDb,
        IUpdateNFTOwnerService updateNFTOwnerService,
        ILogger<OwnerCacheConsumer> logger)
    {
        _perpertualDb = perpertualDb;
        _updateNFTOwnerService = updateNFTOwnerService;
        _web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
        _logger = logger;
    }

    readonly static long[] _SupportedByAlchemy = new[] {
        80001L, //mumbai polygon
        42161L, //arb one
    };

    public async Task Consume(ConsumeContext<UpdateNFtOwner> context)
    {
        _logger.LogInformation($"Consume UpdateNFtOwner", context.Message);

        if (0 == context.Message.chainId)
        {
            context.Message.chainId = _web3Config.defaultChainId;
        }


        var toWatch = _web3Config.contractsToWatch().Where(c => c.chainId == context.Message.chainId && c.contractAddress.ToLower() == context.Message.contractAddress.ToLower()).FirstOrDefault();
        if(null == toWatch)
        {
            _logger.LogError($"NotWatchingContract", context.Message);
            return;
        }

        var contractAddress = context.Message.contractAddress.ToLower();

        if (null != context.Message.tokenId)
        {
            if (_SupportedByAlchemy.Contains(context.Message.chainId))
            {
                await UpdateOneTokenAlchemy(contractAddress, context.Message.tokenId.Value, context.Message.chainId);
                return;
            }
            else
            {
                throw new NotSupportedException($"chain not supported: {context.Message.chainId}");
            }
        }


        var watchKey = $"{contractAddress}_{context.Message.chainId}";

        var lastWatch = await _perpertualDb.getCollection<ContractWatchState>().Find(w => w.contractAddressAndChain == watchKey).SingleOrDefaultAsync();

        if(null != lastWatch && lastWatch.updateMessage.createdAt > context.Message.createdAt)
        {
            _logger.LogInformation("gotNewNFTUpdateMessage", new { lastWatch = lastWatch.updateMessage.createdAt, newWatch = context.Message.createdAt });
            return;
        }

        if (_SupportedByAlchemy.Contains(context.Message.chainId))
        {
            await UpdateAllTokensAlchemy(contractAddress, context.Message.chainId);
        }
        else
        {
            throw new NotSupportedException($"chain not supported: {context.Message.chainId}");
        }

        
        await context.Defer(TimeSpan.FromSeconds(_web3Config.tokenCacheUpdateInveral));

        await _perpertualDb.getCollection<ContractWatchState>().UpdateOneAsync(
            w => w.contractAddressAndChain == watchKey,
            Builders<ContractWatchState>.Update
                .Set(w => w.updateMessage, context.Message)
                .Set(w => w.lastRanAt, DateTime.Now.ToUniversalTime())
                .SetOnInsert(w=>w.contractAddressAndChain, watchKey)
                ,
            new UpdateOptions
            {
                IsUpsert = true
            }
            ) ;

    }

    async Task UpdateOneTokenAlchemy(string contractAddress, int tokenId, long chainId)
    {
        var alchamyURL = $"https://{_web3Config.chainRpc[chainId.ToString()].name}.g.alchemy.com/nft/v2/{_web3Config.chainRpc[chainId.ToString()].alchemyKey}/getOwnersForToken?contractAddress={contractAddress}&tokenId={tokenId}";
        _logger.LogDebug($"calling alchemy with {alchamyURL}");

        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(alchamyURL),
            Headers =
            {
                { "accept", "application/json" },
            },
        };
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();

            var fromAlchemy = JsonConvert.DeserializeAnonymousType(body, new
            {
                owners = new[] { "" }
            });

            if (null == fromAlchemy)
            {
                throw new Exception("got null after desirealizeing alchemy");
            }

            await _updateNFTOwnerService.UpdateOwner(new[] { new NewOwner{
                ownerAddress = fromAlchemy.owners.First().ToLower(),
                tokenId = tokenId
            } },contractAddress,chainId);
        }

    } 


    static readonly Dictionary<string, DateTime> _allTokenKickOffTime = new Dictionary<string, DateTime>();


    async Task UpdateAllTokensAlchemy(string contractAddress, long chainId, string? pageKey = null)
    {
        var alchamyURL = $"https://{_web3Config.chainRpc[chainId.ToString()].name}.g.alchemy.com/nft/v2/{_web3Config.chainRpc[chainId.ToString()].alchemyKey}/getOwnersForCollection?contractAddress={contractAddress}&withTokenBalances=true";
        _logger.LogDebug($"calling alchemy with {alchamyURL}");


        var fetched = (await fetchAllTokensFromAlchemy(alchamyURL, pageKey));

        contractAddress = contractAddress.ToLower();

        await _updateNFTOwnerService.UpdateOwner(fetched.owners.Select(t => new NewOwner{
                ownerAddress = t.owner.ToLower(),
                tokenId = t.tokenId
            } ).ToArray(), contractAddress, chainId);

        if (!string.IsNullOrWhiteSpace(fetched.pageKey))
        {
            _logger.LogDebug("got page key calling again");
            await UpdateAllTokensAlchemy(contractAddress, chainId, fetched.pageKey);
        }

    }

    async Task<FetchedNftOwners> fetchAllTokensFromAlchemy(string alchamyURL, string? pageKey)
    {

        if (!string.IsNullOrWhiteSpace(pageKey))
        {
            alchamyURL += $"&pageKey={pageKey}";
        }

        _logger.LogDebug($"calling alchemy with {alchamyURL}");

        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(alchamyURL),
            Headers =
            {
                { "accept", "application/json" },
            },
        };
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();


            var fromAlchemy = JsonConvert.DeserializeAnonymousType(body, new
            {
                pageKey = "",
                ownerAddresses = new[]
                {
                    new
                    {
                        ownerAddress = "",
                        tokenBalances = new[]
                        {
                            new {tokenId = "" },
                        }
                    }
                }

            });

            if (null == fromAlchemy)
            {
                throw new Exception("got null after desirealizeing alchemy");
            }


            return new FetchedNftOwners
            {
                pageKey = fromAlchemy.pageKey,
                owners = fromAlchemy.ownerAddresses.SelectMany(t=> {


                    var tokens = t.tokenBalances.Select(tb =>
                    {
                        //remove the leading 0x
                        var longTokenId = tb.tokenId.Remove(0, 2);

                        //remove the staring 0s
                        longTokenId = longTokenId.TrimStart('0');

                        return new NftOwner
                        {
                            owner = t.ownerAddress,
                            tokenId = int.Parse(longTokenId, System.Globalization.NumberStyles.HexNumber)
                        };

                    }).ToArray();


                    return tokens;
                }).ToArray()
            };


            
        }

    }


}

public class FetchedNftOwners
{
    public NftOwner[] owners { get; set; } = new NftOwner[] { };
    public string? pageKey { get; set; }

}

/// <summary>
/// this message triggers when we should check for owner updates
/// </summary>
public class UpdateNFtOwner
{
    //we use this to ensure not call Alchemy redundetly
    public DateTime createdAt { get; set; } = DateTime.Now.ToUniversalTime();

    /// <summary>
    /// What contract we are checking for
    /// </summary>
    public string contractAddress { get; set; } = string.Empty;

    [Required]
    public long chainId { get; set; }

    /// <summary>
    /// if null we are updating the whole db
    /// </summary>
    public int? tokenId { get; set; }

}

[MongoCollection("nftOwners")]
public class NftOwner
{
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; } = "";

    public string contractAddress { get; set; } = string.Empty;

    public long chainId { get; set; }

    public int tokenId { get; set; }

    public DateTime lastUpdated { get; set; } = DateTime.Now;

    /// <summary>
    /// the address of the owner wallet (toLower())
    /// </summary>
    public string owner { get; set; } = string.Empty;

    [MongoIndex]
    public static void CreateIndexes(IMongoCollection<NftOwner> collection)
    {
        collection.Indexes.CreateOne(
            new CreateIndexModel<NftOwner>(
            new IndexKeysDefinitionBuilder<NftOwner>()
                .Ascending(f=>f.chainId)
                .Ascending(f => f.contractAddress)
                .Ascending(f => f.tokenId)
                , new CreateIndexOptions<NftOwner>
                {
                    Unique = true
                }
            ));
    }
}



