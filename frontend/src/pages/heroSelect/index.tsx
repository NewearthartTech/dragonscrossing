import HeroCard from "@/components/hero-card/hero-card";
import { withHoc } from "@/components/hoc/hoc";
import { selectHeroes } from "@/state-mgmt/hero/heroSlice";
import { useAppSelector } from "@/state-mgmt/store/hooks";
import Grid from "@mui/material/Grid";
import { isBrowser } from "core/utils";
import type { NextPage } from "next";
import styles from "./heroSelect.module.scss";
import { HeroDto } from "@dcx/dcx-backend";
import { useEffect } from "react";

interface Props {}

const HeroSelect: NextPage<Props> = (props: Props) => {
  const heroes = useAppSelector(selectHeroes).heroes;

  const renderHeroCards = () => {
    const heroCards: JSX.Element[] = [];
    const sortedHeroes: Array<HeroDto> = structuredClone(heroes);
    sortedHeroes.sort(function (x, y) {
      return y.seasonId - x.seasonId;
    });
    sortedHeroes.forEach((hero) => {
      heroCards.push(
        <Grid item xs={12} sm={12} md={6} lg={4} xl={3} key={hero.id}>
          <HeroCard hero={hero} />
        </Grid>
      );
    });
    return heroCards;
  };

  if (isBrowser() && heroes.length > 0) {
    return (
      <Grid container direction="column" className={styles.backgroundImage}>
        <Grid container className={styles.opaqueContainer} />
        <Grid container direction="row" className={styles.container}>
          <Grid container direction="row" className={styles.heroCardsContainer}>
            {renderHeroCards()}
          </Grid>
        </Grid>
      </Grid>
    );
  }
  return null;
};

export default withHoc(HeroSelect);
