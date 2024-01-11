import {
  createAsyncThunk,
  createSlice,
  PayloadAction,
  current,
} from "@reduxjs/toolkit";
import { LoadingStatus, Status } from "../app/appTypes";
import { RootState } from "../store/store";
import { IContractCalls, _USE_DEFAULT_CHAIN } from "@/components/web3/contractCalls";
import { setAppMessage } from "../app/appSlice";
import { formatErrorMessage } from "@/helpers/shared-functions";
import rewardsApi from "./rewardsApi";
import {
  ClaimDcxApi,
  ClaimDcxAutorization,
  RewardsClaims,
} from "@dcx/dcx-backend";
import { ClaimRewardResponse } from "./rewardsTypes";
import { MintResponse } from "../mint/mintTypes";

export interface RewardsState {
  availableReward: RewardsClaims;
  getAvailableRewardStatus: LoadingStatus;
  claimRewardResponse: ClaimRewardResponse;
  claimRewardStatus: string;
  callBackHash: string;
  mintedItemIds: Array<number>;
}

const initialState: RewardsState = {
  availableReward: {},
  getAvailableRewardStatus: {
    status: Status.NotStarted,
    error: "",
  },
  claimRewardResponse: {
    mintedItemIds: [],
    txHash: "",
  },
  claimRewardStatus: "",
  callBackHash: "",
  mintedItemIds: [],
};

//Thunks
export const getAvailableRewards = createAsyncThunk(
  "rewards/getAvailableRewards",
  async (
    {
      rewardType,
      walletAddress,
    }: {
      rewardType: "dcxReward" | "itemsReward";
      walletAddress: string;
    },
    { rejectWithValue, dispatch }
  ) => {
    try {
      return await rewardsApi.getAvailableDCXAuthorization(walletAddress);
    } catch (err: any) {
      dispatch(
        setAppMessage({
          message: formatErrorMessage(err),
          isClearToken: false,
        })
      );
      return rejectWithValue(err);
    }
  }
);

export const claimDCX = createAsyncThunk(
  "rewards/claim",
  async (
    {
      connect,
      mintRequest,
    }: { connect: IContractCalls; mintRequest: RewardsClaims },
    { rejectWithValue, dispatch }
  ) => {
    try {
      const {
        orderId: id,
        amount: priceInDcx,
        authorization: authorizaton,
      } = mintRequest.dcxClaim!;

      const txHash = await connect.claimDcx({
        authorizaton,
        id,
        priceInDcx,
      });

      await rewardsApi.postDcxCompleted(
        _USE_DEFAULT_CHAIN,
        txHash
      );

      return {
        mintedHeroIds: [],
        txHash,
      } as MintResponse;
    } catch (err: any) {
      dispatch(
        setAppMessage({
          message: formatErrorMessage(err),
          isClearToken: false,
        })
      );
      return rejectWithValue(err);
    }
  }
);

export const claimItems = createAsyncThunk(
  "rewards/claim",
  async (
    {
      connect,
      mintRequest,
    }: { connect: IContractCalls; mintRequest: RewardsClaims },
    { rejectWithValue, dispatch }
  ) => {
    let txHash:string ="";
    const mintedHeroIds:number[]=[];
    try {
      for (let i = 0; i < (mintRequest.itemClaims || []).length; i++) {
        const {
          itemNftId,
          authorization:authorizaton
        } = mintRequest.itemClaims![i];

        txHash = await connect.claimItems({
          chainId : _USE_DEFAULT_CHAIN,
          itemNftId,
          authorizaton
        });

        mintedHeroIds.push(itemNftId);


        await rewardsApi.postItemsCompleted(
          _USE_DEFAULT_CHAIN,
          txHash);
      }

      return {
        mintedHeroIds,
        txHash,
      } as MintResponse;

    } catch (err: any) {
      dispatch(
        setAppMessage({
          message: formatErrorMessage(err),
          isClearToken: false,
        })
      );
      return rejectWithValue(err);
    }
  }
);

export const rewardsSlice = createSlice({
  name: "rewards",
  initialState,
  reducers: {
    resetMintStatus: (state, action: PayloadAction) => {
      state.claimRewardStatus = "";
      state.claimRewardResponse = initialState.claimRewardResponse;
      state.mintedItemIds = [];
    },
    setCallBackHash: (state, action: PayloadAction<string>) => {
      state.callBackHash = action.payload;
    },
  },
  extraReducers(builder) {
    builder
      .addCase(getAvailableRewards.pending, (state, action) => {
        state.getAvailableRewardStatus.status = Status.Loading;
      })

      .addCase(
        getAvailableRewards.fulfilled,
        (state, action: PayloadAction<RewardsClaims>) => {
          state.availableReward = action.payload;
          state.getAvailableRewardStatus.status = Status.Loaded;
        }
      )

      .addCase(getAvailableRewards.rejected, (state, action: any) => {
        state.getAvailableRewardStatus.status = Status.Failed;
      })

      .addCase(claimDCX.pending, (state, action) => {
        state.claimRewardStatus = "SUBMITTING CLAIMS TRANSACTION...";
      })
      .addCase(
        claimDCX.fulfilled,
        (state, action: PayloadAction<MintResponse>) => {
          //state.mintedHeroIds = action.payload.mintedHeroIds;
          //state.claimRewardResponse = action.payload;
          //state.claimRewardStatus = "HERO ID(S) " + action.payload.mintedHeroIds.join(", ") + " SUCCESSFULLY MINTED!";
          state.claimRewardStatus = `REWARD CLAIMED with TX:${action.payload.txHash}`;
          if(action.payload.mintedHeroIds.length>0){
            state.claimRewardStatus = "ITEM ID(S) " + action.payload.mintedHeroIds.join(", ") + " SUCCESSFULLY MINTED!";
          }
        }
      )
      .addCase(claimDCX.rejected, (state, action: any) => {
        state.claimRewardStatus = "TRANSACTION FAILED, PLEASE TRY AGAIN.";
      });
  },
});

// Action creators
export const { resetMintStatus, setCallBackHash } = rewardsSlice.actions;

// Selectors
export const selectAvailableReward = (state: RootState) =>
  state.rewards.availableReward;
export const selectAvailableRewardStatus = (state: RootState) =>
  state.rewards.getAvailableRewardStatus;

export const selectClaimRewardResponse = (state: RootState) =>
  state.rewards.claimRewardResponse;
export const selectClaimRewardStatus = (state: RootState) =>
  state.rewards.claimRewardStatus;
export const selectCallBackHash = (state: RootState) =>
  state.rewards.callBackHash;
export const selectMintedHeroIds = (state: RootState) =>
  state.rewards.mintedItemIds;

export default rewardsSlice.reducer;
