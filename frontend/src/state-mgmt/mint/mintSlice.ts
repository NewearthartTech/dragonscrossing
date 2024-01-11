import { createAsyncThunk, createSlice, PayloadAction, current } from "@reduxjs/toolkit";
import { LoadingStatus, Status } from "../app/appTypes";
import { RootState } from "../store/store";
import { IContractCalls } from "@/components/web3/contractCalls";
import { setAppMessage } from "../app/appSlice";
import { formatErrorMessage } from "@/helpers/shared-functions";
import mintApi from "./mintApi";
import { HeroMintOrderReq, HeroMintPrice, HeroMintPriceReq } from "@dcx/dcx-backend";
import { MintResponse } from "./mintTypes";

export interface MintState {
  heroMintPrice: HeroMintPrice;
  getHeroMintPriceStatus: LoadingStatus;
  mintResponse: MintResponse;
  mintStatus: string;
  callBackHash: string;
  mintedHeroIds: Array<number>;
}

const initialState: MintState = {
  heroMintPrice: {
    basePrice: -1,
    boostedPrice: -1,
    maxQuantity: -1,
  },
  getHeroMintPriceStatus: {
    status: Status.NotStarted,
    error: "",
  },
  mintResponse: {
    mintedHeroIds: [],
    txHash: "",
  },
  mintStatus: "",
  callBackHash: "",
  mintedHeroIds: [],
};

//Thunks
export const getMintPrice = createAsyncThunk(
  "mint/getNextHeroId",
  async (heroMintPriceRequest: HeroMintPriceReq, { rejectWithValue, dispatch }) => {
    try {
      const response = await mintApi.getMintPrice(heroMintPriceRequest);
      return response;
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

//this can be used both for normal and boosted
export const mint = createAsyncThunk(
  "mint/mint",
  async (
    { connect, mintRequest }: { connect: IContractCalls; mintRequest: HeroMintOrderReq },
    { rejectWithValue, dispatch }
  ) => {
    try {
      const walletAddress = await connect.getAddress();
      const mintOrder = await mintApi.getHeroMintAuthorization({ ...mintRequest, walletAddress });
      return await connect.mint(mintOrder, mintRequest, (txHash) => dispatch(setCallBackHash(txHash)));
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

export const mintSlice = createSlice({
  name: "mint",
  initialState,
  reducers: {
    resetMintStatus: (state, action: PayloadAction) => {
      state.mintStatus = "";
      state.mintResponse = initialState.mintResponse;
      state.mintedHeroIds = [];
    },
    setCallBackHash: (state, action: PayloadAction<string>) => {
      state.callBackHash = action.payload;
    },
  },
  extraReducers(builder) {
    builder
      .addCase(getMintPrice.pending, (state, action) => {
        state.getHeroMintPriceStatus.status = Status.Loading;
      })
      .addCase(getMintPrice.fulfilled, (state, action: PayloadAction<HeroMintPrice>) => {
        state.heroMintPrice = action.payload;
        state.getHeroMintPriceStatus.status = Status.Loaded;
      })
      .addCase(getMintPrice.rejected, (state, action: any) => {
        state.getHeroMintPriceStatus.status = Status.Failed;
      })
      .addCase(mint.pending, (state, action) => {
        state.mintStatus = "SUBMITTING MINT HERO TRANSACTION...";
      })
      .addCase(mint.fulfilled, (state, action: PayloadAction<MintResponse>) => {
        state.mintedHeroIds = action.payload.mintedHeroIds;
        state.mintResponse = action.payload;
        state.mintStatus = "HERO ID(S) " + action.payload.mintedHeroIds.join(", ") + " SUCCESSFULLY MINTED!";
      })
      .addCase(mint.rejected, (state, action: any) => {
        state.mintStatus = "TRANSACTION FAILED, PLEASE TRY AGAIN.";
      });
  },
});

// Action creators
export const { resetMintStatus, setCallBackHash } = mintSlice.actions;

// Selectors
export const selectHeroMintPrice = (state: RootState) => state.mint.heroMintPrice;
export const selectHeroMintPriceStatus = (state: RootState) => state.mint.getHeroMintPriceStatus;
export const selectMintResponse = (state: RootState) => state.mint.mintResponse;
export const selectMintStatus = (state: RootState) => state.mint.mintStatus;
export const selectCallBackHash = (state: RootState) => state.mint.callBackHash;
export const selectMintedHeroIds = (state: RootState) => state.mint.mintedHeroIds;

export default mintSlice.reducer;
