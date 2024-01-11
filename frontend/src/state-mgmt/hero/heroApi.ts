import {
  HeroesApi,
  MoveItemRequestDto,
  ItemsApi,
  EquipItemRequestDto,
  LevelupApi,
  LevelUpOrder,
  CombatsApi,
  HerbalistApi,
  HerbalistOption,
  SkillsApi,
} from "@dcx/dcx-backend";
import { apiConfig } from "@/components/hoc/verification";

export class HeroApi {
  public async getHeroes(ownedTokenIds: Array<string>) {
    const { data: heros } = await new HeroesApi(apiConfig).apiHeroesMineGet(ownedTokenIds.length === 0?"":ownedTokenIds.join(":"));
    return heros;
  }

  public async getHero() {
    const { data: selectedHero } = await new HeroesApi(apiConfig).apiHeroesSelectedHeroGet();
    return selectedHero;
  }

  public async getHeroById(heroId: number) {
    const { data: selectedHero } = await new HeroesApi(apiConfig).apiHeroesHeroIdGet(heroId);
    return selectedHero;
  }

  public async getHeroCalculatedStats(heroId: number) {
    const { data: calculatedStats } = await new HeroesApi(apiConfig).apiHeroesCalculatedStatsHeroIdGet(heroId);
    return calculatedStats;
  }

  public async getAllocateSkillPoints(skillId: string) {
    const { data: hero } = await new SkillsApi(apiConfig).apiSkillsAllocateUseSkillIdGet(skillId);
    return hero;
  }

  public async getDeAllocateSkillPoints(skillId: string) {
    const { data: hero } = await new SkillsApi(apiConfig).apiSkillsDeAllocateUseSkillIdGet(skillId);
    return hero;
  }

  public async getForgetSkill(skillId: string) {
    const { data: hero } = await new SkillsApi(apiConfig).apiSkillsForgetSkillIdGet(skillId);
    return hero;
  }

  public async getLevelUpHero(level: number) {
    const { data: levelUpOrder } = await new LevelupApi(apiConfig).apiLevelupFromLevelGet(level);
    return levelUpOrder;
  }

  public async postLevelUpHero(levelUpOrderRequest: LevelUpOrder) {
    const { data: levelUpOrder } = await new LevelupApi(apiConfig).apiLevelupPost(levelUpOrderRequest);
    return levelUpOrder;
  }

  public async buyBlacksmithItem(itemSlug: string) {
    const { data: hero } = await new ItemsApi(apiConfig).apiItemsBuyItemSlugPut(itemSlug);
    return hero;
  }

  public async sellItemToBlacksmith(itemIds: string[]) {
    //dee:todo smelter
   
    //const { data: hero } = await new ItemsApi(apiConfig).apiItemsSellItemItemIdPut(itemId);
    const { data: hero } = await new ItemsApi(apiConfig).apiItemsSmeltPut(itemIds);
    return hero;
    
  }

  public async updateHealHero(isFullHeal: boolean) {
    const herbalistOption: HerbalistOption = isFullHeal ? "oneHundredPercent" : "fortyPercent";
    const { data: hero } = await new HerbalistApi(apiConfig).apiHerbalistPurchaseOptionPut(herbalistOption);
    return hero;
  }

  public async moveItem(moveItemRequest: MoveItemRequestDto) {
    const { data: hero } = await new ItemsApi(apiConfig).apiItemsMoveItemInInventoryPost(moveItemRequest);
    return hero;
  }

  public async equipItem(equipItemRequest: EquipItemRequestDto) {
    const { data: hero } = await new ItemsApi(apiConfig).apiItemsEquipItemPost(equipItemRequest);
    return hero;
  }

  public async unequipItem(unequipItemRequest: EquipItemRequestDto) {
    const { data: hero } = await new ItemsApi(apiConfig).apiItemsUnequipItemPost(unequipItemRequest);
    return hero;
  }

  public async destroyItem(destroyItemRequest: EquipItemRequestDto) {
    const { data: hero } = await new ItemsApi(apiConfig).apiItemsDestroyItemPost(destroyItemRequest);
    return hero;
  }

  public async keepItemAtDeath(itemId: string) {
    const { data: hero } = await new CombatsApi(apiConfig).apiCombatsChooseOneItemOnDeathItemIdGet(itemId);
    return hero;
  }
}

const heroApi = new HeroApi();
export default heroApi;
