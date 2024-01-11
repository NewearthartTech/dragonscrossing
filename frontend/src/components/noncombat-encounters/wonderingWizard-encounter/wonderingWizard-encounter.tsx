import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./wonderingWizard-encounter.module.scss";
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

import { SoundType, Status } from "@/state-mgmt/app/appTypes";

import { ChanceEncounter } from "@dcx/dcx-backend";

import { setPlayButtonClickSound } from "@/state-mgmt/app/appSlice";
import DCXAudioPlayer from "@/components/dcx-audio-player/dcx-audio-player";
import { useRouter } from "next/router";
import { Modal } from "@mui/material";
import DCXButton from "@/components/dcx-button/dcx-button";

interface Props {
  encounter: ChanceEncounter;
  encounterImage:string;
  diceSidesVideo?: string;
}

export const WonderingWizardEncounterComponent: React.FC<Props> = (
  {encounter, encounterImage, diceSidesVideo}: Props
) => {
  
  const router = useRouter();
  const { width, height } = useWindowDimensions();
  const dispatch = useAppDispatch();
  const chanceEncounterResponseStatus = useAppSelector(
    selectChanceEncounterResponseStatus
  );
  const chanceEncounterResponse = useAppSelector(selectChanceEncounterResponse);

  const [optionHoverSlug, setOptionHoverSlug] = useState<string>();
  
  const [leaveHover, setLeaveHover] = useState(false);
  const [encounterText, setEncounterText] = useState("");
  const [isChoiceMade, setChoiceMade] = useState(false);

  const [showGambleOptions, setShowGambleOptions] = useState(false);
  const [displayDieResults, setDisplayDieResults] = useState(false);
  const [isRolling, setRolling] = useState(false);
  

  useEffect(() => {
    setEncounterText(encounter.introText!);
  }, []);

  useEffect(() => {
    if (chanceEncounterResponseStatus.status === Status.Loaded) {
        
        if(chanceEncounterResponse.diceResult){
          setShowGambleOptions(true);
        }else{
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

  const handleClose = () => {
    setEncounterText(chanceEncounterResponse.outComeText!);
    setChoiceMade(true);
    setShowGambleOptions(false);
  };

  
  const handleLeaveClick = () => {

    dispatch(setPlayButtonClickSound(true));
    dispatch(resetChanceEncounterResponse());
    
    if(chanceEncounterResponse.encounterResponceSlug ==="BACKTOADOS"){
      setTimeout(()=>router.push("/aedos"),100);
    }else{
      dispatch(setEncounterComplete(true));
    }

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
          {encounter.encounterName}
          </Typography>
        </Grid>
        <Grid container className={styles.wonderingWizardImageContainer}>
          <Grid container className={styles.wonderingWizardBackgroundImage}>
            <Image
              src="/img/unity-assets/shared/action_bg.png"
              height={width <= xsScreenWidth ? 220 : 350}
              width={width <= xsScreenWidth ? 306 : 380}
              quality={100}
            />
          </Grid>
          <Grid container className={styles.wonderingWizardImage}>
            <Image
              src={`/img/miscellaneous/${encounterImage}`}
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
        {!isChoiceMade && encounter.choices && encounter.choices.length >= 1 && (
          <Grid container className={styles.optionsContainer}>
            {(encounter.choices || []).map((choice, i) => (
              <Grid
                key={i}
                container
                className={styles.optionContainer}
                onClick={() => handleOptionClick(choice.choiceSlug)}
                onMouseEnter={() => setOptionHoverSlug(choice.choiceSlug)}
                onMouseLeave={() => setOptionHoverSlug(undefined)}
              >
                <Grid container className={styles.divider} />
                <Grid
                  container
                  className={styles.opacityContainer}
                  style={{ opacity: (optionHoverSlug === choice.choiceSlug ? 0.5 : 0.3) }}
                />
                <Typography component="span" className={styles.optionText}>
                  {choice.choiceText}
                </Typography>
              </Grid>
            ))}
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
              <Grid
                container
                className={styles.opacityContainer}
                style={{ opacity: leaveHover ? 0.5 : 0.3 }}
              />
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
                  url={`video/${diceSidesVideo||'20.mp4'}`}
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
