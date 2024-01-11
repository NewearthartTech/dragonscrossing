import Encounter from "@/components/encounter/encounter";
import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./libraryOfTheArchmageEpilogue.module.scss";
import { useAppSelector } from "@/state-mgmt/store/hooks";
import { selectGameState, selectGameStateStatus } from "@/state-mgmt/game-state/gameStateSlice";
import { useEffect } from "react";
import { DcxTiles } from "@dcx/dcx-backend";
import { useRouter } from "next/router";
import { Status } from "@/state-mgmt/app/appTypes";

interface Props {}

const LibraryOfTheArchmageEpilogue: NextPage<Props> = (props: Props) => {
  const router = useRouter();
  const gameState = useAppSelector(selectGameState);
  const gameStateStatus = useAppSelector(selectGameStateStatus);

  useEffect(() => {
    if (gameState && gameState.slug === DcxTiles.Unknown) {
      router.push("/darkTower");
    }
  }, []);

  useEffect(() => {
    if (gameStateStatus.status === Status.Loaded && router.pathname === "/libraryOfTheArchmageEpilogue") {
      if (gameState.slug !== DcxTiles.DarkTower || !gameState.encounters || gameState.encounters.length === 0) {
        router.push("/darkTower");
      }
    }
  }, [gameStateStatus]);

  return (
    <Grid container direction="column" className={styles.container}>
      <Grid container className={styles.backgroundImage} />
      <Grid container className={styles.opaqueContainer} />
      <Encounter />
    </Grid>
  );
};

export default LibraryOfTheArchmageEpilogue;
