import { AxiosResponse } from "axios";
import { Observable, of } from "rxjs";
import { StakingPool } from "./singleStakingTypes";
import singleStakingData from "../../../mocks/v1/single-staking/single-staking.get.json";

export class SingleStakingApi {
  public getSingleStakingMock(): Observable<AxiosResponse<Array<StakingPool>>> {
    const headers: any = "";
    const response = {
      data: singleStakingData,
      status: 200,
      statusText: "",
      headers: headers,
      config: "",
    } as AxiosResponse<Array<StakingPool>>;
    return of(response);
  }
}

const singleStakingApi = new SingleStakingApi();
export default singleStakingApi;
