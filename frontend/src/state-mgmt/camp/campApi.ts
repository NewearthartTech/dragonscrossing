import { apiConfig } from "@/components/hoc/verification";
import { CampingApi, ClaimDcxOrder, SecuredNFTsOrder } from "@dcx/dcx-backend";

export class CampApi {
  public async getCampOrders() {
    const { data: campingStatus } = await new CampingApi(apiConfig).apiCampingGoCampGet();
    return campingStatus;
  }

  public async getCampOrdersStatus() {
    const { data: campingStatus } = await new CampingApi(apiConfig).apiCampingStatusGet();
    return campingStatus;
  }

  public async postClaimDcx(claimDcxOrder: ClaimDcxOrder) {
    const { data: campingStatus } = await new CampingApi(apiConfig).apiCampingDCXClaimedPost(claimDcxOrder);
    return campingStatus;
  }

  public async postClaimNft(claimNftOrder: SecuredNFTsOrder) {
    
    const { data: campingStatus } = await new CampingApi(apiConfig).apiCampingItemSecureCompletedTxHashGet(claimNftOrder.fulfillmentTxnHash||"empty");
    return campingStatus;
  }


  
}

const campApi = new CampApi();
export default campApi;
