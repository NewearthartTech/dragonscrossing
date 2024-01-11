import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./fix-dice.module.scss";
import Image from "next/image";
import { inputTheme } from "@/helpers/global-constants";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { selectDisplayFixDice, setDisplayFixDice } from "@/state-mgmt/app/appSlice";
import { DiceRollReason } from "@dcx/dcx-backend";
import { useState } from "react";
import Modal from "@mui/material/Modal";
import { ThemeProvider } from "@mui/material/styles";
import TextField from "@mui/material/TextField";
import DCXButton from "../dcx-button/dcx-button";
import { fixDice } from "@/state-mgmt/testing/testingSlice";

interface Props {}

const FixDice: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const displayFixDice = useAppSelector(selectDisplayFixDice);

  const [reason, setReason] = useState("");
  const [value, setValue] = useState(0);

  const handleClose = () => {
    dispatch(setDisplayFixDice(false));
  };

  const handleSubmitClick = () => {
    dispatch(fixDice({ reason: reason as DiceRollReason, value: value }));
  };

  const handleReasonChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setReason(event.target.value);
  };

  const handleValueChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setValue(Number(event.target.value));
  };

  return (
    <Modal open={displayFixDice} onClose={handleClose} className={styles.modalMain}>
      <Grid container className={styles.container}>
        <Image src="/img/unity-assets/shared/tooltip_bg.png" height={120} width={250} quality={100} />
        <Grid container className={styles.transactionRowsContainer}>
          <Grid container className={styles.transactionRow}>
            <Typography component="span" className={styles.detailText}>
              REASON
            </Typography>
            <Grid container className={styles.amountContainer}>
              <Grid container className={styles.inputFieldContainer}>
                <ThemeProvider theme={inputTheme}>
                  <TextField variant="outlined" value={reason} onChange={handleReasonChange} />
                </ThemeProvider>
              </Grid>
            </Grid>
          </Grid>
          <Grid container className={styles.transactionRow}>
            <Typography component="span" className={styles.detailText}>
              VALUE
            </Typography>
            <Grid container className={styles.amountContainer}>
              <Grid container className={styles.inputFieldContainer}>
                <ThemeProvider theme={inputTheme}>
                  <TextField variant="outlined" value={value} type="number" onChange={handleValueChange} />
                </ThemeProvider>
              </Grid>
            </Grid>
          </Grid>

          <Grid container className={styles.buttonContainer}>
            <DCXButton title={"SUBMIT"} height={30} width={100} color="red" onClick={() => handleSubmitClick()} />
          </Grid>
        </Grid>
      </Grid>
    </Modal>
  );
};

export default FixDice;
