import { TestsApi } from "@dcx/dcx-backend";
import { apiConfig } from "@/components/hoc/verification";
import { FixDiceRequest } from "./testingTypes";

export class TestingApi {
  public async fixDice(fixDiceRequest: FixDiceRequest) {
    const { data: isFixedDice } = await new TestsApi(apiConfig).apiTestsFixDiceReasonValueGet(
      fixDiceRequest.reason,
      fixDiceRequest.value
    );
    return isFixedDice;
  }

  public async resetQuests(heroId: number) {
    const { data: isQuestsReset } = await new TestsApi(apiConfig).apiTestsResetQuestAndSkillHeroIdGet(heroId);
    return isQuestsReset;
  }

  public async learnSkill(skillSlug: string) {
    const { data: isSkillLearned } = await new TestsApi(apiConfig).apiTestsLearnSkillSlugGet(skillSlug);
    return isSkillLearned;
  }
}

const testingApi = new TestingApi();
export default testingApi;
