import { configureStore } from "@reduxjs/toolkit";
import appReducer from "@/state-mgmt/app/appSlice";
import heroReducer from "@/state-mgmt/hero/heroSlice";
import playerReducer from "@/state-mgmt/player/playerSlice";
import itemReducer from "@/state-mgmt/item/itemSlice";
import combatReducer from "@/state-mgmt/combat/combatSlice";
import encounterReducer from "@/state-mgmt/encounter/encounterSlice";
import gameStateReducer from "@/state-mgmt/game-state/gameStateSlice";
import vendorStateReducer from "@/state-mgmt/vendor/vendorSlice";
import campStateReducer from "@/state-mgmt/camp/campSlice";
import singleStakingReducer from "@/state-mgmt/single-staking/singleStakingSlice";
import mintReducer from "@/state-mgmt/mint/mintSlice";
import rewardsReducer from "@/state-mgmt/rewards/rewardsSlice";
import seasonReducer from "@/state-mgmt/season/seasonSlice";
import testingReducer from "@/state-mgmt/testing/testingSlice";

export const store = configureStore({
  reducer: {
    app: appReducer,
    hero: heroReducer,
    player: playerReducer,
    item: itemReducer,
    combat: combatReducer,
    encounter: encounterReducer,
    game: gameStateReducer,
    vendor: vendorStateReducer,
    camp: campStateReducer,
    singleStaking: singleStakingReducer,
    mint: mintReducer,
    season: seasonReducer,
    testing: testingReducer,
    rewards: rewardsReducer
  },
});

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
