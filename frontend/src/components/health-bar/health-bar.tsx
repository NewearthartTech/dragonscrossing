import { Monster } from "@/state-mgmt/monster/monsterTypes";
import Grid from "@mui/material/Grid";
import { HeroDto } from "@dcx/dcx-backend";
import HealthBarWithLabel from "./health-bar-with-label";

interface Props {
  hero?: HeroDto;
  monster?: Monster;
  backgroundBarHeight?: number;
  backgroundBarWidth?: number;
  trimmedWidth?: number;
  barHeight?: number;
  marginTop?: number;
  marginBottom?: number;
}

const HealthBar: React.FC<Props> = (props: Props) => {
  const { hero, monster, backgroundBarHeight, backgroundBarWidth, trimmedWidth, barHeight, marginTop, marginBottom } =
    props;

  let currentHP: number;
  let totalHP: number;

  if (hero && hero.id !== -1) {
    currentHP = hero.remainingHitPoints;
    totalHP = hero.totalHitPoints;
  } else if (monster && monster.id !== "") {
    currentHP = monster.hitPoints;
    totalHP = monster.maxHitPoints;
  } else {
    return null;
  }

  const healthPercentage = (currentHP / totalHP) * 100;

  return (
    <Grid container direction="row">
      <Grid item xs={12}>
        <HealthBarWithLabel
          value={healthPercentage > 100 ? 100 : healthPercentage}
          currenthp={currentHP}
          totalhp={totalHP}
          backgroundBarHeight={backgroundBarHeight}
          backgroundBarWidth={backgroundBarWidth}
          trimmedWidth={trimmedWidth}
          barHeight={barHeight}
          marginTop={marginTop}
          marginBottom={marginBottom}
        />
      </Grid>
    </Grid>
  );
};

export default HealthBar;
