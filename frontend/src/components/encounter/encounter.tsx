import Grid from "@mui/material/Grid";
import styles from "./encounter.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useEffect, useState } from "react";
import {
  selectGameState,
  selectGameStateStatus,
  updateGameState,
} from "@/state-mgmt/game-state/gameStateSlice";
import {
  resetEncounterFinishedStatus,
  selectEncounterFinishedStatus,
  selectIsEncounterComplete,
  setEncounterComplete,
} from "@/state-mgmt/encounter/encounterSlice";
import Combat from "../combat/combat";
import DCXAudioPlayer from "../dcx-audio-player/dcx-audio-player";
import { SoundType, Status } from "@/state-mgmt/app/appTypes";
import {
  BossEncounter,
  ChanceEncounter,
  ChanceEncounterEnum,
  DcxTiles,
  LocationEncounter,
  LoreEncounter,
} from "@dcx/dcx-backend";
import { getSelectedHero } from "@/state-mgmt/hero/heroSlice";
import LocationEncounterComponent from "../noncombat-encounters/location-encounter/location-encounter";
import LoreEncounterComponent from "../noncombat-encounters/lore-encounter/lore-encounter";
import BerriesEncounterComponent from "../noncombat-encounters/berries-encounter/berries-encounter";
import FreshwaterOrbEncounterComponent from "../noncombat-encounters/freshwater-orb-encounter/freshwater-orb-encounter";
import GamblerEncounterComponent from "../noncombat-encounters/gambler-encounter/gambler-encounter";
import BossEncounterComponent from "../noncombat-encounters/boss-encounter/boss-encounter";
import { WonderingWizardEncounterComponent } from "../noncombat-encounters/wonderingWizard-encounter/wonderingWizard-encounter";

interface Props {}

const Encounter: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const gameState = useAppSelector(selectGameState);
  const gameStateStatus = useAppSelector(selectGameStateStatus);
  const isEncounterComplete = useAppSelector(selectIsEncounterComplete);
  const encounterFinishedStatus = useAppSelector(selectEncounterFinishedStatus);

  const [encounterIndex, setEncounterIndex] = useState(0);
  const [encounterType, setEncounterType] = useState("");

  useEffect(() => {
    if (gameState.encounters && gameState.encounters.length > 0) {
      updateEncounterType();
    }
  }, []);

  useEffect(() => {
    if (gameStateStatus.status === Status.Loaded) {
      updateEncounterType();
      if (!gameState.encounters || gameState.encounters.length === 0) {
        dispatch(updateGameState(gameState.zone.slug as DcxTiles));
        dispatch(getSelectedHero());
      }
    }
  }, [gameStateStatus]);

  useEffect(() => {
    if (encounterFinishedStatus.status === Status.Loaded) {
      dispatch(setEncounterComplete(true));
      dispatch(resetEncounterFinishedStatus());
    }
    if (encounterFinishedStatus.status === Status.Failed) {
      dispatch(resetEncounterFinishedStatus());
    }
  }, [encounterFinishedStatus]);

  useEffect(() => {
    // It is possible to have 2 encounters back to back
    if (isEncounterComplete) {
      dispatch(getSelectedHero());
      if (encounterIndex === 0 && gameState.encounters!.length > 1) {
        setEncounterIndex(1);
      } else {
        dispatch(updateGameState(gameState.zone.slug as DcxTiles));
      }
      dispatch(setEncounterComplete(false));
    }
  }, [isEncounterComplete]);

  useEffect(() => {
    if (encounterIndex > 0) {
      updateEncounterType();
    }
  }, [encounterIndex]);

  const updateEncounterType = () => {
    if (gameState.encounters && gameState.encounters.length > 0) {
      if (gameState.encounters[encounterIndex].type === "ChanceEncounter") {
        const chanceEncounter = gameState.encounters![
          encounterIndex
        ] as ChanceEncounter;
        if (
          chanceEncounter.chanceEncounterType ===
          ChanceEncounterEnum.ForeignBerries
        ) {
          setEncounterType(ChanceEncounterEnum.ForeignBerries);
        } else if (
          chanceEncounter.chanceEncounterType ===
          ChanceEncounterEnum.FreshwaterOrb
        ) {
          setEncounterType(ChanceEncounterEnum.FreshwaterOrb);
        } else if (
          chanceEncounter.chanceEncounterType === ChanceEncounterEnum.Gambler
        ) {
          setEncounterType(ChanceEncounterEnum.Gambler);
        } else if (
          chanceEncounter.chanceEncounterType ===
          ChanceEncounterEnum.WonderingWizard
        ) {
          setEncounterType(ChanceEncounterEnum.WonderingWizard);
        } else if (
          chanceEncounter.chanceEncounterType ===
          ChanceEncounterEnum.Cocytus2
        ) {
          setEncounterType(ChanceEncounterEnum.Cocytus2);
        } else if (
          chanceEncounter.chanceEncounterType ===
          ChanceEncounterEnum.RustingArandomWeapon
        ) {
          setEncounterType(ChanceEncounterEnum.RustingArandomWeapon);
        } else if (
          chanceEncounter.chanceEncounterType ===
          ChanceEncounterEnum.Riddler
        ) {
          setEncounterType(ChanceEncounterEnum.Riddler);
        } else if (
          chanceEncounter.chanceEncounterType ===
          ChanceEncounterEnum.LovecraftianMonster
        ) {
          setEncounterType(ChanceEncounterEnum.LovecraftianMonster);
        } else {
          setEncounterType("");
        }
      } else {
        setEncounterType(gameState.encounters![encounterIndex].type);
      }
    }
  };

  return (
    <Grid container className={styles.container}>
      {gameState.encounters && gameState.encounters.length > 0 && (
        <Grid container>
          {/* {encounters[encounterIndex].encounterType !== */}
          {encounterType !== "ActionResponseDto" && (
            <DCXAudioPlayer
              audioUrl={"/audio/music/noncombat-encounter"}
              soundType={SoundType.MUSIC}
              loop={true}
            />
          )}
          {/* {encounters[encounterIndex].encounterType === EncounterEnum.COMBAT ? ( */}
          {encounterType === "ActionResponseDto" ? (
            <Combat />
          ) : encounterType === "LocationEncounter" ? (
            <LocationEncounterComponent
              encounter={
                gameState.encounters[encounterIndex] as LocationEncounter
              }
            />
          ) : encounterType === "LoreEncounter" ? (
            <LoreEncounterComponent
              encounter={gameState.encounters[encounterIndex] as LoreEncounter}
            />
          ) : encounterType === ChanceEncounterEnum.ForeignBerries ? (
            <BerriesEncounterComponent
              encounter={
                gameState.encounters[encounterIndex] as ChanceEncounter
              }
            />
          ) : encounterType === ChanceEncounterEnum.FreshwaterOrb ? (
            <FreshwaterOrbEncounterComponent
              encounter={
                gameState.encounters[encounterIndex] as ChanceEncounter
              }
            />
          ) : encounterType === ChanceEncounterEnum.Gambler ? (
            <GamblerEncounterComponent
              encounter={
                gameState.encounters[encounterIndex] as ChanceEncounter
              }
            />
          ) : encounterType === ChanceEncounterEnum.WonderingWizard ? (
            <WonderingWizardEncounterComponent
              encounterImage="wonderingWizard.jpg"
              
              encounter={
                gameState.encounters[encounterIndex] as ChanceEncounter
              }
            />
          ) : encounterType === ChanceEncounterEnum.Cocytus2 ? (
            <WonderingWizardEncounterComponent
              encounterImage="wonderingWizard.jpg"
              
              encounter={
                gameState.encounters[encounterIndex] as ChanceEncounter
              }
            />
          ) : encounterType === ChanceEncounterEnum.RustingArandomWeapon ? (
            <WonderingWizardEncounterComponent
              encounterImage="rustingArandomWeapon.jpg"
              
              encounter={
                gameState.encounters[encounterIndex] as ChanceEncounter
              }
            />
          ) : encounterType === ChanceEncounterEnum.Riddler ? (
            <WonderingWizardEncounterComponent
              encounterImage="riddler.jpg"
              
              encounter={
                gameState.encounters[encounterIndex] as ChanceEncounter
              }
            />
          ) : encounterType === ChanceEncounterEnum.LovecraftianMonster ? (
            <WonderingWizardEncounterComponent
              encounterImage="lovecraftianMonster.jpg"
              
              encounter={
                gameState.encounters[encounterIndex] as ChanceEncounter
              }
            />
          ) : gameState.slug === DcxTiles.DarkTower &&
            encounterType === "BossEncounter" ? (
            <BossEncounterComponent
              encounter={gameState.encounters[encounterIndex] as BossEncounter}
            />
          ) : null}
        </Grid>
      )}
    </Grid>
  );
};

export default Encounter;
