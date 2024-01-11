import { IContractCalls } from "@/components/web3/contractCalls";
import { initializedHero } from "@/helpers/global-constants";
import { formatErrorMessage } from "@/helpers/shared-functions";
import { HerbalistDto, HeroDto, IdentifySkillOrder, ItemDto, LearnSkillOrder, NftActionOrder, SummonHeroOrder } from "@dcx/dcx-backend";
import { createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { setAppMessage } from "../app/appSlice";
import { LoadingStatus, Status } from "../app/appTypes";
import { RootState } from "../store/store";
import vendorApi from "./vendorApi";
import { ItemsToSell,ToggleItemToSell } from "./vendorTypes";

export interface VendorState {
  blacksmithItems: Array<ItemDto>;
  blacksmithItemsStatus: LoadingStatus;
  itemsToSell: ItemsToSell;
  summonedHero: HeroDto;
  getSummonedHeroByIdStatus: LoadingStatus;
  summonHeroResponse: SummonHeroOrder;
  getSummonHeroStatus: LoadingStatus;
  summonHeroSubmittedStatus: string;
  herbalistOptions: Array<HerbalistDto>;
  getHerbalistOptionsStatus: LoadingStatus;
  identifySkillResponse: IdentifySkillOrder;
  getIdentifySkillStatus: LoadingStatus;
  identifySkillSubmittedStatus: string;
  skillToIdentifyId: string;
  skillIdentifyMessage: string;
  skillLearnMessage: string;

  NFTActionUseResponse: NftActionOrder;
  getNFTActionUseStatus: LoadingStatus;
  NFTActionUseSubmittedStatus: string;

  learnSkillResponse: IdentifySkillOrder;
  getLearnSkillStatus: LoadingStatus;
  learnSkillSubmittedStatus: string;


  skillToLearnId: string;
  consumedNFTTokenIds: Array<number>;
  consumeNFTMessage: string;
  summonedHeroId: number;
  updateOwnerNftIds: Array<number>;
}

const initialState: VendorState = {
  blacksmithItems: [],
  blacksmithItemsStatus: {
    status: Status.NotStarted,
    error: "",
  },
  itemsToSell: {
    items:[],
    showConfirmation: false,
  },
  summonedHero: initializedHero,
  getSummonedHeroByIdStatus: {
    status: Status.NotStarted,
    error: "",
  },
  getSummonHeroStatus: {
    status: Status.NotStarted,
    error: "",
  },
  summonHeroSubmittedStatus: "",
  summonHeroResponse: {
    type: "",
    id: "",
    isCompleted: false,
    priceInDcx: -1,
    nftTokenId: -1,
    authorization: "",
    userId: "",
    orderHash: "",
    mintProps: "",
    heroIdToTransfer: 0,
    chainId:0,
  },
  herbalistOptions: [],
  getHerbalistOptionsStatus: {
    status: Status.NotStarted,
    error: "",
  },
  
  identifySkillResponse: {
    type: "",
    id: "",
    isCompleted: false,
    priceInDcx: -1,
    nftTokenId: -1,
    authorization: "",
    userId: "",
    chainId:0,
  },
  getIdentifySkillStatus: {
    status: Status.NotStarted,
    error: "",
  },
  identifySkillSubmittedStatus: "",

  NFTActionUseResponse: {
    type: "",
    id: "",
    isCompleted: false,
    priceInDcx: -1,
    nftTokenId: -1,
    authorization: "",
    userId: "",
    chainId:0,
  },
  getNFTActionUseStatus: {
    status: Status.NotStarted,
    error: "",
  },
  NFTActionUseSubmittedStatus: "",


  skillToIdentifyId: "",
  skillIdentifyMessage: "",
  skillLearnMessage: "",
  learnSkillResponse: {
    type: "",
    id: "",
    isCompleted: false,
    priceInDcx: -1,
    nftTokenId: -1,
    authorization: "",
    userId: "",
    chainId:0,
  },
  getLearnSkillStatus: {
    status: Status.NotStarted,
    error: "",
  },
  learnSkillSubmittedStatus: "",
  skillToLearnId: "",
  consumedNFTTokenIds: [],
  consumeNFTMessage: "",
  summonedHeroId: -1,
  updateOwnerNftIds: [],
};

//Thunks
export const getBlacksmithItems = createAsyncThunk(
  "vendor/getBlacksmithItems",
  async (_, { rejectWithValue, dispatch }) => {
    try {
      return await vendorApi.getBlacksmithItems();
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

export const getNftActionUse = createAsyncThunk(
  "vendor/getNftActionUse",
  async (nftTokenId: number, { rejectWithValue, dispatch }) => {
    try {
      return await vendorApi.getNFTActionUse(nftTokenId);
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

export const submitNftActionUseTransaction = createAsyncThunk(
  "vendor/submitNftActionUseTransaction",
  async (nftUseActionOrder: NftActionOrder & { connect: IContractCalls }, { rejectWithValue, dispatch }) => {
    try {
      const { connect, ...order } = nftUseActionOrder;
      const hash = await connect.actionNftUse(
        nftUseActionOrder,
        (message) => dispatch(setNFtConsumptionMessage(message))
      );

      return vendorApi.postNFTActionUse({
        ...order,
        fulfillmentTxnHash: hash,
      });
    } catch (err: any) {
      return rejectWithValue(err);
    }
  }
);


export const getSummonHero = createAsyncThunk(
  "vendor/getSummonHero",
  async (nftTokenId: number, { rejectWithValue, dispatch }) => {
    try {
      return await vendorApi.getSummonHero(nftTokenId);
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

export const submitSummonHeroTransaction = createAsyncThunk(
  "vendor/submitSummonHeroTransaction",
  async (summonHeroOrder: SummonHeroOrder & { connect: IContractCalls }, { rejectWithValue, dispatch }) => {
    try {
      const { connect, ...order } = summonHeroOrder;
      if (!summonHeroOrder.priceInDcx) {
        throw new Error("Price In DCX is empty.");
      }
      const hash = await connect.summonHero(
        summonHeroOrder,
        (message) => dispatch(setNFtConsumptionMessage(message)),
        (heroId) => dispatch(setSummonedHeroId(heroId))
      );
      return vendorApi.postSummonHero({
        ...order,
        fulfillmentTxnHash: hash,
      });
    } catch (err: any) {
      return rejectWithValue(err);
    }
  }
);

export const getHeroById = createAsyncThunk(
  "vendor/getHeroById",
  async (heroId: number, { rejectWithValue, dispatch }) => {
    try {
      return await vendorApi.getHeroById(heroId);
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

export const getIdentifySkill = createAsyncThunk(
  "vendor/getIdentifySkill",
  async (nftTokenId: number, { rejectWithValue, dispatch }) => {
    try {
      return await vendorApi.getIdentifySkill(nftTokenId);
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

export const submitIdentifySkillTransaction = createAsyncThunk(
  "vendor/submitIdentifySkillTransaction",
  async (identifySkillOrder: IdentifySkillOrder & { connect: IContractCalls }, { rejectWithValue, dispatch }) => {
    try {
      const { connect, ...order } = identifySkillOrder;
      if (!identifySkillOrder.priceInDcx) {
        throw new Error("Price In DCX is empty.");
      }
      const hash = await connect.identifySkill(identifySkillOrder, (message) =>
        dispatch(setIdentifySkillMessage(message))
      );
      return vendorApi.postIdentifySkill({
        ...order,
        fulfillmentTxnHash: hash,
      });
    } catch (err: any) {
      return rejectWithValue(err);
    }
  }
);

export const getLearnSkill = createAsyncThunk(
  "vendor/getLearnSkill",
  async (nftTokenId: number, { rejectWithValue, dispatch }) => {
    try {
      return await vendorApi.getLearnSkill(nftTokenId);
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

export const submitLearnSkillTransaction = createAsyncThunk(
  "vendor/submitLearnSkillTransaction",
  async (learnSkillOrder: LearnSkillOrder & { connect: IContractCalls }, { rejectWithValue, dispatch }) => {
    try {
      const { connect, ...order } = learnSkillOrder;
      // if (!learnSkillOrder.priceInDcx) {
      //   throw new Error("Price In DCX is empty.");
      // }
      const hash = await connect.learnSkill(learnSkillOrder, (message) => dispatch(setLearnSkillMessage(message)));
      return vendorApi.postLearnSkill({
        ...order,
        fulfillmentTxnHash: hash,
      });
    } catch (err: any) {
      return rejectWithValue(err);
    }
  }
);

export const getHerbalistOptions = createAsyncThunk(
  "vendor/getHerbalistOptions",
  async (_, { rejectWithValue, dispatch }) => {
    try {
      return await vendorApi.getHerbalistOptions();
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

export const vendorSlice = createSlice({
  name: "vendor",
  initialState,
  reducers: {
    toggleItemToSell: (state, action: PayloadAction<ToggleItemToSell>) => {

      let items = [...state.itemsToSell.items];

      if(action.payload.unselectedAll){
        items=[];
      }else if(action.payload.item){

        const exists = !!items.find(i=>i.id == action.payload.item?.id);

        if(exists){
          items = items.filter(i=>i.id != action.payload.item?.id);
        }else{
          items = [...items,action.payload.item];
        }
      }

      state.itemsToSell={items, showConfirmation: action.payload.showConfirmation};
    
    },
    resetGetSummonHeroStatus: (state, action: PayloadAction) => {
      state.getSummonHeroStatus.status = Status.NotStarted;
      state.getSummonHeroStatus.error = "";
    },
    resetGetNFTActionUseStatus: (state, action: PayloadAction) => {
      state.getNFTActionUseStatus.status = Status.NotStarted;
      state.getNFTActionUseStatus.error = "";
    },
    setIdentifySkillSubmittedStatus: (state, action: PayloadAction<string>) => {
      state.identifySkillSubmittedStatus = action.payload;
    },
    resetGetIdentifySkillStatus: (state, action: PayloadAction) => {
      state.getIdentifySkillStatus.status = Status.NotStarted;
      state.getIdentifySkillStatus.error = "";
    },
    setSkillToIdentifyId: (state, action: PayloadAction<string>) => {
      state.skillToIdentifyId = action.payload;
    },
    setIdentifySkillMessage: (state, action: PayloadAction<string>) => {
      state.skillIdentifyMessage = action.payload;
    },
    setLearnSkillSubmittedStatus: (state, action: PayloadAction<string>) => {
      state.learnSkillSubmittedStatus = action.payload;
    },
    resetGetLearnSkillStatus: (state, action: PayloadAction) => {
      state.getLearnSkillStatus.status = Status.NotStarted;
      state.getLearnSkillStatus.error = "";
    },
    setSkillToLearnId: (state, action: PayloadAction<string>) => {
      state.skillToLearnId = action.payload;
    },
    setLearnSkillMessage: (state, action: PayloadAction<string>) => {
      state.skillLearnMessage = action.payload;
    },
    resetGetSummonedHeroByIdStatus: (state, action: PayloadAction) => {
      state.getSummonedHeroByIdStatus.status = Status.NotStarted;
      state.getSummonedHeroByIdStatus.error = "";
    },
    setSummonHeroSubmittedStatus: (state, action: PayloadAction<string>) => {
      state.summonHeroSubmittedStatus = action.payload;
    },
    setNFTActionUseSubmittedStatus: (state, action: PayloadAction<string>) => {
      state.NFTActionUseSubmittedStatus = action.payload;
    },
    addToConsumedNFTTokenIds: (state, action: PayloadAction<number>) => {
      state.consumedNFTTokenIds.push(action.payload);
    },
    setConsumeNFTMessage: (state, action: PayloadAction<string>) => {
      state.consumeNFTMessage = action.payload;
    },
    setSummonedHeroId: (state, action: PayloadAction<number>) => {
      state.updateOwnerNftIds.push(action.payload);
      state.summonedHeroId = action.payload;
    },
    setUpdateOwnerNftIds: (state, action: PayloadAction<number>) => {
      state.updateOwnerNftIds.push(action.payload);
    },
    clearUpdateOwnerNftIds: (state, action: PayloadAction) => {
      state.updateOwnerNftIds = [];
    },
  },
  extraReducers(builder) {
    builder
      .addCase(getBlacksmithItems.pending, (state, action) => {
        state.blacksmithItemsStatus.status = Status.Loading;
      })
      .addCase(getBlacksmithItems.fulfilled, (state, action: PayloadAction<Array<ItemDto>>) => {
        state.blacksmithItems = action.payload;
        state.blacksmithItemsStatus.status = Status.Loaded;
      })
      .addCase(getBlacksmithItems.rejected, (state, action: any) => {
        state.blacksmithItemsStatus.status = Status.Failed;
      })
      .addCase(getSummonHero.pending, (state, action) => {
        state.getSummonHeroStatus.status = Status.Loading;
      })
      .addCase(getNftActionUse.pending, (state, action) => {
        state.getNFTActionUseStatus.status = Status.Loading;
      })
      .addCase(getSummonHero.fulfilled, (state, action: PayloadAction<SummonHeroOrder>) => {
        state.summonHeroResponse = action.payload;
        state.getSummonHeroStatus.status = Status.Loaded;
      })
      .addCase(getNftActionUse.fulfilled, (state, action: PayloadAction<NftActionOrder>) => {
        state.NFTActionUseResponse = action.payload;
        state.getNFTActionUseStatus.status = Status.Loaded;
      })
      .addCase(getSummonHero.rejected, (state, action: any) => {
        state.getSummonHeroStatus.status = Status.Failed;
      })
      .addCase(getNftActionUse.rejected, (state, action: any) => {
        state.getNFTActionUseStatus.status = Status.Failed;
      })
      .addCase(submitSummonHeroTransaction.pending, (state, action) => {
        state.summonHeroSubmittedStatus = "processing";
      })
      .addCase(submitNftActionUseTransaction.pending, (state, action) => {
        state.NFTActionUseSubmittedStatus = "processing";
      })
      .addCase(submitSummonHeroTransaction.fulfilled, (state, action: PayloadAction<SummonHeroOrder>) => {
        if (action.payload.nftTokenId) {
          state.updateOwnerNftIds.push(action.payload.nftTokenId);
        }
        state.summonHeroResponse = action.payload;
        state.summonHeroSubmittedStatus = "completed";
      })
      .addCase(submitNftActionUseTransaction.fulfilled, (state, action: PayloadAction<NftActionOrder>) => {
        if (action.payload.nftTokenId) {
          state.updateOwnerNftIds.push(action.payload.nftTokenId);
        }
        state.NFTActionUseResponse = action.payload;
        state.NFTActionUseSubmittedStatus = "completed";
      })
      .addCase(submitSummonHeroTransaction.rejected, (state, action: any) => {
        state.summonHeroSubmittedStatus = "failed";
      })
      .addCase(submitNftActionUseTransaction.rejected, (state, action: any) => {
        state.NFTActionUseSubmittedStatus = "failed";
      })
      .addCase(getHeroById.pending, (state, action) => {
        state.getSummonedHeroByIdStatus.status = Status.Loading;
      })
      .addCase(getHeroById.fulfilled, (state, action: PayloadAction<HeroDto>) => {
        state.summonedHero = action.payload;
        state.getSummonedHeroByIdStatus.status = Status.Loaded;
      })
      .addCase(getHeroById.rejected, (state, action: any) => {
        state.getSummonedHeroByIdStatus.status = Status.Failed;
      })
      .addCase(getHerbalistOptions.pending, (state, action) => {
        state.getHerbalistOptionsStatus.status = Status.Loading;
      })
      .addCase(getHerbalistOptions.fulfilled, (state, action: PayloadAction<Array<HerbalistDto>>) => {
        state.herbalistOptions = action.payload;
        state.getHerbalistOptionsStatus.status = Status.Loaded;
      })
      .addCase(getHerbalistOptions.rejected, (state, action: any) => {
        state.getHerbalistOptionsStatus.status = Status.Failed;
      })
      .addCase(getIdentifySkill.pending, (state, action) => {
        state.getIdentifySkillStatus.status = Status.Loading;
      })
      .addCase(getIdentifySkill.fulfilled, (state, action: PayloadAction<IdentifySkillOrder>) => {
        state.identifySkillResponse = action.payload;
        state.getIdentifySkillStatus.status = Status.Loaded;
      })
      .addCase(getIdentifySkill.rejected, (state, action: any) => {
        state.getIdentifySkillStatus.status = Status.Failed;
      })
      .addCase(submitIdentifySkillTransaction.pending, (state, action) => {
        state.identifySkillSubmittedStatus = "processing";
      })
      .addCase(submitIdentifySkillTransaction.fulfilled, (state, action: PayloadAction<IdentifySkillOrder>) => {
        if (action.payload.nftTokenId && action.payload.newItemTokenId) {
          state.updateOwnerNftIds.push(action.payload.nftTokenId);
          state.updateOwnerNftIds.push(action.payload.newItemTokenId);
        }
        state.identifySkillResponse = action.payload;
        state.identifySkillSubmittedStatus = "completed";
      })
      .addCase(submitIdentifySkillTransaction.rejected, (state, action: any) => {
        state.identifySkillSubmittedStatus = "failed";
      })
      .addCase(getLearnSkill.pending, (state, action) => {
        state.getLearnSkillStatus.status = Status.Loading;
      })
      .addCase(getLearnSkill.fulfilled, (state, action: PayloadAction<LearnSkillOrder>) => {
        state.learnSkillResponse = action.payload;
        state.getLearnSkillStatus.status = Status.Loaded;
      })
      .addCase(getLearnSkill.rejected, (state, action: any) => {
        state.getLearnSkillStatus.status = Status.Failed;
      })
      .addCase(submitLearnSkillTransaction.pending, (state, action) => {
        state.learnSkillSubmittedStatus = "processing";
      })
      .addCase(submitLearnSkillTransaction.fulfilled, (state, action: PayloadAction<LearnSkillOrder>) => {
        if (action.payload.nftTokenId) {
          state.updateOwnerNftIds.push(action.payload.nftTokenId);
        }
        state.learnSkillResponse = action.payload;
        state.learnSkillSubmittedStatus = "completed";
      })
      .addCase(submitLearnSkillTransaction.rejected, (state, action: any) => {
        state.learnSkillSubmittedStatus = "failed";
      });
  },
});

// Action creators
export const {
  toggleItemToSell,
  resetGetSummonHeroStatus,
  resetGetNFTActionUseStatus,
  setIdentifySkillSubmittedStatus,
  resetGetIdentifySkillStatus,
  setSkillToIdentifyId,
  setIdentifySkillMessage,
  setLearnSkillSubmittedStatus,
  resetGetLearnSkillStatus,
  setSkillToLearnId,
  setLearnSkillMessage,
  setSummonHeroSubmittedStatus,
  setNFTActionUseSubmittedStatus,
  resetGetSummonedHeroByIdStatus,
  addToConsumedNFTTokenIds,
  setConsumeNFTMessage: setNFtConsumptionMessage,
  setSummonedHeroId,
  setUpdateOwnerNftIds,
  clearUpdateOwnerNftIds,
} = vendorSlice.actions;

// Selectors
export const selectBlacksmithItems = (state: RootState) => state.vendor.blacksmithItems;
export const selectBlacksmithItemsStatus = (state: RootState) => state.vendor.blacksmithItemsStatus;
export const selectItemsToSell = (state: RootState) => state.vendor.itemsToSell;
export const selectSummonedHero = (state: RootState) => state.vendor.summonedHero;
export const selectGetSummonedHeroByIdStatus = (state: RootState) => state.vendor.getSummonedHeroByIdStatus;
export const selectSummonHeroResponse = (state: RootState) => state.vendor.summonHeroResponse;
export const selectGetSummonHeroStatus = (state: RootState) => state.vendor.getSummonHeroStatus;
export const selectSummonHeroSubmittedStatus = (state: RootState) => state.vendor.summonHeroSubmittedStatus;
export const selectHerbalistOptions = (state: RootState) => state.vendor.herbalistOptions;
export const selectHerbalistOptionsStatus = (state: RootState) => state.vendor.getHerbalistOptionsStatus;
export const selectIdentifySkillResponse = (state: RootState) => state.vendor.identifySkillResponse;
export const selectGetIdentifySkillStatus = (state: RootState) => state.vendor.getIdentifySkillStatus;
export const selectIdentifySkillSubmittedStatus = (state: RootState) => state.vendor.identifySkillSubmittedStatus;
export const selectSkillToIdentifyId = (state: RootState) => state.vendor.skillToIdentifyId;
export const selectSkillIdentifyMessage = (state: RootState) => state.vendor.skillIdentifyMessage;
export const selectLearnSkillResponse = (state: RootState) => state.vendor.learnSkillResponse;
export const selectGetLearnSkillStatus = (state: RootState) => state.vendor.getLearnSkillStatus;
export const selectLearnSkillSubmittedStatus = (state: RootState) => state.vendor.learnSkillSubmittedStatus;
export const selectSkillToLearnId = (state: RootState) => state.vendor.skillToLearnId;
export const selectSkillLearnMessage = (state: RootState) => state.vendor.skillLearnMessage;
export const selectConsumedNFTTokenIds = (state: RootState) => state.vendor.consumedNFTTokenIds;
export const selectConsumeNFTMessage = (state: RootState) => state.vendor.consumeNFTMessage;
export const selectSummonedHeroId = (state: RootState) => state.vendor.summonedHeroId;
export const selectUpdateOwnerNftIds = (state: RootState) => state.vendor.updateOwnerNftIds;

export const selectNftActionUseResponse = (state: RootState) => state.vendor.NFTActionUseResponse;
export const selectNftActionUseSubmittedStatus = (state: RootState) => state.vendor.NFTActionUseSubmittedStatus;
export const selectGetNftActionUseStatus = (state: RootState) => state.vendor.getNFTActionUseStatus;

export default vendorSlice.reducer;
