import { formatErrorMessage } from "@/helpers/shared-functions";
import { ChanceEncounterDto } from "@dcx/dcx-backend";
import { createAsyncThunk, createSlice, PayloadAction, current } from "@reduxjs/toolkit";
import { setAppMessage } from "../app/appSlice";
import { LoadingStatus, Status } from "../app/appTypes";
import { RootState } from "../store/store";
import encounterApi from "./encounterApi";
import { EncounterResponseRequest } from "./encounterTypes";

export interface EncounterState {
  encounterFinishedStatus: LoadingStatus;
  loreEncounterResponseStatus: LoadingStatus;
  chanceEncounterResponseStatus: LoadingStatus;
  loreEncounterResponse: string;
  chanceEncounterResponse: ChanceEncounterDto;
  isEncounterComplete: boolean;
}

const initialState: EncounterState = {
  loreEncounterResponse: "",
  chanceEncounterResponse: {
    isGoodOutcome:false,
    diceResult: -1,
    outComeText: "",
  },
  encounterFinishedStatus: {
    status: Status.NotStarted,
    error: "",
  },
  loreEncounterResponseStatus: {
    status: Status.NotStarted,
    error: "",
  },
  chanceEncounterResponseStatus: {
    status: Status.NotStarted,
    error: "",
  },
  isEncounterComplete: false,
};

//Thunks
export const updateLocationEncounterFinish = createAsyncThunk(
  "encounter/updateLocationEncounterFinish",
  async (encounterId: string, { rejectWithValue, dispatch }) => {
    try {
      return await encounterApi.putLocationEncounterFinish(encounterId);
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

export const retrieveLoreEncounterQuestionResponse = createAsyncThunk(
  "encounter/retrieveLoreEncounterQuestionResponse",
  async (encounterResponseRequest: EncounterResponseRequest, { rejectWithValue, dispatch }) => {
    try {
      return await encounterApi.getLoreEncounterQuestionResponse(encounterResponseRequest);
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

export const retrieveChanceEncounterChoiceResult = createAsyncThunk(
  "encounter/retrieveChanceEncounterChoiceResult",
  async (encounterResponseRequest: EncounterResponseRequest, { rejectWithValue, dispatch }) => {
    try {
      return await encounterApi.getChanceEncounterChoiceResult(encounterResponseRequest);
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

export const retrieveBossEncounterFinish = createAsyncThunk(
  "encounter/retrieveBossEncounterFinish",
  async (encounterId: string, { rejectWithValue, dispatch }) => {
    try {
      return await encounterApi.getBossEncounterFinish(encounterId);
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

export const encounterSlice = createSlice({
  name: "encounter",
  initialState,
  reducers: {
    resetEncounterFinishedStatus: (state, action: PayloadAction) => {
      state.encounterFinishedStatus.error = "";
      state.encounterFinishedStatus.status = Status.NotStarted;
    },
    resetLoreEncounterResponse: (state, action: PayloadAction) => {
      state.loreEncounterResponse = "";
      state.loreEncounterResponseStatus.error = "";
      state.loreEncounterResponseStatus.status = Status.NotStarted;
    },
    resetChanceEncounterResponse: (state, action: PayloadAction) => {
      state.chanceEncounterResponse = initialState.chanceEncounterResponse;
      state.chanceEncounterResponseStatus.error = "";
      state.chanceEncounterResponseStatus.status = Status.NotStarted;
    },
    setEncounterComplete: (state, action: PayloadAction<boolean>) => {
      state.isEncounterComplete = action.payload;
    },
  },
  extraReducers(builder) {
    builder
      .addCase(updateLocationEncounterFinish.pending, (state, action) => {
        state.encounterFinishedStatus.status = Status.Loading;
      })
      .addCase(updateLocationEncounterFinish.fulfilled, (state, action: PayloadAction) => {
        state.encounterFinishedStatus.status = Status.Loaded;
      })
      .addCase(updateLocationEncounterFinish.rejected, (state, action: any) => {
        state.encounterFinishedStatus.status = Status.Failed;
      })
      .addCase(retrieveLoreEncounterQuestionResponse.pending, (state, action) => {
        state.loreEncounterResponseStatus.status = Status.Loading;
      })
      .addCase(retrieveLoreEncounterQuestionResponse.fulfilled, (state, action: PayloadAction<string>) => {
        state.loreEncounterResponse = action.payload;
        state.loreEncounterResponseStatus.status = Status.Loaded;
      })
      .addCase(retrieveLoreEncounterQuestionResponse.rejected, (state, action: any) => {
        state.loreEncounterResponseStatus.status = Status.Failed;
      })
      .addCase(retrieveChanceEncounterChoiceResult.pending, (state, action) => {
        state.chanceEncounterResponseStatus.status = Status.Loading;
      })
      .addCase(retrieveChanceEncounterChoiceResult.fulfilled, (state, action: PayloadAction<ChanceEncounterDto>) => {
        state.chanceEncounterResponse = action.payload;
        state.chanceEncounterResponseStatus.status = Status.Loaded;
      })
      .addCase(retrieveChanceEncounterChoiceResult.rejected, (state, action: any) => {
        state.chanceEncounterResponseStatus.status = Status.Failed;
      })
      .addCase(retrieveBossEncounterFinish.pending, (state, action) => {
        state.encounterFinishedStatus.status = Status.Loading;
      })
      .addCase(retrieveBossEncounterFinish.fulfilled, (state, action: PayloadAction) => {
        state.encounterFinishedStatus.status = Status.Loaded;
      })
      .addCase(retrieveBossEncounterFinish.rejected, (state, action: any) => {
        state.encounterFinishedStatus.status = Status.Failed;
      });
  },
});

// Action creators
export const {
  resetEncounterFinishedStatus,
  resetLoreEncounterResponse,
  resetChanceEncounterResponse,
  setEncounterComplete,
} = encounterSlice.actions;

// Selectors
export const selectEncounterFinishedStatus = (state: RootState) => state.encounter.encounterFinishedStatus;
export const selectLoreEncounterResponseStatus = (state: RootState) => state.encounter.loreEncounterResponseStatus;
export const selectChanceEncounterResponseStatus = (state: RootState) => state.encounter.chanceEncounterResponseStatus;
export const selectLoreEncounterResponse = (state: RootState) => state.encounter.loreEncounterResponse;
export const selectChanceEncounterResponse = (state: RootState) => state.encounter.chanceEncounterResponse;
export const selectIsEncounterComplete = (state: RootState) => state.encounter.isEncounterComplete;

export default encounterSlice.reducer;
