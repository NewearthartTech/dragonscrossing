import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./berries-encounter.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useEffect, useState } from "react";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { xsScreenWidth } from "@/helpers/global-constants";
import {
  resetChanceEncounterResponse,
  retrieveChanceEncounterChoiceResult,
  selectChanceEncounterResponse,
  selectChanceEncounterResponseStatus,
  setEncounterComplete,
} from "@/state-mgmt/encounter/encounterSlice";
import { selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { Status } from "@/state-mgmt/app/appTypes";
import { ChanceEncounter } from "@dcx/dcx-backend";
import { setPlayButtonClickSound } from "@/state-mgmt/app/appSlice";

interface Props {
  encounter: ChanceEncounter;
}

const BerriesEncounterComponent: React.FC<Props> = (props: Props) => {
  const { encounter } = props;
  const { width, height } = useWindowDimensions();
  const dispatch = useAppDispatch();
  const { hero } = useAppSelector(selectSelectedHero);
  const chanceEncounterResponseStatus = useAppSelector(selectChanceEncounterResponseStatus);
  const chanceEncounterResponse = useAppSelector(selectChanceEncounterResponse);

  const [optionOneHover, setOptionOneHover] = useState(false);
  const [optionTwoHover, setOptionTwoHover] = useState(false);
  const [optionThreeHover, setOptionThreeHover] = useState(false);
  const [leaveHover, setLeaveHover] = useState(false);
  const [encounterText, setEncounterText] = useState("");
  const [isChoiceMade, setChoiceMade] = useState(false);
  const [isLeaveClicked, setLeaveClicked] = useState(false);

  useEffect(() => {
    setEncounterText(encounter.introText!);
  }, []);

  useEffect(() => {
    if (chanceEncounterResponseStatus.status === Status.Loaded) {
      if (isLeaveClicked) {
        setChoiceMade(true);
        setLeaveClicked(false);
        dispatch(resetChanceEncounterResponse());
        dispatch(setEncounterComplete(true));
      } else {
        setEncounterText(chanceEncounterResponse.outComeText!);
        setChoiceMade(true);
        dispatch(resetChanceEncounterResponse());
      }
    }
    if (chanceEncounterResponseStatus.status === Status.Failed) {
      dispatch(resetChanceEncounterResponse());
    }
  }, [chanceEncounterResponseStatus]);

  const handleOptionClick = (choiceSlug: string) => {
    if (chanceEncounterResponseStatus.status === Status.NotStarted) {
      dispatch(setPlayButtonClickSound(true));
      dispatch(
        retrieveChanceEncounterChoiceResult({
          id: encounter.id,
          slug: choiceSlug,
        })
      );
    }
  };

  const handleLeaveOptionClick = (choiceSlug: string) => {
    dispatch(setPlayButtonClickSound(true));
    setLeaveClicked(true);
    dispatch(
      retrieveChanceEncounterChoiceResult({
        id: encounter.id,
        slug: choiceSlug,
      })
    );
  };

  const handleLeaveClick = () => {
    dispatch(setPlayButtonClickSound(true));
    dispatch(setEncounterComplete(true));
  };

  return (
    <Grid container direction="row" className={styles.main}>
      <Grid container className={styles.container}>
        <Grid container className={styles.backgroundImage}>
          <Image
            src="/img/unity-assets/shared/action_bg_vertical.png"
            height={width <= xsScreenWidth ? 550 : 750}
            width={width <= xsScreenWidth ? 370 : 540}
            quality={100}
          />
        </Grid>
        <Grid container className={styles.headerContainer}>
          <Typography component="span" className={styles.headerText}>
            FOREIGN BERRIES
          </Typography>
        </Grid>
        <Grid container className={styles.berriesImageContainer}>
          <Grid container className={styles.berriesBackgroundImage}>
            <Image
              src="/img/unity-assets/shared/action_bg.png"
              height={width <= xsScreenWidth ? 200 : 270}
              width={width <= xsScreenWidth ? 320 : 443}
              quality={100}
            />
          </Grid>
          <Grid container className={styles.berriesImage}>
            <Image
              src="/img/miscellaneous/foreign-berries.jpg"
              height={width <= xsScreenWidth ? 200 : 320}
              width={width <= xsScreenWidth ? 290 : 400}
              quality={100}
            />
          </Grid>
        </Grid>
        <Grid container className={styles.textContainer}>
          <Typography component="span" className={styles.text}>
            {encounterText}
          </Typography>
        </Grid>
        {!isChoiceMade && (
          <Grid container className={styles.optionsContainer}>
            <Grid
              container
              className={styles.optionContainer}
              onClick={() => handleOptionClick(encounter.choices![0].choiceSlug)}
              onMouseEnter={() => setOptionOneHover(true)}
              onMouseLeave={() => setOptionOneHover(false)}
            >
              <Grid container className={styles.divider} />
              <Grid container className={styles.opacityContainer} style={{ opacity: optionOneHover ? 0.5 : 0.3 }} />
              <Typography component="span" className={styles.optionText}>
                {encounter.choices && encounter.choices[0] !== undefined && `${encounter.choices[0].choiceText}`}
              </Typography>
            </Grid>
            <Grid
              container
              className={styles.optionContainer}
              onClick={() => handleOptionClick(encounter.choices![1].choiceSlug)}
              onMouseEnter={() => setOptionTwoHover(true)}
              onMouseLeave={() => setOptionTwoHover(false)}
            >
              <Grid container className={styles.divider} />
              <Grid container className={styles.opacityContainer} style={{ opacity: optionTwoHover ? 0.5 : 0.3 }} />
              <Typography component="span" className={styles.optionText}>
                {encounter.choices && encounter.choices[1] !== undefined && `${encounter.choices[1].choiceText}`}
              </Typography>
            </Grid>
            <Grid
              container
              className={styles.optionContainer}
              onClick={() => handleLeaveOptionClick(encounter.choices![2].choiceSlug)}
              onMouseEnter={() => setOptionThreeHover(true)}
              onMouseLeave={() => setOptionThreeHover(false)}
            >
              <Grid container className={styles.divider} />
              <Grid container className={styles.opacityContainer} style={{ opacity: optionThreeHover ? 0.5 : 0.3 }} />
              <Typography component="span" className={styles.optionText}>
                {encounter.choices && encounter.choices[2] !== undefined && `${encounter.choices[2].choiceText}`}
              </Typography>
            </Grid>
          </Grid>
        )}
        {isChoiceMade && (
          <Grid container className={styles.leaveOptionsContainer}>
            <Grid
              container
              className={styles.optionContainer}
              onClick={() => handleLeaveClick()}
              onMouseEnter={() => setLeaveHover(true)}
              onMouseLeave={() => setLeaveHover(false)}
            >
              <Grid container className={styles.divider} />
              <Grid container className={styles.opacityContainer} style={{ opacity: leaveHover ? 0.5 : 0.3 }} />
              <Typography component="span" className={styles.optionText}>
                LEAVE
              </Typography>
            </Grid>
          </Grid>
        )}
      </Grid>
    </Grid>
  );
};

export default BerriesEncounterComponent;
