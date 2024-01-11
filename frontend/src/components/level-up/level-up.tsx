import { ReactPlayer, walletAddressMismatch, xsScreenWidth } from "@/helpers/global-constants";
import useWindowDimensions from "@/helpers/window-dimensions";
import { SoundType, Status } from "@/state-mgmt/app/appTypes";
import {
  getLevelUp,
  getLevelUpPolling,
  getSelectedHero,
  submitLevelUpTransaction,
  selectLevelUpHeroSubmittedStatus,
  selectLevelUpPollingStatus,
  selectLevelUpResponse,
  selectLevelUpStatus,
  setLevelUpHeroSubmittedStatus,
  resetLevelUpStatus,
  resetLevelUpPollingStatus,
} from "@/state-mgmt/hero/heroSlice";
import { Hero } from "@/state-mgmt/hero/heroTypes";
import { selectConnectedWalletAddress } from "@/state-mgmt/player/playerSlice";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import Image from "next/image";
import React, { useEffect, useState } from "react";
import DCXButton from "../dcx-button/dcx-button";
import styles from "./level-up.module.scss";
import { CharacterClassDto, DiceRollReason, LevelUpOrder } from "@dcx/dcx-backend";
import CircularProgress from "@mui/material/CircularProgress";
import { useConnectCalls } from "../web3";
import { setAdventuringGuildTab, setAppMessage } from "@/state-mgmt/app/appSlice";
import DCXAudioPlayer from "../dcx-audio-player/dcx-audio-player";

interface Props {
  hero: Hero;
}

const LevelUp = (props: Props) => {
  const dispatch = useAppDispatch();
  const levelUpResponse = useAppSelector(selectLevelUpResponse);
  const levelUpResponseStatus = useAppSelector(selectLevelUpStatus);
  const levelUpHeroSubmittedStatus = useAppSelector(selectLevelUpHeroSubmittedStatus);
  const levelUpPollingStatus = useAppSelector(selectLevelUpPollingStatus);
  const connectedWalletAddress = useAppSelector(selectConnectedWalletAddress);
  const { width, height } = useWindowDimensions();
  const { hero } = props;

  const [statsConfirmation, setStatsConfirmation] = useState(false);
  const [healthRollingStatus, setHealthRollingStatus] = useState("");
  const [statsRollingStatus, setStatsRollingStatus] = useState("");
  const [remainingStats, setRemainingStats] = useState(0);
  const [totalStats, setTotalStats] = useState(0);
  const [strength, setStrength] = useState(0);
  const [agility, setAgility] = useState(0);
  const [wisdom, setWisdom] = useState(0);
  const [quickness, setQuickness] = useState(0);
  const [charisma, setCharisma] = useState(0);
  const [baseHitPoints, setHitpoints] = useState(0);
  const [showStatusModal, setShowStatusModal] = useState(false);
  const [spinnerModalText, setSpinnerModalText] = useState("");
  const [isPolling, setPolling] = useState(false);
  const [disableStatChangeButtons, setDisableStatChangeButtons] = useState(false);

  const { connect } = useConnectCalls();

  useEffect(() => {
    dispatch(getLevelUp(hero.level));
  }, []);

  useEffect(() => {
    if (levelUpResponseStatus.status === Status.Loaded) {
      if (levelUpResponse.fulfillmentTxnHash && levelUpResponse.fulfillmentTxnHash !== "") {
        setHealthRollingStatus("completed");
        setStatsRollingStatus("completed");
        setStrength(levelUpResponse.chosenProps!.strength);
        setAgility(levelUpResponse.chosenProps!.agility);
        setWisdom(levelUpResponse.chosenProps!.wisdom);
        setQuickness(levelUpResponse.chosenProps!.quickness);
        setCharisma(levelUpResponse.chosenProps!.charisma);
        setHitpoints(levelUpResponse.chosenProps!.baseHitPoints);
        setTotalStats(0);
        setRemainingStats(0);
        if (!levelUpResponse.isCompleted && levelUpHeroSubmittedStatus !== "processing") {
          dispatch(setLevelUpHeroSubmittedStatus("processing"));
        }
        if (levelUpResponse.isCompleted) {
          dispatch(setLevelUpHeroSubmittedStatus("completed"));
        }
      } else {
        const totalStatGain =
          Number(levelUpResponse.statsPoints?.basePoints) + Number(levelUpResponse.statsPoints?.xtraPoints);
        setTotalStats(totalStatGain);
        let totalAllocatedStats = 0;
        if (levelUpResponse.chosenProps.strength) {
          totalAllocatedStats += levelUpResponse.chosenProps.strength;
        }
        if (levelUpResponse.chosenProps.agility) {
          totalAllocatedStats += levelUpResponse.chosenProps.agility;
        }
        if (levelUpResponse.chosenProps.wisdom) {
          totalAllocatedStats += levelUpResponse.chosenProps.wisdom;
        }
        if (levelUpResponse.chosenProps.quickness) {
          totalAllocatedStats += levelUpResponse.chosenProps.quickness;
        }
        if (levelUpResponse.chosenProps.charisma) {
          totalAllocatedStats += levelUpResponse.chosenProps.charisma;
        }
        if (levelUpResponse.chosenProps.baseHitPoints) {
          totalAllocatedStats += levelUpResponse.chosenProps.baseHitPoints;
        }

        setRemainingStats(totalStatGain - totalAllocatedStats);
        if (totalStatGain === totalAllocatedStats) {
          setDisableStatChangeButtons(true);
        }
        levelUpResponse.chosenProps!.strength ? setStrength(levelUpResponse.chosenProps!.strength) : undefined;
        levelUpResponse.chosenProps!.agility ? setAgility(levelUpResponse.chosenProps!.agility) : undefined;
        levelUpResponse.chosenProps!.wisdom ? setWisdom(levelUpResponse.chosenProps!.wisdom) : undefined;
        levelUpResponse.chosenProps!.quickness ? setQuickness(levelUpResponse.chosenProps!.quickness) : undefined;
        levelUpResponse.chosenProps!.charisma ? setCharisma(levelUpResponse.chosenProps!.charisma) : undefined;
        levelUpResponse.chosenProps!.baseHitPoints
          ? setHitpoints(levelUpResponse.chosenProps!.baseHitPoints)
          : undefined;
      }
    }
  }, [levelUpResponseStatus]);

  useEffect(() => {
    if (levelUpPollingStatus.status === Status.Loaded) {
      if (levelUpResponse.fulfillmentTxnHash && levelUpResponse.fulfillmentTxnHash !== "") {
        if (!levelUpResponse.isCompleted && levelUpHeroSubmittedStatus !== "processing") {
          dispatch(setLevelUpHeroSubmittedStatus("processing"));
        }
        if (levelUpResponse.isCompleted) {
          dispatch(setLevelUpHeroSubmittedStatus("completed"));
        }
      }
    }
    if (levelUpPollingStatus.status === Status.Failed) {
      setPolling(false);
      setSpinnerModalText("FAILED TO RETRIEVE LEVELUP STATUS");
      setShowStatusModal(true);
    }
  }, [levelUpPollingStatus, levelUpResponse]);

  useEffect(() => {
    if (levelUpHeroSubmittedStatus === "submitting") {
      setSpinnerModalText("SUBMITTING TRANSACTION");
      setShowStatusModal(true);
    }
    if (levelUpHeroSubmittedStatus === "processing") {
      setSpinnerModalText("PROCESSING TRANSACTION");
      setShowStatusModal(true);
      setPolling(true);
    }
    if (levelUpHeroSubmittedStatus === "failed") {
      setSpinnerModalText("FAILED TO COMPLETE TRANSACTION");
      setShowStatusModal(true);
      setPolling(false);
    }
    if (levelUpHeroSubmittedStatus === "completed") {
      dispatch(getSelectedHero());
      setSpinnerModalText("HERO SUCCESSFULLY LEVELLED UP");
      setShowStatusModal(true);
      setPolling(false);
    }
  }, [levelUpHeroSubmittedStatus]);

  useEffect(() => {
    if (isPolling) {
      let timer1 = setInterval(function () {
        dispatch(getLevelUpPolling(hero.level));
      }, 2000);
      return () => {
        clearInterval(timer1);
      };
    }
  }, [isPolling]);

  const handleConfirmClose = () => {
    setStatsConfirmation(false);
  };

  const handleStatusModalClose = () => {
    setRemainingStats(0);
    setHealthRollingStatus("");
    setStatsRollingStatus("");
    setShowStatusModal(false);
    dispatch(setLevelUpHeroSubmittedStatus(""));
    dispatch(resetLevelUpStatus());
    dispatch(resetLevelUpPollingStatus());
    dispatch(setAdventuringGuildTab("skills"));
  };

  const handleHealthRoll = () => {
    setHealthRollingStatus("rolling");
  };

  const handleStatsRoll = () => {
    setStatsRollingStatus("rolling");
  };

  const handleHealthRollEnded = () => {
    setHealthRollingStatus("completed");
  };

  const handleStatsRollEnded = () => {
    setStatsRollingStatus("completed");
  };

  const handleIncrement = (stat: string) => (e: any) => {
    if (isLimitValid(stat) && !disableStatChangeButtons) {
      if (
        stat === "baseHitPoints" &&
        remainingStats > 1 &&
        (!levelUpResponse.fulfillmentTxnHash || levelUpResponse.fulfillmentTxnHash === "")
      ) {
        setRemainingStats(remainingStats - 2);
        setHitpoints(baseHitPoints + 1);
      } else if (
        remainingStats > 0 &&
        stat !== "baseHitPoints" &&
        (!levelUpResponse.fulfillmentTxnHash || levelUpResponse.fulfillmentTxnHash === "")
      ) {
        setRemainingStats(remainingStats - 1);
        switch (stat) {
          case "strength":
            setStrength(strength + 1);
            break;
          case "agility":
            setAgility(agility + 1);
            break;
          case "wisdom":
            setWisdom(wisdom + 1);
            break;
          case "quickness":
            setQuickness(quickness + 1);
            break;
          case "charisma":
            setCharisma(charisma + 1);
            break;
          default:
            null;
        }
      }
    }
  };

  const handleDecrement = (stat: string) => (e: any) => {
    if (
      remainingStats < totalStats &&
      (!levelUpResponse.fulfillmentTxnHash || levelUpResponse.fulfillmentTxnHash === "") &&
      !disableStatChangeButtons
    ) {
      switch (stat) {
        case "strength":
          if (strength > 0) {
            setStrength(strength - 1);
            setRemainingStats(remainingStats + 1);
          }
          break;
        case "agility":
          if (agility > 0) {
            setAgility(agility - 1);
            setRemainingStats(remainingStats + 1);
          }
          break;
        case "wisdom":
          if (wisdom > 0) {
            setWisdom(wisdom - 1);
            setRemainingStats(remainingStats + 1);
          }
          break;
        case "quickness":
          if (quickness > 0) {
            setQuickness(quickness - 1);
            setRemainingStats(remainingStats + 1);
          }
          break;
        case "charisma":
          if (charisma > 0) {
            setCharisma(charisma - 1);
            setRemainingStats(remainingStats + 1);
          }
          break;
        case "baseHitPoints":
          if (baseHitPoints > 0) {
            setHitpoints(baseHitPoints - 1);
            setRemainingStats(remainingStats + 2);
          }
          break;
        default:
          null;
      }
    }
  };

  const isLimitValid = (stat: string) => {
    if (stat === "strength" && strength >= 2) {
      return false;
    } else if (stat === "agility" && agility >= 2) {
      return false;
    } else if (stat === "wisdom" && wisdom >= 2) {
      return false;
    } else if (stat === "quickness" && quickness >= 2) {
      return false;
    } else if (stat === "charisma" && charisma >= 2) {
      return false;
    } else if (stat === "baseHitPoints" && baseHitPoints >= 1) {
      return false;
    }
    return true;
  };

  const handleSubmitStats = () => {
    setStatsConfirmation(true);
  };

  const handleSubmitStatsConfirmation = async () => {
    (await connect(undefined)).getAddress().then(async (address) => {
      if (address.toLowerCase() === connectedWalletAddress.toLowerCase()) {
        setStatsConfirmation(false);
        const levelUpOrder: LevelUpOrder = {
          ...levelUpResponse,
          chosenProps: {
            strength,
            wisdom,
            agility,
            quickness,
            charisma,
            baseHitPoints: baseHitPoints * 2,
          },
        };
        dispatch(
          submitLevelUpTransaction({
            ...levelUpOrder,
            connect: await connect(undefined),
          })
        );
      } else {
        dispatch(setAppMessage({ message: walletAddressMismatch, isClearToken: true }));
      }
    });
  };

  const renderClassSpecificProps = (): string => {
    let heroClassProps = "";
    if (levelUpResponse.classSpecificProps.baseHitPoints) {
      heroClassProps = "HITPOINTS +" + levelUpResponse.classSpecificProps.baseHitPoints.toString();
    }
    if (hero.heroClass === CharacterClassDto.Ranger || hero.heroClass === CharacterClassDto.Mage) {
      heroClassProps += ", ";
    }
    if (levelUpResponse.classSpecificProps.charisma) {
      heroClassProps += "CHARISMA +" + levelUpResponse.classSpecificProps.charisma.toString();
    }
    if (levelUpResponse.classSpecificProps.quickness) {
      heroClassProps += "QUICKNESS +" + levelUpResponse.classSpecificProps.quickness.toString();
    }
    return heroClassProps;
  };

  return (
    <Grid container direction="row" className={styles.main}>
      {levelUpResponseStatus.status === Status.Loaded && (
        <Grid container direction="row" className={styles.container}>
          {healthRollingStatus === "rolling" && (
            <DCXAudioPlayer audioUrl={`/audio/sound-effects//combat/dice-roll`} soundType={SoundType.SOUND_EFFECT} />
          )}
          {statsRollingStatus === "rolling" && (
            <DCXAudioPlayer audioUrl={`/audio/sound-effects//combat/dice-roll`} soundType={SoundType.SOUND_EFFECT} />
          )}
          <Grid container direction="column" className={styles.categoryContainer}>
            <Grid container className={styles.categoryHeaderBackground}>
              <Image src="/img/unity-assets/shared/header.png" height={33} width={200} quality={100} />
            </Grid>
            <Grid container className={styles.healthHeader}>
              <Typography component="span" className={styles.headerText}>
                HITPOINTS
              </Typography>
            </Grid>
            <Image src="/img/unity-assets/level-up/level_up_category_bg.png" height={200} width={275} quality={100} />
            <Grid container className={styles.healthSymbol}>
              <Image src="/img/unity-assets/level-up/health_symbol.png" height={200} width={200} quality={100} />
            </Grid>
            <Grid container className={styles.dieBackground}>
              <Image src="/img/unity-assets/combat/dice_bg.png" height={80} width={80} quality={100} />
            </Grid>
            <Grid container className={styles.dieContainer}>
              <ReactPlayer
                playing={healthRollingStatus === "rolling"}
                url={`video/${
                  levelUpResponse.diceRolls?.find((d) => d.rollFor === DiceRollReason.LevelUpHitPoints)?.sides
                }.mp4`}
                controls={false}
                playsinline={true}
                muted={true}
                onEnded={() => handleHealthRollEnded()}
              />
            </Grid>
            {healthRollingStatus === "completed" && (
              <Grid container className={styles.dieResults}>
                <Typography component="span" className={styles.dieResultsText}>
                  {levelUpResponse.hitPoints?.xtraPoints}
                </Typography>
              </Grid>
            )}
            <Grid container className={styles.rollButton}>
              <DCXButton
                title="ROLL"
                height={38}
                width={130}
                color="blue"
                disabled={healthRollingStatus === "rolling" || healthRollingStatus === "completed"}
                onClick={() => handleHealthRoll()}
              />
            </Grid>
            <Grid container className={styles.resultsContainer}>
              <Image src="/img/unity-assets/shared/tooltip_bg.png" height={90} width={275} quality={100} />
              {healthRollingStatus === "completed" && (
                <Grid container className={styles.resultsBreakdown}>
                  <Grid container className={styles.resultsRow}>
                    <Typography component="span" className={styles.resultsHeaderText}>
                      BASE HP GAIN
                    </Typography>
                    <Typography component="span" className={styles.resultsText}>
                      {levelUpResponse.hitPoints?.basePoints}
                    </Typography>
                  </Grid>
                  <Grid container className={styles.resultsRow}>
                    <Typography component="span" className={styles.resultsHeaderText}>
                      ROLL HP BONUS
                    </Typography>
                    <Typography component="span" className={styles.resultsText}>
                      {levelUpResponse.hitPoints?.xtraPoints}
                    </Typography>
                  </Grid>
                  <Grid container className={styles.resultsRow}>
                    <Typography component="span" className={styles.resultsHeaderText}>
                      TOTAL HP GAIN
                    </Typography>
                    <Typography component="span" className={styles.resultsText}>
                      {Number(levelUpResponse.hitPoints?.basePoints) + Number(levelUpResponse.hitPoints?.xtraPoints)}
                    </Typography>
                  </Grid>
                </Grid>
              )}
            </Grid>
          </Grid>
          <Grid container direction="column" className={styles.skillCategoryContainer}>
            <Grid container className={styles.categoryHeaderBackground}>
              <Image src="/img/unity-assets/shared/header.png" height={33} width={200} quality={100} />
            </Grid>
            <Grid container className={styles.skillPointsHeader}>
              <Typography component="span" className={styles.headerText}>
                SKILL POINTS
              </Typography>
            </Grid>
            <Image src="/img/unity-assets/level-up/level_up_category_bg.png" height={200} width={275} quality={100} />
            <Grid container className={styles.skillPointsSymbol}>
              <Image src="/img/unity-assets/level-up/skill_points_symbol.png" height={175} width={175} quality={100} />
            </Grid>
            <Grid container className={styles.resultsContainer}>
              <Image src="/img/unity-assets/shared/tooltip_bg.png" height={90} width={275} quality={100} />
              <Grid container className={styles.resultsBreakdown}>
                <Grid container className={styles.resultsRow}>
                  <Typography component="span" className={styles.resultsHeaderText}>
                    TOTAL SKILL POINTS GAIN
                  </Typography>
                  <Typography component="span" className={styles.resultsText}>
                    {levelUpResponse.skillPoints.basePoints + levelUpResponse.skillPoints.xtraPoints}
                  </Typography>
                </Grid>
              </Grid>
            </Grid>
          </Grid>
          <Grid container direction="column" className={styles.categoryContainer}>
            <Grid container className={styles.categoryHeaderBackground}>
              <Image src="/img/unity-assets/shared/header.png" height={33} width={200} quality={100} />
            </Grid>
            <Grid container className={styles.statsHeader}>
              <Typography component="span" className={styles.headerText}>
                STAT POINTS
              </Typography>
            </Grid>
            <Image src="/img/unity-assets/level-up/level_up_category_bg.png" height={200} width={275} quality={100} />
            <Grid container className={styles.statsSymbol}>
              <Image src="/img/unity-assets/level-up/stats_symbol.png" height={189} width={159} quality={100} />
            </Grid>
            <Grid container className={styles.dieBackground}>
              <Image src="/img/unity-assets/combat/dice_bg.png" height={80} width={80} />
            </Grid>
            <Grid container className={styles.dieContainer}>
              <ReactPlayer
                playing={statsRollingStatus === "rolling"}
                url={`video/${
                  levelUpResponse.diceRolls?.find((d) => d.rollFor === DiceRollReason.LevelUpStats)?.sides
                }.mp4`}
                controls={false}
                playsinline={true}
                muted={true}
                onEnded={() => handleStatsRollEnded()}
              />
            </Grid>
            {statsRollingStatus === "completed" && (
              <Grid container className={styles.dieResults}>
                <Typography component="span" className={styles.dieResultsText}>
                  {levelUpResponse.statsPoints?.xtraPoints}
                </Typography>
              </Grid>
            )}
            <Grid container className={styles.rollButton}>
              <DCXButton
                title="ROLL"
                height={38}
                width={130}
                color="blue"
                disabled={statsRollingStatus === "rolling" || statsRollingStatus === "completed"}
                onClick={() => handleStatsRoll()}
              />
            </Grid>
            <Grid container className={styles.resultsContainer}>
              <Image src="/img/unity-assets/shared/tooltip_bg.png" height={90} width={275} quality={100} />
              {statsRollingStatus === "completed" && (
                <Grid container className={styles.resultsBreakdown}>
                  <Grid container className={styles.resultsRow}>
                    <Typography component="span" className={styles.resultsHeaderText}>
                      BASE STAT POINTS GAIN
                    </Typography>
                    <Typography component="span" className={styles.resultsText}>
                      {levelUpResponse.statsPoints?.basePoints}
                    </Typography>
                  </Grid>
                  <Grid container className={styles.resultsRow}>
                    <Typography component="span" className={styles.resultsHeaderText}>
                      ROLL STAT POINTS BONUS
                    </Typography>
                    <Typography component="span" className={styles.resultsText}>
                      {levelUpResponse.statsPoints?.xtraPoints}
                    </Typography>
                  </Grid>
                  <Grid container className={styles.resultsRow}>
                    <Typography component="span" className={styles.resultsHeaderText}>
                      TOTAL STAT POINTS GAIN
                    </Typography>
                    <Typography component="span" className={styles.resultsText}>
                      {Number(levelUpResponse.statsPoints?.basePoints) +
                        Number(levelUpResponse.statsPoints?.xtraPoints)}
                    </Typography>
                  </Grid>
                </Grid>
              )}
            </Grid>
          </Grid>
          {healthRollingStatus === "completed" && statsRollingStatus === "completed" && (
            <Grid container direction="row" className={styles.bottomSectionContainer}>
              {levelUpResponse.classSpecificProps.baseHitPoints &&
              levelUpResponse.classSpecificProps.baseHitPoints > 0 ? (
                <Grid container className={styles.classBonusContainer}>
                  <Typography component="span" className={styles.classBonusText}>
                    CLASS BONUS:
                  </Typography>
                  <Grid container direction="row" className={styles.classPropsContainer}>
                    <Typography component="span" className={styles.classPropsText}>
                      {renderClassSpecificProps()}
                    </Typography>
                  </Grid>
                </Grid>
              ) : undefined}
              <Grid container className={styles.statDistributionContainer}>
                <Image
                  src="/img/unity-assets/shared/window_bg_right_small.png"
                  height={134}
                  width={width > xsScreenWidth ? 350 : 310}
                  quality={100}
                />
                <Grid container className={styles.statDistributionInfoContainer}>
                  <Typography component="span" className={styles.infoText}>
                    MAX 2 POINTS PER STAT - HP COSTS 2 POINTS PER STAT
                  </Typography>
                </Grid>
                <Grid container className={styles.statsContainer}>
                  <Grid container className={styles.statRow}>
                    <Typography component="span" className={styles.statText}>
                      STRENGTH
                    </Typography>
                    <Grid container direction="row" className={styles.iconPlusMinusContainer}>
                      <Grid container className={styles.minusIcon} onClick={handleDecrement("strength")}>
                        <Image
                          src="/img/unity-assets/shared/minus_icon.png"
                          height={10}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                      <Grid container className={styles.stats}>
                        <Typography component="span" className={[styles.statText, styles.statPadding].join(" ")}>
                          {hero.strength}
                        </Typography>
                        <Typography component="span" className={styles.statText}>
                          {`(`}
                          <span className={strength > 0 ? styles.statIncreasePositive : styles.statIncrease}>
                            {strength}
                          </span>
                          {`)`}
                        </Typography>
                      </Grid>
                      <Grid container className={styles.plusIcon} onClick={handleIncrement("strength")}>
                        <Image
                          src="/img/unity-assets/shared/plus_icon.png"
                          height={18}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                    </Grid>
                  </Grid>
                  <Grid container className={styles.statRow}>
                    <Typography component="span" className={styles.statText}>
                      WISDOM
                    </Typography>
                    <Grid container direction="row" className={styles.iconPlusMinusContainer}>
                      <Grid container className={styles.minusIcon} onClick={handleDecrement("wisdom")}>
                        <Image
                          src="/img/unity-assets/shared/minus_icon.png"
                          height={10}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                      <Grid container className={styles.stats}>
                        <Typography component="span" className={[styles.statText, styles.statPadding].join(" ")}>
                          {hero.wisdom}
                        </Typography>
                        <Typography component="span" className={styles.statText}>
                          {`(`}
                          <span className={wisdom > 0 ? styles.statIncreasePositive : styles.statIncrease}>
                            {wisdom}
                          </span>
                          {`)`}
                        </Typography>
                      </Grid>
                      <Grid container className={styles.plusIcon} onClick={handleIncrement("wisdom")}>
                        <Image
                          src="/img/unity-assets/shared/plus_icon.png"
                          height={18}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                    </Grid>
                  </Grid>
                  <Grid container className={styles.statRow}>
                    <Typography component="span" className={styles.statText}>
                      AGILITY
                    </Typography>
                    <Grid container direction="row" className={styles.iconPlusMinusContainer}>
                      <Grid container className={styles.minusIcon} onClick={handleDecrement("agility")}>
                        <Image
                          src="/img/unity-assets/shared/minus_icon.png"
                          height={10}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                      <Grid container className={styles.stats}>
                        <Typography component="span" className={[styles.statText, styles.statPadding].join(" ")}>
                          {hero.agility}
                        </Typography>
                        <Typography component="span" className={styles.statText}>
                          {`(`}
                          <span className={agility > 0 ? styles.statIncreasePositive : styles.statIncrease}>
                            {agility}
                          </span>
                          {`)`}
                        </Typography>
                      </Grid>
                      <Grid container className={styles.plusIcon} onClick={handleIncrement("agility")}>
                        <Image
                          src="/img/unity-assets/shared/plus_icon.png"
                          height={18}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                    </Grid>
                  </Grid>
                  <Grid container className={styles.statRow}>
                    <Typography component="span" className={styles.statText}>
                      QUICKNESS
                    </Typography>
                    <Grid container direction="row" className={styles.iconPlusMinusContainer}>
                      <Grid container className={styles.minusIcon} onClick={handleDecrement("quickness")}>
                        <Image
                          src="/img/unity-assets/shared/minus_icon.png"
                          height={10}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                      <Grid container className={styles.stats}>
                        <Typography component="span" className={[styles.statText, styles.statPadding].join(" ")}>
                          {hero.quickness}
                        </Typography>
                        <Typography component="span" className={styles.statText}>
                          {`(`}
                          <span className={quickness > 0 ? styles.statIncreasePositive : styles.statIncrease}>
                            {quickness}
                          </span>
                          {`)`}
                        </Typography>
                      </Grid>
                      <Grid container className={styles.plusIcon} onClick={handleIncrement("quickness")}>
                        <Image
                          src="/img/unity-assets/shared/plus_icon.png"
                          height={18}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                    </Grid>
                  </Grid>
                  <Grid container className={styles.statRow}>
                    <Typography component="span" className={styles.statText}>
                      CHARISMA
                    </Typography>
                    <Grid container direction="row" className={styles.iconPlusMinusContainer}>
                      <Grid container className={styles.minusIcon} onClick={handleDecrement("charisma")}>
                        <Image
                          src="/img/unity-assets/shared/minus_icon.png"
                          height={10}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                      <Grid container className={styles.stats}>
                        <Typography component="span" className={[styles.statText, styles.statPadding].join(" ")}>
                          {hero.charisma}
                        </Typography>
                        <Typography component="span" className={styles.statText}>
                          {`(`}
                          <span className={charisma > 0 ? styles.statIncreasePositive : styles.statIncrease}>
                            {charisma}
                          </span>
                          {`)`}
                        </Typography>
                      </Grid>
                      <Grid container className={styles.plusIcon} onClick={handleIncrement("charisma")}>
                        <Image
                          src="/img/unity-assets/shared/plus_icon.png"
                          height={18}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                    </Grid>
                  </Grid>
                  <Grid container className={styles.statRow}>
                    <Typography component="span" className={styles.statText}>
                      HITPOINTS
                    </Typography>
                    <Grid container direction="row" className={styles.iconPlusMinusContainer}>
                      <Grid container className={styles.minusIcon} onClick={handleDecrement("baseHitPoints")}>
                        <Image
                          src="/img/unity-assets/shared/minus_icon.png"
                          height={10}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                      <Grid container className={styles.stats}>
                        <Typography component="span" className={[styles.statText, styles.statPadding].join(" ")}>
                          {hero.baseHitPoints}
                        </Typography>
                        <Typography component="span" className={styles.statText}>
                          {`(`}
                          <span className={baseHitPoints > 0 ? styles.statIncreasePositive : styles.statIncrease}>
                            {baseHitPoints}
                          </span>
                          {`)`}
                        </Typography>
                      </Grid>
                      <Grid container className={styles.plusIcon} onClick={handleIncrement("baseHitPoints")}>
                        <Image
                          src="/img/unity-assets/shared/plus_icon.png"
                          height={18}
                          width={18}
                          quality={100}
                          className={styles.hoverPointer}
                        />
                      </Grid>
                    </Grid>
                  </Grid>
                </Grid>
              </Grid>
              <Grid container direction="column" className={styles.bottomRightContainer}>
                <Grid container className={styles.remainingStatsContainer}>
                  <Image src="/img/unity-assets/shared/text_bg.png" height={35} width={160} quality={100} />
                  <Grid container className={styles.remainingStats}>
                    <Typography
                      component="span"
                      className={styles.remainingStatsText}
                    >{`REMAINING POINTS: ${remainingStats}`}</Typography>
                  </Grid>
                </Grid>
                <Grid container className={styles.submitButton}>
                  <DCXButton
                    title="SUBMIT LEVEL UP"
                    height={38}
                    width={130}
                    color="blue"
                    disabled={
                      remainingStats > 0 ||
                      (levelUpResponse.fulfillmentTxnHash !== null && levelUpResponse.fulfillmentTxnHash !== "")
                    }
                    onClick={() => handleSubmitStats()}
                  />
                </Grid>
              </Grid>
            </Grid>
          )}
          <Modal open={statsConfirmation} onClose={handleConfirmClose} className={styles.modalMain}>
            <Grid container className={styles.levelUpConfirmModalContainer}>
              <Image src="/img/unity-assets/shared/tooltip_bg.png" height={125} width={262} quality={100} />
              <Grid container direction="column" className={styles.levelUpConfirmContainer}>
                <Typography component="span" className={styles.dcxCostHeader}>
                  {`CONFIRM STAT SELECTIONS?`}
                </Typography>
                {/* <Grid container className={styles.divider} />
                <Typography component="span" className={styles.dcxCostText}>
                  {levelUpResponse.priceInDcx}
                </Typography> */}
                <Grid container className={styles.confirmButtonContainer}>
                  <DCXButton
                    title="CONFIRM"
                    height={30}
                    width={100}
                    color="blue"
                    hideArrows={true}
                    disabled={levelUpHeroSubmittedStatus !== ""}
                    onClick={() => handleSubmitStatsConfirmation()}
                  />
                </Grid>
              </Grid>
            </Grid>
          </Modal>
          <Modal open={showStatusModal} onClose={handleStatusModalClose} className={styles.modalMain}>
            <Grid container className={styles.pendingTransactionModalContainer}>
              <Image src="/img/unity-assets/shared/tooltip_bg.png" height={110} width={168} quality={100} />
              <Grid container direction="column" className={styles.levelUpStatusContainer}>
                {levelUpHeroSubmittedStatus !== "failed" &&
                  levelUpHeroSubmittedStatus !== "completed" &&
                  levelUpPollingStatus.status !== Status.Failed && (
                    <Grid container className={styles.spinnerContainer}>
                      <CircularProgress className={styles.spinner} />
                    </Grid>
                  )}
                <Typography component="span" className={styles.message}>
                  {spinnerModalText}
                </Typography>
              </Grid>
            </Grid>
          </Modal>
        </Grid>
      )}
    </Grid>
  );
};

export default LevelUp;
