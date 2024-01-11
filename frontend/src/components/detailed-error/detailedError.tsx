import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import Typography from "@mui/material/Typography";
import styles from "./detailedError.module.scss";
import { useEffect } from "react";
import { useAppDispatch } from "@/state-mgmt/store/hooks";
import { setDisplayFooter } from "@/state-mgmt/app/appSlice";

interface Props {
  message: string;
}

const DetailedError: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  useEffect(() => {
    dispatch(setDisplayFooter(false));
  }, []);

  return (
    <Grid container direction="column" className={styles.container}>
      <Container maxWidth="md">
        <Grid item xs={12}>
          <Typography component="span" variant="h4" color="white">
            {props.message}
          </Typography>
        </Grid>
      </Container>
    </Grid>
  );
};

export default DetailedError;
