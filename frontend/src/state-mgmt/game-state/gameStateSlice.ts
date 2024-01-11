import { formatErrorMessage } from "@/helpers/shared-functions";
import { DcxTiles, DcxZones, GameStateDto } from "@dcx/dcx-backend";
import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { setAppMessage } from "../app/appSlice";
import { LoadingStatus, Status } from "../app/appTypes";
import { RootState } from "../store/store";
import gameStateApi from "./gameStateApi";
import { GameState } from "./gameStateTypes";

export interface GameStateState {
  gameState: GameState;
  gameStateStatus: LoadingStatus;
  polledGameState: GameState;
}

const initialState: GameStateState = {
  gameState: {
    slug: DcxTiles.Unknown,
    zone: {
      slug: DcxZones.Unknown,
      discoveredTiles: [],
      undiscoveredTileCount: 0,
    },
    encounters: [],
  },
  gameStateStatus: {
    status: Status.NotStarted,
    error: "",
  },
  polledGameState: {
    slug: DcxTiles.Unknown,
    zone: {
      slug: DcxZones.Unknown,
      discoveredTiles: [],
      undiscoveredTileCount: 0,
    },
    encounters: [],
  },
};

//Thunks
export const getGameState = createAsyncThunk(
  "gameState/getGameState",
  async (heroId: number, { rejectWithValue, dispatch }) => {
    try {
      const response = await gameStateApi.getGameState(heroId);
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

export const getPollGameState = createAsyncThunk(
  "gameState/getPollGameState",
  async (heroId: number, { rejectWithValue, dispatch }) => {
    try {
      const response = await gameStateApi.getGameState(heroId);
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

export const updateGameState = createAsyncThunk(
  "gameState/updateGameState",
  async (slug: DcxTiles, { rejectWithValue, dispatch }) => {
    try {
      const response = await gameStateApi.updateGameStateLocation(slug);
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

export const gameStateSlice = createSlice({
  name: "gameState",
  initialState,
  reducers: {
    resetGameStateStatuses: (state, action: PayloadAction) => {
      state.gameStateStatus.status = Status.NotStarted;
      state.gameStateStatus.error = "";
    },
    setGameState: (state, action: PayloadAction<GameStateDto>) => {
      state.gameStateStatus.status = Status.Loading;
      state.gameState = action.payload;
      state.gameStateStatus.status = Status.Loaded;
    },
    clearGameState: (state, action: PayloadAction) => {
      state.gameState = initialState.gameState;
      state.gameStateStatus.status = Status.NotStarted;
      state.gameStateStatus.error = "";
    },
  },
  extraReducers(builder) {
    builder
      .addCase(getGameState.pending, (state, action) => {
        state.gameStateStatus.status = Status.Loading;
      })
      .addCase(getGameState.fulfilled, (state, action: PayloadAction<GameStateDto>) => {
        state.gameState = action.payload;
        state.gameStateStatus.status = Status.Loaded;
      })
      .addCase(getGameState.rejected, (state, action: any) => {
        state.gameStateStatus.status = Status.Failed;
      })
      .addCase(updateGameState.pending, (state, action) => {
        state.gameStateStatus.status = Status.Loading;
      })
      .addCase(updateGameState.fulfilled, (state, action: PayloadAction<GameStateDto>) => {
        state.gameState = action.payload;
        state.gameStateStatus.status = Status.Loaded;
      })
      .addCase(updateGameState.rejected, (state, action: any) => {
        state.gameStateStatus.status = Status.Failed;
      })
      .addCase(getPollGameState.pending, (state, action) => {})
      .addCase(getPollGameState.fulfilled, (state, action: PayloadAction<GameStateDto>) => {
        state.polledGameState = action.payload;
      })
      .addCase(getPollGameState.rejected, (state, action: any) => {});
  },
});

// Action creators
export const { resetGameStateStatuses, setGameState, clearGameState } = gameStateSlice.actions;

// Selectors
export const selectGameState = (state: RootState) => state.game.gameState;
export const selectGameStateStatus = (state: RootState) => state.game.gameStateStatus;
export const selectPolledGameState = (state: RootState) => state.game.polledGameState;

export default gameStateSlice.reducer;
