import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import styles from "./herbalist.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import useWindowDimensions from "@/helpers/window-dimensions";
import Image from "next/image";
import { useEffect, useState } from "react";
import DCXButton from "../dcx-button/dcx-button";
import {
  buyHeal,
  resetHeroHealedStatus,
  selectHeroHealedStatus,
  selectSelectedHero,
} from "@/state-mgmt/hero/heroSlice";
import { getHerbalistOptions, selectHerbalistOptions } from "@/state-mgmt/vendor/vendorSlice";
import { SoundType, Status } from "@/state-mgmt/app/appTypes";
import DCXAudioPlayer from "../dcx-audio-player/dcx-audio-player";

interface Props {}

const Herbalist: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const hero = useAppSelector(selectSelectedHero).hero;
  const heroHealedStatus = useAppSelector(selectHeroHealedStatus);
  const herbalistOptions = useAppSelector(selectHerbalistOptions);
  const { width, height } = useWindowDimensions();

  const [showConfirmation, setShowConfirmation] = useState(false);
  const [isFullHeal, setFullHeal] = useState(false);
  const [playHealResponse, setPlayHealResponse] = useState(false);

  useEffect(() => {
    dispatch(getHerbalistOptions());
  }, []);

  useEffect(() => {
    if (heroHealedStatus.status === Status.Loaded) {
      setPlayHealResponse(true);
      dispatch(resetHeroHealedStatus());
    }
  }, [heroHealedStatus]);

  const handleSelectClick = (fullHeal: boolean) => {
    setFullHeal(fullHeal);
    setShowConfirmation(true);
  };

  const handleConfirmClick = () => {
    dispatch(buyHeal(isFullHeal));
    setShowConfirmation(false);
  };

  const handleConfirmClose = () => {
    setShowConfirmation(false);
  };

  const clearSoundResponse = () => {
    setPlayHealResponse(false);
  };

  return (
    <Grid container className={styles.main}>
      {herbalistOptions && herbalistOptions.length > 0 && (
        <Grid container className={styles.mainContainer}>
          {playHealResponse && (
            <DCXAudioPlayer
              audioUrl={`/audio/voice/heroes/${hero.gender.toLowerCase()}-${hero.heroClass.toLowerCase()}/heal-response`}
              soundType={SoundType.VOICE}
              onEnded={() => clearSoundResponse()}
            />
          )}
          <Grid container className={styles.container}>
            <Grid container className={styles.healContainer}>
              <Image src="/img/unity-assets/shared/action_bg.png" height={360} width={360} quality={100} />
              <Grid container className={styles.healImage}>
                <Image src="/img/miscellaneous/partial-heal.png" height={328} width={342} quality={100} />
              </Grid>
              <Grid container className={styles.healTextContainer}>
                <Typography component="span" className={styles.healText}>
                  {herbalistOptions[0].percentage}% HEAL
                </Typography>
              </Grid>
              <DCXButton
                title="SELECT"
                height={32}
                width={120}
                disabled={hero.remainingHitPoints === hero.totalHitPoints || hero.remainingQuests < 1}
                color="blue"
                onClick={() => handleSelectClick(false)}
              />
            </Grid>
            <Grid container className={styles.healContainer}>
              <Image src="/img/unity-assets/shared/action_bg.png" height={360} width={360} quality={100} />
              <Grid container className={styles.healImage}>
                <Image src="/img/miscellaneous/full-heal.png" height={328} width={342} quality={100} />
              </Grid>
              <Grid container className={styles.healTextContainer}>
                <Typography component="span" className={styles.healText}>
                  {herbalistOptions[1].percentage}% HEAL
                </Typography>
              </Grid>
              <DCXButton
                title="SELECT"
                height={32}
                width={120}
                disabled={hero.remainingHitPoints === hero.totalHitPoints || hero.remainingQuests < 2}
                color="blue"
                onClick={() => handleSelectClick(true)}
              />
            </Grid>
          </Grid>
          <Modal open={showConfirmation} onClose={handleConfirmClose} className={styles.modalMain}>
            <Grid container className={styles.confirmModalContainer}>
              <Grid item>
                <Image src="/img/unity-assets/shared/tooltip_bg.png" height={125} width={262.5} quality={100} />
              </Grid>
              <Grid container className={styles.confirmContainer}>
                <Typography component="span" className={styles.headerText}>
                  {`A ${isFullHeal ? herbalistOptions[1].percentage : herbalistOptions[0].percentage}% HEAL WILL COST `}{" "}
                  <span className={styles.healCostText}>
                    {isFullHeal ? herbalistOptions[1].quests : herbalistOptions[0].quests}
                  </span>
                  {` ${isFullHeal ? "QUESTS" : "QUEST"}. DO YOU WANT TO PROCEED?`}
                </Typography>
              </Grid>
              <Grid container className={styles.confirmButton}>
                <DCXButton
                  title="CONFIRM"
                  height={32}
                  width={120}
                  color="blue"
                  disabled={heroHealedStatus.status !== Status.NotStarted}
                  onClick={() => handleConfirmClick()}
                />
              </Grid>
            </Grid>
          </Modal>
        </Grid>
      )}
    </Grid>
  );
};

export default Herbalist;
