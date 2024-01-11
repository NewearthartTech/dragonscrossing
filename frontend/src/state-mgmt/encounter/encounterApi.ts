import { NonCombatEncounterApi } from "@dcx/dcx-backend";
import { apiConfig } from "@/components/hoc/verification";
import { EncounterResponseRequest } from "./encounterTypes";

export class EncounterApi {
  public async putLocationEncounterFinish(encounterId: string) {
    const { data: isSuccess } = await new NonCombatEncounterApi(
      apiConfig
    ).apiNonCombatEncounterLocationEncounterFinishEncounterIdPut(encounterId);
    return isSuccess;
  }

  public async getLoreEncounterQuestionResponse(encounterResponseRequest: EncounterResponseRequest) {
    const { data: loreResponse } = await new NonCombatEncounterApi(
      apiConfig
    ).apiNonCombatEncounterLoreEncounterEncounterIdQuestionSlugGet(
      encounterResponseRequest.id,
      encounterResponseRequest.slug
    );
    return loreResponse;
  }

  public async getChanceEncounterChoiceResult(encounterResponseRequest: EncounterResponseRequest) {
    const { data: chanceResult } = await new NonCombatEncounterApi(
      apiConfig
    ).apiNonCombatEncounterChanceEncounterEncounterIdChoiceSlugGet(
      encounterResponseRequest.id,
      encounterResponseRequest.slug
    );
    return chanceResult;
  }

  public async getBossEncounterFinish(encounterId: string) {
    const { data: isSuccess } = await new NonCombatEncounterApi(
      apiConfig
    ).apiNonCombatEncounterBossEncounterEncounterIdGet(encounterId);
    return isSuccess;
  }
}

const encounterApi = new EncounterApi();
export default encounterApi;
