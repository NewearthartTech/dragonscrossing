import { apiConfig } from "@/components/hoc/verification";
import { ItemsApi, MoveItemRequestDto, PlayersApi } from "@dcx/dcx-backend";

export class PlayerApi {
  public async getPlayer() {
    const { data: player } = await new PlayersApi(apiConfig).apiPlayersMeGet();
    return player;
  }

  public async getNftItems(ownedTokenIds: Array<string>) {
    const { data: items } = await new ItemsApi(apiConfig).apiItemsFromNftsIdsGet(ownedTokenIds.join(":"));
    return items;
  }

  public async storeItemInStash(itemId: string) {
    const { data: player } = await new ItemsApi(apiConfig).apiItemsMoveToStashGet(itemId);
    return player;
  }

  public async grabItemFromStash(itemId: string) {
    const { data: player } = await new ItemsApi(apiConfig).apiItemsMoveFromStashGet(itemId);
    return player;
  }
}

const playerApi = new PlayerApi();
export default playerApi;
