using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Core.Sagas;
using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Services;

public class DFKPayDetails<T> where T : DcxOrder
{
    public DFKDeposit deposit { get; set; } = new DFKDeposit();

    public T? order { get; set; }
}


/// <summary>
/// This is only used internally to sync with the blockchain. Shouldn't be used by the API... unless we need it to.
/// </summary>
public interface IBlockchainService
{
    Task<bool> DoesPlayerExist(int playerId);
    

    //Task MintHero(int tokenId, string? recepientWallet = null);

    WebInfo Web3Info();

    Task<int[]> OwnedHeros(int[] herosToCheck, string walletAddress);

    

    Task<DcxTxConfirmation> getDepositTxnStatus(string txnId, long chainId);

    Task<ItemWithOwner> GetItemNFTInfo(int ItemId);

    Task burnItem(int ItemId);

    string AuthrorizeMintItem(string walletAddress, int tokenId);


    Web3Config config { get; }

    Task<DFKPayDetails<T>> GetDFKPayDetails<T>(string txHash) where T : DcxOrder;

    Task<string> AuthrorizeClaimDcx(string walletAddress, decimal price, string orderId, long chainId);

    Task<T?> GetTxnStatus<T>(string txnHash, long chainId) where T : Nethereum.Contracts.FunctionMessage, new();

    Task<int> GetMintedHeroId(byte[] orderHash, long chainId);

    Task<DFkHeroWrapper> DFkHerofromDcxId(int dcxId);

    Task<DFkHeroWrapper> DFkHerofromDfkChainId(long dfkId);

    Task<bool> MintDFKItem(string forAccount, int tokenId);

}

public class WebInfo
{
    public string ServerAddress { get; set; } = "";
}

public class ItemWithOwner
{
    public string owner { get; set; } = String.Empty;
    public ItemDto item { get; set; } = new ItemDto();
}

