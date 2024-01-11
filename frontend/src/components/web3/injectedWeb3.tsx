import { ethers } from "ethers";

export type ChainInfo = {
  priority: number;
  hexChainId: string;
  rpcProvider: string;
  sym: string;
  rate: number;
  name: string;
  addressScan: string;
  txScan: string;
  contracts: {};
  chainId: string;
  transferFees: number;
  defiGraphQlUrl?: string;
};

export type ChainConfigs = {
  chainInfo: { [chainId: string]: ChainInfo };
};

export interface ICbCalls {
  connect: (chainConfig: ChainConfigs, chainId?: string) => Promise<ethers.providers.Web3Provider>;
}

// docs ate https://docs.metamask.io/guide/rpc-api.html#permissions

export class InjectedWeb3 implements ICbCalls {
  readonly injected: any = undefined;

  constructor() {
    if (typeof window !== "undefined") {
      this.injected = (window as any)?.ethereum;
    }

    if (!this.injected) {
      throw new Error("no injected provider found");
    }
  }

  connect = async (chainConfig: ChainConfigs, chainId?: string) => {
    if (!!chainId) {
      await this.ensureCorrectChain(chainConfig, chainId);
    }

    const accounts: string[] = await this.injected.request({
      method: "eth_requestAccounts",
    });

    console.log(`injected : provider connected :${accounts[0]}`);

    return new ethers.providers.Web3Provider(this.injected);
  };

  private ensureCorrectChain = async (chainConfig: ChainConfigs, chainId: string) => {
    const chainInfo = chainConfig.chainInfo[chainId];

    try {
      console.log(`current chain id ${this.injected.networkVersion}`);

      if (this.injected.networkVersion == chainId) {
        console.log(`current chain id ${chainId} is correct`);
        return;
      }


      await this.injected.request({
        method: "wallet_switchEthereumChain",
        params: [{ chainId: chainInfo?.hexChainId }],
      });

      console.log(`switched to chain id ${this.injected.networkVersion}`);
    } catch (switchError: any) {
      const j = switchError.code;

      // This error code indicates that the chain has not been added to MetaMask.
      if (switchError.code === 4902) {
        try {
          if (!chainInfo?.rpcProvider) throw new Error(`no rpc defined for chainId ${chainId}`);

          await this.injected.request({
            method: "wallet_addEthereumChain",
            params: [
              {
                chainId: chainInfo?.hexChainId,
                chainName: chainInfo.name,

                nativeCurrency:{
                  decimals:18,
                  name: chainInfo.sym,
                  symbol: chainInfo.sym,
                },

                rpcUrls: [chainInfo?.rpcProvider],
              },
            ],
          });

          console.log(`added and switched to chain id ${this.injected.networkVersion}`);

          throw new Error("We added the network to your wallet. Please try your operations again");
        } catch (addError: any) {
          console.error(`failed to add network : ${addError?.message}`);
          throw new Error("failed to switch network. Please switch manually and try again");
        }
      }

      console.error(`failed to switch network : ${switchError}`);

      throw new Error("failed to switch network. Please switch manually and try again");
    }
  };
}
