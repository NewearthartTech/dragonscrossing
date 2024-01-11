import Grid from "@mui/material/Grid";
import styles from "./dcx-undiscovered-tile.module.scss";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { xsScreenWidth } from "@/helpers/global-constants";
import { TileBorderType } from "@/state-mgmt/app/appTypes";
import LockIcon from "@mui/icons-material/Lock";

interface Props {
  borderType?: TileBorderType;
}

const DCXUndiscoveredTile: React.FC<Props> = (props: Props) => {
  const { borderType } = props;
  const { width, height } = useWindowDimensions();

  return (
    <Grid container className={styles.tileContainer}>
      <Grid container className={styles.tile}>
        <Grid container className={styles.tileImageContainer}>
          <LockIcon className={styles.icon} />
        </Grid>
        <Grid container className={styles.tileBorderContainer}>
          <Image
            src={
              borderType ? `/img/backgrounds/${borderType}-tile-border.png` : `/img/backgrounds/armored-tile-border.png`
            }
            height={width <= xsScreenWidth ? "225" : "295"}
            width={width <= xsScreenWidth ? "316" : "412"}
          />
        </Grid>
      </Grid>
    </Grid>
  );
};

export default DCXUndiscoveredTile;
