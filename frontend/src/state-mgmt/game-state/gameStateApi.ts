import { DcxTiles, GameStatesApi } from "@dcx/dcx-backend";
import { apiConfig } from "@/components/hoc/verification";

export class GameStateApi {
  public async getGameState(heroId: number) {
    const { data: gameState } = await new GameStatesApi(apiConfig).apiGameStatesGet(heroId);
    return gameState;
  }

  public async updateGameStateLocation(slug: DcxTiles) {
    const { data: gameState } = await new GameStatesApi(apiConfig).apiGameStatesUpdateLocationNewLocationGet(slug);
    return gameState;
  }
}

const gameStateApi = new GameStateApi();
export default gameStateApi;
