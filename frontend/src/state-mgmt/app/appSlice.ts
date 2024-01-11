import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { RootState } from "../store/store";
import { AppMessage, SnackbarMessage } from "./appTypes";

export interface AppState {
  displayCharacter: boolean;
  displayWallet: boolean;
  displayInventory: boolean;
  displayLoadout: boolean;
  displayStaking: boolean;
  displaySettings: boolean;
  displayGuildModal: boolean;
  displayCampModal: boolean;
  displayCombatPhases: string;
  displayCombatSkills: string;
  displaySharedStash: boolean;
  displayAchievementModal: boolean;
  displayFooter: boolean;
  displayFixDice: boolean;
  displayLearnSkill: boolean;
  browserNavigationUsed: boolean;
  playButtonClickSound: boolean;
  playTileClickSound: boolean;
  droppedItemSoundSlug: string;
  musicSlug: string;
  adventuringGuildTab: string;
  isAppInitialized: boolean;
  appMessage: AppMessage;
  snackbarMessage: SnackbarMessage;
  refreshItemNFTs: boolean;
  refreshItemNFTsDelayed: boolean;
  refreshHeroNFTs: boolean;
  refreshHeroNFTsDelayed: boolean;
}

const initialState: AppState = {
  displayCharacter: false,
  displayWallet: false,
  displayInventory: false,
  displayLoadout: false,
  displayStaking: false,
  displaySettings: false,
  displayGuildModal: false,
  displayCampModal: false,
  displayCombatPhases: "",
  displayCombatSkills: "",
  displaySharedStash: false,
  displayAchievementModal: false,
  displayFooter: true,
  displayFixDice: false,
  displayLearnSkill: false,
  browserNavigationUsed: false,
  playButtonClickSound: false,
  playTileClickSound: false,
  droppedItemSoundSlug: "",
  musicSlug: "",
  adventuringGuildTab: "",
  isAppInitialized: false,
  appMessage: {
    message: "",
    isClearToken: false,
  },
  snackbarMessage: {
    isOpen: false,
    message: "",
  },
  refreshItemNFTs: true, // Default to true so we get itemNFTs when app is initialized
  refreshItemNFTsDelayed: false,
  refreshHeroNFTs: false,
  refreshHeroNFTsDelayed: false,
};

export const appSlice = createSlice({
  name: "app",
  initialState,
  reducers: {
    setDisplayCharacter: (state, action: PayloadAction<boolean>) => {
      state.displayCharacter = action.payload;
      state.displayWallet = false;
      state.displayInventory = false;
      state.displayLoadout = false;
      state.displayStaking = false;
      state.displaySettings = false;
      state.displayGuildModal = false;
    },
    setDisplayWallet: (state, action: PayloadAction<boolean>) => {
      state.displayCharacter = false;
      state.displayWallet = action.payload;
      state.displayInventory = false;
      state.displayLoadout = false;
      state.displayStaking = false;
      state.displaySettings = false;
      state.displayGuildModal = false;
    },
    setDisplayInventory: (state, action: PayloadAction<boolean>) => {
      state.displayCharacter = false;
      state.displayWallet = false;
      state.displayInventory = action.payload;
      state.displayLoadout = false;
      state.displayStaking = false;
      state.displaySettings = false;
      state.displayGuildModal = false;
    },
    setDisplayLoadout: (state, action: PayloadAction<boolean>) => {
      state.displayCharacter = false;
      state.displayWallet = false;
      state.displayInventory = false;
      state.displayLoadout = action.payload;
      state.displayStaking = false;
      state.displaySettings = false;
      state.displayGuildModal = false;
    },
    setDisplayStaking: (state, action: PayloadAction<boolean>) => {
      state.displayCharacter = false;
      state.displayWallet = false;
      state.displayInventory = false;
      state.displayLoadout = false;
      state.displayStaking = action.payload;
      state.displaySettings = false;
      state.displayGuildModal = false;
    },
    setDisplaySettings: (state, action: PayloadAction<boolean>) => {
      state.displayCharacter = false;
      state.displayWallet = false;
      state.displayInventory = false;
      state.displayLoadout = false;
      state.displayStaking = false;
      state.displaySettings = action.payload;
      state.displayGuildModal = false;
    },
    setDisplayGuildModal: (state, action: PayloadAction<boolean>) => {
      state.displayCharacter = false;
      state.displayWallet = false;
      state.displayInventory = false;
      state.displayLoadout = false;
      state.displayStaking = false;
      state.displaySettings = false;
      state.displayGuildModal = action.payload;
    },
    setDisplayCampModal: (state, action: PayloadAction<boolean>) => {
      state.displayCampModal = action.payload;
    },
    setDisplayCombatPhases: (state, action: PayloadAction<string>) => {
      state.displayCombatPhases = action.payload;
    },
    setDisplayCombatSkills: (state, action: PayloadAction<string>) => {
      state.displayCombatSkills = action.payload;
    },
    setDisplayFooter: (state, action: PayloadAction<boolean>) => {
      state.displayFooter = action.payload;
    },
    setDisplaySharedStash: (state, action: PayloadAction<boolean>) => {
      state.displaySharedStash = action.payload;
    },
    setBrowserNavigationUsed: (state, action: PayloadAction<boolean>) => {
      state.browserNavigationUsed = action.payload;
    },
    setPlayButtonClickSound: (state, action: PayloadAction<boolean>) => {
      state.playButtonClickSound = action.payload;
    },
    setPlayTileClickSound: (state, action: PayloadAction<boolean>) => {
      state.playTileClickSound = action.payload;
    },
    setDroppedItemSoundSlug: (state, action: PayloadAction<string>) => {
      state.droppedItemSoundSlug = action.payload;
    },
    setMusicSlug: (state, action: PayloadAction<string>) => {
      state.musicSlug = action.payload;
    },
    setAdventuringGuildTab: (state, action: PayloadAction<string>) => {
      state.adventuringGuildTab = action.payload;
    },
    setAppInitialized: (state, action: PayloadAction) => {
      state.isAppInitialized = true;
    },
    setAppMessage: (state, action: PayloadAction<AppMessage>) => {
      state.appMessage = action.payload;
    },
    setSnackbarMessage: (state, action: PayloadAction<SnackbarMessage>) => {
      state.snackbarMessage = action.payload;
    },
    setDisplayFixDice: (state, action: PayloadAction<boolean>) => {
      state.displayFixDice = action.payload;
    },
    setDisplayLearnSkill: (state, action: PayloadAction<boolean>) => {
      state.displayLearnSkill = action.payload;
    },
    setRefreshItemNFTs: (state, action: PayloadAction<boolean>) => {
      state.refreshItemNFTs = action.payload;
    },
    setRefreshItemNFTsDelayed: (state, action: PayloadAction<boolean>) => {
      state.refreshItemNFTsDelayed = action.payload;
    },
    setRefreshHeroNFTs: (state, action: PayloadAction<boolean>) => {
      state.refreshHeroNFTs = action.payload;
    },
    setRefreshHeroNFTsDelayed: (state, action: PayloadAction<boolean>) => {
      state.refreshHeroNFTsDelayed = action.payload;
    },
    setDisplayAchievementModal: (state, action: PayloadAction<boolean>) => {
      state.displayAchievementModal = action.payload;
    },
  },
});

// Action creators
export const {
  setDisplayCharacter,
  setDisplayWallet,
  setDisplayInventory,
  setDisplayLoadout,
  setDisplayStaking,
  setDisplaySettings,
  setDisplayGuildModal,
  setDisplayCampModal,
  setDisplayCombatPhases,
  setDisplayCombatSkills,
  setDisplayFooter,
  setDisplaySharedStash,
  setBrowserNavigationUsed,
  setPlayButtonClickSound,
  setPlayTileClickSound,
  setDroppedItemSoundSlug,
  setMusicSlug,
  setAdventuringGuildTab,
  setAppInitialized,
  setAppMessage,
  setSnackbarMessage,
  setDisplayFixDice,
  setDisplayLearnSkill,
  setRefreshItemNFTs,
  setRefreshItemNFTsDelayed,
  setRefreshHeroNFTs,
  setRefreshHeroNFTsDelayed,
  setDisplayAchievementModal,
} = appSlice.actions;

// Selectors
export const selectDisplayCharacter = (state: RootState) => state.app.displayCharacter;
export const selectDisplayWallet = (state: RootState) => state.app.displayWallet;
export const selectDisplayInventory = (state: RootState) => state.app.displayInventory;
export const selectDisplayLoadout = (state: RootState) => state.app.displayLoadout;
export const selectDisplayStaking = (state: RootState) => state.app.displayStaking;
export const selectDisplaySettings = (state: RootState) => state.app.displaySettings;
export const selectDisplayGuildModal = (state: RootState) => state.app.displayGuildModal;
export const selectDisplayCampModal = (state: RootState) => state.app.displayCampModal;
export const selectDisplayCombatPhases = (state: RootState) => state.app.displayCombatPhases;
export const selectDisplayCombatSkills = (state: RootState) => state.app.displayCombatSkills;
export const selectDisplayFooter = (state: RootState) => state.app.displayFooter;
export const selectDisplaySharedStash = (state: RootState) => state.app.displaySharedStash;
export const selectBrowserNavigationUsed = (state: RootState) => state.app.browserNavigationUsed;
export const selectPlayButtonClickSound = (state: RootState) => state.app.playButtonClickSound;
export const selectPlayTileClickSound = (state: RootState) => state.app.playTileClickSound;
export const selectDroppedItemSoundSlug = (state: RootState) => state.app.droppedItemSoundSlug;
export const selectMusicSlug = (state: RootState) => state.app.musicSlug;
export const selectAdventuringGuildTab = (state: RootState) => state.app.adventuringGuildTab;
export const selectAppInitialized = (state: RootState) => state.app.isAppInitialized;
export const selectAppMessage = (state: RootState) => state.app.appMessage;
export const selectSnackbarMessage = (state: RootState) => state.app.snackbarMessage;
export const selectDisplayFixDice = (state: RootState) => state.app.displayFixDice;
export const selectDisplayLearnSkill = (state: RootState) => state.app.displayLearnSkill;
export const selectRefreshItemNFTs = (state: RootState) => state.app.refreshItemNFTs;
export const selectRefreshItemNFTsDelayed = (state: RootState) => state.app.refreshItemNFTsDelayed;
export const selectRefreshHeroNFTs = (state: RootState) => state.app.refreshHeroNFTs;
export const selectRefreshHeroNFTsDelayed = (state: RootState) => state.app.refreshHeroNFTsDelayed;
export const selectDisplayAchievementModal = (state: RootState) => state.app.displayAchievementModal;

export default appSlice.reducer;
