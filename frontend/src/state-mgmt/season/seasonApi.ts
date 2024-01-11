import { apiConfig } from "@/components/hoc/verification";
import { SeasonApi, SignUpToSeasonOrder } from "@dcx/dcx-backend";
import { LeaderboardRequest } from "./seasonTypes";

export class DCXSeasonApi {
  public async getOpenSeasons() {
    const { data: seasons } = await new SeasonApi(apiConfig).apiSeasonOpenSeasonsIdGet("");
    return seasons;
  }

  public async getSeasonSignUpOrder(id: number) {
    const { data: order } = await new SeasonApi(apiConfig).apiSeasonSignupOrderSeasonIdGet(id);
    return order;
  }

  public async postSeasonSignUpOrder(chainId: number,txHash: string) {
    const { data: order } = await new SeasonApi(apiConfig).apiSeasonSignupCompletedChainIdFulfillmentTxnHashGet(chainId,txHash);
    return order;
  }

  public async getSeasonLeaderboard(leaderboardRequest: LeaderboardRequest) {
    const { data: leaderboard } = await new SeasonApi(apiConfig).apiSeasonLeaderBoardSeasonIdHeroIdGet(
      leaderboardRequest.seasonId,
      leaderboardRequest.heroId
    );
    return leaderboard;
  }
}

const dcxSeasonApi = new DCXSeasonApi();
export default dcxSeasonApi;
