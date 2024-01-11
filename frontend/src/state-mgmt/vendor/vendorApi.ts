import axios, { AxiosResponse } from "axios";
import { Observable, of } from "rxjs";
import {
  HerbalistApi,
  HeroDto,
  HeroesApi,
  IdentifySkillApi,
  IdentifySkillOrder,
  ItemDto,
  ItemsApi,
  LearnSkillApi,
  LearnSkillOrder,
  NftActionApi,
  NftActionOrder,
  SummonHeroApi,
  SummonHeroOrder,
} from "@dcx/dcx-backend";
import blacksmithItemsMock from "../../../mocks/v1/vendor/blacksmith-items.get.json";
import summonHeroMock from "../../../mocks/v1/vendor/summon-hero.get.json";
import { apiConfig } from "@/components/hoc/verification";

export class VendorApi {
  public async getBlacksmithItems() {
    const { data: blacksmithItems } = await new ItemsApi(apiConfig).apiItemsBlackSmithItemsGet();
    return blacksmithItems;
  }

  public async getSummonHeroTokenCost(shardId: number) {
    const { data: cost } = await new SummonHeroApi(apiConfig).apiSummonHeroSummonCostNftTokenIdGet(shardId);
    return cost;
  }

  public async getSummonHero(shardId: number) {
    const { data: summonedHero } = await new SummonHeroApi(apiConfig).apiSummonHeroCreateNftTokenIdGet(shardId);
    return summonedHero;
  }

  public async postSummonHero({fulfillmentTxnHash, chainId}: SummonHeroOrder) {
    const { data: summonedHero } = await new SummonHeroApi(apiConfig).apiSummonHeroCompletedChainIdTxHashGet(chainId,fulfillmentTxnHash!);
    return summonedHero;
  }

  public async getHeroById(heroId: number) {
    const { data: hero } = await new HeroesApi(apiConfig).apiHeroesHeroIdGet(heroId);
    return hero;
  }

  public async getIdentifySkill(nftTokenId: number) {
    const { data: identifySkillOrder } = await new IdentifySkillApi(apiConfig).apiIdentifySkillCreateNftTokenIdGet(
      nftTokenId
    );
    return identifySkillOrder;
  }

  public async postIdentifySkill({fulfillmentTxnHash, chainId}: IdentifySkillOrder) {
    const { data: identifyOrder } = await new IdentifySkillApi(apiConfig).apiIdentifySkillCompletedChainIdTxHashGet(chainId,fulfillmentTxnHash!);
    return identifyOrder;
  }

  public async getLearnSkill(nftTokenId: number) {
    const { data: learnSkillOrder } = await new LearnSkillApi(apiConfig).apiLearnSkillCreateNftTokenIdGet(nftTokenId);
    return learnSkillOrder;
  }

  public async postLearnSkill({chainId,fulfillmentTxnHash}: LearnSkillOrder) {
    const { data: learnOrder } = await new LearnSkillApi(apiConfig).apiLearnSkillCompletedChainIdTxHashGet(chainId,fulfillmentTxnHash!);
    return learnOrder;
  }

  public async getNFTActionUse(nftTokenId: number) {
    const { data: learnSkillOrder } = await new NftActionApi(apiConfig).apiNftActionCreateNftTokenIdGet(nftTokenId);
    return learnSkillOrder;
  }

  public async postNFTActionUse({chainId,fulfillmentTxnHash}: NftActionOrder) {
    const { data: learnOrder } = await new NftActionApi(apiConfig).apiNftActionCompletedChainIdTxHashGet(chainId,fulfillmentTxnHash!);
    return learnOrder;
  }

  public async getHerbalistOptions() {
    const { data: options } = await new HerbalistApi(apiConfig).apiHerbalistHerbalistOptionsGet();
    return options;
  }
}

const vendorApi = new VendorApi();
export default vendorApi;
