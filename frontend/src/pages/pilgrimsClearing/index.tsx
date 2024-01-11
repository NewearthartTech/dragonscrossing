import Encounter from "@/components/encounter/encounter";
import { withHoc } from "@/components/hoc/hoc";
import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./pilgrimsClearing.module.scss";

interface Props {}

const PilgrimsClearing: NextPage<Props> = (props: Props) => {
  return (
    <Grid container direction="column" className={styles.container}>
      <Grid container className={styles.backgroundImage} />
      <Grid container className={styles.opaqueContainer} />
      <Encounter />
    </Grid>
  );
};

export default withHoc(PilgrimsClearing);
