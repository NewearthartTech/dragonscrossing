import { Configuration, ClaimDcxApi,  } from "@dcx/dcx-backend";

export class RewardsApi {

  public async getAvailableDCXAuthorization(walletAddress:string) {
    const apiConfig: Configuration = new Configuration({
      basePath: process.env.NEXT_PUBLIC_API || ".",
    });

    const { data } = await new ClaimDcxApi(apiConfig)
      .apiClaimDcxAvailableWalletAddressGet(walletAddress);
    return data;
  }

  public async postItemsCompleted(chainId: number,txHash:string) {
    const apiConfig: Configuration = new Configuration({
      basePath: process.env.NEXT_PUBLIC_API || ".",
    });

    await new ClaimDcxApi(apiConfig)
      .apiClaimDcxClaimItemCompletedChainIdTxHashGet(chainId,txHash);
    
  }

  public async postDcxCompleted(chainId:number, txHash:string) {
    const apiConfig: Configuration = new Configuration({
      basePath: process.env.NEXT_PUBLIC_API || ".",
    });

    await new ClaimDcxApi(apiConfig)
      .apiClaimDcxClaimDCXCompletedChainIdTxHashGet(chainId,txHash);
    
  }

  
}

const rewardsApi = new RewardsApi();
export default rewardsApi;
