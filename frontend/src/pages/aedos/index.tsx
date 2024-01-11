import { withHoc } from "@/components/hoc/hoc";
import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./aedos.module.scss";
import DCXTile from "@/components/dcx-tile/dcx-tile";
import { TileBorderType } from "@/state-mgmt/app/appTypes";
import { DcxTiles } from "@dcx/dcx-backend";

interface Props {}

const Aedos: NextPage<Props> = (props: Props) => {
  return (
    <Grid container direction="column" className={styles.main}>
      {/* <Grid container className={styles.opaqueContainer} /> */}
      <Grid container direction="row" className={styles.container}>
        <DCXTile tileName={DcxTiles.Blacksmith} borderType={TileBorderType.WOODEN} />
        <DCXTile tileName={DcxTiles.HerbalistAedos} borderType={TileBorderType.WOODEN} />
        <DCXTile tileName={DcxTiles.AdventuringGuild} borderType={TileBorderType.WOODEN} />
        <DCXTile tileName={DcxTiles.SharedStash} borderType={TileBorderType.WOODEN} />
        <DCXTile tileName={"camp"} borderType={TileBorderType.WOODEN} />
        <DCXTile tileName={DcxTiles.WildPrairie} borderType={TileBorderType.WOODEN} />
      </Grid>
    </Grid>
  );
};

export default withHoc(Aedos);
