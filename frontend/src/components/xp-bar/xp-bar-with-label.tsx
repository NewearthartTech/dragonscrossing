import { Hero, SelectedHero } from "@/state-mgmt/hero/heroTypes";
import { Monster } from "@/state-mgmt/monster/monsterTypes";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./xp-bar.module.scss";
import Image from "next/image";
import { HeroDto } from "@dcx/dcx-backend";

interface Props {
  value: number;
  currentxp: number;
  nextlvlxp: number;
  backgroundBarHeight?: number;
  backgroundBarWidth?: number;
  trimmedWidth: number;
  barHeight?: number;
  marginTop?: number;
  marginBottom?: number;
}

const XPBarWithLabel: React.FC<Props> = (props: Props) => {
  const {
    value,
    currentxp,
    nextlvlxp,
    backgroundBarHeight,
    backgroundBarWidth,
    trimmedWidth,
    barHeight,
    marginTop,
    marginBottom,
  } = props;

  return (
    <Grid item xs={12}>
      <Grid container direction="row" className={styles.container}>
        <Grid
          container
          className={styles.xpBarContainer}
          style={{
            width: backgroundBarWidth,
            height: backgroundBarHeight,
          }}
        >
          <Grid container style={{ height: backgroundBarHeight, width: backgroundBarWidth }}>
            <Image
              src="/img/unity-assets/shared/empty_bar.png"
              height={backgroundBarHeight}
              width={backgroundBarWidth}
              quality={100}
            />
          </Grid>
          <Grid
            container
            className={styles.trimmedContainer}
            style={{
              width: trimmedWidth + 1,
              marginTop: marginTop,
              marginBottom: marginBottom,
            }}
          >
            <Grid
              container
              className={styles.xpBar}
              style={{
                width: `${value}%`,
                height: barHeight,
              }}
            />
          </Grid>
          <Typography variant="body2" className={styles.xpBarLabel}>
            {`${currentxp}/${nextlvlxp} XP`}
          </Typography>
        </Grid>
      </Grid>
    </Grid>
  );
};

export default XPBarWithLabel;
