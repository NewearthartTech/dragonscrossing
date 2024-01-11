using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Players;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Microsoft.Extensions.Configuration;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using MongoDB.Driver;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using DragonsCrossing.Core.Helper;
using System.Numerics;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Sagas;

namespace DragonsCrossing.Core.Services;

public class BlockchainService : IBlockchainService
{
    readonly ILogger _logger;
    readonly Web3Config _web3Config;
    readonly IPerpetualDbService _perpetualDb;

    public BlockchainService(

        IConfiguration config,
        IPerpetualDbService db,
        ILogger<BlockchainService> logger
        )
    {
        _perpetualDb = db;
        this._logger = logger;
        this._web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
    }

    public Web3Config config { get { return _web3Config; } }


    public  string AuthrorizeMintItem(string walletAddress, int tokenId)
    {
        return Web3Utils.generateSignature(_web3Config,
            new ABIValue("uint256", tokenId),
            new ABIValue("address", walletAddress));
    }

    public async Task<string> AuthrorizeClaimDcx(string walletAddress, decimal price, string orderId, long chainId)
    {
        var amountIWei = await Web3Utils.weiValueForDcxToken(_web3Config!, price, chainId);

        return Web3Utils.generateSignature(_web3Config,
                new ABIValue("uint256", amountIWei),
                new ABIValue("string", orderId),
                new ABIValue("address", walletAddress));

    }

    

    public async Task<DFKPayDetails<T>> GetDFKPayDetails<T>(string txHash) where T : DcxOrder
    {
        var chainId = config.shadowChain.playChainId;

        var txDetails = await GetTxnStatus<cbContract.DCXToken.ContractDefinition.TransferFunction>(txHash, chainId);
        if (null == txDetails)
            throw new ExceptionWithCode("transaction not found");

        DFKDeposit deposit;
        try
        {
            if (txDetails.To.ToLower() != _web3Config.shadowChain.depositWallet.ToLower())
            {
                throw new ExceptionWithCode("deposit not to correct wallet");
            }

            deposit = new DFKDeposit
            {
                depositFrom = txDetails.FromAddress.ToLower(),
                txHash = txHash,
                dcxValue = await Web3Utils.DcxValueFromWei(_web3Config, txDetails.Amount, chainId)
            };

            await _perpetualDb.getCollection<DFKDeposit>().InsertOneAsync(deposit);

        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("DuplicateKey"))
            {
                _logger.LogDebug($"dfk deposit already recorded {txHash}");

                deposit = await _perpetualDb.getCollection<DFKDeposit>().Find(d => d.txHash == txHash).SingleAsync();

                if (!string.IsNullOrWhiteSpace(deposit.usedUpBy))
                {
                    throw new Exception($"txAlreadyUsed :{deposit.usedUpBy}");
                }

            }
            else
            {
                throw;
            }

        }

        var orderCollection = _perpetualDb.getCollection<DcxOrder>().OfType<T>();

        /*
        var Q =  orderCollection.Find(l =>
            l.priceInDcx == deposit.dcxValue && l.forWallet == txDetails.FromAddress.ToLower()
            //&& l.chainId == chainId
            && null == l.fulfillmentTxnHash);

        var g = Q.ToString();


        Console.WriteLine(g);
        */

        return new DFKPayDetails<T>
        {
            deposit = deposit,
            order = await orderCollection.Find(l =>
            l.priceInDcx == deposit.dcxValue && l.forWallet == txDetails.FromAddress.ToLower()
            && l.chainId == chainId
            && null == l.fulfillmentTxnHash
            ).SingleOrDefaultAsync()
        };

        
    }

    public async Task<bool> MintDFKItem(string forAccount, int tokenId)
    {
        var account = new Account(_web3Config.serverPrivateKey, chainId: _web3Config.defaultChainId);
        var web3 = new Web3(account, _web3Config.chainRpc[_web3Config.defaultChainId.ToString()].rpcUrl);

        var contract = new cbContract.DCXItemDFK.DCXItemDFKService(web3, _web3Config.deployedContracts(_web3Config.defaultChainId)!.DcxDfkItemsContract);

        try
        {
            var currentOwner = await contract.OwnerOfQueryAsync(tokenId);

            return false;

            /*
            if (!string.IsNullOrWhiteSpace(currentOwner))
            {
                throw new Exception($"tokenId already minted");
            }
            */
        }catch(Exception ex)
        {
            if(ex.Message.Contains("invalid token ID"))
            {
                _logger.LogDebug("token not yet minted");
            }
            else
            {
                throw;
            }

            
        }


        var tx = await contract.MintRequestAndWaitForReceiptAsync(forAccount,tokenId);

        _logger.LogDebug("MintDFKItem_minted", new { hash = tx.TransactionHash, forAccount, tokenId });

        return true;

    }
    //public async 

    public async Task<int> GetMintedHeroId(byte[] orderHash, long chainId)
    {
        var account = new Account(_web3Config.serverPrivateKey, chainId: chainId);
        var web3 = new Web3(account, _web3Config.chainRpc[chainId.ToString()].rpcUrl);

        var tokenomics = new cbContract.Tokenomics.TokenomicsService(web3, _web3Config.deployedContracts(chainId)!.TokenomicsContract);

        var mintedProps = await tokenomics.GetMintedHeroByHashQueryAsync(orderHash);

        var mintedBigId = mintedProps.ReturnValue1.First();

        return ((int)mintedBigId);

    }


    public async Task<DFkHeroWrapper> DFkHerofromDcxId(int dcxId)
    {
        return await _perpetualDb.getCollection<DFkHeroWrapper>().Find(w => w.dcxId == dcxId).SingleAsync();
    }

    public async Task<DFkHeroWrapper> DFkHerofromDfkChainId(long dfkId)
    {

        var wrapper = await _perpetualDb.getCollection<DFkHeroWrapper>().Find(w => w.dfkChainId == dfkId).SingleOrDefaultAsync();

        if(null != wrapper)
        {
            return wrapper;
        }

        _logger.LogDebug("newDKHeroSeen", new { dfkId });

        var dcxId = await _perpetualDb.GetNextSequence<DFkHeroWrapper>();

        await _perpetualDb.getCollection<DFkHeroWrapper>().UpdateOneAsync(
            w => w.dfkChainId == dfkId,
            Builders<DFkHeroWrapper>.Update
            .SetOnInsert(w => w.dfkChainId, dfkId)
            .SetOnInsert(w => w.dcxId, dcxId),
            new UpdateOptions
            {
                IsUpsert = true
            }) ;


        return await DFkHerofromDfkChainId(dfkId);

    }


    public async Task<int[]> OwnedHeros(int[] allHerosToCheck, string walletAddress)
    {
        if (0 == allHerosToCheck.Length)
            return new int[] { };


        var defaultChainHeroIds = allHerosToCheck.Where(id => HeroDto.isDefaultChainFromId(id)).ToArray();
        var dfkChainHeroIds = allHerosToCheck.Where(id => !defaultChainHeroIds.Contains(id)).ToArray();

        var toCheck = new[]
        {
            new
            {
                idsToCheck = defaultChainHeroIds.Select(id=>new
                {
                    idInChain = (long)id,
                    idInDcx = id,
                }).ToArray(),
                chainId = _web3Config.defaultChainId,
                heroContract = _web3Config.deployedContracts(_web3Config.defaultChainId)!.DcxHeroContract
            },
            new
            {
                idsToCheck = await Task.WhenAll( dfkChainHeroIds.Select(async id=>{

                    var wrapper = await DFkHerofromDcxId(id);

                    return new
                    {
                        idInChain =  wrapper.dfkChainId,
                        idInDcx = id,
                    };

                })),
                chainId = _web3Config.shadowChain.heroContractChainId,
                heroContract = _web3Config.shadowChain.heroContractAddress
            }
        };


        var heroIdArray = await Task.WhenAll(toCheck.Select(async g =>
        {
            if (0 == g.idsToCheck.Length)
                return new int[] { };

            // get heroes from blockchain
            var account = new Account(_web3Config.serverPrivateKey, chainId: g.chainId);
            var web3 = new Web3(account, _web3Config.chainRpc[g.chainId.ToString()].rpcUrl);
            var dcxContract = new cbContract.DCXHero.DCXHeroService(web3, g.heroContract);

            var heroIds = (await Task.WhenAll(g.idsToCheck.Select(async aIdToCheck =>
            {
                try
                {
                    var owner = await dcxContract.OwnerOfQueryAsync(aIdToCheck.idInChain);
                    return owner.ToLower() == walletAddress.ToLower() ? aIdToCheck.idInDcx : 0;

                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, $"failed to get owner for heroId, {aIdToCheck}");
                    return 0;
                }
            }))).Where(o => 0 != o).ToList();

            return heroIds.ToArray();
        }));

        

        return heroIdArray.SelectMany(id=>id). ToArray();
    }

    public WebInfo Web3Info()
    {
        var account = new Account(_web3Config.serverPrivateKey);

        return new WebInfo
        {
            ServerAddress = account.Address
        };
    }

    public Task<T?> GetTxnStatus<T>(string txnHash, long chainId) where T : FunctionMessage, new()
    {
        return GetTxnStatusWithRetry<T>(txnHash,chainId,0);
    }


    async Task<T?> GetTxnStatusWithRetry<T>(string txnHash, long chainId, int retryCount) where T: FunctionMessage, new()
    {
        var account = new Account(_web3Config.serverPrivateKey, chainId: chainId);
        var web3 = new Web3(account, _web3Config.chainRpc[chainId.ToString()].rpcUrl);

        var txRcpt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txnHash);
        if (null == txRcpt)
        {
            _logger.LogDebug($"tx {txnHash} is not yet done, retry = {retryCount}");

            if(retryCount > 5)
            {
                _logger.LogError($"tx {txnHash} is not yet done");
                return null;
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                return await GetTxnStatusWithRetry<T>(txnHash, chainId, ++retryCount);
            }
            
        }

        _logger.LogInformation($"tx {txnHash} FOUND");

        //http://playground.nethereum.com/csharp/id/1075
        //https://ethereum.stackexchange.com/questions/99429/decoding-transaction-input-data-in-nethereum
        var tx = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txnHash);

        if (!tx.IsTransactionForFunctionMessage<T>())
        {
            throw new InvalidOperationException($"tx {txnHash} is not a valid deposit operation");
        }

        return new T().DecodeTransaction(tx);

    }


    public async Task<DcxTxConfirmation> getDepositTxnStatus(string txnHash, long chainId)
    {
        var deposit = await GetTxnStatus<cbContract.Tokenomics.ContractDefinition.RegisterForSeasonFunction>(txnHash, chainId);
        if(null == deposit)
        {
            return new DcxTxConfirmation
            {
                txnHash = txnHash,
                isConfirmed = false,
            };
        }

        return new DcxTxConfirmation
        {
            orderId = deposit.OrderId,
            //dcxAmountInEther = Web3.Convert.FromWei(deposit.Amount),
            dcxAmountInEther = await Web3Utils.DcxValueFromWei (_web3Config, deposit.Amount, chainId),
            isConfirmed = true,
            txnHash = txnHash
        };

    }

    public Task<ItemWithOwner> GetItemNFTInfo(int ItemId)
    {
        throw new NotImplementedException();
    }

    public Task burnItem(int ItemId)
    {
        throw new NotImplementedException();
    }

    

    public async Task<bool> DoesPlayerExist(int playerId)
    {
        return false;
        // TODO: Verify player exists on blockchain
        //return await blockchainRepository.DoesPlayerExist(playerId);
    }


}

public class DcxTxConfirmation
{
    public bool isConfirmed { get; set; }
    public decimal dcxAmountInEther { get; set; }
    public string orderId { get; set; } = String.Empty;
    public string txnHash { get; set; } = String.Empty;
}

public class DeployedContractsForChain : Dictionary<string /*chainId*/, DeployedContracts> { }

public class DeployedContracts
{
    /// <summary>
    /// contract address for DCXhero
    /// </summary>
    [Required]
    public string DcxHeroContract { get; set; } = "";

    [Required]
    public string DcxItemsContract { get; set; } = "";

    [Required]
    public string DcxDfkItemsContract { get; set; } = "";

    /// <summary>
    /// contract address for Tokenomics
    /// </summary>
    [Required]
    public string TokenomicsContract { get; set; } = "";

    public string dcxTokenContract { get; set; } = "0x14cea1f2A1D0C197397b2251Fd0B1eE97e27f3e8";

}

public class ChainRPCConfig
{
    public string name { get; set; } = @"polygon-mumbai";

    public string rpcUrl { get; set; } = @"https://rpc.ankr.com/polygon_mumbai";

    public string alchemyKey { get; set; } = @"XXXXXXXXXXXXXXXXX";

}

/// <summary>
/// This class is used to allow Heros from other chains to be used 
/// </summary>
public class ShadowChain
{
    [Required]
    public string heroContractAddress { get; set; } = "0xEb9B61B145D6489Be575D3603F4a704810e143dF"; //DFK mainnet hero

    /// <summary>
    /// The chain ID to observer to shadow NFT
    /// </summary>
    [Required]
    public long heroContractChainId { get; set; } = 53935; //DFK main net chain

    /// <summary>
    /// The chain where tx for the shadow will happen
    /// </summary>
    [Required]
    public long playChainId { get; set; } = 53935; //DFK main net chain
                                                   //335; //DFK test net

    [Required]
    public string depositWallet { get; set; } = "0x85004ce5b8770e04F39df10fb8e0EAb2B69C2EE7"; //dee dev

}

public class ToWatch
{
    public string contractAddress { get; set; } = string.Empty;
    public long chainId { get; set; }
}

public class Web3Config
{
    public long defaultChainId { get; set; } =80001 ;

    public ShadowChain shadowChain { get; set; } = new ShadowChain();

    public ToWatch[] contractsToWatch()
    {
        var ret= new[] {
            new ToWatch{
                chainId = this.defaultChainId,
                contractAddress = deployedContracts(defaultChainId)?.DcxHeroContract??""
            },
            new ToWatch{
                chainId = this.defaultChainId,
                contractAddress = deployedContracts(defaultChainId)?.DcxItemsContract??""
            },
            new ToWatch{
                chainId = this.defaultChainId,
                contractAddress = deployedContracts(defaultChainId)?.DcxDfkItemsContract??""
            }
        };

        return ret.Where(r => !string.IsNullOrWhiteSpace(r.contractAddress)).ToArray();
    }

    public Dictionary<string, ChainRPCConfig> chainRpc { get; set; } = new Dictionary<string, ChainRPCConfig>
    {
        //{80001, new ChainRPCConfig()},
        {"53935", new ChainRPCConfig{
            name = "defi-kingdoms",
            rpcUrl = "https://subnets.avax.network/defi-kingdoms/dfk-chain/rpc"
        } },
        {"335", new ChainRPCConfig{
            name = "defi-kingdoms-testnet",
            rpcUrl = "https://subnets.avax.network/defi-kingdoms/dfk-chain-testnet/rpc"
        } },
    };
    public int reservedHeroCount { get; set; } = 230;

    /// <summary>
    /// in seconds
    /// </summary>
    public int tokenCacheUpdateInveral { get; set; } = 15 * 60;

    /// <summary>
    /// Checks if in developer mode
    /// </summary>
    public bool devMode { get; set; } = false;

    public Dictionary<string, DeployedContractsForChain> contractsForChain { get; set; } = new Dictionary<string, DeployedContractsForChain>();

    /// <summary>
    /// todo: we will need to figure out how to secure this
    /// </summary>
    public string serverPrivateKey { get; set; } = "";

    

    public bool isTestEndPointAvailable { get; set; } = true;

    public string metaDataBaseURI { get; set; } = "http://localhost:8080/api/metadata/";

    public string dcxHeroName { get; set; } = "DCX Hero";
    public string dcxHeroSymbol { get; set; } = "DCX_Hero";

    public string dcxItemName { get; set; } = "DCX Item";
    public string dcxItemSymbol { get; set; } = "DCX_Item";

    public static string latestVersion
    {
        get
        {
            return System.Reflection.Assembly.GetAssembly(typeof(cbContract.Tokenomics.ContractDefinition.TokenomicsDeployment))?
                                .GetName()?.Version?.ToString() ?? "";
        }
    }

    public DeployedContractsForChain? deployedContractsForChain(string version = "latest")
    {
        if ("latest" == version)
        {
            version = Web3Config.latestVersion;
        }

        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentNullException(nameof(version));

        DeployedContractsForChain? contractForVersion;
        if (!contractsForChain.TryGetValue(version, out contractForVersion))
            return null;

        return contractForVersion;

    }

    public DeployedContracts? deployedContracts(long chainId, string version = "latest")
    {

        return deployedContractsForChain(version)?[chainId.ToString()];

    }

}

public class CreateHeroParameters
{
    public HeroMintedParams mintParams { get; set; } = new HeroMintedParams();
    public bool isGenesisHero { get; set; }
}

public class HeroMintedParams
{
    //heroClass
    public string? c { get; set; }

    //heroGender
    public string? g { get; set; }

    public CharacterClassDto? heroClass()
    {
        if (string.IsNullOrWhiteSpace(c))
            return null;

        return Enum.Parse<CharacterClassDto>(c);
    }

    public Gender? gender()
    {
        if (string.IsNullOrWhiteSpace(g))
            return null;

        return Enum.Parse<Gender>(g);
    }
}

public class Web3Utils
{
    static double dcxDecimals = 0;

    static async Task<double> getDcxDecimals(Web3Config config, long chainId)
    {
        if (0 != dcxDecimals)
            return dcxDecimals;

        if (string.IsNullOrWhiteSpace(config?.serverPrivateKey))
        {
            throw new Exception("serverPrivateKey not set");
        }

        var account = new Account(config.serverPrivateKey, chainId: chainId);
        var web3 = new Web3(account, config.chainRpc[chainId.ToString()].rpcUrl);
        var dcxContract = new cbContract.DCXToken.DCXTokenService(web3, config.deployedContracts(chainId)!.dcxTokenContract);


        dcxDecimals = Convert.ToDouble(await dcxContract.DecimalsQueryAsync());


        return dcxDecimals;

    }

    public static async Task<decimal> DcxValueFromWei(Web3Config config, System.Numerics.BigInteger weiValue, long chainId)
    {
        if (string.IsNullOrWhiteSpace(config?.serverPrivateKey))
        {
            throw new Exception("serverPrivateKey not set");
        }

        var account = new Account(config.serverPrivateKey, chainId: chainId);
        var web3 = new Web3(account, config.chainRpc[chainId.ToString()].rpcUrl);
        var dcxContract = new cbContract.DCXToken.DCXTokenService(web3, config.deployedContracts(chainId)!.dcxTokenContract);

        var decimals = await getDcxDecimals(config, chainId);

        var eterVal = Web3.Convert.FromWei(weiValue);

        var dcxValue = eterVal * (decimal)Math.Pow(10, (18 - decimals));

        return dcxValue;

    }

    public static async Task<System.Numerics.BigInteger> weiValueForDcxToken(Web3Config config, decimal dcxValue, long chainId)
    {
        if (string.IsNullOrWhiteSpace(config?.serverPrivateKey))
        {
            throw new Exception("serverPrivateKey not set");
        }

        var account = new Account(config.serverPrivateKey, chainId: chainId);
        var web3 = new Web3(account, config.chainRpc[chainId.ToString()].rpcUrl);
        var dcxContract = new cbContract.DCXToken.DCXTokenService(web3, config.deployedContracts(chainId)!.dcxTokenContract);

        var decimals = Convert.ToDouble(  await dcxContract.DecimalsQueryAsync());

        var weiValue = Web3.Convert.ToWei(dcxValue);

        var toDiv = ((long)Math.Pow(10, (18 - decimals)));

        weiValue = weiValue / toDiv;


        var t = weiValue.ToString();


        return weiValue;

    }


    public static string generateSignature(Web3Config config, params ABIValue[] abiValues)
    {
        if (string.IsNullOrWhiteSpace(config?.serverPrivateKey))
        {
            throw new Exception("serverPrivateKey not set");
        }

        var encoded = new ABIEncode().GetABIEncoded(abiValues);//.ToHex();

        byte[] msgHash = new Sha3Keccack().CalculateHash(encoded);

        var signer1 = new EthereumMessageSigner();

        var signed = signer1.Sign(msgHash, new EthECKey(config.serverPrivateKey));

        return signed;
    }

    public static string extractAddressFromSignature(string originalMessage, string signatureString)
    {
        string msg = "\x19" + "Ethereum Signed Message:\n" + originalMessage.Length + originalMessage;
        byte[] msgHash = new Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes(msg));

        var signature = MessageSigner.ExtractEcdsaSignature(signatureString);

        var key = EthECKey.RecoverFromSignature(signature, msgHash);

        bool isValid = key.Verify(msgHash, signature);

        return key.GetPublicAddress();

    }

    public class ReportData
    {
        public long? costInBuyToken { get; set; }
        //public string tokenId { get; set; }
        public long? quantity { get; set; }

        public string? address { get; set; } = string.Empty;

        public string txHash { get; set; } = string.Empty;
    }

    public class TxReportData
    {
        public long? costInBuyToken { get; set; }
        //public string tokenId { get; set; }
        public long? quantity { get; set; }
        public string? address { get; set; } = string.Empty;

        public string[] tokenIds { get; set; } = new string[] { };

    }


    /*
    public static async Task FindAllMints()
    {
        var inFile = "/Users/dee/dragonCross/mainnet_dump/allNFTMints_1.json";

        var outFile = "/Users/dee/dragonCross/mainnet_dump/allNFTMints_report.json";

        var outFileTx = "/Users/dee/dragonCross/mainnet_dump/allNFTMints_report_tx.json";

        var ourStruct = new Dictionary<string, ReportData>();

        var txRport = new Dictionary<string, TxReportData>();

        if (File.Exists(outFileTx))
        {
            using (StreamReader r = new StreamReader(outFileTx))
            {
                string json = r.ReadToEnd();

                txRport = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(json, txRport);
            }
        }

        if (File.Exists(outFile))
        {
            using (StreamReader r = new StreamReader(outFile))
            {
                string json = r.ReadToEnd();

                ourStruct = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(json, ourStruct);
            }
        }

        using (StreamReader r = new StreamReader(inFile))
        {
            string json = r.ReadToEnd();
            var items = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(json, new
            {
                result = new
                {
                    transfers = new[] { new {
                        hash="",
                        erc721TokenId=(string?)"",
                    } }
                }
            }); ;


            var web3 = new Web3("https://arb-mainnet.g.alchemy.com/v2/XXXXXXXXXXXXXXXXX");


            foreach (var t in items!.result.transfers)
            {
                if(string.IsNullOrEmpty(t.erc721TokenId))
                {
                    continue;
                }

                if (ourStruct!.ContainsKey(t.erc721TokenId))
                {
                    Console.WriteLine($" token id {t.erc721TokenId} done");
                    continue;
                }


                Console.WriteLine($"processing token id {t.erc721TokenId}");

                var txRcpt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(t.hash);
                if (null == txRcpt)
                {
                    //_logger.LogInformation($"tx {txnHash} is not yet done");
                    continue;
                }


                var tx = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(t.hash);

                ourStruct[t.erc721TokenId] = new ReportData
                {
                    txHash = t.hash
                };

                if (tx.IsTransactionForFunctionMessage<cbContract.Tokenomics.ContractDefinition.MintHeroFunction>())
                {
                    var g = new cbContract.Tokenomics.ContractDefinition.MintHeroFunction().DecodeTransaction(tx);
                    //continue;
                    //throw new InvalidOperationException($"tx {t.hash} is not a valid deposit operation");


                    if (txRport.ContainsKey(t.hash))
                    {
                        txRport[t.hash].tokenIds = txRport[t.hash].tokenIds.Concat(new[] { t.erc721TokenId}).ToArray();
                    }
                    else
                    {
                        txRport[t.hash] = new TxReportData
                        {
                            quantity = (long)g.Quantity,
                            costInBuyToken = ((long)g.CostInBuyToken)/1000000,
                            address = g.FromAddress,
                            tokenIds = new[] { t.erc721TokenId }
                        };

                        /*
                        if(txRport[t.hash].quantity > 1)
                        {
                            var k2 = 0;
                        }
                        

                        ourStruct[t.erc721TokenId].costInBuyToken = txRport[t.hash].costInBuyToken;
                        ourStruct[t.erc721TokenId].quantity = txRport[t.hash].quantity;
                        ourStruct[t.erc721TokenId].address = txRport[t.hash].address;
                    }

                    File.WriteAllText(outFileTx, Newtonsoft.Json.JsonConvert.SerializeObject(txRport, Newtonsoft.Json.Formatting.Indented));
                }
                else
                {
                    Console.WriteLine($"hash for {t.erc721TokenId} is not mint");
                }


                File.WriteAllText(outFile, Newtonsoft.Json.JsonConvert.SerializeObject(ourStruct,Newtonsoft.Json.Formatting.Indented));

            }

        }

    }
    */

    public static async Task<Web3Config> deployContracts(Web3Config config, long chainId)
    {
        try
        {

            if (string.IsNullOrWhiteSpace(config?.serverPrivateKey))
            {
                throw new Exception("serverPrivateKey not set");
            }

            var account = new Account(config.serverPrivateKey, chainId: chainId);
            var web3 = new Web3(account, config.chainRpc[chainId.ToString()].rpcUrl);

            var balance = await web3.Eth.GetBalance.SendRequestAsync(account.Address);

            Console.WriteLine($"deploying with account {account.Address}, balance = {Web3.Convert.FromWei(balance)} ");

            var deployedContract = new DeployedContracts();

            cbContract.DCXHero.DCXHeroService heroContract;
            //deployedContract.DcxHeroContract = "0xd96e55e11c651e22b90f3011b8714966ebc6f7a9";
            if (string.IsNullOrWhiteSpace(deployedContract.DcxHeroContract))
            {
                var deployment = new cbContract.DCXHero.ContractDefinition.DCXHeroDeployment
                {
                    Name = config.dcxHeroName,
                    Symbol= config.dcxHeroSymbol,
                    BaseUri = config.metaDataBaseURI + "hero/"
                };

                Console.WriteLine($"deploying dcxHero ...");
                heroContract = await cbContract.DCXHero.DCXHeroService.DeployContractAndGetServiceAsync(
                    web3,
                    deployment);

                deployedContract.DcxHeroContract = heroContract.ContractHandler.ContractAddress;

                Console.WriteLine($"dcxHero deployed at {deployedContract.DcxHeroContract }, updating ...");

                _ = await heroContract.UpdateAuthorizerRequestAndWaitForReceiptAsync(account.Address);

                Console.WriteLine($"updated dcxHero");
            }
            else
            {
                Console.WriteLine($"dcxHero exists attaching to {deployedContract.DcxHeroContract}");
                heroContract = new cbContract.DCXHero.DCXHeroService(web3, deployedContract.DcxHeroContract);
            }

            cbContract.DCXItem.DCXItemService itemsContract;
            //deployedContract.DcxItemsContract = "0x2d7b64ae6750e16d5c55522e7085a93511afa05c";
            if (string.IsNullOrWhiteSpace(deployedContract.DcxItemsContract))
            {
                var deployment = new cbContract.DCXItem.ContractDefinition.DCXItemDeployment
                {
                    Name = config.dcxItemName,
                    Symbol = config.dcxItemSymbol,
                    BaseUri = config.metaDataBaseURI + "item/"
                };

                Console.WriteLine($"deploying dcxItem ...");
                itemsContract = await cbContract.DCXItem.DCXItemService.DeployContractAndGetServiceAsync(
                    web3,
                    deployment);

                deployedContract.DcxItemsContract = itemsContract.ContractHandler.ContractAddress;

                Console.WriteLine($"dcxItem deployed at {deployedContract.DcxItemsContract }");

                //_ = await rcpt.UpdateAuthorizerRequestAndWaitForReceiptAsync(account.Address);

                //_ = await rcpt.UpdateMinterRequestAndWaitForReceiptAsync(deployedContract.TokenomicsContract);

            }
            else
            {
                Console.WriteLine($"dcxItems exists attaching to {deployedContract.DcxItemsContract}");
                itemsContract = new cbContract.DCXItem.DCXItemService(web3, deployedContract.DcxItemsContract);
            }

            if (string.IsNullOrWhiteSpace(deployedContract.TokenomicsContract))
            {
                var deployment = new cbContract.Tokenomics.ContractDefinition.TokenomicsDeployment
                {
                    DcxTokenAddress = deployedContract.dcxTokenContract,
                    DcxHeroAddress = deployedContract.DcxHeroContract,
                    DcxItemAddress = deployedContract.DcxItemsContract,
                    //Payees = new List<string>(new string[] { account.Address }),
                    //Shares = new List<System.Numerics.BigInteger>(new System.Numerics.BigInteger[] { 10 })
                };

                Console.WriteLine($"deploying tokenomics ...");
                var rcpt = await cbContract.Tokenomics.TokenomicsService.DeployContractAndGetServiceAsync(
                    web3,
                    deployment);

                deployedContract.TokenomicsContract = rcpt.ContractHandler.ContractAddress;

                Console.WriteLine($"tokenomics deployed at {deployedContract.TokenomicsContract }, updating ...");

                _ = await rcpt.UpdateAuthorizerRequestAndWaitForReceiptAsync(account.Address);

                _ = await heroContract.UpdateMinterRequestAndWaitForReceiptAsync(deployedContract.TokenomicsContract);

                _ = await itemsContract.UpdateMinterRequestAndWaitForReceiptAsync(deployedContract.TokenomicsContract);


                Console.WriteLine($"updated tokenomics");
            }

            if (!config.contractsForChain.ContainsKey(Web3Config.latestVersion))
            {
                config.contractsForChain.Add(Web3Config.latestVersion, new DeployedContractsForChain());
            }

            config.contractsForChain[Web3Config.latestVersion].Add(chainId.ToString(), deployedContract);

            return config;
        }
        catch (Exception)
        {
            throw;
        }
    }


}

[MongoCollection("dfkHeroWrappers")]
public class DFkHeroWrapper
{
    readonly static long _dfkIdBias = 1000000000000;

    [BsonId]
    /// <summary>
    /// The id in DFK chain
    /// </summary>
    public long dfkChainId { get; set; }

    /// <summary>
    /// The short form DFK Id
    /// </summary>
    public string getDFKShortName()
    {
        //sample dfkId
        //1000000040374
        //2000000218906
        var str = dfkChainId.ToString();

        if (str.Length > 7)
        {
            return str.Substring(str.Length - 7, 7).TrimStart('0');
        }

        return str;

    }

    /// <summary>
    /// The heroId in DCX
    /// </summary>
    public int dcxId { get; set; }

    [MongoIndex]
    public static void CreateIndexes(IMongoCollection<DFkHeroWrapper> collection)
    {
        collection.Indexes.CreateOne(
            new CreateIndexModel<DFkHeroWrapper>(
            new IndexKeysDefinitionBuilder<DFkHeroWrapper>()
                .Ascending(f => f.dcxId)
                , new CreateIndexOptions<DFkHeroWrapper>
                {
                    Unique = true
                }
            ));
    }


    

    
}


