import { withHoc } from "@/components/hoc/hoc";
import Loot from "@/components/loot/loot";
import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./plunder.module.scss";
import Image from "next/image";
import { useAppSelector } from "@/state-mgmt/store/hooks";
import { selectGameState } from "@/state-mgmt/game-state/gameStateSlice";

interface Props {}

const Plunder: NextPage<Props> = (props: Props) => {
  const gameState = useAppSelector(selectGameState);

  return (
    <Grid container direction="column" className={styles.main}>
      <Image src={`/img/backgrounds/${gameState.slug}-interior.jpg`} layout="fill" quality={100} />
      <Grid container className={styles.opaqueContainer} />
      <Loot />
    </Grid>
  );
};

export default withHoc(Plunder);
