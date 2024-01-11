import { ActionResponseDto, CombatantType, ItemDto } from "@dcx/dcx-backend";
import { createAsyncThunk, createSlice, PayloadAction, current } from "@reduxjs/toolkit";
import { LoadingStatus, Status } from "../app/appTypes";
import { RootState } from "../store/store";
import combatApi from "./combatApi";
import { ActionType } from "./combatTypes";
import { initializedHero, initializedMonster } from "@/helpers/global-constants";
import { setAppMessage, setSnackbarMessage } from "../app/appSlice";
import { formatErrorMessage } from "@/helpers/shared-functions";

export interface CombatState {
  combatActionResult: ActionResponseDto;
  combatActionStatus: LoadingStatus;
  combatActionType: ActionType;
  monsterLootUpdatedStatus: LoadingStatus;
  isFleeAttempt: boolean;
  isPersuadeAttempt: boolean;
}

const initialState: CombatState = {
  combatActionResult: {
    id: "",
    type: "",
    round: -1,
    isCharismaOpportunityAvailable: false,
    didHeroFlee: false,
    isSkillUseAvailable: false,
    initiative: CombatantType.Unknown,
    heroSkillStatusEffects: [],
    monsterSpecialAbilityStatusEffects: [],
    monsterResult: {
      isDead: false,
      attackResult: {
        isHit: false,
        monsteralteredStats: [],
      },
      monster: initializedMonster,
    },
    heroResult: {
      isDead: false,
      levelLoss: -1,
      attackResult: {
        isHit: false,
        heroAlteredStats: [],
        dice: [],
      },
      hero: initializedHero,
    },
  },
  combatActionStatus: {
    status: Status.NotStarted,
    error: "",
  },
  combatActionType: ActionType.UNKNOWN,
  monsterLootUpdatedStatus: {
    status: Status.NotStarted,
    error: "",
  },
  isFleeAttempt: false,
  isPersuadeAttempt: false,
};

//Thunks
export const getCombatStartRound = createAsyncThunk(
  "combat/getCombatStartRound",
  async (undefined, { rejectWithValue, dispatch }) => {
    try {
      const response = await combatApi.getCombatStartRound();
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

export const getCombatAttack = createAsyncThunk(
  "combat/getCombatAttack",
  async (undefined, { rejectWithValue, dispatch }) => {
    try {
      const response = await combatApi.getCombatAttack();
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

export const getCombatSkill = createAsyncThunk(
  "combat/getCombatSkill",
  async (skillSlug: string, { rejectWithValue, dispatch }) => {
    try {
      const response = await combatApi.getCombatSkill(skillSlug);
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

export const getCombatPersuade = createAsyncThunk(
  "combat/postCombatPersuade",
  async (undefined, { rejectWithValue, dispatch }) => {
    try {
      const response = await combatApi.getCombatPersuade();
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

export const getCombatFlee = createAsyncThunk(
  "combat/getCombatFlee",
  async (undefined, { rejectWithValue, dispatch }) => {
    try {
      const response = await combatApi.getCombatFlee();
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

export const pickUpLootRequest = createAsyncThunk(
  "combat/pickUpLootRequest",
  async (itemIds: Array<string>, { rejectWithValue, dispatch }) => {
    try {
      const response = await combatApi.pickUpLoot(itemIds);
      return response;
    } catch (err: any) {
      dispatch(
        setSnackbarMessage({
          isOpen: true,
          message: err.response.data.message,
        })
      );
      return rejectWithValue(err);
    }
  }
);

export const combatSlice = createSlice({
  name: "combat",
  initialState,
  reducers: {
    clearCombatActionStatus: (state, action: PayloadAction) => {
      state.combatActionStatus.status = Status.NotStarted;
      state.combatActionStatus.error == "";
    },
    setCombatActionResult: (state, action: PayloadAction<ActionResponseDto>) => {
      state.combatActionResult = action.payload;
      state.combatActionStatus.status = Status.Loaded;
    },
    clearCombatActionResult: (state, action: PayloadAction) => {
      state.combatActionResult = initialState.combatActionResult;
    },
    clearCombatActionType: (state, action: PayloadAction) => {
      state.combatActionType = ActionType.UNKNOWN;
    },
    clearMonsterLootUpdatedStatus: (state, action: PayloadAction) => {
      state.monsterLootUpdatedStatus.status = Status.NotStarted;
      state.monsterLootUpdatedStatus.error = "";
    },
    setFleeAttempt: (state, action: PayloadAction<boolean>) => {
      state.isFleeAttempt = action.payload;
    },
    setPersuadeAttempt: (state, action: PayloadAction<boolean>) => {
      state.isPersuadeAttempt = action.payload;
    },
  },
  extraReducers(builder) {
    builder
      .addCase(getCombatStartRound.pending, (state, action) => {
        state.combatActionStatus.status = Status.Loading;
      })
      .addCase(getCombatStartRound.fulfilled, (state, action: PayloadAction<ActionResponseDto>) => {
        state.combatActionResult = action.payload;
        state.combatActionType = ActionType.START_ROUND;
        state.combatActionStatus.status = Status.Loaded;
      })
      .addCase(getCombatStartRound.rejected, (state, action: any) => {
        state.combatActionStatus.status = Status.Failed;
      })
      .addCase(getCombatAttack.pending, (state, action) => {
        state.combatActionStatus.status = Status.Loading;
      })
      .addCase(getCombatAttack.fulfilled, (state, action: PayloadAction<ActionResponseDto>) => {
        state.combatActionResult = action.payload;
        state.combatActionType = ActionType.ATTACK;
        state.combatActionStatus.status = Status.Loaded;
      })
      .addCase(getCombatAttack.rejected, (state, action: any) => {
        state.combatActionStatus.status = Status.Failed;
      })
      .addCase(getCombatSkill.pending, (state, action) => {
        state.combatActionStatus.status = Status.Loading;
      })
      .addCase(getCombatSkill.fulfilled, (state, action: PayloadAction<ActionResponseDto>) => {
        state.combatActionResult = action.payload;
        state.combatActionType = ActionType.SKILL;
        state.combatActionStatus.status = Status.Loaded;
      })
      .addCase(getCombatSkill.rejected, (state, action: any) => {
        state.combatActionStatus.status = Status.Failed;
      })
      .addCase(getCombatPersuade.pending, (state, action) => {
        state.combatActionStatus.status = Status.Loading;
      })
      .addCase(getCombatPersuade.fulfilled, (state, action: PayloadAction<ActionResponseDto>) => {
        state.combatActionResult = action.payload;
        state.combatActionType = ActionType.PERSUADE;
        state.combatActionStatus.status = Status.Loaded;
      })
      .addCase(getCombatPersuade.rejected, (state, action: any) => {
        state.combatActionStatus.status = Status.Failed;
      })
      .addCase(getCombatFlee.pending, (state, action) => {
        state.combatActionStatus.status = Status.Loading;
      })
      .addCase(getCombatFlee.fulfilled, (state, action: PayloadAction<ActionResponseDto>) => {
        state.combatActionResult = action.payload;
        state.combatActionType = ActionType.FLEE;
        state.combatActionStatus.status = Status.Loaded;
      })
      .addCase(getCombatFlee.rejected, (state, action: any) => {
        state.combatActionStatus.status = Status.Failed;
      })
      .addCase(pickUpLootRequest.pending, (state, action) => {
        state.monsterLootUpdatedStatus.status = Status.Loading;
      })
      .addCase(pickUpLootRequest.fulfilled, (state, action: PayloadAction<Array<ItemDto>>) => {
        if (state.combatActionResult.monsterResult.loot) {
          state.combatActionResult.monsterResult.loot.items = action.payload;
          state.monsterLootUpdatedStatus.status = Status.Loaded;
        }
      })
      .addCase(pickUpLootRequest.rejected, (state, action: any) => {
        state.monsterLootUpdatedStatus.status = Status.Failed;
      });
  },
});

// Action creators
export const {
  clearCombatActionStatus,
  clearCombatActionResult,
  setCombatActionResult,
  clearCombatActionType,
  setFleeAttempt,
  setPersuadeAttempt,
  clearMonsterLootUpdatedStatus,
} = combatSlice.actions;

// Selectors
export const selectCombatActionResult = (state: RootState) => state.combat.combatActionResult;
export const selectCombatActionResultStatus = (state: RootState) => state.combat.combatActionStatus.status;
export const selectCombatActionType = (state: RootState) => state.combat.combatActionType;
export const selectMonsterLootUpdatedStatus = (state: RootState) => state.combat.monsterLootUpdatedStatus;
export const selectFleeAttempt = (state: RootState) => state.combat.isFleeAttempt;
export const selectPersuadeAttempt = (state: RootState) => state.combat.isPersuadeAttempt;

export default combatSlice.reducer;
