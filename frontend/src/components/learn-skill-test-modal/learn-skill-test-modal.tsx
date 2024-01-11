import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./learn-skill-test-modal.module.scss";
import Image from "next/image";
import { inputTheme } from "@/helpers/global-constants";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import {
  selectDisplayFixDice,
  selectDisplayLearnSkill,
  setDisplayFixDice,
  setDisplayLearnSkill,
} from "@/state-mgmt/app/appSlice";
import { DiceRollReason } from "@dcx/dcx-backend";
import { useState } from "react";
import Modal from "@mui/material/Modal";
import { ThemeProvider } from "@mui/material/styles";
import TextField from "@mui/material/TextField";
import DCXButton from "../dcx-button/dcx-button";
import { fixDice, learnSkill } from "@/state-mgmt/testing/testingSlice";

interface Props {}

const LearnSkillTestModal: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const displayLearnSkill = useAppSelector(selectDisplayLearnSkill);

  const [skillSlug, setSkillSlug] = useState("");

  const handleClose = () => {
    dispatch(setDisplayLearnSkill(false));
  };

  const handleSubmitClick = () => {
    dispatch(learnSkill(skillSlug));
  };

  const handleSkillSlugChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSkillSlug(event.target.value);
  };

  return (
    <Modal open={displayLearnSkill} onClose={handleClose} className={styles.modalMain}>
      <Grid container className={styles.container}>
        <Image src="/img/unity-assets/shared/tooltip_bg.png" height={120} width={250} quality={100} />
        <Grid container className={styles.transactionRowsContainer}>
          <Grid container className={styles.transactionRow}>
            <Typography component="span" className={styles.detailText}>
              SKILL SLUG
            </Typography>
            <Grid container className={styles.amountContainer}>
              <Grid container className={styles.inputFieldContainer}>
                <ThemeProvider theme={inputTheme}>
                  <TextField variant="outlined" value={skillSlug} onChange={handleSkillSlugChange} />
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

export default LearnSkillTestModal;
