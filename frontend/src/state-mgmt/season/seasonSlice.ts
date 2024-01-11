import { createAsyncThunk, createSlice, PayloadAction, current } from "@reduxjs/toolkit";
import { RootState } from "../store/store";
import { HeroDto, Season, SeasonLeaderboard, SignUpToSeasonOrder } from "@dcx/dcx-backend";
import dcxSeasonApi from "./seasonApi";
import { setAppMessage } from "../app/appSlice";
import { formatErrorMessage } from "@/helpers/shared-functions";
import { IContractCalls } from "@/components/web3/contractCalls";
import { LoadingStatus, Status } from "../app/appTypes";
import { LeaderboardRequest } from "./seasonTypes";

export interface SeasonState {
  heroToRegister: HeroDto|undefined;
  showRegistrationModal: boolean;
  showLeaderboardModal: boolean;
  openSeasons: Array<Season>;
  getOpenSeasonsStatus: LoadingStatus;
  signUpToSeasonResponse: SignUpToSeasonOrder;
  getSignUpToSeasonStatus: LoadingStatus;
  seasonSignUpSubmittedStatus: LoadingStatus;
  getLeaderboardStatus: LoadingStatus;
  leaderBoard: SeasonLeaderboard;
}

const initialState: SeasonState = {
  heroToRegister: undefined,
  showRegistrationModal: false,
  showLeaderboardModal: false,
  openSeasons: [],
  getOpenSeasonsStatus: {
    status: Status.NotStarted,
    error: "",
  },
  signUpToSeasonResponse: {
    type: "",
    id: "",
    isCompleted: false,
    priceInDcx: -1,
    chainId:0,
    heroId:0,
  },
  getSignUpToSeasonStatus: {
    status: Status.NotStarted,
    error: "",
  },
  seasonSignUpSubmittedStatus: {
    status: Status.NotStarted,
    error: "",
  },
  getLeaderboardStatus: {
    status: Status.NotStarted,
    error: "",
  },
  leaderBoard: {
    seasonId: -1,
    seasonName: "",
    heroesInSeason: -1,
    heroRanks: [],
  },
};

//Thunks
export const getOpenSeasons = createAsyncThunk("season/getOpenSeasons", async (_, { rejectWithValue, dispatch }) => {
  try {
    const response = await dcxSeasonApi.getOpenSeasons();
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
});

export const getSeasonSignUpOrder = createAsyncThunk(
  "season/getSeasonSignUpOrder",
  async (id: number, { rejectWithValue, dispatch }) => {
    try {
      const response = await dcxSeasonApi.getSeasonSignUpOrder(id);
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

export const submitSeasonSignUpRequest = createAsyncThunk(
  "season/submitSeasonSignUpRequest",
  async (signUpToSeasonOrder: SignUpToSeasonOrder & { connect: IContractCalls }, { rejectWithValue, dispatch }) => {
    try {
      
      const { connect, ...order } = signUpToSeasonOrder;
      // if (!signUpToSeasonOrder.priceInDcx) {
      //   throw new Error("Price In DCX is empty.");
      // }
      const hash = await connect.registerForSeason(order);
      return await dcxSeasonApi.postSeasonSignUpOrder(order.chainId,hash);
    } catch (err: any) {
      if (err.response) {
        dispatch(
          setAppMessage({
            message: formatErrorMessage(err),
            isClearToken: false,
          })
        );
        return rejectWithValue(err);
      } else {
        dispatch(
          setAppMessage({
            message: err.message.length > 232 ? err.message.substring(0, 232) : err.message,
            isClearToken: false,
          })
        );
        return rejectWithValue(err);
      }
    }
  }
);

export const getSeasonLeaderboard = createAsyncThunk(
  "season/getSeasonLeaderboard",
  async (leaderboardRequest: LeaderboardRequest, { rejectWithValue, dispatch }) => {
    try {
      const response = await dcxSeasonApi.getSeasonLeaderboard(leaderboardRequest);
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

export const seasonSlice = createSlice({
  name: "season",
  initialState,
  reducers: {
    setHeroToRegister: (state, action: PayloadAction<HeroDto|undefined>) => {
      state.heroToRegister = action.payload;
    },
    setShowRegistrationModal: (state, action: PayloadAction<boolean>) => {
      state.showRegistrationModal = action.payload;
    },
    setShowLeaderboardModal: (state, action: PayloadAction<boolean>) => {
      state.showLeaderboardModal = action.payload;
    },
    resetGetSignUpToSeasonStatus: (state, action: PayloadAction) => {
      state.getSignUpToSeasonStatus.status = Status.NotStarted;
      state.getSignUpToSeasonStatus.error = "";
    },
    resetSeasonSignUpSubmittedStatus: (state, action: PayloadAction) => {
      state.seasonSignUpSubmittedStatus.status = Status.NotStarted;
      state.seasonSignUpSubmittedStatus.error = "";
    },
  },
  extraReducers(builder) {
    builder
      .addCase(getOpenSeasons.pending, (state, action) => {
        state.getOpenSeasonsStatus.status = Status.Loading;
      })
      .addCase(getOpenSeasons.fulfilled, (state, action: PayloadAction<Array<Season>>) => {
        state.openSeasons = action.payload;
        state.getOpenSeasonsStatus.status = Status.Loaded;
      })
      .addCase(getOpenSeasons.rejected, (state, action: any) => {
        state.getOpenSeasonsStatus.status = Status.Failed;
      })
      .addCase(getSeasonSignUpOrder.pending, (state, action) => {
        state.getSignUpToSeasonStatus.status = Status.Loading;
      })
      .addCase(getSeasonSignUpOrder.fulfilled, (state, action: PayloadAction<SignUpToSeasonOrder>) => {
        state.signUpToSeasonResponse = action.payload;
        state.getSignUpToSeasonStatus.status = Status.Loaded;
      })
      .addCase(getSeasonSignUpOrder.rejected, (state) => {
        state.getSignUpToSeasonStatus.status = Status.Failed;
      })
      .addCase(submitSeasonSignUpRequest.pending, (state) => {
        state.seasonSignUpSubmittedStatus.status = Status.Loading;
      })
      .addCase(submitSeasonSignUpRequest.fulfilled, (state) => {
        state.seasonSignUpSubmittedStatus.status = Status.Loaded;
      })
      .addCase(submitSeasonSignUpRequest.rejected, (state, action: any) => {
        state.seasonSignUpSubmittedStatus.status = Status.Failed;
      })
      .addCase(getSeasonLeaderboard.pending, (state, action) => {
        state.getLeaderboardStatus.status = Status.Loading;
      })
      .addCase(getSeasonLeaderboard.fulfilled, (state, action: PayloadAction<SeasonLeaderboard>) => {
        state.leaderBoard = action.payload;
        state.getLeaderboardStatus.status = Status.Loaded;
      })
      .addCase(getSeasonLeaderboard.rejected, (state, action: any) => {
        state.getLeaderboardStatus.status = Status.Failed;
      });
  },
});

// Action creators
export const {
  setHeroToRegister,
  setShowRegistrationModal,
  setShowLeaderboardModal,
  resetGetSignUpToSeasonStatus,
  resetSeasonSignUpSubmittedStatus,
} = seasonSlice.actions;

// Selectors
export const selectHeroToRegister = (state: RootState) => state.season.heroToRegister;
export const selectShowRegistrationModal = (state: RootState) => state.season.showRegistrationModal;
export const selectShowLeaderboardModal = (state: RootState) => state.season.showLeaderboardModal;
export const selectOpenSeasons = (state: RootState) => state.season.openSeasons;
export const selectGetOpenSeasonsStatus = (state: RootState) => state.season.getOpenSeasonsStatus;
export const selectSignUpToSeasonResponse = (state: RootState) => state.season.signUpToSeasonResponse;
export const selectGetSignUpToSeasonStatus = (state: RootState) => state.season.getSignUpToSeasonStatus;
export const selectSeasonSignUpSubmittedStatus = (state: RootState) => state.season.seasonSignUpSubmittedStatus;
export const selectLeaderboard = (state: RootState) => state.season.leaderBoard;
export const selectGetLeaderboardStatus = (state: RootState) => state.season.getLeaderboardStatus;

export default seasonSlice.reducer;
