import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { FixDiceRequest } from "./testingTypes";
import testingApi from "./testingApi";
import { RootState } from "../store/store";

export interface TestingState {
  isTestEndpointsAvailable: boolean;
}

const initialState: TestingState = {
  isTestEndpointsAvailable: false,
};

//Thunks
export const fixDice = createAsyncThunk(
  "testing/fixDice",
  async (fixDiceRequest: FixDiceRequest, { rejectWithValue }) => {
    try {
      const response = await testingApi.fixDice(fixDiceRequest);
      return response;
    } catch (err: any) {
      return rejectWithValue(err);
    }
  }
);

export const resetQuests = createAsyncThunk("testing/resetQuests", async (heroId: number, { rejectWithValue }) => {
  try {
    const response = await testingApi.resetQuests(heroId);
    return response;
  } catch (err: any) {
    return rejectWithValue(err);
  }
});

export const learnSkill = createAsyncThunk("testing/learnSkill", async (skillSlug: string, { rejectWithValue }) => {
  try {
    const response = await testingApi.learnSkill(skillSlug);
    return response;
  } catch (err: any) {
    return rejectWithValue(err);
  }
});

export const testingSlice = createSlice({
  name: "testing",
  initialState,
  reducers: {
    setIsTestEndpointsAvailable: (state, action: PayloadAction<boolean>) => {
      state.isTestEndpointsAvailable = action.payload;
    },
  },
});

// Action creators
export const { setIsTestEndpointsAvailable } = testingSlice.actions;

// Selectors
export const selectIsTestEndpointsAvailable = (state: RootState) => state.testing.isTestEndpointsAvailable;

export default testingSlice.reducer;
