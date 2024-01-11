import { useEffect, useState, useRef } from "react";
import { useMemo } from "react";
import { ethers } from "ethers";
import { ChainConfigs, ICbCalls, InjectedWeb3 } from "./injectedWeb3";
import { ContractCalls, IContractCalls, _USE_DEFAULT_CHAIN, _USE_SHADOW_CHAIN } from "./contractCalls";
import constate from "constate";
import { GameConfig, Web3Api } from "@dcx/dcx-backend";
import { useAppDispatch } from "@/state-mgmt/store/hooks";
import { setIsTestEndpointsAvailable } from "@/state-mgmt/testing/testingSlice";

let _gameConfig: GameConfig | undefined;

export const [Web3Provider, useConnectCalls] = constate(useWeb3, (v) => v.connectCtx);

function useWeb3() {
  const dispatch = useAppDispatch();

  const [chainConfigs, setChainConfigs] = useState<ChainConfigs>();
  const chainConfigsRef = useRef<ChainConfigs>();

  const injectedProvider = useMemo(() => {
    return typeof window !== "undefined" && (window as any)?.ethereum
      ? new ethers.providers.Web3Provider((window as any).ethereum)
      : undefined;
  }, []);

  const getChainConfigs = async () => {
    if (!!chainConfigsRef.current) {
      return chainConfigsRef.current;
    }

    /* TODO: load chain config from server
        chainConfigsRef.current = await fetchJsonAsync<chainConfigsModel>(
            fetch(`${connectTo.dataSvr}/api/nft/chainConfigs`));
        */

    chainConfigsRef.current = {
      chainInfo: {
        "5": {
          priority: 1,
          hexChainId: "0x5",
          rpcProvider:
            process.env.NEXT_PUBLIC_GOERLI_RPC_URL ||
            "https://eth-goerli.alchemyapi.io/v2/wUa6CvTsOs5kmoeByPGofX8aa3xmiV-a",
          sym: "RinkEth",
          rate: 0,
          name: "Goerli",
          addressScan: "",
          txScan: "",
          contracts: {},
          chainId: "5",
          transferFees: 0,
        },
        "80001": {
          priority: 1,
          hexChainId: "0x13881",
          rpcProvider: process.env.NEXT_PUBLIC_MUMBAI_RPC_URL || "https://rpc.ankr.com/polygon_mumbai",
          sym: "matic",
          rate: 0,
          name: "polygon_mumbai",
          addressScan: "",
          txScan: "",
          contracts: {},
          chainId: "80001",
          transferFees: 0,
        },
        "42161": {
          priority: 1,
          hexChainId: "0xa4b1",
          rpcProvider: process.env.NEXT_PUBLIC_ARB_RPC_URL || "https://rpc.ankr.com/arbitrum",
          sym: "ETH",
          rate: 0,
          name: "arb-mainnet",
          addressScan: "",
          txScan: "",
          contracts: {},
          chainId: "42161",
          transferFees: 0,
        },
        "53935":{
          priority: 1,
          hexChainId: "0xD2AF",
          rpcProvider: "https://subnets.avax.network/defi-kingdoms/dfk-chain/rpc",
          sym: "JEWEL",
          rate: 0,
          name: "defi-kingdoms",
          addressScan: "",
          txScan: "",
          contracts: {},
          chainId: "53935",
          transferFees: 0,
          defiGraphQlUrl:"https://defi-kingdoms-community-api-gateway-co06z8vi.uc.gateway.dev/graphql"
        },
        "335": {
          priority: 1,
          hexChainId: "0x14F",
          rpcProvider: "https://subnets.avax.network/defi-kingdoms/dfk-chain-testnet/rpc",
          sym: "JEWEL",
          rate: 0,
          name: "defi-kingdoms-testnet",
          addressScan: "",
          txScan: "",
          contracts: {},
          chainId: "335",
          transferFees: 0,
        },
      },
    };

    setChainConfigs(chainConfigsRef.current);

    return chainConfigsRef.current;
  };

  const ensureGameConfig = async () => {
    if (!_gameConfig) {
      const api = new Web3Api(undefined, process.env.NEXT_PUBLIC_API || ".");
      const { data: gameConfig } = await api.apiWeb3GameConfigGet();
      _gameConfig = gameConfig;
      dispatch(setIsTestEndpointsAvailable(gameConfig.isTestEndPointAvailable));
    }
    return _gameConfig;
  };

  const injectedAvailable = /*Platform.OS === 'web' &&*/ typeof window !== "undefined" && !!(window as any)?.ethereum;

  const readOnlyWeb3 = async (chainId?: string) => {
    const chainConfigs = await getChainConfigs();
    if (!chainConfigs.chainInfo[chainId || "4"]?.rpcProvider) throw new Error(`chain Id ${chainId} not supported`);

    return new ethers.providers.Web3Provider({
      host: chainConfigs.chainInfo[chainId || "4"]?.rpcProvider,
    });
  };

  const connect = async (chainId:string|number|undefined) => {
    try {
      console.log(`web3 connect called ${chainId}`);

      const gameConfig = await ensureGameConfig();


      let chainIdStr:string|undefined = undefined;
      if(_USE_DEFAULT_CHAIN === chainId){
        chainIdStr = gameConfig.defaultChaninId.toString();
      }else if(_USE_SHADOW_CHAIN === chainId){
        chainIdStr = gameConfig.shadowChain.playChainId.toString();
      }
      else if(chainId){
        chainIdStr = chainId.toString()
      }

      const chainConfigs = await getChainConfigs();

      let caller: ICbCalls | undefined = undefined;

      if (true /*Platform.OS === 'web'*/) {
        console.log("web3 : connecting for web platform");

        if ((window as any)?.ethereum) {
          console.log("web3 : connecting using injected");
          caller = new InjectedWeb3();
        } else {
          console.log("web3 : No injected Found");
        }
      }

      if (!caller) {
        throw new Error("platform not supported");
        /*caller = new Dummy();*/
      }


      const web3 = await caller.connect(chainConfigs ,chainIdStr);

      /*
        const t = web3.getSigner().getAddress();

        const account = (await web3.getSigner())[0];

        if (account != web3AccountRef.current) {
            web3AccountRef.current = account;
            setWeb3Account(web3AccountRef.current);
        }
        */

      return new ContractCalls(web3, chainConfigs, gameConfig) as IContractCalls;
    } catch (error: any) {
      console.error(`failed to connect to web3 :${error}`);
      throw error;
    }
  };

  const connectCtx = useMemo(
    () => ({
      connect,
      readOnlyWeb3,
      //    getchainConfigs,
      injectedAvailable,
      //    setWeb3Account
    }),
    []
  );

  return { connectCtx /*, web3Account, chainConfigs*/ };
}
