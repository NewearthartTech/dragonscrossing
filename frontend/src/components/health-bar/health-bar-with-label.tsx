import { Hero, SelectedHero } from "@/state-mgmt/hero/heroTypes";
import { Monster } from "@/state-mgmt/monster/monsterTypes";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./health-bar.module.scss";
import Image from "next/image";
import { HeroDto } from "@dcx/dcx-backend";

interface Props {
  value: number;
  currenthp: number;
  totalhp: number;
  backgroundBarHeight?: number;
  backgroundBarWidth?: number;
  trimmedWidth?: number;
  barHeight?: number;
  marginTop?: number;
  marginBottom?: number;
}

const HealthBarWithLabel: React.FC<Props> = (props: Props) => {
  const {
    value,
    currenthp,
    totalhp,
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
          className={styles.healthBarContainer}
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
              width: trimmedWidth,
              marginTop: marginTop,
              marginBottom: marginBottom,
            }}
          >
            <Grid
              container
              className={styles.healthBar}
              style={{
                width: `${value}%`,
                height: barHeight,
              }}
            />
          </Grid>
          <Typography variant="body2" className={styles.healthBarLabel}>
            {`${currenthp}/${totalhp}`}
          </Typography>
        </Grid>
      </Grid>
    </Grid>
  );
};

export default HealthBarWithLabel;
