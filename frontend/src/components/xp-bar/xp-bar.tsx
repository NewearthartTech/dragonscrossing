import Grid from "@mui/material/Grid";
import { HeroDto } from "@dcx/dcx-backend";
import XPBarWithLabel from "./xp-bar-with-label";

interface Props {
  hero: HeroDto;
  backgroundBarHeight?: number;
  backgroundBarWidth?: number;
  trimmedWidth: number;
  barHeight?: number;
  marginTop?: number;
  marginBottom?: number;
}

const XPBar: React.FC<Props> = (props: Props) => {
  const { hero, backgroundBarHeight, backgroundBarWidth, trimmedWidth, barHeight, marginTop, marginBottom } = props;

  const currentXP = hero.experiencePoints;
  const currentLvlStartXP = 0;
  const nextLvlXP = hero.maxExperiencePoints;
  const totalLvlExp = nextLvlXP - currentLvlStartXP;
  const currentLvlProgress = currentXP - currentLvlStartXP;

  return (
    <Grid container direction="row">
      <Grid item xs={12}>
        <XPBarWithLabel
          value={(currentLvlProgress / totalLvlExp) * 100}
          currentxp={currentXP}
          nextlvlxp={nextLvlXP}
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

export default XPBar;
