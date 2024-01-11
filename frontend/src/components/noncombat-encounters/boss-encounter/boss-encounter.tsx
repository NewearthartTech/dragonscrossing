import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./boss-encounter.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useState } from "react";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { xsScreenWidth } from "@/helpers/global-constants";
import { retrieveBossEncounterFinish } from "@/state-mgmt/encounter/encounterSlice";
import DCXAudioPlayer from "@/components/dcx-audio-player/dcx-audio-player";
import { SoundType } from "@/state-mgmt/app/appTypes";
import { BossEncounter } from "@dcx/dcx-backend";
import { setDisplayAchievementModal, setPlayButtonClickSound } from "@/state-mgmt/app/appSlice";
import { selectGameState } from "@/state-mgmt/game-state/gameStateSlice";

interface Props {
  encounter: BossEncounter;
}

const BossEncounterComponent: React.FC<Props> = (props: Props) => {
  const { encounter } = props;
  const { width, height } = useWindowDimensions();
  const dispatch = useAppDispatch();

  const gameState = useAppSelector(selectGameState);

  const [leaveHover, setLeaveHover] = useState(false);

  const handleLeaveClick = () => {
    dispatch(setPlayButtonClickSound(true));
    dispatch(retrieveBossEncounterFinish(encounter.id));
    if (encounter.slug === "acheron-outro") {
      dispatch(setDisplayAchievementModal(true));
    }
  };

  return (
    <Grid container direction="row" className={styles.main}>
      {encounter && encounter.type && encounter.slug && encounter.narratedText !== "" && (
        <DCXAudioPlayer
          audioUrl={`/audio/voice/${encounter.type.toLowerCase()}s/${encounter.slug.toLowerCase()}`}
          soundType={SoundType.VOICE}
        />
      )}
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
            src={
              encounter.slug?.toLowerCase() === "abaddon-outro"
                ? "/img/api/monsters/libraryOfTheArchmage/abaddon-the-destroyer.png"
                : "/img/npc/acheron.jpg"
            }
            height={width <= xsScreenWidth ? 490 : 620}
            width={width <= xsScreenWidth ? 338 : 495}
            quality={100}
          />
        </Grid>
        <Grid container className={styles.headerContainer}>
          <Typography component="span" className={styles.headerText}>
            {encounter.slug?.toLowerCase() === "abaddon-outro" ? `ABADDON ENCOUNTER` : `ACHERON ENCOUNTER`}
          </Typography>
        </Grid>
        <Grid container className={styles.answerContainer}>
          <Typography component="span" className={styles.answerText}>
            {encounter.narratedText}
          </Typography>
        </Grid>
        <Grid container className={styles.optionsContainer}>
          <Grid
            container
            className={styles.optionContainer}
            onClick={handleLeaveClick}
            onMouseEnter={() => setLeaveHover(true)}
            onMouseLeave={() => setLeaveHover(false)}
          >
            <Grid container className={styles.divider} />
            <Grid container className={styles.opacityContainer} style={{ opacity: leaveHover ? 0.5 : 0.3 }} />
            <Typography component="span" className={styles.leaveText}>
              LEAVE
            </Typography>
          </Grid>
        </Grid>
      </Grid>
    </Grid>
  );
};

export default BossEncounterComponent;
