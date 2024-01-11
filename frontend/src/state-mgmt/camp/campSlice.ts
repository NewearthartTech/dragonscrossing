import { createAsyncThunk, createSlice, PayloadAction, current } from "@reduxjs/toolkit";
import { LoadingStatus, Status } from "../app/appTypes";
import { RootState } from "../store/store";
import {
  CampingStatus,
  ClaimDcxAutorization,
  ClaimDcxOrder,
  SecuredNFTsOrder,
} from "@dcx/dcx-backend";
import campApi from "./campApi";
import { IContractCalls } from "@/components/web3/contractCalls";
import { setUpdateOwnerNftIds } from "../vendor/vendorSlice";
import { setRefreshItemNFTsDelayed } from "../app/appSlice";

export interface CampState {
  campOrders: CampingStatus;
  campOrdersStatus: LoadingStatus;
  claimDcxSubmittedStatus: string;
  claimNftSubmittedStatus: string;
  transactionError: string;
  submittedTransactionOrderId: string;
  claimDcxRewardsAuthorization: ClaimDcxAutorization;
  getClaimDcxRewardsAuthorizationStatus: LoadingStatus;
  submitClaimDcxRewardsStatus: LoadingStatus;
  claimDcxRewardsTxHash: string;
}

const initialState: CampState = {
  campOrders: {
    nftOrders: [],
    claimDcxOrders: [],
  },
  campOrdersStatus: {
    status: Status.NotStarted,
    error: "",
  },
  claimDcxSubmittedStatus: "",
  claimNftSubmittedStatus: "",
  transactionError: "",
  submittedTransactionOrderId: "",
  claimDcxRewardsAuthorization: {
    amount: -1,
    orderId: "",
    authorization: "",
  },
  getClaimDcxRewardsAuthorizationStatus: {
    status: Status.NotStarted,
    error: "",
  },
  submitClaimDcxRewardsStatus: {
    status: Status.NotStarted,
    error: "",
  },
  claimDcxRewardsTxHash: "",
};

//Thunks
export const getCampOrders = createAsyncThunk("camp/getCampOrders", async (_, { rejectWithValue }) => {
  try {
    return await campApi.getCampOrders();
  } catch (err: any) {
    return rejectWithValue(err);
  }
});

export const getCampOrdersStatus = createAsyncThunk("camp/getCampOrdersStatus", async (_, { rejectWithValue }) => {
  try {
    return await campApi.getCampOrdersStatus();
  } catch (err: any) {
    return rejectWithValue(err);
  }
});

export const submitClaimDcxTransaction = createAsyncThunk(
  "camp/submitClaimDcxTransaction",
  async (claimDcxOrder: ClaimDcxOrder & { connect: IContractCalls }, { rejectWithValue }) => {
    try {
      const { connect, ...order } = claimDcxOrder;
      await campApi.postClaimDcx(order);
      if (!claimDcxOrder.priceInDcx) {
        throw new Error("Price In DCX is empty.");
      }
      const hash = await connect.claimDcx(claimDcxOrder);
      return await campApi.postClaimDcx({
        ...order,
        fulfillmentTxnHash: hash,
      });
    } catch (err: any) {
      return rejectWithValue(err);
    }
  }
);



export const submitClaimNftTransaction = createAsyncThunk(
  "camp/submitClaimNftTransaction",
  async (claimNftOrder: SecuredNFTsOrder & { connect: IContractCalls }, { rejectWithValue, dispatch }) => {
    try {
      const { connect, ...order } = claimNftOrder;
      await campApi.postClaimNft(order);
      const hash = await connect.claimItems(claimNftOrder);
      dispatch(setUpdateOwnerNftIds(claimNftOrder.itemNftId));
      return await campApi.postClaimNft({
        ...order,
        fulfillmentTxnHash: hash,
      });
    } catch (err: any) {
      return rejectWithValue(err);
    }
  }
);

export const campSlice = createSlice({
  name: "camp",
  initialState,
  reducers: {
    setSubmittedTransactionOrderId: (state, action: PayloadAction<string>) => {
      state.submittedTransactionOrderId = action.payload;
    },
    setClaimDcxSubmittedStatus: (state, action: PayloadAction<string>) => {
      state.claimDcxSubmittedStatus = action.payload;
    },
    setClaimNftSubmittedStatus: (state, action: PayloadAction<string>) => {
      state.claimNftSubmittedStatus = action.payload;
    },
    setClaimDcxRewardsTxHash: (state, action: PayloadAction<string>) => {
      state.claimDcxRewardsTxHash = action.payload;
    },
  },
  extraReducers(builder) {
    builder
      .addCase(getCampOrders.pending, (state, action) => {
        state.campOrdersStatus.status = Status.Loading;
      })
      .addCase(getCampOrders.fulfilled, (state, action: PayloadAction<CampingStatus>) => {
        state.campOrders = action.payload;
        state.campOrdersStatus.status = Status.Loaded;
      })
      .addCase(getCampOrders.rejected, (state, action: any) => {
        state.campOrdersStatus.status = Status.Failed;
      })
      .addCase(getCampOrdersStatus.pending, (state, action) => {
        state.campOrdersStatus.status = Status.Loading;
      })
      .addCase(getCampOrdersStatus.fulfilled, (state, action: PayloadAction<CampingStatus>) => {
        state.campOrders = action.payload;
        state.campOrdersStatus.status = Status.Loaded;
      })
      .addCase(getCampOrdersStatus.rejected, (state, action: any) => {
        state.campOrdersStatus.status = Status.Failed;
      })
      .addCase(submitClaimDcxTransaction.pending, (state, action) => {
        state.claimDcxSubmittedStatus = "submitting";
      })
      .addCase(submitClaimDcxTransaction.fulfilled, (state, action: PayloadAction<CampingStatus>) => {
        state.campOrders = action.payload;
        state.claimDcxSubmittedStatus = "processing";
      })
      .addCase(submitClaimDcxTransaction.rejected, (state, action: any) => {
        state.transactionError = action.payload.message;
        state.claimDcxSubmittedStatus = "failed";
      })
      .addCase(submitClaimNftTransaction.pending, (state, action) => {
        state.claimNftSubmittedStatus = "submitting";
      })
      .addCase(submitClaimNftTransaction.fulfilled, (state, action: PayloadAction<CampingStatus>) => {
        state.campOrders = action.payload;
        state.claimNftSubmittedStatus = "processing";
      })
      .addCase(submitClaimNftTransaction.rejected, (state, action: any) => {
        state.transactionError = action.payload.message;
        state.claimNftSubmittedStatus = "failed";
      });
  },
});

// Action creators
export const {
  setSubmittedTransactionOrderId,
  setClaimDcxSubmittedStatus,
  setClaimNftSubmittedStatus,
  setClaimDcxRewardsTxHash,
} = campSlice.actions;

// Selectors
export const selectCampOrders = (state: RootState) => state.camp.campOrders;
export const selectCampOrdersStatus = (state: RootState) => state.camp.campOrdersStatus;
export const selectClaimDcxSubmittedStatus = (state: RootState) => state.camp.claimDcxSubmittedStatus;
export const selectClaimNftSubmittedStatus = (state: RootState) => state.camp.claimNftSubmittedStatus;
export const selectSubmittedTransactionOrderId = (state: RootState) => state.camp.submittedTransactionOrderId;
export const selectClaimDcxRewardsAuthorization = (state: RootState) => state.camp.claimDcxRewardsAuthorization;
export const selectSubmitClaimDcxRewardsStatus = (state: RootState) => state.camp.submitClaimDcxRewardsStatus;
export const selectClaimDcxRewardsTxHash = (state: RootState) => state.camp.claimDcxRewardsTxHash;

export default campSlice.reducer;
