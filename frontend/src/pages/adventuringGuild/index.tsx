import { withHoc } from "@/components/hoc/hoc";
import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./adventuringGuild.module.scss";
import Vendor from "@/components/vendor/vendor";
import Image from "next/image";
import { VendorType } from "@/state-mgmt/app/appTypes";

interface Props {}

const AdventuringGuild: NextPage<Props> = (props: Props) => {
  return (
    <Grid container direction="column" className={styles.main}>
      <Image src="/img/backgrounds/adventuringGuild-interior.jpg" layout="fill" quality={100} />
      <Grid container className={styles.opaqueContainer} />
      <Grid container direction="row" className={styles.container}>
        <Vendor vendorType={VendorType.ADVENTURING_GUILD} />
      </Grid>
    </Grid>
  );
};

export default withHoc(AdventuringGuild);
