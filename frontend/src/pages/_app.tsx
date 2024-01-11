import "../styles/globals.scss";
import type { AppProps } from "next/app";
import { store } from "@/state-mgmt/store/store";
import { Provider } from "react-redux";
import Layout from "@/components/layout/layout";
import { createTheme, StyledEngineProvider, ThemeProvider } from "@mui/material/styles";
import React, { useEffect, useState } from "react";
import Inventory from "@/components/inventory/inventory";
import Character from "@/components/character/character";
import { handleRightClick } from "@/helpers/helper-functions";
import { isBrowser } from "core/utils";
import { Web3Provider } from "../components/web3";
import { LoginProvider, useLoginDlg } from "../components/auth";
import Staking from "@/components/staking/staking";
import Settings from "@/components/settings/settings";
import DCXAudioPlayer from "@/components/dcx-audio-player/dcx-audio-player";
import { SoundType } from "@/state-mgmt/app/appTypes";
import {
  selectDroppedItemSoundSlug,
  selectMusicSlug,
  selectPlayButtonClickSound,
  selectPlayTileClickSound,
  setDroppedItemSoundSlug,
  setMusicSlug,
  setPlayButtonClickSound,
  setPlayTileClickSound,
} from "@/state-mgmt/app/appSlice";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useRouter } from "next/router";
import AppMessage from "@/components/app-message/app-message";
import FixDice from "@/components/fix-dice/fix-dice";
import ToastMessage from "@/components/toast-message/toast-message";
import GuildModal from "@/components/guild-modal/guild-modal";
import Register from "@/components/register/register";
import CampModal from "@/components/camp-modal/camp-modal";
import LeaderboardModal from "@/components/leaderboard-modal/leaderboard-modal";
import LearnSkillTestModal from "@/components/learn-skill-test-modal/learn-skill-test-modal";
import AchievementModal from "@/components/achievement-modal/achievement-modal";

const theme = createTheme({
  typography: {
    fontFamily: `Whatacolour, Verdana, serif, sans-serif, "Segoe UI", "Roboto"`,
  },
  palette: {
    action: {
      disabled: "rgb(175, 175, 175)",
    },
  },
});

function WithProviders({ Component, pageProps }: AppProps) {
  const router = useRouter();
  const LoginDlg = useLoginDlg();

  const dispatch = useAppDispatch();
  const playButtonClickSound = useAppSelector(selectPlayButtonClickSound);
  const playTileClickSound = useAppSelector(selectPlayTileClickSound);
  const droppedItemSoundSlug = useAppSelector(selectDroppedItemSoundSlug);
  const musicSlug = useAppSelector(selectMusicSlug);

  useEffect(() => {
    if (router.pathname === "/heroSelect") {
      dispatch(setMusicSlug("character-select"));
    } else if (
      router.pathname === "/aedos" ||
      router.pathname === "/blacksmith" ||
      router.pathname === "/herbalist" ||
      router.pathname === "/adventuringGuild" ||
      router.pathname === "/sharedStash"
    ) {
      dispatch(setMusicSlug("aedos"));
    } else if (router.pathname === "/wildPrairie" || router.pathname === "/enchantedFields") {
      dispatch(setMusicSlug("wild-prairie"));
    } else if (
      router.pathname === "/mysteriousForest" ||
      router.pathname === "/sylvanWoodlands" ||
      router.pathname === "/pilgrimsClearing"
    ) {
      dispatch(setMusicSlug("mysterious-forest"));
    } else if (
      router.pathname === "/foulWastes" ||
      router.pathname === "/odorousBog" ||
      router.pathname === "/ancientBattlefield" ||
      router.pathname === "/terrorswamp"
    ) {
      dispatch(setMusicSlug("foul-wastes"));
    } else if (
      router.pathname === "/treacherousPeaks" ||
      router.pathname === "/mountainFortress" ||
      router.pathname === "/griffonsNest" ||
      router.pathname === "/summonersSummit"
    ) {
      dispatch(setMusicSlug("treacherous-peaks"));
    } else if (
      router.pathname === "/darkTower" ||
      router.pathname === "/labrynthianDungeon" ||
      router.pathname === "/theBarracks" ||
      router.pathname === "/slaverRow" ||
      router.pathname === "/laboratoryOfTheArchmagus"
    ) {
      dispatch(setMusicSlug("dark-tower"));
    } else if (router.pathname === "/death" || router.pathname === "/plunder") {
      // Do nothing, continue to play the current zone music
    } else {
      dispatch(setMusicSlug(""));
    }
  }, [router.pathname]);

  return (
    <Layout>
      {(playButtonClickSound || playTileClickSound) && (
        <DCXAudioPlayer
          audioUrl={
            playButtonClickSound
              ? "/audio/sound-effects/miscellaneous/button-click"
              : "/audio/sound-effects/miscellaneous/tile-click"
          }
          soundType={SoundType.SOUND_EFFECT}
          onEnded={() =>
            playButtonClickSound ? dispatch(setPlayButtonClickSound(false)) : dispatch(setPlayTileClickSound(false))
          }
        />
      )}
      {droppedItemSoundSlug && droppedItemSoundSlug !== "" && (
        <DCXAudioPlayer
          audioUrl={`/audio/sound-effects/item/${droppedItemSoundSlug}`}
          soundType={SoundType.SOUND_EFFECT}
          onEnded={() => dispatch(setDroppedItemSoundSlug(""))}
        />
      )}
      {musicSlug !== "" && (
        <DCXAudioPlayer audioUrl={`/audio/music/${musicSlug}`} soundType={SoundType.MUSIC} loop={true} />
      )}
      <LoginDlg />
      <Inventory />
      <Character />
      <Staking />
      <GuildModal />
      <LeaderboardModal />
      <CampModal />
      <Settings />
      <Register />
      <AppMessage />
      <ToastMessage />
      <AchievementModal />
      <FixDice />
      <LearnSkillTestModal />
      {/* For some reason Component was throwing a type error. It can be resolved by upgrading the 
      react types and dom versions but then this caused other errors. I am ts-ignoring for now */}
      {/* // eslint-disable-next-line @typescript-eslint/ban-ts-comment
      // @ts-ignore */}
      <Component {...pageProps} />
    </Layout>
  );
}

function MyApp(props: AppProps) {
  // Disable right-click context menu on the entire app
  if (isBrowser() && process.env.NEXT_PUBLIC_LEAVE_RIGHT_CLICK_ALONE != "true"  ) {
    document.addEventListener("contextmenu", handleRightClick);
  }
  return (
    <Provider store={store}>
      <LoginProvider>
        <Web3Provider>
          <StyledEngineProvider injectFirst>
            <ThemeProvider theme={theme}>{isBrowser() && <WithProviders {...props} />}</ThemeProvider>
          </StyledEngineProvider>
        </Web3Provider>
      </LoginProvider>
    </Provider>
  );
}

export default MyApp;
