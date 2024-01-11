import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./link-wallet.module.scss";

interface Props {}

declare const window: any;

const LinkWallet: React.FC<Props> = (props: Props) => {
  return (
    <Grid container direction="column" className={styles.main}>
      {false && (
        <Grid container direction="column" className={styles.container}>
          <Typography component="span" variant="h5" color="white">
            Please login to your Metamask wallet
          </Typography>
        </Grid>
      )}
    </Grid>
  );
};

export default LinkWallet;
