import {
  createAsyncThunk,
  createSlice,
  PayloadAction,
  current,
} from "@reduxjs/toolkit";
import { LoadingStatus, Status } from "../app/appTypes";
import { RootState } from "../store/store";
import { IContractCalls } from "@/components/web3/contractCalls";
import singleStakingApi from "./singleStakingApi";
import { StakingPool } from "./singleStakingTypes";

export interface SingleStakingState {
  stakingPools: Array<StakingPool>;
  getStakingPoolsStatus: LoadingStatus;
}

const initialState: SingleStakingState = {
  stakingPools: [],
  getStakingPoolsStatus: {
    status: Status.NotStarted,
    error: "",
  },
};

//Thunks
export const getStakingPools = createAsyncThunk(
  "singleStaking/getStakingPools",
  async (
    walletAddress: string & { connect: IContractCalls },
    { rejectWithValue }
  ) => {
    try {
      // const { connect } = walletAddress;
      // const hash = await connect.depositDcx({
      //   amountInDcx: levelUpOrder.priceInDcx,
      //   orderId: levelUpOrder.id,
      // });
      // return await heroApi.postLevelUpHero({
      //   ...order,
      //   fulfillmentTxnHash: hash,
      // });
      let test: any;
      return test;
    } catch (err: any) {
      return rejectWithValue(err);
    }
  }
);

export const singleStakingSlice = createSlice({
  name: "singleStaking",
  initialState,
  reducers: {
    getSingleStakingMock: (state, action: PayloadAction) => {
      singleStakingApi.getSingleStakingMock().subscribe((res) => {
        state.stakingPools = res.data;
      });
      state.getStakingPoolsStatus.status = Status.Loaded;
    },
  },
  extraReducers(builder) {
    builder
      .addCase(getStakingPools.pending, (state, action) => {
        state.getStakingPoolsStatus.status = Status.Loading;
      })
      .addCase(
        getStakingPools.fulfilled,
        (state, action: PayloadAction<Array<StakingPool>>) => {
          state.stakingPools = action.payload;
          state.getStakingPoolsStatus.status = Status.Loaded;
        }
      )
      .addCase(getStakingPools.rejected, (state, action: any) => {
        state.getStakingPoolsStatus.status = Status.Failed;
      });
  },
});

// Action creators
export const { getSingleStakingMock } = singleStakingSlice.actions;

// Selectors
export const selectStakingPools = (state: RootState) =>
  state.singleStaking.stakingPools;
export const selectGetStakingPoolsStatus = (state: RootState) =>
  state.singleStaking.getStakingPoolsStatus;

export default singleStakingSlice.reducer;
