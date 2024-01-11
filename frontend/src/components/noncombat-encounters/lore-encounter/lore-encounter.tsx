import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./lore-encounter.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useEffect, useState } from "react";
import { selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { xsScreenWidth } from "@/helpers/global-constants";
import {
  resetLoreEncounterResponse,
  retrieveLoreEncounterQuestionResponse,
  selectLoreEncounterResponse,
  selectLoreEncounterResponseStatus,
  setEncounterComplete,
} from "@/state-mgmt/encounter/encounterSlice";
import DCXAudioPlayer from "@/components/dcx-audio-player/dcx-audio-player";
import { SoundType, Status } from "@/state-mgmt/app/appTypes";
import { LoreDialog, LoreEncounter } from "@dcx/dcx-backend";
import { setPlayButtonClickSound } from "@/state-mgmt/app/appSlice";

interface Props {
  encounter: LoreEncounter;
}

const LoreEncounterComponent: React.FC<Props> = (props: Props) => {
  const { encounter } = props;
  const { width, height } = useWindowDimensions();
  const dispatch = useAppDispatch();
  const { hero } = useAppSelector(selectSelectedHero);
  const loreEncounterResponseStatus = useAppSelector(selectLoreEncounterResponseStatus);
  const loreEncounterResponse = useAppSelector(selectLoreEncounterResponse);

  const [hoveredDialogId, setHoveredDialogId] = useState("");
  const [leaveHover, setLeaveHover] = useState(false);
  const [narrationId, setNarrationId] = useState("");
  const [narratedText, setNarratedText] = useState("");
  const [isQuestionAsked, setQuestionAsked] = useState(false);
  const [askedQuestionSlug, setAskedQuestionSlug] = useState("");

  useEffect(() => {
    setNarrationId(encounter.loreNumber.toLowerCase());
    setNarratedText(encounter.narratedText!);
  }, []);

  useEffect(() => {
    if (loreEncounterResponseStatus.status === Status.Loaded) {
      setQuestionAsked(true);
      setNarrationId(askedQuestionSlug);
      setNarratedText(loreEncounterResponse);
      dispatch(resetLoreEncounterResponse());
    }
    if (loreEncounterResponseStatus.status === Status.Failed) {
      dispatch(resetLoreEncounterResponse());
    }
  }, [loreEncounterResponseStatus]);

  const onDialogHover = (id: string) => {
    setHoveredDialogId(id);
  };

  const handleQuestionClick = (dialog: LoreDialog) => {
    dispatch(setPlayButtonClickSound(true));
    setAskedQuestionSlug(dialog.slug);
    dispatch(
      retrieveLoreEncounterQuestionResponse({
        id: encounter.id,
        slug: dialog.slug,
      })
    );
  };

  const handleLeaveClick = () => {
    dispatch(setPlayButtonClickSound(true));
    dispatch(setEncounterComplete(true));
  };

  const renderDialogues = () => {
    if (encounter.dialogues) {
      const dialogues: JSX.Element[] = [];
      encounter.dialogues.forEach((d) => {
        dialogues.push(
          <Grid
            container
            className={[styles.optionContainer, isQuestionAsked ? styles.cursorDisabled : undefined].join(" ")}
            onClick={() => (isQuestionAsked ? null : handleQuestionClick(d))}
            onMouseEnter={() => onDialogHover(d.slug)}
            onMouseLeave={() => setHoveredDialogId("")}
            key={d.slug}
          >
            <Grid container className={styles.divider} />
            <Grid
              container
              className={styles.opacityContainer}
              style={{ opacity: hoveredDialogId === d.slug ? 0.5 : 0.3 }}
            />
            <Typography component="span" className={styles.optionText}>
              {d.questionText}
            </Typography>
            {isQuestionAsked && <Grid container className={styles.disabledContainer} />}
          </Grid>
        );
      });
      return dialogues;
    }
  };

  return (
    <Grid container direction="row" className={styles.main}>
      <DCXAudioPlayer
        audioUrl={`/audio/voice/${encounter.type.toLowerCase()}s/${encounter.slug!.toLowerCase()}/${narrationId}`}
        soundType={SoundType.VOICE}
      />
      <Grid container className={styles.container}>
        <Grid container className={styles.backgroundImage}>
          <Image
            src="/img/unity-assets/shared/action_bg_vertical.png"
            height={width <= xsScreenWidth ? 570 : 700}
            width={width <= xsScreenWidth ? 370 : 540}
            quality={100}
          />
        </Grid>
        <Grid container className={styles.acheronContainer}>
          <Image
            src="/img/npc/acheron.jpg"
            height={width <= xsScreenWidth ? 302 : 480}
            width={width <= xsScreenWidth ? 338 : 495}
            quality={100}
          />
        </Grid>
        <Grid container className={styles.headerContainer}>
          <Typography component="span" className={styles.headerText}>
            {`ACHERON ENCOUNTER`}
          </Typography>
        </Grid>
        <Grid container className={styles.answerContainer}>
          <Typography component="span" className={styles.answerText}>
            {narratedText}
          </Typography>
        </Grid>
        <Grid container className={styles.optionsContainer}>
          {renderDialogues()}
          <Grid
            container
            className={[styles.optionContainer, !isQuestionAsked ? styles.cursorDisabled : undefined].join(" ")}
            onClick={isQuestionAsked ? handleLeaveClick : undefined}
            onMouseEnter={() => setLeaveHover(true)}
            onMouseLeave={() => setLeaveHover(false)}
          >
            <Grid container className={styles.divider} />
            <Grid container className={styles.opacityContainer} style={{ opacity: leaveHover ? 0.5 : 0.3 }} />
            <Typography component="span" className={styles.leaveText}>
              LEAVE
            </Typography>
            {!isQuestionAsked && <Grid container className={styles.disabledContainer} />}
          </Grid>
        </Grid>
      </Grid>
    </Grid>
  );
};

export default LoreEncounterComponent;
