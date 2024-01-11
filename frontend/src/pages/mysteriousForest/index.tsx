import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./mysteriousForest.module.scss";
import { withHoc } from "@/components/hoc/hoc";
import DCXTile from "@/components/dcx-tile/dcx-tile";
import { DcxTiles } from "@dcx/dcx-backend";
import { useAppSelector } from "@/state-mgmt/store/hooks";
import { selectGameState } from "@/state-mgmt/game-state/gameStateSlice";
import { renderDiscoveredTiles, renderUndiscoveredTiles } from "@/helpers/shared-functions";

interface Props {}

const MysteriousForest: NextPage<Props> = (props: Props) => {
  const gameState = useAppSelector(selectGameState);

  return (
    <Grid container direction="column" className={styles.main}>
      <Grid container className={styles.opaqueContainer} />
      <Grid container direction="row" className={styles.container}>
        <DCXTile tileName={DcxTiles.WildPrairie} />
        <DCXTile tileName={DcxTiles.SylvanWoodlands} />
        {renderDiscoveredTiles(gameState)}
        {renderUndiscoveredTiles(gameState)}
      </Grid>
    </Grid>
  );
};

export default withHoc(MysteriousForest);
