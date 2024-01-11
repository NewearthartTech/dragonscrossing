import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./darkTower.module.scss";
import { withHoc } from "@/components/hoc/hoc";
import DCXTile from "@/components/dcx-tile/dcx-tile";
import { DcxTiles } from "@dcx/dcx-backend";
import { useAppSelector } from "@/state-mgmt/store/hooks";
import { selectGameState, selectGameStateStatus } from "@/state-mgmt/game-state/gameStateSlice";
import { renderDiscoveredTiles, renderUndiscoveredTiles } from "@/helpers/shared-functions";
import { useEffect } from "react";
import { useRouter } from "next/router";
import { Status } from "@/state-mgmt/app/appTypes";

interface Props {}

const DarkTower: NextPage<Props> = (props: Props) => {
  const router = useRouter();
  const gameState = useAppSelector(selectGameState);
  const gameStateStatus = useAppSelector(selectGameStateStatus);

  useEffect(() => {
    if (gameState.slug === DcxTiles.DarkTower && gameState.encounters && gameState.encounters.length > 0) {
      router.push("/libraryOfTheArchmageEpilogue");
    }
  }, []);

  useEffect(() => {
    if (gameStateStatus.status === Status.Loaded) {
      if (gameState.slug === DcxTiles.DarkTower && gameState.encounters && gameState.encounters.length > 0) {
        router.push("/libraryOfTheArchmageEpilogue");
      }
    }
  }, [gameStateStatus]);

  return (
    <Grid container direction="column" className={styles.main}>
      <Grid container className={styles.opaqueContainer} />
      <Grid container direction="row" className={styles.container}>
        <DCXTile tileName={DcxTiles.TreacherousPeaks} />
        <DCXTile tileName={DcxTiles.FallenTemples} />
        <DCXTile tileName={DcxTiles.LabyrinthianDungeon} />
        {renderDiscoveredTiles(gameState)}
        {renderUndiscoveredTiles(gameState)}
      </Grid>
    </Grid>
  );
};

export default withHoc(DarkTower);
