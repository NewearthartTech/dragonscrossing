import { createAsyncThunk, createSlice, PayloadAction, current } from "@reduxjs/toolkit";
import { LoadingStatus, Status } from "../app/appTypes";
import { RootState } from "../store/store";
import { LoginRequest, Player, PlayerSettings } from "./playerTypes";
import playerApi from "./playerApi";
import { ItemDto, PlayerDto } from "@dcx/dcx-backend";
import { setAppMessage } from "../app/appSlice";
import { formatErrorMessage } from "@/helpers/shared-functions";
import { IContractCalls } from "@/components/web3/contractCalls";

export interface PlayerState {
  player: Player;
  walletItems: Array<ItemDto>;
  connectedWalletAddress: string;
  retrievePlayerStatus: LoadingStatus;
  getPlayerStatus: LoadingStatus;
  isAuthenticated: boolean;
  stashItemMovedStatus: LoadingStatus;
  dcxBalance: number;
}

const initialState: PlayerState = {
  player: {
    id: "",
    blockchainPublicAddress: "",
    sharedStash: [],
    playerSettings: {
      autoRoll: false,
      playMusic: true,
      musicVolume: 10,
      playVoice: true,
      voiceVolume: 50,
      playSoundEffects: true,
      soundEffectsVolume: 80,
    },
  },
  walletItems: [],
  connectedWalletAddress: "",
  isAuthenticated: true,
  retrievePlayerStatus: {
    status: Status.NotStarted,
    error: "",
  },
  getPlayerStatus: {
    status: Status.NotStarted,
    error: "",
  },
  stashItemMovedStatus: {
    status: Status.NotStarted,
    error: "",
  },
  dcxBalance: 0,
};

//Thunks
export const retrievePlayer = createAsyncThunk("player/retrievePlayer", async (_, { rejectWithValue, dispatch }) => {
  try {
    return await playerApi.getPlayer();
  } catch (err: any) {
    dispatch(
      setAppMessage({
        message: formatErrorMessage(err),
        isClearToken: err.response.status === 401 ? true : false,
      })
    );
    return rejectWithValue(err);
  }
});

export const getNftItems = createAsyncThunk(
  "player/getNftItems",
  async (ownedTokenIds: Array<string>, { rejectWithValue, dispatch }) => {
    try {
      return await playerApi.getNftItems(ownedTokenIds);
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

export const storeItemInStash = createAsyncThunk(
  "player/storeItemInStash",
  async (itemId: string, { rejectWithValue, dispatch }) => {
    try {
      return await playerApi.storeItemInStash(itemId);
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

export const grabItemFromStash = createAsyncThunk(
  "player/grabItemFromStash",
  async (itemId: string, { rejectWithValue, dispatch }) => {
    try {
      return await playerApi.grabItemFromStash(itemId);
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

export const playerSlice = createSlice({
  name: "player",
  initialState,
  reducers: {
    validateCredentials: (state, action: PayloadAction<LoginRequest>) => {
      if (
        action.payload.username.match(/^DCXus3rN4me9481$/) &&
        action.payload.password.match(/^Secur3dH0r!ZonV3ntu4er#%$/)
      ) {
        state.isAuthenticated = true;
      } else {
        state.isAuthenticated = false;
      }
    },
    setPlayer: (state, action: PayloadAction<Player>) => {
      state.player = action.payload;
      state.getPlayerStatus.status = Status.Loaded;
    },
    setAutoRoll: (state, action: PayloadAction<boolean>) => {
      state.player.playerSettings.autoRoll = action.payload;
    },
    setMusic: (state, action: PayloadAction<boolean>) => {
      state.player.playerSettings.playMusic = action.payload;
    },
    setMusicVolume: (state, action: PayloadAction<number>) => {
      state.player.playerSettings.musicVolume = action.payload;
    },
    setVoice: (state, action: PayloadAction<boolean>) => {
      state.player.playerSettings.playVoice = action.payload;
    },
    setVoiceVolume: (state, action: PayloadAction<number>) => {
      state.player.playerSettings.voiceVolume = action.payload;
    },
    setSoundEffects: (state, action: PayloadAction<boolean>) => {
      state.player.playerSettings.playSoundEffects = action.payload;
    },
    setSoundEffectsVolume: (state, action: PayloadAction<number>) => {
      state.player.playerSettings.soundEffectsVolume = action.payload;
    },
    setPlayerSettingsInLocalStorage: (state, action: PayloadAction<PlayerSettings>) => {
      state.player.playerSettings = action.payload;
      localStorage.setItem("playerSettings", JSON.stringify(action.payload));
    },
    setConnectedWalletAddress: (state, action: PayloadAction<string>) => {
      state.connectedWalletAddress = action.payload;
    },
    resetRetrievePlayerStatus: (state, action: PayloadAction) => {
      state.retrievePlayerStatus.status = Status.NotStarted;
      state.retrievePlayerStatus.error = "";
    },
    setDcxBalance: (state, action: PayloadAction<number>) => {
      state.dcxBalance = action.payload;
      state.retrievePlayerStatus.error = "";
    },
    resetStashItemMovedStatus: (state, action: PayloadAction) => {
      state.stashItemMovedStatus.status = Status.NotStarted;
      state.stashItemMovedStatus.error = "";
    },
  },
  extraReducers(builder) {
    builder
      .addCase(retrievePlayer.pending, (state, action) => {
        state.retrievePlayerStatus.status = Status.Loading;
      })
      .addCase(retrievePlayer.fulfilled, (state, action: PayloadAction<PlayerDto>) => {
        state.player.id = action.payload.id;
        state.player.blockchainPublicAddress = action.payload.blockchainPublicAddress;
        state.player.sharedStash = action.payload.sharedStash;
        state.player.lastDailyResetAt = action.payload.lastDailyResetAt;
        state.player.timeTillNextReset = action.payload.timeTillNextReset;
        state.retrievePlayerStatus.status = Status.Loaded;
      })
      .addCase(retrievePlayer.rejected, (state, action: any) => {
        state.retrievePlayerStatus.status = Status.Failed;
      })
      .addCase(getNftItems.pending, (state, action) => {})
      .addCase(getNftItems.fulfilled, (state, action: PayloadAction<Array<ItemDto>>) => {
        state.walletItems = action.payload;
      })
      .addCase(getNftItems.rejected, (state, action: any) => {})
      .addCase(storeItemInStash.pending, (state, action) => {
        state.stashItemMovedStatus.status = Status.Loading;
      })
      .addCase(storeItemInStash.fulfilled, (state, action: PayloadAction<PlayerDto>) => {
        state.player.id = action.payload.id;
        state.player.blockchainPublicAddress = action.payload.blockchainPublicAddress;
        state.player.sharedStash = action.payload.sharedStash;
        state.stashItemMovedStatus.status = Status.Loaded;
      })
      .addCase(storeItemInStash.rejected, (state, action: any) => {
        state.stashItemMovedStatus.status = Status.Failed;
      })
      .addCase(grabItemFromStash.pending, (state, action) => {
        state.stashItemMovedStatus.status = Status.Loading;
      })
      .addCase(grabItemFromStash.fulfilled, (state, action: PayloadAction<PlayerDto>) => {
        state.player.id = action.payload.id;
        state.player.blockchainPublicAddress = action.payload.blockchainPublicAddress;
        state.player.sharedStash = action.payload.sharedStash;
        state.stashItemMovedStatus.status = Status.Loaded;
      })
      .addCase(grabItemFromStash.rejected, (state, action: any) => {
        state.stashItemMovedStatus.status = Status.Failed;
      });
  },
});

// Action creators
export const {
  setPlayer,
  setConnectedWalletAddress,
  validateCredentials,
  setAutoRoll,
  setMusic,
  setMusicVolume,
  setVoice,
  setVoiceVolume,
  setSoundEffects,
  setSoundEffectsVolume,
  setPlayerSettingsInLocalStorage,
  resetRetrievePlayerStatus,
  setDcxBalance,
  resetStashItemMovedStatus,
} = playerSlice.actions;

// Selectors
export const selectPlayer = (state: RootState) => state.player.player;
export const selectConnectedWalletAddress = (state: RootState) => state.player.connectedWalletAddress;
export const selectWalletItems = (state: RootState) => state.player.walletItems;
export const selectRetrievePlayerStatus = (state: RootState) => state.player.retrievePlayerStatus;
export const selectGetPlayerStatus = (state: RootState) => state.player.getPlayerStatus;
export const selectPlayerSettings = (state: RootState) => state.player.player.playerSettings;
export const selectAuthenticationStatus = (state: RootState) => state.player.isAuthenticated;
export const selectStashItemMovedStatus = (state: RootState) => state.player.stashItemMovedStatus;
export const selectDcxBalance = (state: RootState) => state.player.dcxBalance;

export default playerSlice.reducer;
