import { withHoc } from "@/components/hoc/hoc";
import KeepItemChoice from "@/components/keep-item-choice/keep-item-choice";
import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./death.module.scss";
import Image from "next/image";
import { useAppSelector } from "@/state-mgmt/store/hooks";
import { selectGameState } from "@/state-mgmt/game-state/gameStateSlice";

interface Props {}

const Death: NextPage<Props> = (props: Props) => {
  const gameState = useAppSelector(selectGameState);

  return (
    <Grid container direction="column" className={styles.main}>
      <Image src={`/img/backgrounds/${gameState.slug}-interior.jpg`} layout="fill" quality={100} />
      <Grid container className={styles.opaqueContainer} />
      <KeepItemChoice />
    </Grid>
  );
};

export default withHoc(Death);
