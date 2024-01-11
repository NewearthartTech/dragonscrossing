import axios, { AxiosResponse } from "axios";
import { Observable, of } from "rxjs";
import { MonsterLoot } from "./combatTypes";
import combatActionStart from "../../../mocks/v1/combat/combat-action-attack-start.get.json";
import combatActionHitHit from "../../../mocks/v1/combat/combat-action-attack-hit-hit.get.json";
import combatActionHitMiss from "../../../mocks/v1/combat/combat-action-attack-hit-miss.get.json";
import combatActionMissHit from "../../../mocks/v1/combat/combat-action-attack-miss-hit.get.json";
import combatActionMissMiss from "../../../mocks/v1/combat/combat-action-attack-miss-miss.get.json";
import combatActionHitDead from "../../../mocks/v1/combat/combat-action-attack-hit-dead.get.json";
import combatActionDeadHit from "../../../mocks/v1/combat/combat-action-attack-dead-hit.get.json";
import combatActionSkill from "../../../mocks/v1/combat/combat-action-skill.get.json";
import loot from "../../../mocks/v1/loot/loot.get.json";
import { CombatsApi, ItemsApi, LearnedHeroSkill } from "@dcx/dcx-backend";
import { apiConfig } from "@/components/hoc/verification";

export class CombatApi {
  public async getCombatStartRound() {
    const { data: actionResponse } = await new CombatsApi(apiConfig).apiCombatsStartRoundGet();
    return actionResponse;
  }

  public async getCombatAttack() {
    const { data: actionResponse } = await new CombatsApi(apiConfig).apiCombatsAttackGet();
    return actionResponse;
  }

  public async getCombatSkill(skillSlug: string) {
    const { data: actionResponse } = await new CombatsApi(apiConfig).apiCombatsApplySkillSlugGet(skillSlug);
    return actionResponse;
  }

  public async getCombatPersuade() {
    const { data: actionResponse } = await new CombatsApi(apiConfig).apiCombatsPersuadeGet();
    return actionResponse;
  }

  public async getCombatFlee() {
    const { data: actionResponse } = await new CombatsApi(apiConfig).apiCombatsFleeGet();
    return actionResponse;
  }

  public async pickUpLoot(itemIds: Array<string>) {
    const { data: loot } = await new ItemsApi(apiConfig).apiItemsPickupLootPost(itemIds);
    return loot;
  }

  // public getLootMock(
  //   monsterId: number
  // ): Observable<AxiosResponse<MonsterLoot>> {
  //   const headers: any = "";
  //   const response = {
  //     data: loot,
  //     status: 200,
  //     statusText: "",
  //     headers: headers,
  //     config: "",
  //   } as AxiosResponse<MonsterLoot>;
  //   return of(response);
  // }
}

const combatApi = new CombatApi();
export default combatApi;
