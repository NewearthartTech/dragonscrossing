import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./wildPrairie.module.scss";
import { withHoc } from "@/components/hoc/hoc";
import DCXTile from "@/components/dcx-tile/dcx-tile";
import { DcxTiles } from "@dcx/dcx-backend";
import { useAppSelector } from "@/state-mgmt/store/hooks";
import { selectGameState } from "@/state-mgmt/game-state/gameStateSlice";
import { renderDiscoveredTiles, renderUndiscoveredTiles } from "@/helpers/shared-functions";

interface Props {}

const WildPrairie: NextPage<Props> = (props: Props) => {
  const gameState = useAppSelector(selectGameState);

  return (
    <Grid container direction="column" className={styles.main}>
      <Grid container className={styles.opaqueContainer} />
      <Grid container direction="row" className={styles.container}>
        <DCXTile tileName={DcxTiles.Aedos} />
        <DCXTile tileName={DcxTiles.EnchantedFields} />
        {renderDiscoveredTiles(gameState)}
        {renderUndiscoveredTiles(gameState)}
      </Grid>
    </Grid>
  );
};

export default withHoc(WildPrairie);
