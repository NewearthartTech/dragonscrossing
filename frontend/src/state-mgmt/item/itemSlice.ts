import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { RootState } from "../store/store";
import { DisplayItemDetails, DisplayItemMenu, Item } from "./itemTypes";
import { initializedItem } from "@/helpers/global-constants";

export interface ItemState {
  itemMenuOpen: DisplayItemMenu;
  itemDetailsOpen: DisplayItemDetails;
  itemToKeep: Item;
  isDisableInventory: boolean;
}

const initialState: ItemState = {
  itemMenuOpen: {
    item: initializedItem,
    open: false,
    x: -1,
    y: -1,
  },
  itemDetailsOpen: {
    item: initializedItem,
    open: false,
  },
  itemToKeep: initializedItem,
  isDisableInventory: false,
};

//Thunks

export const itemSlice = createSlice({
  name: "item",
  initialState,
  reducers: {
    setItemMenuOpen: (state, action: PayloadAction<DisplayItemMenu>) => {
      state.itemMenuOpen = action.payload;
    },
    setItemDetailsOpen: (state, action: PayloadAction<DisplayItemDetails>) => {
      state.itemDetailsOpen = action.payload;
    },
    setItemToKeep: (state, action: PayloadAction<Item>) => {
      state.itemToKeep = action.payload;
    },
    clearItemToKeep: (state, action: PayloadAction) => {
      state.itemToKeep = initialState.itemToKeep;
    },
    setDisableInventory: (state, action: PayloadAction<boolean>) => {
      state.isDisableInventory = action.payload;
    },
  },
});

// Action creators
export const { setItemMenuOpen, setItemDetailsOpen, setItemToKeep, setDisableInventory, clearItemToKeep } =
  itemSlice.actions;

// Selectors
export const selectItemMenuOpen = (state: RootState) => state.item.itemMenuOpen;
export const selectItemDetailsOpen = (state: RootState) => state.item.itemDetailsOpen;
export const selectItemToKeep = (state: RootState) => state.item.itemToKeep;
export const selectDisableInventory = (state: RootState) => state.item.isDisableInventory;

export default itemSlice.reducer;
