using DragonsCrossing.Api.Utilities;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.GameStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using System.ComponentModel.DataAnnotations;
using DragonsCrossing.Core.Services;
using Nethereum.ABI;
using Nethereum.Web3;
using Nethereum.Util;
using System.Text;
using DragonsCrossing.Core.Sagas;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class HeroesController : ControllerBase
{
    readonly IHeroesService _heroesService;
    readonly ISeasonsDbService _seasonsDb;
    readonly ILogger _logger;
    protected readonly Web3Config _web3Config;
    readonly IPerpetualDbService _perpetualDb;
    readonly IUpdateNFTOwnerService _updateNFTOwnerService;
    readonly IBlockchainService _blockchainService;

    public HeroesController(IHeroesService heroesRepository,
        ISeasonsDbService db,
        IPerpetualDbService perpetualDb,
        IBlockchainService blockchainService,
        IUpdateNFTOwnerService updateNFTOwnerService,
        IConfiguration config,
        ILogger<HeroesController> logger)
    {
        this._heroesService = heroesRepository;
        _seasonsDb = db;
        _blockchainService = blockchainService;
        _logger = logger;
        _perpetualDb = perpetualDb;
        _updateNFTOwnerService = updateNFTOwnerService;
        _web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
    }

    /// <summary>
    /// Used to get heros owned by a player
    /// </summary>
    /// <param name="ownedToken">, seperated list of HeroNFTs from blockchain</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("Mine")]
    public async Task<HeroDto[]> Mine(string? ownedShadowTokenIds)
    {

        var userId = this.GetUserId()!;

        var player = await _perpetualDb.getCollection<PlayerDto>().Find(p => p.Id == userId).SingleAsync();
        if (string.IsNullOrWhiteSpace(player.BlockchainPublicAddress))
            throw new Exception("player walletr address is null");


        var owned = await _updateNFTOwnerService.GetOwnedNfts(_web3Config.deployedContracts(_web3Config.defaultChainId)!.DcxHeroContract, player.BlockchainPublicAddress);

        var ownedHeroIdsFromFrontend = owned.Select(o => o.tokenId).ToArray();

        var myHeros = ownedHeroIdsFromFrontend.Length > 0 ?
            await Task.WhenAll(ownedHeroIdsFromFrontend.Select(heroId => this.getHeroById(heroId)))
            : new HeroDto[] {};


        var ownedShadowHeroIdsFromFrontend = string.IsNullOrWhiteSpace(ownedShadowTokenIds) ? new long[] { } : ExtentionMethods.ParseColonSeperatedIds(ownedShadowTokenIds);

        _logger.LogDebug($"found {ownedShadowHeroIdsFromFrontend.Length} shadow NFTs");

        var ownedShadowHeroIdsConvertedFromFrontend =await Task.WhenAll( ownedShadowHeroIdsFromFrontend
            .Select(async id => await _blockchainService.DFkHerofromDfkChainId(id))
            );


        var shadowDFKherosDcxIds = ownedShadowHeroIdsConvertedFromFrontend.Select(s => s.dcxId).ToArray();

        var foundShadowDFKherosDcxIds = (await _perpetualDb.getCollection<HeroDto>().Find(h => shadowDFKherosDcxIds.Contains( h.id ))
           .Project(h => h.id)
            .ToListAsync()).ToArray();

        _logger.LogDebug($"shadowClountCOnverted count {foundShadowDFKherosDcxIds.Length}");

        if(foundShadowDFKherosDcxIds.Length > 0)
        {
            myHeros = myHeros.Concat(await Task.WhenAll(foundShadowDFKherosDcxIds.Select(heroId => this.getHeroById(heroId)))).ToArray();
        }

        myHeros = myHeros.Concat(ownedShadowHeroIdsConvertedFromFrontend
            .Where(id=> !foundShadowDFKherosDcxIds.Contains(id.dcxId))
            .Select(id => new HeroDto
        {
            id = id.dcxId,
            name = $"DFK - {id.getDFKShortName()}",
            isLoanedHero = new LoanedHero
            {
                loanerType = LoanerHeroType.ClaimDFK
            }
        })).ToArray();


        var loanedHerosIds = (await _perpetualDb.getCollection<HeroDto>().Find(h => h.isLoanedHero!.loanedToUserId == userId)
           .Project(h=>h.id)
            .ToListAsync()).ToArray();

        if(loanedHerosIds.Length > 0)
        {
            var convertedLoanHeros = loanedHerosIds.Where(heroId => ownedHeroIdsFromFrontend.Contains(heroId)).ToArray();

            if(convertedLoanHeros.Length > 0)
            {
                _logger.LogDebug($"found {convertedLoanHeros.Length} converted heros ");
                loanedHerosIds = loanedHerosIds.Where(heroId => !convertedLoanHeros.Contains(heroId)).ToArray();
            }


            myHeros = myHeros.Concat(await Task.WhenAll(loanedHerosIds.Select(heroId => this.getHeroById(heroId)))).ToArray();
        }

        if(0 == myHeros.Length)
        {
            myHeros = new[] {
                new HeroDto
                {
                    id = HeroDto.LOANERHEROID,
                    name = "Loan a demo Hero",
                }
            };
        }
        

        var found = await _perpetualDb.getCollection<RewardsBase>()
        .Find(r =>
            r.address == player.BlockchainPublicAddress.ToLower() && string.IsNullOrEmpty(r.claimedHash)
        ).FirstOrDefaultAsync();

            return myHeros.Select(h =>
        {
            h.dcxRewards = null != found ? 1 : 0;
            return h;
        }).ToArray();
    }

    [Authorize(Policy = "HeroSelected")]
    [HttpGet("selectedHero")]
    public async Task<HeroDto> GetSelectedHero()
    {
        var heroId = this.GetHeroId();

        if (heroId <= 0)
            throw new ArgumentOutOfRangeException(nameof(heroId));
        return await this.getHeroById(heroId);
    }

    
    [HttpGet("{heroId}")]
    public async Task<HeroDto> getHeroById(int heroId)
    {
        if (heroId <= 0)
            throw new ArgumentOutOfRangeException(nameof(heroId));
        return await _heroesService.GetHero(heroId);
    }

    [Authorize(Policy = "SeasonJoined")]
    [HttpGet("CalculatedStats/{heroId}")]
    public async Task<CalculatedCharacterStats> GetCalculatedStates(int heroId)
    {
        if (heroId <= 0)
            throw new ArgumentOutOfRangeException(nameof(heroId));

        var collection = _seasonsDb.getCollection<NewCombatLogic.DbGameState>();
        var gameState = await collection.Find(c => c.HeroId == heroId).SingleAsync();

        var encounters = gameState.CurrentEncounters?.Where(e => e.type == nameof(CombatEncounter)).Select(e => (CombatEncounter)e).ToArray();

        var combat = encounters?.Where(e => e.Monster != null).FirstOrDefault();


        return gameState.Hero.GenerateCalculatedCharacterStats(gameState);
    }
    

}

