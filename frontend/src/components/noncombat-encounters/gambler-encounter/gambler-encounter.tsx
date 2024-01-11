import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./gambler-encounter.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useEffect, useState } from "react";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { ReactPlayer, xsScreenWidth } from "@/helpers/global-constants";
import {
  resetChanceEncounterResponse,
  retrieveChanceEncounterChoiceResult,
  selectChanceEncounterResponse,
  selectChanceEncounterResponseStatus,
  setEncounterComplete,
} from "@/state-mgmt/encounter/encounterSlice";
import { selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { SoundType, Status } from "@/state-mgmt/app/appTypes";
import Modal from "@mui/material/Modal";
import { ChanceEncounter } from "@dcx/dcx-backend";
import DCXButton from "@/components/dcx-button/dcx-button";
import { setPlayButtonClickSound } from "@/state-mgmt/app/appSlice";
import DCXAudioPlayer from "@/components/dcx-audio-player/dcx-audio-player";

interface Props {
  encounter: ChanceEncounter;
}

const GamblerEncounterComponent: React.FC<Props> = (props: Props) => {
  const { encounter } = props;
  const { width, height } = useWindowDimensions();
  const dispatch = useAppDispatch();
  const { hero } = useAppSelector(selectSelectedHero);
  const chanceEncounterResponseStatus = useAppSelector(selectChanceEncounterResponseStatus);
  const chanceEncounterResponse = useAppSelector(selectChanceEncounterResponse);

  const [optionOneHover, setOptionOneHover] = useState(false);
  const [optionTwoHover, setOptionTwoHover] = useState(false);
  const [leaveHover, setLeaveHover] = useState(false);
  const [encounterText, setEncounterText] = useState("");
  const [isChoiceMade, setChoiceMade] = useState(false);
  const [showGambleOptions, setShowGambleOptions] = useState(false);
  const [isGambleSelected, setGambleSelected] = useState(false);
  const [displayDieResults, setDisplayDieResults] = useState(false);
  const [isRolling, setRolling] = useState(false);

  useEffect(() => {
    setEncounterText(encounter.introText!);
  }, []);

  useEffect(() => {
    if (chanceEncounterResponseStatus.status === Status.Loaded) {
      if (isGambleSelected) {
        setShowGambleOptions(true);
      } else {
        setEncounterText(chanceEncounterResponse.outComeText!);
        setChoiceMade(true);
      }
    }
    if (chanceEncounterResponseStatus.status === Status.Failed) {
      dispatch(resetChanceEncounterResponse());
    }
  }, [chanceEncounterResponseStatus]);

  const handleOptionClick = (choiceSlug: string) => {
    if (chanceEncounterResponseStatus.status === Status.NotStarted) {
      dispatch(setPlayButtonClickSound(true));
      choiceSlug === "true" ? setGambleSelected(true) : setGambleSelected(false);
      dispatch(
        retrieveChanceEncounterChoiceResult({
          id: encounter.id,
          slug: choiceSlug,
        })
      );
    }
  };

  const handleRollClick = () => {
    setRolling(true);
  };

  const handleRollEnded = () => {
    setRolling(false);
    setDisplayDieResults(true);
  };

  const handleLeaveClick = () => {
    dispatch(setPlayButtonClickSound(true));
    dispatch(resetChanceEncounterResponse());
    dispatch(setEncounterComplete(true));
  };

  const handleClose = () => {
    setEncounterText(chanceEncounterResponse.outComeText!);
    setChoiceMade(true);
    setShowGambleOptions(false);
  };

  return (
    <Grid container direction="row" className={styles.main}>
      {isRolling && (
        <DCXAudioPlayer audioUrl={"/audio/sound-effects/combat/dice-roll"} soundType={SoundType.SOUND_EFFECT} />
      )}
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
            {encounter.chanceEncounterType}
          </Typography>
        </Grid>
        <Grid container className={styles.gamblerImageContainer}>
          <Grid container className={styles.gamblerBackgroundImage}>
            <Image
              src="/img/unity-assets/shared/action_bg.png"
              height={width <= xsScreenWidth ? 220 : 350}
              width={width <= xsScreenWidth ? 306 : 380}
              quality={100}
            />
          </Grid>
          <Grid container className={styles.gamblerImage}>
            <Image
              src="/img/miscellaneous/gambler.jpg"
              height={width <= xsScreenWidth ? 200 : 320}
              width={width <= xsScreenWidth ? 290 : 401}
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
              onClick={() => handleOptionClick("true")}
              onMouseEnter={() => setOptionOneHover(true)}
              onMouseLeave={() => setOptionOneHover(false)}
            >
              <Grid container className={styles.divider} />
              <Grid container className={styles.opacityContainer} style={{ opacity: optionOneHover ? 0.5 : 0.3 }} />
              <Typography component="span" className={styles.optionText}>
                {`ROLL A 6 SIDED DICE. 3 OR LOWER AND YOU'LL GET A NICE REWARD! 4 OR HIGHER, HOWEVER, AND I TAKE SOMETHING FROM YOU.`}
              </Typography>
            </Grid>
            <Grid
              container
              className={styles.optionContainer}
              onClick={() => handleOptionClick("false")}
              onMouseEnter={() => setOptionTwoHover(true)}
              onMouseLeave={() => setOptionTwoHover(false)}
            >
              <Grid container className={styles.divider} />
              <Grid container className={styles.opacityContainer} style={{ opacity: optionTwoHover ? 0.5 : 0.3 }} />
              <Typography component="span" className={styles.optionText}>
                {`WALK AWAY, YOUR FATE IS YOUR OWN.`}
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
      <Modal
        open={showGambleOptions}
        onClose={displayDieResults ? handleClose : undefined}
        className={styles.modalMain}
      >
        <Grid container className={styles.modalContainer}>
          <Image src="/img/unity-assets/shared/tooltip_bg.png" height={175} width={175} quality={100} />
          <Grid container className={styles.modalDiceContainer}>
            <Grid container className={styles.die}>
              <Image src="/img/unity-assets/combat/dice_bg.png" height={90} width={90} quality={100} />
              <Grid container className={styles.diePlayerContainer}>
                <ReactPlayer
                  playing={isRolling}
                  url={`video/6.mp4`}
                  controls={false}
                  playsinline={true}
                  muted={true}
                  onEnded={() => handleRollEnded()}
                />
              </Grid>
              {displayDieResults && (
                <Typography component="span" className={styles.dieDamage}>
                  {chanceEncounterResponse.diceResult}
                </Typography>
              )}
            </Grid>
            <Grid container className={styles.buttonContainer}>
              <DCXButton
                title={displayDieResults ? "CLOSE" : "ROLL"}
                height={30}
                width={105}
                color="red"
                arrowTopAdjustment={8}
                disabled={isRolling}
                onClick={() => (displayDieResults ? handleClose() : handleRollClick())}
              />
            </Grid>
          </Grid>
        </Grid>
      </Modal>
    </Grid>
  );
};

export default GamblerEncounterComponent;
