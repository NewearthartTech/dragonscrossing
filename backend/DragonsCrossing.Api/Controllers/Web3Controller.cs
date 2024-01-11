using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Sagas;
using DragonsCrossing.Core.Services;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace DragonsCrossing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Web3Controller : ControllerBase
{

    readonly ILogger _logger;
    readonly Web3Config _web3Config;
    readonly IPerpetualDbService _perpertualDb;
    readonly IPublishEndpoint _publishEp;
    readonly IBlockchainService _blockChain;
    readonly IUpdateNFTOwnerService _updateNFTOwnerService;

    static bool _cacheQTriggered = false;

    public Web3Controller(
            IConfiguration config,
            IUpdateNFTOwnerService updateNFTOwnerService,
            IPerpetualDbService perpertualDb,
            IPublishEndpoint publishEp,
            IBlockchainService blockChain,
            ILogger<Web3Controller> logger
            )
    {
        _blockChain = blockChain;
        _logger = logger;
        _perpertualDb = perpertualDb;
        _web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
        _publishEp = publishEp;
        _updateNFTOwnerService = updateNFTOwnerService;

        //hacky place to do this
        if (!_cacheQTriggered)
        {

            Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation("Triggering cache Queue");

                    foreach (var contract in _web3Config.contractsToWatch())
                    {
                        await _publishEp.Publish(new UpdateNFtOwner
                        {
                            contractAddress = contract.contractAddress,
                            chainId = contract.chainId
                        });

                    }

                    _logger.LogInformation("Cache Queue Triggered");

                }
                catch (Exception ex)
                {
                    _logger.LogCritical("failed to trigger cache Q", ex);
                }
            });

            _cacheQTriggered = true;
        }
        
    }

    /// <summary>
    /// gets the smart contracts in use
    /// </summary>
    /// <returns></returns>
    [HttpGet("gameConfig")]
    public GameConfig getGameConfig()
    {
        _logger.LogDebug("getGameConfig called");

        //_logger.LogDebug($"web3 config is {Newtonsoft.Json.JsonConvert.SerializeObject(_web3Config)}");

        return new GameConfig
        {
            backendContractAddressForChain = _web3Config.deployedContractsForChain()!,
            
            isTestEndPointAvailable = _web3Config.isTestEndPointAvailable,
            defaultChaninId = _web3Config.defaultChainId,
            shadowChain = _web3Config.shadowChain,
        };
    }

    [HttpGet("updateNftOwner/{chainId}/{walletAddress}/{contractAddress}/{tokenId}")]
    public async Task updateNftOwner(long chainId, string walletAddress, string contractAddress, int tokenId)
    {
        await _publishEp.Publish(new UpdateNFtOwner
        {
            contractAddress = contractAddress.ToLower(),
            tokenId = tokenId,
            chainId = chainId
        }) ;
    }

    [HttpGet("ownedItems/{walletAddress}")]
    public async Task<OwnedNft[]> GetOwnedItems(string walletAddress)

    {
        var items = await  _updateNFTOwnerService.GetOwnedNfts(_web3Config.deployedContracts(_web3Config.defaultChainId)!.DcxItemsContract, walletAddress);
        var dfkItems = await _updateNFTOwnerService.GetOwnedNfts(_web3Config.deployedContracts(_web3Config.defaultChainId)!.DcxDfkItemsContract, walletAddress);

        return items.Concat(dfkItems).ToArray();
    }

    [HttpGet("ownedHeros/{walletAddress}")]
    public Task<OwnedNft[]> GetOwnedHeros(string walletAddress)
    {
        return _updateNFTOwnerService.GetOwnedNfts(_web3Config.deployedContracts(_web3Config.defaultChainId)!.DcxHeroContract, walletAddress);
    }

    

    [HttpGet("depositInfo/{chainId}/{txHash}")]
    public async Task<DcxTxConfirmation> GetDepositInfo(long chainId, string txHash)
    {
        return await _blockChain.getDepositTxnStatus(txHash, chainId);
    }

}




public class GameConfig
{
    [Required]
    public DeployedContractsForChain backendContractAddressForChain { get; set; } = new DeployedContractsForChain();


    [Required]
    public bool isTestEndPointAvailable { get; set; } = true;


    [Required]
    public long defaultChaninId { get; set; }


    [Required]
    public ShadowChain shadowChain { get; set; } = new ShadowChain();

}

