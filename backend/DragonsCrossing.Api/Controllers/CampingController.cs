using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.GameStates;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Core.Sagas;
using DragonsCrossing.Core.Services;
using DragonsCrossing.Domain.GameStates;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers;

/// <summary>
/// This is a child of heroes and shouldn't have a root url.
/// </summary>

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "SeasonJoined")]
public class CampingController : ControllerBase
{
    readonly ISeasonsDbService _seasonDb;
    readonly IPublishEndpoint _publishEp;
    readonly IBlockchainService _web3;
    readonly IHeroesService _heroesService;
    readonly IUpdateNFTOwnerService _updateNFTOwnerService;
    readonly IPerpetualDbService _perpetualDb;
    readonly IServiceProvider _sp;

    public CampingController(ISeasonsDbService db,
        IConfiguration config,
        IPerpetualDbService perpetualDb,
        IUpdateNFTOwnerService updateNFTOwnerService,
        IBlockchainService web3,
        IHeroesService heroesService,
        IServiceProvider sp,
        IPublishEndpoint publishEp)
    {
        _sp = sp;
        _updateNFTOwnerService = updateNFTOwnerService;
        _web3 = web3;
        _publishEp = publishEp;
        _seasonDb = db;
        _heroesService = heroesService;
        _perpetualDb = perpetualDb;
    }


    [HttpPost("DCXClaimed")]
    public async Task<CampingStatus> dcxClaimed([FromBody] ClaimDcxOrder order)
    {
        var heroId = this.GetHeroId();

        //todo: verify that it actually worked
        if (!string.IsNullOrWhiteSpace(order.fulfillmentTxnHash))
        {

            var gameStateCollection = _seasonDb.getCollection<DbGameState>();
            var gameState = await _seasonDb.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            var done = await _seasonDb.getCollection<DbGameState>()
                            .UpdateOneAsync(g => g.HeroId == heroId
                                && g.Hero.unsecuredDCXValue.withdrawlOrderId == order.correlationId.ToString(),

                                Builders<DbGameState>.Update
                            .Set(g => g.Hero.unsecuredDCXValue, new UnsecurdDcx()));

            var claimDcxcollection = _seasonDb.getCollection<DcxOrder>().OfType<ClaimDcxOrder>();

            done = await claimDcxcollection.UpdateOneAsync(
                c => c.heroId == heroId && c.correlationId == order.correlationId,
                Builders<ClaimDcxOrder>.Update.Set(c => c.IsCompleted, true)
                );
        }


        return await fulfillmentStatus();
    }


    [AllowAnonymous]
    [HttpGet("ItemSecureCompleted/{txHash}")]
    public async Task<CampingStatus> itemSecured(string txHash)
    {
        if (string.IsNullOrWhiteSpace(txHash))
        {
            throw new ExceptionWithCode("empty hash");
        }

        if (txHash == "empty")
            return await fulfillmentStatus();

        var openSeason = (await _perpetualDb.getCollection<SeasonWrapper>().Find(s => true).ToListAsync())
            .Where(s => s.details.isGamePlayOpen)
            .SingleOrDefault();

        if (null == openSeason)
        {
            throw new ExceptionWithCode("failed to get open season");
        }

        var seasonsDb = ActivatorUtilities.CreateInstance<SeasonsDbService>(_sp, openSeason.id);

        int itemNftId;
        if (txHash.StartsWith("noClaimsForDFK"))
        {
            var itmeStr = txHash.Split("_")[1];

            if(!int.TryParse(itmeStr, out var tokenId)){
                throw new ExceptionWithCode($"{txHash} not valid for chain item claim");
            }

            itemNftId = tokenId;
        }
        else
        {
            var txDetails = await _web3.GetTxnStatus<cbContract.Tokenomics.ContractDefinition.MintItemFunction>(txHash, _web3.config.defaultChainId);
            if (null == txDetails)
                throw new ExceptionWithCode("transaction not found");
            itemNftId = (int)txDetails.TokenId;
        }

        var securedItemOrder = await seasonsDb.getCollection<DcxOrder>().OfType<SecuredNFTsOrder>().Find(o => o.itemNftId == itemNftId).SingleAsync();


        if(securedItemOrder.chainId == _web3.config.shadowChain.playChainId)
        {
            if(await _web3.MintDFKItem(securedItemOrder.recepientAddress, itemNftId))
            {
                await _updateNFTOwnerService.UpdateOwner(new[] {
                    new NewOwner
                    {
                        ownerAddress = securedItemOrder.recepientAddress,
                        tokenId = itemNftId
                    }
                },_web3.config.deployedContracts(_web3.config.defaultChainId)!.DcxDfkItemsContract, _web3.config.defaultChainId);
            }
        }

        //var heroId = this.GetHeroId();
        var heroId = securedItemOrder.heroId;



        //todo: verify that it actually worked
        if (!string.IsNullOrWhiteSpace(txHash))
        {
            var gameStateCollection = seasonsDb.getCollection<DbGameState>();
            var gameState = await seasonsDb.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            var done = await seasonsDb.getCollection<DbGameState>()
                            .UpdateOneAsync(g => g.HeroId == heroId
                                && g.nftClaims.Any(i => i.id == securedItemOrder.itemId),

                                Builders<DbGameState>.Update
                            .PullFilter(i => i.nftClaims,
                            Builders<ItemDto>.Filter.Eq(i => i.id, securedItemOrder.itemId)));

            var itemNftsCollection = seasonsDb.getCollection<DcxOrder>().OfType<SecuredNFTsOrder>();

            done = await itemNftsCollection.UpdateOneAsync(
                c => c.heroId == heroId && c.itemId == securedItemOrder.itemId,
                Builders<SecuredNFTsOrder>
                .Update.Set(c => c.fulfillmentTxnHash, txHash)
                .Set(c => c.IsCompleted, true)
                );
        }

        return await fulfillmentStatus();
    }


    [HttpGet("status")]
    public async Task<CampingStatus> fulfillmentStatus()
    {
        var heroId = this.GetHeroId();

        var claimDcxcollection = _seasonDb.getCollection<DcxOrder>().OfType<ClaimDcxOrder>();
        var itemNftsCollection = _seasonDb.getCollection<DcxOrder>().OfType<SecuredNFTsOrder>();


        return new CampingStatus
        {
            claimDcxOrders = (await claimDcxcollection
                .Find(c => c.heroId == heroId && c.IsCompleted == false).ToListAsync()).ToArray(),
            nftOrders= (await itemNftsCollection
                .Find(c => c.heroId == heroId && c.IsCompleted == false).ToListAsync()).ToArray(),
        };
        
    }

    [HttpGet("goCamp")]
    public async Task<CampingStatus> goCamp()
    {
        var heroId = this.GetHeroId();
        var userId = this.GetUserId();

        var player = await _seasonDb.getCollection<PlayerDto>().Find(p => p.Id == userId).SingleAsync();

        using (var seasonSession = await _seasonDb.StartTransaction())
        /* We are seeing weird db errors with transactions on both dbs
         * using (var perpetualSession = await _perpetualDb.StartTransaction())*/
        {
            var gameStateCollection = _seasonDb.getCollection<DbGameState>();
            var gameState = await _seasonDb.getCollection<DbGameState>().Find(seasonSession, c => c.HeroId == heroId).SingleAsync();

            if (string.IsNullOrWhiteSpace(player.BlockchainPublicAddress))
                throw new Exception("player walletr address is null");

            var nftAbleItems = gameState.Hero.inventory.Where(i => i.isNftAble()).ToArray();
            if (nftAbleItems.Length > 0)
            {
                await Task.WhenAll(nftAbleItems.Select(async item =>
                {
                    item.nftTokenId = HeroDto.isDefaultChainFromId(gameState.HeroId)?
                    await _perpetualDb.GetNextSequence<NftizedItem>():
                    await _perpetualDb.GetNextSequence<NftizedDFKItem>();

                    var claimOrder = new SecuredNFTsOrder
                    {
                        recepientAddress = player.BlockchainPublicAddress,
                        chainId = gameState.Hero.isDefaultChain?_web3.config.defaultChainId:_web3.config.shadowChain.playChainId,
                        seasonId = _seasonDb.seasonId,
                        itemNftId = item.nftTokenId,
                        heroId = gameState.HeroId,
                        itemId = item.id,
                        authorizaton = _web3.AuthrorizeMintItem(
                             player.BlockchainPublicAddress,
                             item.nftTokenId)
                    };

                    await _publishEp.Publish(claimOrder, ctx =>
                    {
                        ctx.AddDelay(TimeSpan.FromSeconds(1));
                    });

                    await _seasonDb.getCollection<DcxOrder>().OfType<SecuredNFTsOrder>().InsertOneAsync(seasonSession, claimOrder);


                    //todo: here are bunch of things here that should be in transatcion, we need to put these in a Q
                    // and prevent re-entrancy attack
                    await _perpetualDb.getCollection<NftizedItem>().InsertOneAsync(/*perpetualSession,*/ new NftizedItem
                    {
                        item = item
                    });

                    var done = await _seasonDb.getCollection<DbGameState>()
                     .UpdateOneAsync(seasonSession, g => g.HeroId == heroId
                         && g.Hero.inventory.Any(i => i.id == item.id),
                         Builders<DbGameState>.Update
                         .PullFilter(i => i.Hero.inventory,
                             Builders<ItemDto>.Filter.Eq(i => i.id, item.id))
                         .Push(i => i.nftClaims, item)
                         );
                }));

            }

            await CreateDCXClaimOrder(gameState, player, seasonSession);
            

            gameState.Hero.AvailableQuestSetToZero();

            await _seasonDb.getCollection<DbGameState>()
                .UpdateOneAsync(seasonSession, g => g.HeroId == heroId,
                Builders<DbGameState>.Update
                .Set(g => g.Hero.extraDailyQuestGiven, gameState.Hero.extraDailyQuestGiven)
                .Set(g => g.Hero.dailyQuestsUsed, gameState.Hero.dailyQuestsUsed));

            await seasonSession.CommitTransactionAsync();
            //await perpetualSession.CommitTransactionAsync();

        }
        
        return await fulfillmentStatus();
    }

    [HttpGet("claimSeasonRewards")]
    public CampingStatus claimSeasonRewards()
    {
        return new CampingStatus
        {

        };

    }

    async Task CreateDCXClaimOrder(DbGameState gameState, PlayerDto player, IClientSessionHandle seasonSession)
    {
        if (gameState.Hero.unsecuredDCXValue.amount > 0 && string.IsNullOrWhiteSpace(gameState.Hero.unsecuredDCXValue.withdrawlOrderId))
        {
            var claimOrder = new ClaimDcxOrder
            {
                priceInDcx = gameState.Hero.unsecuredDCXValue.amount,
                heroId = gameState.HeroId,
                chainId = gameState.Hero.isDefaultChain ?_web3.config.defaultChainId: _web3.config.shadowChain.playChainId
            };


            claimOrder.authorizaton = await _web3.AuthrorizeClaimDcx(
                    player.BlockchainPublicAddress!,
                    claimOrder.priceInDcx,
                    claimOrder.id,
                    claimOrder.chainId
                    );

            //the ClaimOrder saga starts the procesing of the order
            //we want it delayed by a second, while we actually put the claims order in the database
            await _publishEp.Publish(claimOrder, ctx =>
            {
                ctx.AddDelay(TimeSpan.FromSeconds(1));
            });

            await _seasonDb.getCollection<DcxOrder>().OfType<ClaimDcxOrder>().InsertOneAsync(seasonSession, claimOrder);

            //if we fail to update the gamesate, then the claimOrder processing will fail which is
            //what we want to avoid double withdraws

            var done = await _seasonDb.getCollection<DbGameState>()
                .UpdateOneAsync(seasonSession, g => g.HeroId == gameState.HeroId
                    && g.Hero.unsecuredDCXValue.amount == claimOrder.priceInDcx
                    && String.IsNullOrEmpty(g.Hero.unsecuredDCXValue.withdrawlOrderId),
                    Builders<DbGameState>.Update
                    .Set(g => g.Hero.unsecuredDCXValue.withdrawlOrderId,
                    claimOrder.correlationId.ToString()));

        }
    }

}

public class CampingStatus
{
    public SecuredNFTsOrder[] nftOrders { get; set; } = new SecuredNFTsOrder[] { };
    public ClaimDcxOrder[] claimDcxOrders { get; set; } = new ClaimDcxOrder[] { };

}
