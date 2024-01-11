using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using DragonsCrossing.Api.Utilities;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Core.Sagas;
using DragonsCrossing.Core.Services;
using DragonsCrossing.Domain.GameStates;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Players;
using DragonsCrossing.NewCombatLogic;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Nethereum.ABI;
using Nethereum.Util;

namespace DragonsCrossing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SeasonController : ControllerBase
{
    readonly IPerpetualDbService _perpetualDb;
    readonly IPublishEndpoint _publishEp;
    readonly ILogger _logger;
    readonly SeasonConfig _config;
    readonly IBlockchainService _blockChain;
    readonly IServiceProvider _sp;
    readonly Web3Config _web3Config;
    readonly IHeroesService _heroesService;

    public SeasonController(
        IPerpetualDbService perpetualDb,
        IPublishEndpoint publishEp,
        IConfiguration config,
        ILogger<SeasonController> logger,
        IBlockchainService blockChain,
        IHeroesService heroesService,
        IServiceProvider sp
        )
    {
        _sp = sp;
        _heroesService = heroesService;
        _web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
        _perpetualDb = perpetualDb;
        _publishEp = publishEp;
        _logger = logger;
        _config = config.GetSection("seasons").Get<SeasonConfig>() ?? new SeasonConfig();
        _blockChain = blockChain;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">
    /// [optional] ommited show open seasons,
    /// "all" show all,
    /// id shows the season with id</param>
    /// <returns></returns>
    [HttpGet("openSeasons/{id?}")]
    public async Task<Season[]> GetOpenSeasons(string? id = null)
    {
        // we shouldn't have more then 100 seasons just putting code here so
        // that we don't crash
        var all = (await _perpetualDb.getCollection<SeasonWrapper>()
            .Find(s => true)
            .Project(s=>s.details)
            .Limit(100)
            .ToListAsync()).ToArray();
            ;

        if (string.IsNullOrWhiteSpace(id))
        {
            //return open season
            return all.Where(s => s.isGamePlayOpen || s.isRegistrationOpen).ToArray();
        }

        if(id.ToLower() == "all")
        {
            return all;
        }

        if(!int.TryParse(id, out var idInt))
        {
            throw new InvalidOperationException($"id {id} is not an integer");
        }

        return all.Where(s => s.seasonId == idInt).ToArray();
    }

    /// <summary>
    /// Load the seasons from a URL
    /// </summary>
    /// <returns></returns>
    [HttpGet("loadSeason/{fileName}")]
    public async Task<Season> loadSeason(string fileName)
    {
        var url = $"{_config.urlPrefix}/{_config.seasonsFolder}/{fileName}";

        _logger.LogInformation($"Loading season from {url}");

        try
        {
            var season = await ExtentionMethods.LoadObjectFromURL<Season>(url);

            await _perpetualDb.getCollection<SeasonWrapper>().UpdateOneAsync(
                s => s.id == season.seasonId,
                Builders<SeasonWrapper>.Update
                    .Set(s => s.details, season),
                new UpdateOptions
                {
                    IsUpsert = true
                });

            return season;

        }
        catch(Exception ex)
        {
            throw new ExceptionWithCode(ex.Message, innerException: ex);
        }

    }

    #region Hero Signup
    [AllowAnonymous]
    [HttpGet("signupCompleted/{chainId}/{fulfillmentTxnHash}")]
    public async Task<string> PostSignUpOrder(long chainId, string fulfillmentTxnHash, int retryCount = 0)
    {
        fulfillmentTxnHash = fulfillmentTxnHash.ToLower();

        if (string.IsNullOrWhiteSpace(fulfillmentTxnHash))
            throw new ArgumentNullException(nameof(fulfillmentTxnHash));

        var orderCollection = _perpetualDb.getCollection<DcxOrder>().OfType<SignUpToSeasonOrder>();
        SignUpToSeasonOrder? existingSignUpOrder = null;
        BigInteger? txAmmount = null;

        if (chainId == _web3Config.defaultChainId)
        {
            try
            {
                var txDetails = await _blockChain.GetTxnStatus<cbContract.Tokenomics.ContractDefinition.RegisterForSeasonFunction>(fulfillmentTxnHash, chainId);


                if (null != txDetails)
                {
                    existingSignUpOrder = await orderCollection.Find(l => l.id == txDetails.OrderId).SingleAsync();
                    txAmmount = txDetails.Amount;
                }
                else
                {
                    if (retryCount < 5)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        return await PostSignUpOrder(chainId, fulfillmentTxnHash, ++retryCount);
                    }
                    else
                    {
                        throw new Exception($"tx {fulfillmentTxnHash} is null ");
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogDebug("NotRegisterForSeasonFunctionTx", ex);

                if (ex.Message.Contains("not a valid deposit operation"))
                {

                    var tx2Details = await _blockChain.GetTxnStatus<cbContract.Tokenomics.ContractDefinition.MintLoanerHeroFunction>(fulfillmentTxnHash, chainId);

                    if (null == tx2Details)
                        throw new ExceptionWithCode("transaction not found");

                    existingSignUpOrder = await orderCollection.Find(l => l.loanerDetails!.orderHash == tx2Details.OrderHash).SingleAsync();
                    txAmmount = 0;

                }
                else
                {
                    throw;
                }

            }
        }
        else if( chainId == _web3Config.shadowChain.playChainId )
        {

            var txDetails = await _blockChain.GetTxnStatus<cbContract.DCXToken.ContractDefinition.TransferFunction>(fulfillmentTxnHash, chainId);
            if (null == txDetails)
                throw new ExceptionWithCode("transaction not found");


            try
            {
                if(txDetails.To.ToLower() != _web3Config.shadowChain.depositWallet.ToLower())
                {
                    throw new ExceptionWithCode("deposit not to correct wallet");
                }

                await _perpetualDb.getCollection<DFKDeposit>().InsertOneAsync(new DFKDeposit
                {
                    depositFrom = txDetails.FromAddress.ToLower(),
                    txHash = fulfillmentTxnHash,
                    dcxValue = await Web3Utils.DcxValueFromWei(_web3Config, txDetails.Amount, chainId)
                });

            }catch(Exception ex)
            {
                if (ex.Message.Contains("DuplicateKey")){
                    _logger.LogDebug($"dfk deposit already recorded {fulfillmentTxnHash}");


                    var tx = await _perpetualDb.getCollection<DFKDeposit>().Find(d => d.txHash == fulfillmentTxnHash).SingleAsync();

                    if (!string.IsNullOrWhiteSpace(tx.usedUpBy))
                    {
                        throw new Exception($"txAlreadyUsed :{tx.usedUpBy}");
                    }

                }
                else
                {
                    throw;
                }
                
            }

            txAmmount = txDetails.Amount;

            var dxAmount = await Web3Utils.DcxValueFromWei(_web3Config, txDetails.Amount, chainId);

            var g = txDetails.FromAddress.ToLower();

            var Q = orderCollection.Find(l =>
                l.priceInDcx == dxAmount && l.forWallet == txDetails.FromAddress.ToLower()
                && l.chainId == chainId
                && null == l.fulfillmentTxnHash);

            var k = Q.ToString();

            existingSignUpOrder = await orderCollection.Find(l =>
                l.priceInDcx == dxAmount && l.forWallet == txDetails.FromAddress.ToLower()
                && l.chainId == chainId
                && null == l.fulfillmentTxnHash
                ).FirstAsync();

            if (null == existingSignUpOrder)
                throw new Exception($"noOrderFound {dxAmount }, {chainId}, {txDetails.FromAddress.ToLower()} ");

        }
        else
        {
            throw new Exception($"chain {chainId} not supported");
        }

        if(null == existingSignUpOrder || null == txAmmount)
        {
            throw new ExceptionWithCode("transaction not found");
        }


        var orderamount = await Web3Utils.weiValueForDcxToken(_web3Config!, existingSignUpOrder.priceInDcx, chainId);

        if(orderamount > txAmmount)
        {
            _logger.LogError($"orderamount {orderamount} > txAmount {txAmmount}");
            throw new Exception("bad tx amount");
        }


        if (!String.IsNullOrWhiteSpace(existingSignUpOrder.fulfillmentTxnHash))
            throw new ExceptionWithCode("transaction already submitted");

        var heroId = existingSignUpOrder.heroId;

        if(null != existingSignUpOrder.loanerDetails)
        {

            if(chainId == _web3Config.defaultChainId)
            {
                _logger.LogDebug("registeringDemoHero");
                heroId = await _blockChain.GetMintedHeroId(existingSignUpOrder.loanerDetails.orderHash, chainId);

                var loanedHero = await _heroesService.GetHero(heroId, perpetualOnly: true);

                if (!string.IsNullOrWhiteSpace(loanedHero.isLoanedHero?.loanedToUserId))
                {
                    if (loanedHero.isLoanedHero.loanedToUserId != existingSignUpOrder.loanerDetails.loanedToUserId)
                    {
                        throw new Exception($"incorrectLoanerHero loaned to {loanedHero.isLoanedHero.loanedToUserId}, expected {existingSignUpOrder.loanerDetails.loanedToUserId}");
                    }
                }
                else
                {
                    await _perpetualDb.getCollection<HeroDto>().UpdateOneAsync(h => h.id == loanedHero.id,
                        Builders<HeroDto>.Update.Set(h => h.isLoanedHero, new LoanedHero
                        {
                            loanedToUserId = existingSignUpOrder.loanerDetails.loanedToUserId,
                            loanerType = LoanerHeroType.Demo
                        }));
                }

            }else if(chainId == _web3Config.shadowChain.playChainId)
            {
                _logger.LogDebug("registeringDFKHero");

                var loanedHero = await _heroesService.GetHero(heroId, perpetualOnly: true);

                await _perpetualDb.getCollection<HeroDto>().UpdateOneAsync(h => h.id == loanedHero.id,
                    Builders<HeroDto>.Update.Set(h => h.isLoanedHero, new LoanedHero
                    {
                        //loanedToUserId = existingSignUpOrder.loanerDetails.loanedToUserId,
                        loanerType = LoanerHeroType.DFK
                    }));

            }
            else
            {
                throw new NotSupportedException("chain not supported");
            }

        }

        await _publishEp.Publish(new SeasonSignupMessage
        {
            heroId = heroId,
            seasonId = existingSignUpOrder.seasonId
        });

        //await _publishEp.Publish(new SignUpToSeasonOrdered(existing, newId));

        var done = await orderCollection.UpdateOneAsync(l => l.id == existingSignUpOrder.id
                && string.IsNullOrEmpty(l.fulfillmentTxnHash),
                Builders<SignUpToSeasonOrder>.Update
                    .Set(o => o.fulfillmentTxnHash, fulfillmentTxnHash)
                    .Set(o => o.IsCompleted, true)
                );

        if(1 != done.ModifiedCount)
        {
            throw new Exception($"couldn't update order id {existingSignUpOrder.id} with hash");
        }

        await _perpetualDb.getCollection<DFKDeposit>().UpdateOneAsync(
            d => d.txHash == fulfillmentTxnHash,
            Builders<DFKDeposit>.Update.Set(d=>d.usedUpBy,$"dcxOrder:existingSignUpOrder.id")
            
            );

        return "sign up complete";
    }

    [Authorize(Policy = "HeroSelected")]
    [HttpGet("signupOrder/{seasonId}")]
    public async Task<SignUpToSeasonOrder> GetSignUpOrder(int seasonId)
    {
        var userId = this.GetUserId()!;
        var heroId = this.GetHeroId();
        var orderCollection = _perpetualDb.getCollection<DcxOrder>().OfType<SignUpToSeasonOrder>();

        var hero = await _perpetualDb.getCollection<HeroDto>().Find(c => c.id == heroId).SingleOrDefaultAsync();


        var existingOrder = (HeroDto.LOANERHEROID == heroId)?
            await orderCollection.Find(l => l.loanerDetails!.loanedToUserId == userId && l.seasonId == seasonId).SingleOrDefaultAsync():
            await orderCollection.Find(l => l.heroId == heroId && l.seasonId == seasonId).SingleOrDefaultAsync();

        if (null != hero && 0 != hero.seasonId)
        {
            if(hero.seasonId == seasonId)
            {
                if(null == existingOrder)
                {
                    throw new InvalidOperationException($"hero {heroId} is signed up for season {seasonId} but no order found");
                }

                existingOrder.IsCompleted = true;

                return existingOrder;
            }

            throw new Exception($"Hero {heroId} is already signed up to season {hero.seasonId}");
        }


        if (null != existingOrder)
        {
            _logger.LogDebug($"order for hero {heroId} to sign up for season {seasonId} exists");
            return existingOrder;
        }

        var season = await _perpetualDb.getCollection<SeasonWrapper>().Find(s => s.id == seasonId).SingleAsync();

        var player = await _perpetualDb.getCollection<PlayerDto>().Find(p => p.Id == userId).SingleAsync();

        if (string.IsNullOrWhiteSpace(player.BlockchainPublicAddress))
            throw new Exception("Play wallet address is NULL");


        var order = new SignUpToSeasonOrder
        {
            seasonId = seasonId,
            heroId = heroId,
            priceInDcx = season.details.dcxCostToRegister,
            chainId = HeroDto.isDefaultChainFromId(heroId) ? _web3Config.defaultChainId : _web3Config.shadowChain.playChainId,
            forWallet = player.BlockchainPublicAddress.ToLower()
        };

        if (HeroDto.LOANERHEROID == heroId)
        {
            _logger.LogDebug("signUpHeroRegistartion");
            order.heroId = 0;
            order.priceInDcx = 0;
            order.chainId = _web3Config.defaultChainId;


            byte[] orderHash = new Sha3Keccack().CalculateHash(System.Text.Encoding.UTF8.GetBytes(order.id));

            order.loanerDetails = new LoanerSignupDetails
            {
                orderHash = orderHash,
                authorization = Web3Utils.generateSignature(_web3Config,
                    new ABIValue("bytes32", orderHash),
                    new ABIValue("uint256", order.priceInDcx),
                    new ABIValue("address", player.BlockchainPublicAddress)),
                loanedToUserId = userId
            };
        }else if (!HeroDto.isDefaultChainFromId(heroId))
        {
            _logger.LogDebug("dfkHeroRegistration");

            order.loanerDetails = new LoanerSignupDetails
            {
                loanedToUserId = userId
            };
        }

        await orderCollection.InsertOneAsync(order);

        return order;

    }
    #endregion



    #region LeaderBoard


    readonly static Dictionary<int, SeasonLeaderboard> _rankDictionary = new Dictionary<int, SeasonLeaderboard>();
    

    async Task<SeasonLeaderboard> CalculateHeroRanks(int seasonId)
    {
        if(_rankDictionary.TryGetValue(seasonId, out var lastRanks))
        {
            if(lastRanks.lastUpdated.AddSeconds(_config.leaderBoardRefreshInterval) > DateTime.Now)
            {
                _logger.LogTrace($"CalculateHeroRanks for season {seasonId} last updated at {lastRanks.lastUpdated}. Skipping");
                return lastRanks;
            }
        }


        //This is quite CPU intensive and inefficient. But given the really low number of Heros sold
        //we will optimize this once We volume grows

        //we mainly call this to ensure the season db actually exists
        var season = await _perpetualDb.getCollection<SeasonWrapper>().Find(s => s.id == seasonId).SingleAsync();
        var seasonsDb = ActivatorUtilities.CreateInstance<SeasonsDbService>(_sp, seasonId);


        //we only grab the IDs now to reduce the memory footprint
        var bossBeaterIds = (await seasonsDb.getCollection<DbGameState>()
            .Find(g => g.leaderStatus.isFinalBossDefeated &&
            (null == g.Hero.isLoanedHero || g.Hero.isLoanedHero!.loanerType == LoanerHeroType.DFK)
            )
            .SortBy(g => g.leaderStatus.benchmarkUsedQuests)
            .Project(g => g.HeroId)
            .ToListAsync())
            .ToArray();

        var farthestZoneTraveledIds = (await seasonsDb.getCollection<DbGameState>()
            .Find(g => (null == g.Hero.isLoanedHero || g.Hero.isLoanedHero!.loanerType == LoanerHeroType.DFK)
            && !bossBeaterIds.Contains( g.HeroId))
            .SortByDescending(g => g.leaderStatus.farthestZoneDiscoveredOrder)
            .ThenBy(g => g.leaderStatus.benchmarkUsedQuests)
            .Project(g => g.HeroId)
            .ToListAsync())
            .ToArray();


        var realRanks = bossBeaterIds.Concat(farthestZoneTraveledIds).Select((heroId, rank) => new HeroRank
        {
            heroId= heroId,
            rank = rank + 1
        })
        .ToArray();


        _rankDictionary[seasonId] = new SeasonLeaderboard
        {
            heroRanks = realRanks,
            lastUpdated = DateTime.Now
        };

        return _rankDictionary[seasonId];
    }

    /// <summary>
    /// Hero ID is optional
    /// </summary>
    /// <param name="seasonId"></param>
    /// <param name="heroId"></param>
    /// <returns></returns>
    /// <exception cref="ExceptionWithCode"></exception>
    [HttpGet("leaderBoard/{seasonId}/{heroId?}")]
    public async Task<SeasonLeaderboard> getLeaderBoard(int seasonId, int heroId = 0, [FromQuery] bool allRanks = false)
    {
        var ranksInCache = (await CalculateHeroRanks(seasonId)).heroRanks;

        var rankstoSend = allRanks? ranksInCache:ranksInCache.Take(10);

        if (0 != heroId)
        {
            var herosRank = ranksInCache.Where(h => h.heroId == heroId).SingleOrDefault();

            if(null == herosRank)
            {
                throw new ExceptionWithCode("Rank not available please try later");
            }

            //if hero is ranked 15 this will be 10
            //if hero is ranked 5 this will be 0
            //if hero is under 5 this will be negative
            if(herosRank.rank > 10)
            {
                rankstoSend = rankstoSend.Concat(new[] { herosRank }).ToArray();
            }
            
        }

        //we mainly call this to ensure the season db actually exists
        var season = await _perpetualDb.getCollection<SeasonWrapper>().Find(s => s.id == seasonId).SingleAsync();
        var seasonsDb = ActivatorUtilities.CreateInstance<SeasonsDbService>(_sp, seasonId);
        var gameStateCollection = seasonsDb.getCollection<DbGameState>();



        var ownerCollection = _perpetualDb.getCollection<NftOwner>();
        var heroContract = "0xfdac927e174fe3eea7f1492feb37f323e29d56a9";// _web3Config.deployedContracts()!.DcxHeroContract.ToLower();

        var ranksToSend = (await Task.WhenAll(rankstoSend
                .Select(async r =>
                {
                    var heroGameState = await seasonsDb.getCollection<DbGameState>().Find(g => g.HeroId == r.heroId).SingleAsync();

                    return new HeroRank
                    {
                        heroId = heroGameState.HeroId,
                        currentOwner = await ownerCollection
                            .Find(o =>
                                o.contractAddress == heroContract && o.tokenId == heroGameState.HeroId)
                                .Project(o=>o.owner)
                                .SingleOrDefaultAsync(),
                        rank = r.rank,
                        level = heroGameState.Hero.level,
                        isFinalBossDefeated = heroGameState.leaderStatus.isFinalBossDefeated,
                        benchmarkQuestsUsed = heroGameState.leaderStatus.benchmarkUsedQuests,
                        farthestZoneDiscovered = heroGameState.leaderStatus.farthestZoneDiscovered,
                        currentTotalUsedQuests = HeroRank.GetCurrentTotalQuestsUsed(heroGameState)
                    };
                })))
                .OrderBy(h=>h.rank)
                .ToArray();

        return new SeasonLeaderboard
        {
            seasonId = seasonId,
            seasonName = season.details.seasonName,
            heroesInSeason = (int)(await gameStateCollection.CountDocumentsAsync(g => true)),
            heroRanks = ranksToSend
        };
    }

    #endregion

}

public class LeaderHero
{
    [Required]
    public int HeroId { get; set; }

    [Required]
    public LeaderStatus leaderStatus { get; set; } = new LeaderStatus();

}

public class SeasonLeaderboard
{
    [Required]
    public int seasonId { get; set; }

    [Required]
    public string seasonName { get; set; } = String.Empty;

    [Required]
    public int heroesInSeason { get; set; }

    [Required]
    public HeroRank[] heroRanks { get; set; } = new HeroRank[] { };

    public DateTime lastUpdated { get; set; } = DateTime.MinValue;

}


public class HeroRank
{

    [Required]
    public int heroId { get; set; }

    [Required]
    public int rank { get; set; }

    [Required]
    public int level { get; set; }

    [Required]
    public bool isFinalBossDefeated { get; set; }

    [Required]
    public DcxZones farthestZoneDiscovered { get; set; } = DcxZones.aedos;

    [Required]
    public int benchmarkQuestsUsed { get; set; }

    [Required]
    public int currentTotalUsedQuests { get; set; }

    [Required]
    public string currentOwner { get; set; } = string.Empty;

    public static int GetCurrentTotalQuestsUsed(DbGameState gameState)
    {
        return gameState.leaderStatus.totalQuestsUsed + gameState.Hero.dailyQuestsUsed;
    }

}

public class SeasonConfig
{
    /// <summary>
    /// The file we are using for seasons
    /// </summary>
    public string seasonsFolder { get; set; } = "main.staging";

    /// <summary>
    /// The url preefix. actual season is picked up from urlprefix + fileName
    /// </summary>
    public string urlPrefix { get; set; } = "https://raw.githubusercontent.com/deeNewearth/dcxgamestates/main/seasons";

    /// <summary>
    /// leaderboard refresh rate in seconds
    /// </summary>
    public int leaderBoardRefreshInterval { get; set; } = 60;

}

[MongoCollection("Seasons")]
public class SeasonWrapper
{
    public int id
    {
        get { return details.seasonId; }
        set { }
    }

    public Season details { get; set; } = new Season();

    /// <summary>
    /// If set to true, we have run the Steps needed after game play expires
    /// </summary>
    public bool seasonCloseExecuted { get; set; }

    /// <summary>
    /// This is used to ensure we don't Queue multiple end tasks
    /// Qing multiple don't hurt but puts unnecessary pressure on the system
    /// We always setup the seasonEnd tasks on server reboot, so that in case the Q is flushed
    /// and we loose a task, it is created again.
    /// </summary>
    public DateTime? seasonEndTaskSetupTime { get; set; } = new DateTime();
}



public class Season
{
    [Required]
    [BsonId]
    public int seasonId { get; set; }

    [Required]
    public string seasonName { get; set; } = String.Empty;

    [Required]
    public DateTime registrationOpenDate { get; set; }

    [Required]
    public DateTime registrationCloseDate { get; set; }

    [Required]
    public DateTime seasonOpenDate { get; set; }

    [Required]
    public DateTime seasonCloseDate { get; set; }

    [Required]
    public decimal dcxCostToRegister { get; set; }

    [Required]
    [BsonIgnore]
    public bool isRegistrationOpen {
        get { return DateTime.Now >= registrationOpenDate && DateTime.Now <= registrationCloseDate; }
        set { }
    }

    [Required]
    [BsonIgnore]
    public bool isGamePlayOpen
    {
        get { return DateTime.Now >= seasonOpenDate && DateTime.Now <= seasonCloseDate; }
        set { }
    }  
}


