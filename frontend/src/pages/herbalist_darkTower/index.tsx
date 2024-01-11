import { withHoc } from "@/components/hoc/hoc";
import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./herbalist-dark-tower.module.scss";
import Image from "next/image";
import Vendor from "@/components/vendor/vendor";
import { VendorType } from "@/state-mgmt/app/appTypes";

interface Props {}

const HerbalistDarkTower: NextPage<Props> = (props: Props) => {
  return (
    <Grid container direction="column" className={styles.main}>
      <Image src="/img/backgrounds/herbalist-interior.jpg" layout="fill" quality={100} />
      <Grid container className={styles.opaqueContainer} />
      <Grid container direction="row" className={styles.container}>
        <Vendor vendorType={VendorType.HERBALIST} />
      </Grid>
    </Grid>
  );
};

export default withHoc(HerbalistDarkTower);
