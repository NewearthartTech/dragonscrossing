import { createAsyncThunk, createSlice, PayloadAction, current } from "@reduxjs/toolkit";
import { LoadingStatus, Status } from "../app/appTypes";
import { RootState } from "../store/store";
import heroApi from "./heroApi";
import { v4 } from "uuid";
import {
  EquipItemRequestDto,
  HeroDto,
  ItemDto,
  ItemSlotTypeDto,
  LevelUpOrder,
  MoveItemRequestDto,
} from "@dcx/dcx-backend";
import { EquipUnequipItemRequest, HeroList, SelectedHero } from "./heroTypes";
import { IContractCalls } from "@/components/web3/contractCalls";
import { initializedHero } from "@/helpers/global-constants";
import { setAppMessage, setSnackbarMessage } from "../app/appSlice";
import { formatErrorMessage } from "@/helpers/shared-functions";

export interface HeroState {
  heroList: HeroList;
  selectedHero: SelectedHero;
  isNewHeroSelected: boolean;
  levelUpResponse: LevelUpOrder;
  getHeroesStatus: LoadingStatus;
  selectedHeroStatus: LoadingStatus;
  allocateSkillPointsStatus: LoadingStatus;
  deAllocateSkillPointsStatus: LoadingStatus;
  forgetSkillStatus: LoadingStatus;
  levelUpStatus: LoadingStatus;
  levelUpPollingStatus: LoadingStatus;
  levelUpHeroSubmittedStatus: string;
  inventoryItemMovedStatus: LoadingStatus;
  inventoryUpdatedStatus: LoadingStatus;
  draggedItem: ItemDto;
  heroHealedStatus: LoadingStatus;
  getSelectedHeroStatus: LoadingStatus;
  buyItemStatus: LoadingStatus;
  sellItemStatus: LoadingStatus;
  keepItemAtDeathStatus: LoadingStatus;
}

const initialState: HeroState = {
  heroList: {
    heroes: [],
  },
  selectedHero: {
    hero: initializedHero,
  },
  isNewHeroSelected: false,
  levelUpResponse: {
    type: "",
    id: "",
    chainId:0,
    isCompleted: false,
    priceInDcx: -1,
    heroId: -1,
    statsPoints: {
      basePoints: -1,
      xtraPoints: -1,
    },
    skillPoints: {
      basePoints: -1,
      xtraPoints: -1,
    },
    hitPoints: {
      basePoints: -1,
      xtraPoints: -1,
    },
    chosenProps: {},
    classSpecificProps: {},
    currentLevel: -1,
    newLevel: -1,
    diceRolls: [],
  },
  getHeroesStatus: {
    status: Status.NotStarted,
    error: "",
  },
  selectedHeroStatus: {
    status: Status.NotStarted,
    error: "",
  },
  allocateSkillPointsStatus: {
    status: Status.NotStarted,
    error: "",
  },
  deAllocateSkillPointsStatus: {
    status: Status.NotStarted,
    error: "",
  },
  forgetSkillStatus: {
    status: Status.NotStarted,
    error: "",
  },
  levelUpStatus: {
    status: Status.NotStarted,
    error: "",
  },
  levelUpPollingStatus: {
    status: Status.NotStarted,
    error: "",
  },
  levelUpHeroSubmittedStatus: "",
  inventoryItemMovedStatus: {
    status: Status.NotStarted,
    error: "",
  },
  inventoryUpdatedStatus: {
    status: Status.NotStarted,
    error: "",
  },
  draggedItem: {
    type: "",
    id: "",
    name: "",
    slug: "",
    itemDropSound: "",
    slot: ItemSlotTypeDto.Unknown,
    levelRequirement: -1,
    dieDamage: [],
    affectedAttributes: { [""]: -1 },
    allowedHeroClassList: [],
    heroStatComplianceDictionary: { [""]: -1 },
    isDefaultChain:true,
  },
  heroHealedStatus: {
    status: Status.NotStarted,
    error: "",
  },
  getSelectedHeroStatus: {
    status: Status.NotStarted,
    error: "",
  },
  buyItemStatus: {
    status: Status.NotStarted,
    error: "",
  },
  sellItemStatus: {
    status: Status.NotStarted,
    error: "",
  },
  keepItemAtDeathStatus: {
    status: Status.NotStarted,
    error: "",
  },
};

//Thunks
export const getHeroes = createAsyncThunk(
  "hero/getHeroes",
  async (ownedTokenIds: Array<string>, { rejectWithValue, dispatch }) => {
    try {
      return await heroApi.getHeroes(ownedTokenIds);
    } catch (err: any) {
      // Only on 401 clear token, otherwise just display the message
      dispatch(
        setAppMessage({
          message: formatErrorMessage(err),
          isClearToken: err.response.status === 401 ? true : false,
        })
      );
      return rejectWithValue(err);
    }
  }
);

export const getSelectedHero = createAsyncThunk("hero/getSelectedHero", async (undefined, { rejectWithValue }) => {
  try {
    return await heroApi.getHero();
  } catch (err: any) {
    return rejectWithValue(err);
  }
});

export const getTokenHero = createAsyncThunk("hero/getTokenHero", async (_, { rejectWithValue }) => {
  try {
    return await heroApi.getHero();
  } catch (err: any) {
    return rejectWithValue(err);
  }
});

export const getAllocateSkillPoints = createAsyncThunk(
  "hero/getAllocateSkillPoints",
  async (skillId: string, { rejectWithValue, dispatch }) => {
    try {
      return await heroApi.getAllocateSkillPoints(skillId);
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

export const getDeAllocateSkillPoints = createAsyncThunk(
  "hero/getDeAllocateSkillPoints",
  async (skillId: string, { rejectWithValue, dispatch }) => {
    try {
      return await heroApi.getDeAllocateSkillPoints(skillId);
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

export const getForgetSkill = createAsyncThunk(
  "hero/getForgetSkill",
  async (skillId: string, { rejectWithValue, dispatch }) => {
    try {
      return await heroApi.getForgetSkill(skillId);
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

export const getLevelUp = createAsyncThunk("hero/getLevelUp", async (level: number, { rejectWithValue, dispatch }) => {
  try {
    return await heroApi.getLevelUpHero(level);
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

export const getLevelUpPolling = createAsyncThunk(
  "hero/getLevelUpPolling",
  async (level: number, { rejectWithValue, dispatch }) => {
    try {
      return await heroApi.getLevelUpHero(level);
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

export const submitLevelUpTransaction = createAsyncThunk(
  "hero/submitLevelUpTransaction",
  async (levelUpOrder: LevelUpOrder & { connect: IContractCalls }, { rejectWithValue }) => {
    try {
      const { connect, ...order } = levelUpOrder;
      // Non-Blockchain logic:
      return await heroApi.postLevelUpHero({ ...order, fulfillmentTxnHash: `DUMMY HASH -${v4()}` });

      // Blockchain logic:
      // await heroApi.postLevelUpHero(order);
      // if (!levelUpOrder.priceInDcx) {
      //   throw new Error("Price In DCX is empty.");
      // }
      // const hash = await connect.levelUp({
      //   amountInDcx: levelUpOrder.priceInDcx,
      //   orderId: levelUpOrder.id,
      // });
      // // Run test here. Kill the abckend when the code stops here. What will likely happen is
      // // We will take their money but they won't actually get levelled up. We need to handle this properly.
      // return await heroApi.postLevelUpHero({
      //   ...order,
      //   fulfillmentTxnHash: hash,
      // });
    } catch (err: any) {
      return rejectWithValue(err);
    }
  }
);

export const buyBlacksmithItem = createAsyncThunk(
  "hero/buyBlacksmithItem",
  async (itemSlug: string, { rejectWithValue, dispatch }) => {
    try {
      return await heroApi.buyBlacksmithItem(itemSlug);
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

export const sellItemToBlacksmith = createAsyncThunk(
  "hero/sellItemToBlacksmith",
  async (itemIds: string[], { rejectWithValue, dispatch }) => {
    try {
      return await heroApi.sellItemToBlacksmith(itemIds);
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

export const buyHeal = createAsyncThunk("hero/buyHeal", async (isFullHeal: boolean, { rejectWithValue, dispatch }) => {
  try {
    return await heroApi.updateHealHero(isFullHeal);
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

export const moveItemRequest = createAsyncThunk(
  "hero/moveItemRequest",
  async (moveItemRequest: MoveItemRequestDto, { rejectWithValue, dispatch }) => {
    try {
      const response = await heroApi.moveItem(moveItemRequest);
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

export const equipItemRequest = createAsyncThunk(
  "hero/equipItemRequest",
  async (equipItemRequest: EquipUnequipItemRequest, { rejectWithValue, dispatch }) => {
    try {
      const response = await heroApi.equipItem(equipItemRequest);
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

export const unequipItemRequest = createAsyncThunk(
  "hero/unequipItemRequest",
  async (unequipItemRequest: EquipUnequipItemRequest, { rejectWithValue, dispatch }) => {
    try {
      const response = await heroApi.unequipItem(unequipItemRequest);
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

export const destroyItemRequest = createAsyncThunk(
  "hero/destroyItemRequest",
  async (destroyItemRequest: EquipItemRequestDto, { rejectWithValue, dispatch }) => {
    try {
      const response = await heroApi.destroyItem(destroyItemRequest);
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

export const keepItemAtDeath = createAsyncThunk(
  "hero/keepItemAtDeath",
  async (itemId: string, { rejectWithValue, dispatch }) => {
    try {
      const response = await heroApi.keepItemAtDeath(itemId);
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

export const heroSlice = createSlice({
  name: "hero",
  initialState,
  reducers: {
    setSelectedHero: (state, action: PayloadAction<HeroDto>) => {
      state.selectedHero.hero = action.payload;
      state.selectedHeroStatus.status = Status.Loaded;
    },
    clearSelectedHero: (state, _action: PayloadAction) => {
      state.selectedHero = initialState.selectedHero;
    },
    setNewHeroSelected: (state, action: PayloadAction<boolean>) => {
      state.isNewHeroSelected = action.payload;
    },
    resetForgetSkillStatus: (state, action: PayloadAction) => {
      state.forgetSkillStatus.status = Status.NotStarted;
      state.forgetSkillStatus.error = "";
    },
    setLevelUpHeroSubmittedStatus: (state, action: PayloadAction<string>) => {
      state.levelUpHeroSubmittedStatus = action.payload;
    },
    resetInventoryItemMovedStatus: (state, action: PayloadAction) => {
      state.inventoryItemMovedStatus.status = Status.NotStarted;
      state.inventoryItemMovedStatus.error = "";
    },
    setDraggedItem: (state, action: PayloadAction<ItemDto>) => {
      state.draggedItem = action.payload;
    },
    resetBuyItemStatus: (state, action: PayloadAction) => {
      state.buyItemStatus.status = Status.NotStarted;
      state.buyItemStatus.error = "";
    },
    resetSellItemStatus: (state, action: PayloadAction) => {
      state.sellItemStatus.status = Status.NotStarted;
      state.sellItemStatus.error = "";
    },
    resetKeepItemAtDeathStatus: (state, action: PayloadAction) => {
      state.keepItemAtDeathStatus.status = Status.NotStarted;
      state.keepItemAtDeathStatus.error = "";
    },
    resetSelectedHeroStatus: (state, action: PayloadAction) => {
      state.selectedHeroStatus.status = Status.NotStarted;
      state.selectedHeroStatus.error = "";
    },
    resetHeroHealedStatus: (state, action: PayloadAction) => {
      state.heroHealedStatus.status = Status.NotStarted;
      state.heroHealedStatus.error = "";
    },
    resetLevelUpStatus: (state, action: PayloadAction) => {
      state.levelUpStatus.status = Status.NotStarted;
      state.levelUpStatus.error = "";
    },
    resetLevelUpPollingStatus: (state, action: PayloadAction) => {
      state.levelUpPollingStatus.status = Status.NotStarted;
      state.levelUpPollingStatus.error = "";
    },
    resetAllocateSkillPointsStatus: (state, action: PayloadAction) => {
      state.allocateSkillPointsStatus.status = Status.NotStarted;
      state.allocateSkillPointsStatus.error = "";
    },
    resetDeAllocateSkillPointsStatus: (state, action: PayloadAction) => {
      state.deAllocateSkillPointsStatus.status = Status.NotStarted;
      state.deAllocateSkillPointsStatus.error = "";
    },
  },
  extraReducers(builder) {
    builder
      .addCase(getHeroes.pending, (state) => {
        state.getHeroesStatus.status = Status.Loading;
      })
      .addCase(getHeroes.fulfilled, (state, action: PayloadAction<Array<HeroDto>>) => {
        state.heroList.heroes = action.payload;
        state.getHeroesStatus.status = Status.Loaded;
      })
      .addCase(getHeroes.rejected, (state, action: any) => {
        state.getHeroesStatus.status = Status.Failed;
      })
      .addCase(getSelectedHero.pending, (state) => {
        state.getSelectedHeroStatus.status = Status.Loading;
      })
      .addCase(getSelectedHero.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.getSelectedHeroStatus.status = Status.Loaded;
      })
      .addCase(getSelectedHero.rejected, (state, action: any) => {
        state.getSelectedHeroStatus.status = Status.Failed;
      })
      .addCase(getTokenHero.pending, (state) => {
        state.selectedHeroStatus.status = Status.Loading;
      })
      .addCase(getTokenHero.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.selectedHeroStatus.status = Status.Loaded;
      })
      .addCase(getTokenHero.rejected, (state, action: any) => {
        state.selectedHeroStatus.status = Status.Failed;
      })
      .addCase(getAllocateSkillPoints.pending, (state, action) => {
        state.allocateSkillPointsStatus.status = Status.Loading;
      })
      .addCase(getAllocateSkillPoints.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.allocateSkillPointsStatus.status = Status.Loaded;
      })
      .addCase(getAllocateSkillPoints.rejected, (state, action: any) => {
        state.allocateSkillPointsStatus.status = Status.Failed;
      })
      .addCase(getDeAllocateSkillPoints.pending, (state, action) => {
        state.deAllocateSkillPointsStatus.status = Status.Loading;
      })
      .addCase(getDeAllocateSkillPoints.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.deAllocateSkillPointsStatus.status = Status.Loaded;
      })
      .addCase(getDeAllocateSkillPoints.rejected, (state, action: any) => {
        state.deAllocateSkillPointsStatus.status = Status.Failed;
      })
      .addCase(getForgetSkill.pending, (state, action) => {
        state.forgetSkillStatus.status = Status.Loading;
      })
      .addCase(getForgetSkill.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.forgetSkillStatus.status = Status.Loaded;
      })
      .addCase(getForgetSkill.rejected, (state, action: any) => {
        state.forgetSkillStatus.status = Status.Failed;
      })
      .addCase(getLevelUp.pending, (state, action) => {
        state.levelUpStatus.status = Status.Loading;
      })
      .addCase(getLevelUp.fulfilled, (state, action: PayloadAction<LevelUpOrder>) => {
        state.levelUpResponse = action.payload;
        state.levelUpStatus.status = Status.Loaded;
      })
      .addCase(getLevelUp.rejected, (state, action: any) => {
        state.levelUpStatus.status = Status.Failed;
      })
      .addCase(getLevelUpPolling.pending, (state, action) => {
        state.levelUpPollingStatus.status === Status.Loading;
      })
      .addCase(getLevelUpPolling.fulfilled, (state, action: PayloadAction<LevelUpOrder>) => {
        state.levelUpResponse = action.payload;
        state.levelUpPollingStatus.status = Status.Loaded;
      })
      .addCase(getLevelUpPolling.rejected, (state, action: any) => {
        state.levelUpPollingStatus.status = Status.Failed;
      })
      .addCase(submitLevelUpTransaction.pending, (state, action) => {
        state.levelUpHeroSubmittedStatus = "submitting";
      })
      .addCase(submitLevelUpTransaction.fulfilled, (state, action: PayloadAction<LevelUpOrder>) => {
        state.levelUpResponse = action.payload;
        state.levelUpHeroSubmittedStatus = "processing";
      })
      .addCase(submitLevelUpTransaction.rejected, (state, action: any) => {
        state.levelUpHeroSubmittedStatus = "failed";
      })
      .addCase(buyBlacksmithItem.pending, (state, action) => {
        state.inventoryUpdatedStatus.status = Status.Loading;
        state.buyItemStatus.status = Status.Loading;
      })
      .addCase(buyBlacksmithItem.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.inventoryUpdatedStatus.status = Status.Loaded;
        state.buyItemStatus.status = Status.Loaded;
      })
      .addCase(buyBlacksmithItem.rejected, (state, action: any) => {
        state.inventoryUpdatedStatus.status = Status.Failed;
        state.buyItemStatus.status = Status.Failed;
      })
      .addCase(sellItemToBlacksmith.pending, (state, action) => {
        state.inventoryUpdatedStatus.status = Status.Loading;
        state.sellItemStatus.status = Status.Loading;
      })
      .addCase(sellItemToBlacksmith.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.inventoryUpdatedStatus.status = Status.Loaded;
        state.sellItemStatus.status = Status.Loaded;
      })
      .addCase(sellItemToBlacksmith.rejected, (state, action: any) => {
        state.inventoryUpdatedStatus.status = Status.Failed;
        state.sellItemStatus.status = Status.Failed;
      })
      .addCase(buyHeal.pending, (state, action) => {
        state.heroHealedStatus.status = Status.Loading;
      })
      .addCase(buyHeal.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.heroHealedStatus.status = Status.Loaded;
      })
      .addCase(buyHeal.rejected, (state, action: any) => {
        state.heroHealedStatus.status = Status.Failed;
      })
      .addCase(moveItemRequest.pending, (state, action) => {
        state.inventoryItemMovedStatus.status = Status.Loading;
      })
      .addCase(moveItemRequest.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.inventoryItemMovedStatus.status = Status.Loaded;
      })
      .addCase(moveItemRequest.rejected, (state, action: any) => {
        state.inventoryItemMovedStatus.status = Status.Failed;
      })
      .addCase(equipItemRequest.pending, (state, action) => {
        state.inventoryItemMovedStatus.status = Status.Loading;
      })
      .addCase(equipItemRequest.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.inventoryItemMovedStatus.status = Status.Loaded;
      })
      .addCase(equipItemRequest.rejected, (state, action: any) => {
        state.inventoryItemMovedStatus.status = Status.Failed;
      })
      .addCase(unequipItemRequest.pending, (state, action) => {
        state.inventoryItemMovedStatus.status = Status.Loading;
      })
      .addCase(unequipItemRequest.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.inventoryItemMovedStatus.status = Status.Loaded;
      })
      .addCase(unequipItemRequest.rejected, (state, action: any) => {
        state.inventoryItemMovedStatus.status = Status.Failed;
      })
      .addCase(destroyItemRequest.pending, (state, action) => {
        state.inventoryItemMovedStatus.status = Status.Loading;
      })
      .addCase(destroyItemRequest.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.inventoryItemMovedStatus.status = Status.Loaded;
      })
      .addCase(destroyItemRequest.rejected, (state, action: any) => {
        state.inventoryItemMovedStatus.status = Status.Failed;
      })
      .addCase(keepItemAtDeath.pending, (state, action) => {
        state.keepItemAtDeathStatus.status = Status.Loading;
      })
      .addCase(keepItemAtDeath.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.selectedHero.hero = action.payload;
        state.keepItemAtDeathStatus.status = Status.Loaded;
      })
      .addCase(keepItemAtDeath.rejected, (state, action: any) => {
        state.keepItemAtDeathStatus.status = Status.Failed;
      });
  },
});

// Action creators
export const {
  resetForgetSkillStatus,
  setLevelUpHeroSubmittedStatus,
  setSelectedHero,
  clearSelectedHero,
  resetInventoryItemMovedStatus,
  setDraggedItem,
  resetBuyItemStatus,
  resetSellItemStatus,
  resetKeepItemAtDeathStatus,
  resetSelectedHeroStatus,
  resetHeroHealedStatus,
  resetLevelUpStatus,
  resetLevelUpPollingStatus,
  resetAllocateSkillPointsStatus,
  resetDeAllocateSkillPointsStatus,
} = heroSlice.actions;

// Selectors
export const selectHeroes = (state: RootState) => state.hero.heroList;
export const selectGetHeroesStatus = (state: RootState) => state.hero.getHeroesStatus;
export const selectSelectedHero = (state: RootState) => state.hero.selectedHero;
export const selectSelectedHeroStatus = (state: RootState) => state.hero.selectedHeroStatus;
export const selectEquippedItems = (state: RootState) => state.hero.selectedHero.hero.equippedItems;
export const selectAllocateSkillPointsStatus = (state: RootState) => state.hero.allocateSkillPointsStatus;
export const selectDeAllocateSkillPointsStatus = (state: RootState) => state.hero.deAllocateSkillPointsStatus;
export const selectForgetSkillStatus = (state: RootState) => state.hero.forgetSkillStatus;
export const selectLevelUpResponse = (state: RootState) => state.hero.levelUpResponse;
export const selectLevelUpStatus = (state: RootState) => state.hero.levelUpStatus;
export const selectLevelUpPollingStatus = (state: RootState) => state.hero.levelUpPollingStatus;
export const selectLevelUpHeroSubmittedStatus = (state: RootState) => state.hero.levelUpHeroSubmittedStatus;
export const selectInventory = (state: RootState) => state.hero.selectedHero.hero.inventory;
export const selectInventoryItemMovedStatus = (state: RootState) => state.hero.inventoryItemMovedStatus;
export const selectInventoryUpdatedStatus = (state: RootState) => state.hero.inventoryUpdatedStatus;
export const selectDraggedItem = (state: RootState) => state.hero.draggedItem;
export const selectHeroHealedStatus = (state: RootState) => state.hero.heroHealedStatus;
export const selectGetSelectedHeroStatus = (state: RootState) => state.hero.getSelectedHeroStatus;
export const selectBuyItemStatus = (state: RootState) => state.hero.buyItemStatus;
export const selectSellItemStatus = (state: RootState) => state.hero.sellItemStatus;
export const selectKeepItemAtDeathStatus = (state: RootState) => state.hero.keepItemAtDeathStatus;

export default heroSlice.reducer;
