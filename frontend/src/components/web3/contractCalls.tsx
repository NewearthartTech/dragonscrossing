import { BigNumber, ethers } from "ethers";
import { ChainConfigs } from "./injectedWeb3";
import {
  DCXToken__factory,
  DCXHero__factory,
  Tokenomics__factory,
  DCXItem__factory,
} from "@dcx/dcx-contracts";
import {
  ClaimDcxOrder,
  IdentifySkillOrder,
  LearnSkillOrder,
  SecuredNFTsOrder,
  SummonHeroOrder,
  GameConfig,
  HeroMintOrder,
  HeroMintOrderReq,
  Web3Api,
  Configuration,
  NftActionOrder,
  SignUpToSeasonOrder,
} from "@dcx/dcx-backend";
import { MintResponse } from "@/state-mgmt/mint/mintTypes";

import { GraphQLClient, gql } from "graphql-request";

export const _USE_DEFAULT_CHAIN = 0;
export const _USE_SHADOW_CHAIN = -1;

export interface IContractCalls {
  //web3: ethers.providers.Web3Provider;
  signMsg: (message: string) => Promise<string>;
  getAddress: () => Promise<string>;
  getDcxBalance: () => Promise<string>;
  levelUp: (props: { amountInDcx: number; orderId: string }) => Promise<string>;
  registerForSeason: (props: SignUpToSeasonOrder) => Promise<string>;
  mint: (
    order: HeroMintOrder,
    req: HeroMintOrderReq,
    txGenerated: (txHash: string) => void
  ) => Promise<MintResponse>;

  claimDcx: ({
    authorizaton,
    priceInDcx,
    id,
  }: {
    authorizaton: string;
    priceInDcx: number;
    id: string;
  }) => Promise<string>;
  claimItems: (props: {
    authorizaton: string;
    itemNftId: number;
    chainId: number
  }) => Promise<string>;
  summonHero: (
    summonHeroOrder: SummonHeroOrder,
    status: (message: string) => void,
    heroMinted: (heroId: number) => void
  ) => Promise<string>;
  getNfts: (
    type: "heroes" | "items",
    tokenIdsToUpdate?: Array<number>
  ) => Promise<Array<string>>;
  identifySkill: (
    identifySkillOrder: IdentifySkillOrder,
    status: (message: string) => void
  ) => Promise<string>;
  learnSkill: (
    identifySkillOrder: IdentifySkillOrder,
    status: (message: string) => void
  ) => Promise<string>;
  actionNftUse: (
    nftActionOrder: NftActionOrder,
    status: (message: string) => void
  ) => Promise<string>;
  getMintedHeroesSupply: () => Promise<number>;
}

// let _isMoralisInitialized = false;

export class ContractCalls implements IContractCalls {
  public readonly web3: ethers.providers.Web3Provider;
  public readonly chainConfig: ChainConfigs;
  readonly gameConfig: GameConfig;

  constructor(
    web3: ethers.providers.Web3Provider,
    chainConfig: ChainConfigs,
    gameConfig: GameConfig
  ) {
    this.web3 = web3;
    this.chainConfig = chainConfig;
    this.gameConfig = gameConfig;
  }

  getDcxBalance = async () => {
    try {
      const { dcxTokenContract } =
        this.gameConfig.backendContractAddressForChain[
          this.gameConfig.defaultChaninId
        ];
      if (!dcxTokenContract) throw new Error("Vault Address not defined");

      const me = this.web3.getSigner();

      const token = DCXToken__factory.connect(dcxTokenContract, me);

      const myBalance = await token.balanceOf(await me.getAddress());
      const decimals = await token.decimals();
      const balanceEth = myBalance.div(10 ** decimals);
      return balanceEth.toString();
    } catch (error: any) {
      console.error(`failed to get token balance ${error}`);
      return "";
    }
  };

  levelUp = async ({}: { amountInDcx: number; orderId: string }) => {
    /*
    const { tokenomicsContract } = this.gameConfig.backendContractAddress;

    if (!tokenomicsContract) throw new Error("Tokenomics Contract Address not defined");

    console.log(`using tokenomics address ${tokenomicsContract}`);
    const vault = Tokenomics__factory.connect(tokenomicsContract, this.web3.getSigner());

    const { dcxTokenContract } = this.gameConfig;

    const amountInWei = this.ensureAnyTokenAllowance(amountInDcx, tokenomicsContract, dcxTokenContract);

    console.log("contractCalls: submitting level up transaction");
    const tx = await vault.(amountInWei, orderId, 1);
    console.log(`contractCalls: waiting for level up tx ${tx.hash}`);
    tx.wait();
    
    console.log(`contractCalls: submitted level up tx ${tx.hash}`);
    return tx.hash;
    */

    return "";
  };

  protected ensureAnyTokenAllowance = async (
    amountInDcx: number,
    spenderAddress: string,
    tokenAddress: string
  ) => {
    console.log(`using token address ${tokenAddress}`);
    if (!tokenAddress) throw new Error("dcxTokenContract not defined");

    const me = this.web3.getSigner();

    const token = DCXToken__factory.connect(tokenAddress, me);
    const decimals = await token.decimals();

    const amountInWei = ethers.utils
      .parseEther(amountInDcx.toString())
      .div(10 ** (18 - decimals));

    if (!spenderAddress) {
      console.debug("ensureAnyTokenAllowance: no spender");
      return amountInWei;
    }

    let allowance = ethers.utils.parseEther("0");

    try {
      const myAddress = await me.getAddress();
      console.log(
        `getting allowance for me ${myAddress}, spenderAddress :${spenderAddress}`
      );
      allowance = await token.allowance(myAddress, spenderAddress);

      console.log("got allowance");
    } catch (error) {
      console.error(`failed to get allowance ${error}`);
    }

    if (allowance.lt(amountInWei)) {
      console.log("contractCalls: increasing allowance");

      const allowanceTx = await token.increaseAllowance(
        spenderAddress,
        amountInWei.sub(allowance)
      );

      console.log(`increasing allowance with tx ${allowanceTx.hash}`);

      await allowanceTx.wait();
    }

    console.log(`allowance adjusted for spenderAddress :${spenderAddress}`);

    return amountInWei;
  };

  identifySkill = async (
    identifySkillOrder: IdentifySkillOrder,
    status: (message: string) => void
  ) => {
    
    try {

      if(identifySkillOrder.chainId === this.gameConfig.shadowChain.playChainId){
        console.log("pay dfk")
        return this.payDfk(identifySkillOrder.priceInDcx);
      }

      const { tokenomicsContract, dcxItemsContract, dcxTokenContract } =
        this.gameConfig.backendContractAddressForChain[
          identifySkillOrder.chainId
        ];

        const amountInWei = this.ensureAnyTokenAllowance(
          identifySkillOrder.priceInDcx,
          tokenomicsContract,
          dcxTokenContract!
        );


      if (
        !identifySkillOrder.nftTokenId ||
        !identifySkillOrder.newItemTokenId
      ) {
        throw new Error(
          `Missing NFT Token Id or new NFT Token Id for order ${identifySkillOrder.id}`
        );
      }
      if (!tokenomicsContract) {
        throw new Error("Missing Tokenomics Address");
      }
      if (!dcxItemsContract) {
        throw new Error("Missing DCX Item Address");
      }
      console.log(
        `Identifying skill using old token Id ${identifySkillOrder.nftTokenId} and new token Id ${identifySkillOrder.newItemTokenId}`
      );
      status(
        `Identifying skill using token ID: ${identifySkillOrder.nftTokenId}`
      );

      const me = this.web3.getSigner();
      const tokenomics = Tokenomics__factory.connect(tokenomicsContract!, me);

      let isItemApproved = false;
      const dcxItemContract = DCXItem__factory.connect(dcxItemsContract!, me);
      if (
        await dcxItemContract.isApprovedForAll(
          me.getAddress(),
          tokenomicsContract
        )
      ) {
        isItemApproved = true;
      }
      if (!isItemApproved) {
        const currentApproved = await dcxItemContract.getApproved(
          identifySkillOrder.nftTokenId
        );
        if (
          currentApproved.toLowerCase() === tokenomicsContract.toLowerCase()
        ) {
          isItemApproved = true;
        }
      }
      if (!isItemApproved) {
        const tx = await dcxItemContract.approve(
          tokenomicsContract!,
          identifySkillOrder.nftTokenId
        );
        console.log("Transaction Approval Hash", tx.hash);
        await tx.wait();
        console.debug("Approval Wait Completed");
        isItemApproved = true;
      }
      if (!isItemApproved) {
        throw new Error("DID NOT GET APPROVAL FOR THIS TRANSACTION");
      }


      status(
        `Submitting identify skill transaction for token ID: ${identifySkillOrder.nftTokenId}`
      );

      console.debug("Approval completed");

      const tx = await tokenomics.exchangeItem(
        identifySkillOrder.newItemTokenId,
        identifySkillOrder.nftTokenId,
        amountInWei,
        identifySkillOrder.authorization
      );
      console.log("Identifying skill transaction hash:", tx.hash);

      status(`Waiting for identify skill completion. TX hash: ${tx.hash}`);

      await tx.wait();
      console.debug("Identify Skill Wait Completed");
      status(`Skill successfully identifed! TX hash: ${tx.hash}`);
      return tx.hash;
    } catch (err: any) {
      status(`Identify skill failed! Message: ${err.message}`);
      throw err;
    }
  };

  actionNftUse = async (
    nftActionOrder: NftActionOrder,
    status: (message: string) => void
  ) => {
    try {
      const { tokenomicsContract, dcxItemsContract, dcxTokenContract } =
        this.gameConfig.backendContractAddressForChain[nftActionOrder.chainId];

      if (!nftActionOrder.nftTokenId) {
        throw new Error(`Missing NFT Token Id for order ${nftActionOrder.id}`);
      }
      if (!tokenomicsContract) {
        throw new Error("Missing Tokenomics Address");
      }
      if (!dcxItemsContract) {
        throw new Error("Missing DCX Item Address");
      }
      if (!dcxTokenContract) {
        throw new Error("Missing dcxTokenContract");
      }

      console.log(`Using NFT with token Id ${nftActionOrder.nftTokenId}`);
      status(`Using NFT with token ID: ${nftActionOrder.nftTokenId}`);
      const me = this.web3.getSigner();
      const tokenomics = Tokenomics__factory.connect(tokenomicsContract!, me);

      let isItemApproved = false;
      const dcxItemContract = DCXItem__factory.connect(dcxItemsContract!, me);
      if (
        await dcxItemContract.isApprovedForAll(
          me.getAddress(),
          tokenomicsContract
        )
      ) {
        isItemApproved = true;
      }
      if (!isItemApproved) {
        const currentApproved = await dcxItemContract.getApproved(
          nftActionOrder.nftTokenId
        );
        if (
          currentApproved.toLowerCase() === tokenomicsContract.toLowerCase()
        ) {
          isItemApproved = true;
        }
      }
      if (!isItemApproved) {
        const tx = await dcxItemContract.approve(
          tokenomicsContract!,
          nftActionOrder.nftTokenId
        );
        console.log("Transaction Approval Hash", tx.hash);
        await tx.wait();
        console.debug("Approval Wait Completed");
        isItemApproved = true;
      }
      if (!isItemApproved) {
        throw new Error("DID NOT GET APPROVAL FOR THIS TRANSACTION");
      }

      // TODO: This section can probably be removed if priceInDcx is 0
      status(`Checking token allowance`);
      const amountInWei = this.ensureAnyTokenAllowance(
        nftActionOrder.priceInDcx,
        tokenomicsContract,
        dcxTokenContract
      );
      console.debug("Approval completed");

      status(
        `Submitting Using NFT transaction for token ID: ${nftActionOrder.nftTokenId}`
      );

      const tx = await tokenomics.exchangeItem(
        0,
        nftActionOrder.nftTokenId,
        amountInWei,
        nftActionOrder.authorization
      );
      console.log("Learning Item transaction hash:", tx.hash);

      status(`Waiting for Using NFT completion. TX hash: ${tx.hash}`);

      await tx.wait();
      console.debug("Learn Skill Wait Completed");
      status(`NFT consumed! TX hash: ${tx.hash}`);
      return tx.hash;
    } catch (err: any) {
      status(`failed! Message: ${err.message}`);
      throw err;
    }
  };

  learnSkill = async (
    learnSkillOrder: LearnSkillOrder,
    status: (message: string) => void
  ) => {
    try {
      const { tokenomicsContract, dcxItemsContract, dcxTokenContract } =
        this.gameConfig.backendContractAddressForChain[learnSkillOrder.chainId];

      if (!learnSkillOrder.nftTokenId) {
        throw new Error(`Missing NFT Token Id for order ${learnSkillOrder.id}`);
      }
      if (!tokenomicsContract) {
        throw new Error("Missing Tokenomics Address");
      }
      if (!dcxItemsContract) {
        throw new Error("Missing DCX Item Address");
      }
      if (!dcxTokenContract) {
        throw new Error("Missing dcxTokenContract");
      }
      console.log(`Learning skill with token Id ${learnSkillOrder.nftTokenId}`);
      status(`Learning skill with token ID: ${learnSkillOrder.nftTokenId}`);
      const me = this.web3.getSigner();
      const tokenomics = Tokenomics__factory.connect(tokenomicsContract!, me);

      let isItemApproved = false;
      const dcxItemContract = DCXItem__factory.connect(dcxItemsContract!, me);
      if (
        await dcxItemContract.isApprovedForAll(
          me.getAddress(),
          tokenomicsContract
        )
      ) {
        isItemApproved = true;
      }
      if (!isItemApproved) {
        const currentApproved = await dcxItemContract.getApproved(
          learnSkillOrder.nftTokenId
        );
        if (
          currentApproved.toLowerCase() === tokenomicsContract.toLowerCase()
        ) {
          isItemApproved = true;
        }
      }
      if (!isItemApproved) {
        const tx = await dcxItemContract.approve(
          tokenomicsContract!,
          learnSkillOrder.nftTokenId
        );
        console.log("Transaction Approval Hash", tx.hash);
        await tx.wait();
        console.debug("Approval Wait Completed");
        isItemApproved = true;
      }
      if (!isItemApproved) {
        throw new Error("DID NOT GET APPROVAL FOR THIS TRANSACTION");
      }

      // TODO: This section can probably be removed if priceInDcx is 0

      status(`Checking token allowance`);
      const amountInWei = this.ensureAnyTokenAllowance(
        learnSkillOrder.priceInDcx,
        tokenomicsContract,
        dcxTokenContract
      );
      console.debug("Approval completed");

      status(
        `Submitting learn skill transaction for token ID: ${learnSkillOrder.nftTokenId}`
      );

      const tx = await tokenomics.exchangeItem(
        0,
        learnSkillOrder.nftTokenId,
        amountInWei,
        learnSkillOrder.authorization
      );
      console.log("Learning Item transaction hash:", tx.hash);

      status(`Waiting for learn skill completion. TX hash: ${tx.hash}`);

      await tx.wait();
      console.debug("Learn Skill Wait Completed");
      status(`Skill successfully learned! TX hash: ${tx.hash}`);
      return tx.hash;
    } catch (err: any) {
      status(`Learn skill failed! Message: ${err.message}`);
      throw err;
    }
  };

  summonHero = async (
    summonHeroOrder: SummonHeroOrder,
    status: (message: string) => void,
    heroMinted: (heroId: number) => void
  ) => {
    try {
      const { tokenomicsContract, dcxItemsContract, dcxTokenContract } =
        this.gameConfig.backendContractAddressForChain[summonHeroOrder.chainId];

      if (!summonHeroOrder.nftTokenId) {
        throw new Error(`Missing NFT Token Id ${summonHeroOrder.id}`);
      }
      if (!tokenomicsContract) {
        throw new Error("Missing Tokenomics Address");
      }
      if (!dcxItemsContract) {
        throw new Error("Missing DCX Item Address");
      }

      console.log(
        `Summoning hero using shard token id ${summonHeroOrder.nftTokenId}`
      );
      status(
        `Summoning hero using shard token ID: ${summonHeroOrder.nftTokenId}`
      );
      const me = this.web3.getSigner();
      const tokenomics = Tokenomics__factory.connect(tokenomicsContract!, me);

      let isItemApproved = false;
      const dcxItemContract = DCXItem__factory.connect(dcxItemsContract!, me);
      if (
        await dcxItemContract.isApprovedForAll(
          me.getAddress(),
          tokenomicsContract
        )
      ) {
        isItemApproved = true;
      }
      if (!isItemApproved) {
        const currentApproved = await dcxItemContract.getApproved(
          summonHeroOrder.nftTokenId
        );
        if (
          currentApproved.toLowerCase() === tokenomicsContract.toLowerCase()
        ) {
          isItemApproved = true;
        }
      }
      if (!isItemApproved) {
        const tx = await dcxItemContract.approve(
          tokenomicsContract!,
          summonHeroOrder.nftTokenId
        );
        console.log("Transaction Approval Hash", tx.hash);
        await tx.wait();
        console.debug("Approval Wait Completed");
        isItemApproved = true;
      }
      if (!isItemApproved) {
        throw new Error("DID NOT GET APPROVAL FOR THIS TRANSACTION");
      }

      status(`Checking token allowance`);

      const amountInWei = await this.ensureAnyTokenAllowance(
        summonHeroOrder.priceInDcx,
        tokenomicsContract,
        dcxTokenContract!
      );

      console.debug("Approval Completed");

      const orderHash = ethers.utils.keccak256(
        ethers.utils.toUtf8Bytes(summonHeroOrder.orderHash)
      );

      console.debug(
        `Submitting summon hero transaction with token ID: ${summonHeroOrder.nftTokenId}, transferHero is ${summonHeroOrder.heroIdToTransfer}`
      );

      status(
        `Submitting summon hero transaction with token ID: ${summonHeroOrder.nftTokenId}, transferHero is ${summonHeroOrder.heroIdToTransfer}`
      );

      const tx = await tokenomics.summonHero(
        orderHash,
        summonHeroOrder.nftTokenId,
        amountInWei,
        ethers.utils.toUtf8Bytes(summonHeroOrder.mintProps),
        summonHeroOrder.heroIdToTransfer,
        summonHeroOrder.authorization
      );
      console.log("Summon Hero Transaction Hash:", tx.hash);

      status(`Waiting for summon hero completion. TX hash: ${tx.hash}`);

      await tx.wait();

      //here we get the heroIdCreated
      const [[heroId]] = await tokenomics.getMintedHeroByHash(orderHash);

      console.debug(`Summon Hero Wait Completed and summoned ${heroId}`);

      if (heroId) {
        heroMinted(heroId.toNumber());
      }

      status(`Hero successfully summoned! TX hash: ${tx.hash}`);
      return tx.hash;
    } catch (err: any) {
      status(`Summon hero failed! Message: ${err.message}`);
      throw err;
    }
  };

  claimDcx = async ({
    authorizaton,
    priceInDcx,
    id,
  }: {
    authorizaton: string;
    priceInDcx: number;
    id: string;
  }) => {
    const { tokenomicsContract, dcxTokenContract } =
      this.gameConfig.backendContractAddressForChain[
        this.gameConfig.defaultChaninId
      ];

    const me = this.web3.getSigner();
    const tokenomics = Tokenomics__factory.connect(tokenomicsContract!, me);

    const token = DCXToken__factory.connect(dcxTokenContract!, me);
    const decimals = await token.decimals();

    const amountInWei = ethers.utils
      .parseEther(priceInDcx.toString())
      .div(10 ** (18 - decimals));

    const tx = await tokenomics.claimDCX(amountInWei, id, authorizaton);
    console.log(`Claim DCX order id: ${id} tx hash: ${tx.hash}`);

    return tx.hash;
  };

  claimItems = async ({
    authorizaton,
    itemNftId,
    chainId
  }: {
    authorizaton: string;
    itemNftId: number;
    chainId: number;
  }) => {
    
    const { tokenomicsContract } =
      this.gameConfig.backendContractAddressForChain[
        chainId
      ];

    if(chainId === this.gameConfig.defaultChaninId){
      const me = this.web3.getSigner();
      const tokenomics = Tokenomics__factory.connect(tokenomicsContract!, me);
  
      const tx = await tokenomics.mintItem(itemNftId, authorizaton);
  
      return tx.hash;
  
    }else{
      console.log("on dfk chain");
      return `noClaimsForDFK_${itemNftId}`;
    }

  };

  registerForSeason = async ({
    priceInDcx,
    id: orderId,
    loanerDetails,
    chainId,
  }: SignUpToSeasonOrder) => {
    const { tokenomicsContract, dcxTokenContract } =
      this.gameConfig.backendContractAddressForChain[chainId];

    //if (!tokenomicsContract)
    //   throw new Error("Tokenomics Contract Address not defined");

    console.log(`using tokenomics address ${tokenomicsContract}`);
    const vault =
      (tokenomicsContract &&
        Tokenomics__factory.connect(
          tokenomicsContract,
          this.web3.getSigner()
        )) ||
      undefined;

    const amountInWei =
      priceInDcx === 0
        ? 0
        : await this.ensureAnyTokenAllowance(
            priceInDcx,
            tokenomicsContract,
            dcxTokenContract!
          );

    console.log("contractCalls: submitting register for season transaction");

    if (chainId === this.gameConfig.defaultChaninId) {
      if (!vault) throw new Error("failed to access vault");

      const orderHash = ethers.utils.keccak256(
        ethers.utils.toUtf8Bytes(orderId)
      );

      const tx = loanerDetails
        ? await vault.mintLoanerHero(
            orderHash,
            amountInWei,
            loanerDetails.authorization
          )
        : await vault.registerForSeason(amountInWei, orderId);

      console.log(
        `contractCalls: waiting for register for season tx ${tx.hash}`
      );
      tx.wait();
      console.log(`contractCalls: submitted register for season tx ${tx.hash}`);
      return tx.hash;
    } else {
      console.debug("dfk Hero registration");
      const tokenContract = DCXToken__factory.connect(
        dcxTokenContract!,
        this.web3.getSigner()
      );

      const tx = await tokenContract.transfer(
        this.gameConfig.shadowChain.depositWallet,
        amountInWei
      );

      console.log(
        `contractCalls: waiting for register for season tx ${tx.hash}`
      );
      tx.wait();
      console.log(`contractCalls: submitted register for season tx ${tx.hash}`);
      return tx.hash;
    }
  };

  payDfk = async(priceInDcx:number)=>{

    const { dcxTokenContract } =
    this.gameConfig.backendContractAddressForChain[
      this.gameConfig.shadowChain.playChainId
    ];

    const amountInWei = await this.ensureAnyTokenAllowance(
      priceInDcx,
      "",
      dcxTokenContract!
    );

    const tokenContract = DCXToken__factory.connect(
      dcxTokenContract!,
      this.web3.getSigner()
    );

    const tx = await tokenContract.transfer(
      this.gameConfig.shadowChain.depositWallet,
      amountInWei
    );

    console.log(
      `contractCalls: waiting for register for season tx ${tx.hash}`
    );
    tx.wait();
    console.log(`contractCalls: submitted register for season tx ${tx.hash}`);
    return tx.hash;

  }

  mint = async (
    {
      totalPrice,
      orderHash: orderId,
      mintProps,
      paymentTokenAddress,
      authorization,
    }: HeroMintOrder,
    { quantity }: HeroMintOrderReq,
    txGenerated: (txHash: string) => void
  ) => {
    const { tokenomicsContract } =
      this.gameConfig.backendContractAddressForChain[
        this.gameConfig.defaultChaninId
      ];
    const me = this.web3.getSigner();

    const tokenomics = Tokenomics__factory.connect(tokenomicsContract!, me);

    const amountInWei = await this.ensureAnyTokenAllowance(
      totalPrice,
      tokenomicsContract,
      paymentTokenAddress
    );

    const orderHash = ethers.utils.keccak256(ethers.utils.toUtf8Bytes(orderId));

    const tx = await tokenomics.mintHero(
      orderHash,
      quantity,
      paymentTokenAddress,
      amountInWei,
      ethers.utils.toUtf8Bytes(mintProps),
      authorization
    );

    txGenerated(tx.hash);

    await tx.wait();

    await new Promise((r) => setTimeout(r, 2000));

    const [heroIds] = await tokenomics.getMintedHeroByHash(orderHash);
    const mintedHeroIds = heroIds.map((id) => id.toNumber());

    console.log("Hero Id(s) " + mintedHeroIds + " minted with hash", tx.hash);

    const mintResponse: MintResponse = {
      mintedHeroIds: mintedHeroIds,
      txHash: tx.hash,
    };

    return mintResponse;
  };

  getMintedHeroesSupply = async () => {
    const me = this.web3.getSigner();
    const { dcxHeroContract } =
      this.gameConfig.backendContractAddressForChain[
        this.gameConfig.defaultChaninId
      ];
    const heroFactory = DCXHero__factory.connect(dcxHeroContract, me);
    const supply = await heroFactory.totalSupply();
    return supply.toNumber();
  };

  signMsg = async (message: string) => {
    const signer = this.web3.getSigner();
    return await signer.signMessage(message);
  };

  getAddress = async () => {
    const signer = this.web3.getSigner();
    return await signer.getAddress();
  };

  getNfts = async (
    type: "heroes" | "items",
    tokenIdsToUpdate?: Array<number>
  ) => {
    const apiConfig: Configuration = new Configuration({
      basePath: process.env.NEXT_PUBLIC_API || ".",
    });

    const { dcxHeroContract, dcxItemsContract } =
      this.gameConfig.backendContractAddressForChain[
        this.gameConfig.defaultChaninId
      ];

    const myAddress = await this.getAddress();

    if (tokenIdsToUpdate && tokenIdsToUpdate.length > 0) {
      const updaters = tokenIdsToUpdate.map(async (ti) => {
        try {
          await new Web3Api(
            apiConfig
          ).apiWeb3UpdateNftOwnerChainIdWalletAddressContractAddressTokenIdGet(
            0,
            myAddress,
            type === "heroes" ? dcxHeroContract : dcxItemsContract,
            ti
          );
        } catch (err: any) {
          // throw new Error("Failed to Update NFT Owner for token");
          console.error("Failed to Update NFT Owner for token");
        }
      });
      await Promise.all(updaters);
    }

    await new Promise((r) => setTimeout(r, 1000));

    if (type === "heroes") {
      const query = gql`{
        heroes(where: {owner: "${myAddress}"}) {
          id
        }
      }
      `;

      const client = new GraphQLClient(
        "https://defi-kingdoms-community-api-gateway-co06z8vi.uc.gateway.dev/graphql"
      );
      
      const data: { heroes: { id: string }[] } = await client.request(query);
      
      const ret =  data.heroes.map((h) => h.id).slice(0,20);
      
      return ret;
    } else {
      const response = /*type === "heroes"
    ? await new Web3Api(apiConfig).apiWeb3OwnedHerosWalletAddressGet(await this.getAddress())
    :*/ await new Web3Api(apiConfig).apiWeb3OwnedItemsWalletAddressGet(
        myAddress
      );

      const tokenIds = response.data.map((r) => r.tokenId.toString());
      
      return tokenIds || [];
    }
  };

  getMyItems = async () => {};
}
