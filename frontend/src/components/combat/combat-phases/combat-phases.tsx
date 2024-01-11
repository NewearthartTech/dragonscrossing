import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import styles from "./combat-phases.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { selectDisplayCombatPhases, setDisplayCombatPhases } from "@/state-mgmt/app/appSlice";
import DCXItem from "@/components/dcx-item/dcx-item";
import { useEffect, useState } from "react";
import DCXButton from "@/components/dcx-button/dcx-button";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { mdScreenWidth, ReactPlayer, tooltipTheme, voicelessMonsters, xsScreenWidth } from "@/helpers/global-constants";
import { selectPlayerSettings } from "@/state-mgmt/player/playerSlice";
import {
  ActionResponseDto,
  CharismaOpportunityResultType,
  CombatantType,
  DiceRollReason,
  DieResultDto,
  MonsterPersonalityTypeDto,
} from "@dcx/dcx-backend";
import DCXAudioPlayer from "@/components/dcx-audio-player/dcx-audio-player";
import { SoundType } from "@/state-mgmt/app/appTypes";
import Tooltip from "@mui/material/Tooltip";
import Zoom from "@mui/material/Zoom";
import { ThemeProvider } from "@mui/material/styles";
import CloseButton from "@/components/close-button/close-button";
import {
  selectFleeAttempt,
  selectPersuadeAttempt,
  setFleeAttempt,
  setPersuadeAttempt,
} from "@/state-mgmt/combat/combatSlice";
import { rng } from "@/helpers/shared-functions";
import { selectGameState } from "@/state-mgmt/game-state/gameStateSlice";
import PriorityHighIcon from "@mui/icons-material/PriorityHigh";

interface Props {
  combatActionResult: ActionResponseDto;
  introsPlaying: boolean;
  outrosPlaying: boolean;
}

const CombatPhases: React.FC<Props> = (props: Props) => {
  const { combatActionResult, introsPlaying, outrosPlaying } = props;
  const dispatch = useAppDispatch();
  const displayCombatPhases = useAppSelector(selectDisplayCombatPhases);
  const isFleeAttempt = useAppSelector(selectFleeAttempt);
  const isPersuadeAttempt = useAppSelector(selectPersuadeAttempt);
  const playerSettings = useAppSelector(selectPlayerSettings);
  const gameState = useAppSelector(selectGameState);
  const { width, height } = useWindowDimensions();

  const [heroRollingStatus, setHeroRollingStatus] = useState("waiting");
  const [monsterRollingStatus, setMonsterRollingStatus] = useState("waiting");
  const [secondaryDiceRollingStatus, setSecondaryDiceRollingStatus] = useState("waiting");
  const [displayHeroResults, setDisplayHeroResults] = useState(false);
  const [displayMonsterResults, setDisplayMonsterResults] = useState(false);
  const [displaySecondaryResults, setDisplaySecondaryResults] = useState(false);
  const [flipHeroAccuracyCard, setFlipHeroAccuracyCard] = useState(false);
  const [heroAccuracyFlipping, setHeroAccuracyFlipping] = useState(false);
  const [heroAccuracyFlipped, setHeroAccuracyFlipped] = useState(false);
  const [flipMonsterAccuracyCard, setFlipMonsterAccuracyCard] = useState(false);
  const [monsterAccuracyFlipping, setMonsterAccuracyFlipping] = useState(false);
  const [monsterAccuracyFlipped, setMonsterAccuracyFlipped] = useState(false);
  const [displayMonsterDmgBreakdown, setDisplayMonsterDmgBreakdown] = useState(false);
  const [displayHeroDmgBreakdown, setDisplayHeroDmgBreakdown] = useState(false);
  const [heroTotalDiceDamage, setHeroTotalDiceDamage] = useState(0);
  const [monsterTotalDiceDamage, setMonsterTotalDiceDamage] = useState(0);
  const [heroVoiceSlug, setHeroVoiceSlug] = useState("");
  const [monsterVoiceSlug, setMonsterVoiceSlug] = useState("");
  const [isHeroVoicePlaying, setHeroVoicePlaying] = useState(false);
  const [isMonsterVoicePlaying, setMonsterVoicePlaying] = useState(false);
  const [displayHeroCritEvent, setDisplayHeroCritEvent] = useState(false);
  const [displayHeroParryEvent, setDisplayHeroParryEvent] = useState(false);
  const [displayHeroArmorEvent, setDisplayHeroArmorEvent] = useState(false);
  const [displayMonsterCritEvent, setDisplayMonsterCritEvent] = useState(false);
  const [displayMonsterParryEvent, setDisplayMonsterParryEvent] = useState(false);
  const [displayMonsterArmorEvent, setDisplayMonsterArmorEvent] = useState(false);
  const [flipMonsterAndHeroAccuracy, setFlipMonsterAndHeroAccuracy] = useState(false);

  useEffect(() => {
    if (displayCombatPhases === "yes") {
      if (playerSettings.autoRoll) {
        setHeroRollingStatus("completed");
        setMonsterRollingStatus("completed");
        setSecondaryDiceRollingStatus("completed");
        setHeroAccuracyFlipping(true);
        setMonsterAccuracyFlipping(true);
        setHeroAccuracyFlipped(true);
        setMonsterAccuracyFlipped(true);
        setDisplayHeroResults(true);
        setDisplayMonsterResults(true);
        setDisplaySecondaryResults(true);
      }
      if (combatActionResult.initiative === CombatantType.Monster && !playerSettings.autoRoll) {
        handleMonsterAccuracyFlip();
      }
      if (combatActionResult.initiative === CombatantType.Hero && !playerSettings.autoRoll) {
        if (!isFleeAttempt && !isPersuadeAttempt) {
          handleHeroAccuracyFlip();
        }
      }
    }
  }, [displayCombatPhases]);

  useEffect(() => {
    if (
      combatActionResult.initiative === CombatantType.Hero &&
      heroRollingStatus === "completed" &&
      !playerSettings.autoRoll
    ) {
      if (combatActionResult.didHeroFlee) {
        setMonsterRollingStatus("completed");
      } else {
        if (!isPersuadeAttempt && !isFleeAttempt) {
          handleMonsterAccuracyFlip();
        }
      }
    }
  }, [heroRollingStatus]);

  useEffect(() => {
    if (
      combatActionResult.initiative === CombatantType.Monster &&
      monsterRollingStatus === "completed" &&
      !playerSettings.autoRoll
    ) {
      handleHeroAccuracyFlip();
    }
  }, [monsterRollingStatus]);

  useEffect(() => {
    if (secondaryDiceRollingStatus === "completed" && !playerSettings.autoRoll) {
      const timer3 = setTimeout(() => {
        setFlipMonsterAndHeroAccuracy(true);
      }, 50);
      return () => {
        clearTimeout(timer3);
      };
    }
  }, [secondaryDiceRollingStatus]);

  useEffect(() => {
    if (flipHeroAccuracyCard) {
      setHeroAccuracyFlipping(true);
      const timer2 = setTimeout(() => {
        setHeroAccuracyFlipped((prevVal) => !prevVal);
      }, 750);
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [flipHeroAccuracyCard]);

  useEffect(() => {
    if (heroAccuracyFlipped && !playerSettings.autoRoll) {
      if (!combatActionResult.heroResult.attackResult.isHit) {
        setHeroRollingStatus("completed");
      }
    }
  }, [heroAccuracyFlipped]);

  useEffect(() => {
    if (flipMonsterAccuracyCard) {
      setMonsterAccuracyFlipping(true);
      const timer2 = setTimeout(() => {
        setMonsterAccuracyFlipped((prevVal) => !prevVal);
      }, 750);
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [flipMonsterAccuracyCard]);

  useEffect(() => {
    if (flipMonsterAndHeroAccuracy) {
      setHeroAccuracyFlipping(true);
      setMonsterAccuracyFlipping(true);
      const timer2 = setTimeout(() => {
        setHeroAccuracyFlipped((prevVal) => !prevVal);
        setMonsterAccuracyFlipped((prevVal) => !prevVal);
      }, 750);
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [flipMonsterAndHeroAccuracy]);

  useEffect(() => {
    if (monsterAccuracyFlipped && !playerSettings.autoRoll) {
      combatActionResult.monsterResult.attackResult.isHit
        ? handleMonsterRollToHit()
        : setMonsterRollingStatus("completed");
    }
  }, [monsterAccuracyFlipped]);

  useEffect(() => {
    if (heroRollingStatus === "completed") {
      if (playerSettings.playVoice) {
        if (Number(combatActionResult.heroResult.attackResult.critDamage) > 0 && !isHeroVoicePlaying) {
          setHeroVoicePlaying(true);
          setHeroVoiceSlug(
            combatActionResult.heroResult.hero.gender.toLowerCase() +
              "-" +
              combatActionResult.heroResult.hero.heroClass.toLowerCase() +
              "/crit-" +
              rng(1, 2)
          );
        }
        if (
          Number(combatActionResult.heroResult.attackResult.parryMitigation) > 0 &&
          !isVoicelessMonster &&
          !isMonsterVoicePlaying
        ) {
          setMonsterVoicePlaying(true);
          setMonsterVoiceSlug(gameState.slug + "/" + combatActionResult.monsterResult.monster.monsterSlug + "/parry");
        }
      }
      if (Number(combatActionResult.heroResult.attackResult.critDamage)) {
        setDisplayHeroCritEvent(true);
      }
      if (Number(combatActionResult.heroResult.attackResult.parryMitigation)) {
        setDisplayMonsterParryEvent(true);
      }
      if (Number(combatActionResult.heroResult.attackResult.armorMitigation)) {
        setDisplayMonsterArmorEvent(true);
      }
    }
  }, [heroRollingStatus]);

  useEffect(() => {
    if (monsterRollingStatus === "completed") {
      if (playerSettings.playVoice) {
        if (
          Number(combatActionResult.monsterResult.attackResult.critDamage) > 0 &&
          !isVoicelessMonster &&
          !isMonsterVoicePlaying
        ) {
          setMonsterVoicePlaying(true);
          setMonsterVoiceSlug(gameState.slug + "/" + combatActionResult.monsterResult.monster.monsterSlug + "/crit");
        }
        if (Number(combatActionResult.monsterResult.attackResult.parryMitigation) > 0 && !isHeroVoicePlaying) {
          setHeroVoicePlaying(true);
          setHeroVoiceSlug(
            combatActionResult.heroResult.hero.gender.toLowerCase() +
              "-" +
              combatActionResult.heroResult.hero.heroClass.toLowerCase() +
              "/parry-" +
              rng(1, 2)
          );
        }
      }
      if (Number(combatActionResult.monsterResult.attackResult.critDamage)) {
        setDisplayMonsterCritEvent(true);
      }
      if (Number(combatActionResult.monsterResult.attackResult.parryMitigation)) {
        setDisplayHeroParryEvent(true);
      }
      if (Number(combatActionResult.monsterResult.attackResult.armorMitigation)) {
        setDisplayHeroArmorEvent(true);
      }
    }
  }, [monsterRollingStatus]);

  useEffect(() => {
    if (heroAccuracyFlipped && playerSettings.playVoice) {
      if (combatActionResult.heroResult.attackResult.isHit) {
        if (rng(1, 2) === 2 && !isHeroVoicePlaying) {
          setHeroVoicePlaying(true);
          setHeroVoiceSlug(
            combatActionResult.heroResult.hero.gender.toLowerCase() +
              "-" +
              combatActionResult.heroResult.hero.heroClass.toLowerCase() +
              "/" +
              "attack-" +
              rng(1, 5)
          );
        }
      } else {
        if (rng(1, 4) === 4 && !isMonsterVoicePlaying && !voicelessMonsters) {
          setMonsterVoicePlaying(true);
          setMonsterVoiceSlug(gameState.slug + "/" + combatActionResult.monsterResult.monster.monsterSlug + "/dodge");
        }
      }
    }
  }, [heroAccuracyFlipped]);

  useEffect(() => {
    if (monsterAccuracyFlipped && playerSettings.playVoice) {
      if (!combatActionResult.monsterResult.attackResult.isHit) {
        if (rng(1, 4) === 4 && !isHeroVoicePlaying) {
          setHeroVoicePlaying(true);
          setHeroVoiceSlug(
            combatActionResult.heroResult.hero.gender.toLowerCase() +
              "-" +
              combatActionResult.heroResult.hero.heroClass.toLowerCase() +
              "/dodge-" +
              rng(1, 2).toString()
          );
        }
      }
    }
  }, [monsterAccuracyFlipped]);

  useEffect(() => {
    if (displayHeroCritEvent) {
      const timer2 = setTimeout(() => {
        setDisplayHeroCritEvent((prevVal) => !prevVal);
      }, 2000);
      // clear the timeout if user closes modal before timeout length (750ms) completes
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [displayHeroCritEvent]);

  useEffect(() => {
    if (displayHeroParryEvent) {
      const timer2 = setTimeout(() => {
        setDisplayHeroParryEvent((prevVal) => !prevVal);
      }, 2000);
      // clear the timeout if user closes modal before timeout length (750ms) completes
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [displayHeroParryEvent]);

  useEffect(() => {
    if (displayHeroArmorEvent) {
      const timer2 = setTimeout(() => {
        setDisplayHeroArmorEvent((prevVal) => !prevVal);
      }, 2000);
      // clear the timeout if user closes modal before timeout length (750ms) completes
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [displayHeroArmorEvent]);

  useEffect(() => {
    if (displayMonsterCritEvent) {
      const timer2 = setTimeout(() => {
        setDisplayMonsterCritEvent((prevVal) => !prevVal);
      }, 2000);
      // clear the timeout if user closes modal before timeout length (750ms) completes
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [displayMonsterCritEvent]);

  useEffect(() => {
    if (displayMonsterParryEvent) {
      const timer2 = setTimeout(() => {
        setDisplayMonsterParryEvent((prevVal) => !prevVal);
      }, 2000);
      // clear the timeout if user closes modal before timeout length (750ms) completes
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [displayMonsterParryEvent]);

  useEffect(() => {
    if (displayMonsterArmorEvent) {
      const timer2 = setTimeout(() => {
        setDisplayMonsterArmorEvent((prevVal) => !prevVal);
      }, 2000);
      // clear the timeout if user closes modal before timeout length (750ms) completes
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [displayMonsterArmorEvent]);

  const handleCloseModal = () => {
    if (heroRollingStatus === "completed" && monsterRollingStatus === "completed") {
      setHeroRollingStatus("waiting");
      setMonsterRollingStatus("waiting");
      setSecondaryDiceRollingStatus("waiting");
      setDisplaySecondaryResults(false);
      setFlipMonsterAndHeroAccuracy(false);
      setDisplayHeroResults(false);
      setDisplayMonsterResults(false);
      setFlipHeroAccuracyCard(false);
      setHeroAccuracyFlipping(false);
      setHeroAccuracyFlipped(false);
      setFlipMonsterAccuracyCard(false);
      setMonsterAccuracyFlipping(false);
      setMonsterAccuracyFlipped(false);
      setDisplayMonsterDmgBreakdown(false);
      setDisplayHeroDmgBreakdown(false);
      setDisplayHeroArmorEvent(false);
      setDisplayHeroCritEvent(false);
      setDisplayHeroParryEvent(false);
      setDisplayMonsterArmorEvent(false);
      setDisplayMonsterCritEvent(false);
      setDisplayMonsterParryEvent(false);
      dispatch(setFleeAttempt(false));
      dispatch(setPersuadeAttempt(false));
      dispatch(setDisplayCombatPhases("no"));
    }
  };

  const handleHeroRollToHit = () => {
    setHeroRollingStatus("rolling");
  };

  const handleMonsterRollToHit = () => {
    setMonsterRollingStatus("rolling");
  };

  const handleSecondaryRoll = () => {
    setSecondaryDiceRollingStatus("rolling");
  };

  const handleHeroAccuracyFlip = () => {
    const timer4 = setTimeout(() => {
      setFlipHeroAccuracyCard(true);
    }, 50);
    return () => {
      clearTimeout(timer4);
    };
  };

  const handleMonsterAccuracyFlip = () => {
    const timer5 = setTimeout(() => {
      setFlipMonsterAccuracyCard(true);
    }, 50);
    return () => {
      clearTimeout(timer5);
    };
  };

  const handleMonsterDmgBreakdown = () => {
    calculateTotalDiceDamage("monster");
    displayMonsterDmgBreakdown ? setDisplayMonsterDmgBreakdown(false) : setDisplayMonsterDmgBreakdown(true);
  };

  const handleHeroDmgBreakdown = () => {
    calculateTotalDiceDamage("hero");
    displayHeroDmgBreakdown ? setDisplayHeroDmgBreakdown(false) : setDisplayHeroDmgBreakdown(true);
  };

  const calculateTotalDiceDamage = (character: string) => {
    if (character === "hero" && combatActionResult.heroResult.attackResult.dice) {
      let totalDiceDamage = 0;
      let dice: Array<DieResultDto> = combatActionResult.heroResult.attackResult.dice.filter(
        (d) => d.rollFor === DiceRollReason.EquipmentDamage
      );
      if (dice.length === 0) {
        dice = combatActionResult.heroResult.attackResult.dice.filter(
          (d) => d.rollFor === DiceRollReason.HeroDieDamage
        );
      }
      dice.forEach((d) => {
        totalDiceDamage += d.result;
      });
      setHeroTotalDiceDamage(totalDiceDamage);
    }
    if (character === "monster" && combatActionResult.monsterResult.attackResult.dice) {
      let totalDiceDamage = 0;
      const dice: Array<DieResultDto> = combatActionResult.monsterResult.attackResult.dice.filter(
        (d) => d.rollFor === DiceRollReason.MonsterDieDamage
      );
      dice.forEach((d) => {
        totalDiceDamage += d.result;
      });
      setMonsterTotalDiceDamage(totalDiceDamage);
    }
  };

  const handleRollEnded = (character: string) => {
    if (character === "hero") {
      setDisplayHeroResults(true);
      setHeroRollingStatus("completed");
    } else {
      setDisplayMonsterResults(true);
      setMonsterRollingStatus("completed");
    }
  };

  const handleSecondaryRollEnded = () => {
    setDisplaySecondaryResults(true);
    setSecondaryDiceRollingStatus("completed");
  };

  const getHeroButtonTitle = (): string => {
    if (heroRollingStatus === "waiting") {
      return `ROLL FOR DAMAGE`;
    } else if (heroRollingStatus === "rolling") {
      return `...ROLLING...`;
    } else if (heroRollingStatus === "completed" && monsterRollingStatus !== "completed") {
      return "...WAITING...";
    } else {
      return `RESOLVE`;
    }
  };

  const getMonsterButtonTitle = (): string => {
    if (monsterRollingStatus === "waiting") {
      return `...WAITING...`;
    } else if (monsterRollingStatus === "rolling") {
      return `...ROLLING...`;
    } else if (monsterRollingStatus === "completed" && heroRollingStatus !== "completed") {
      return "...WAITING...";
    } else {
      if (width <= mdScreenWidth) {
        return `RESOLVE`;
      } else {
        return `ATTACK COMPLETE`;
      }
    }
  };

  const getSecondaryButtonTitle = (): string => {
    if (secondaryDiceRollingStatus === "waiting") {
      return isPersuadeAttempt ? `ROLL TO PERSUADE` : `ROLL TO FLEE`;
    } else if (secondaryDiceRollingStatus === "rolling") {
      return `...ROLLING...`;
    } else {
      return `ROLL COMPLETE`;
    }
  };

  const getCharismaOpportunityResultDescription = (): string => {
    if (combatActionResult.charismaOpportunityResultType === CharismaOpportunityResultType.Loss) {
      return `YOUR ENEMY'S SUPERIOR CHARISMA LENDS THEM A FREE OPPORTUNITY TO STRIKE!`;
    } else if (combatActionResult.charismaOpportunityResultType === CharismaOpportunityResultType.Tie) {
      return `IT SEEMS NEITHER YOU OR YOUR FOE WERE ABLE TO GET THE UPPERHAND IN THIS BATTLE OF WITS.`;
    } else if (combatActionResult.charismaOpportunityResultType === CharismaOpportunityResultType.PartialWin) {
      return `YOUR SUPERIOR CHARISMA GRANTS YOU VALUABLE EXPERIENCE.`;
    } else if (combatActionResult.charismaOpportunityResultType === CharismaOpportunityResultType.Win) {
      return `YOUR VASTLY SUPERIOR CHARISMA LEAVES YOUR ENEMY CONFUSED AND FLUSTERED. YOU WALK AWAY WITH EVERYTHING!`;
    } else {
      return ``;
    }
  };

  const clearHeroVoice = () => {
    setHeroVoiceSlug("");
    if (isHeroVoicePlaying) {
      setHeroVoicePlaying(false);
    }
  };

  const clearMonsterVoice = () => {
    setMonsterVoiceSlug("");
    if (isMonsterVoicePlaying) {
      setMonsterVoicePlaying(false);
    }
  };

  const getDiceType = (): Array<DieResultDto> => {
    let dice: Array<DieResultDto> = [];
    if (combatActionResult.heroResult.attackResult.dice) {
      dice = combatActionResult.heroResult.attackResult.dice.filter(
        (d) => d.rollFor === DiceRollReason.EquipmentDamage
      );
      if (dice.length === 0) {
        dice = combatActionResult.heroResult.attackResult.dice.filter(
          (d) => d.rollFor === DiceRollReason.HeroDieDamage
        );
      }
    }
    return dice;
  };

  const checkHeroPhaseOne = (): boolean => {
    if (isPersuadeAttempt || isFleeAttempt) {
      if (secondaryDiceRollingStatus === "completed") {
        if (
          combatActionResult.initiative === CombatantType.Hero ||
          (combatActionResult.initiative === CombatantType.Monster && monsterRollingStatus === "completed")
        ) {
          return true;
        }
      }
    } else {
      if (
        combatActionResult.initiative === CombatantType.Hero ||
        (combatActionResult.initiative === CombatantType.Monster && monsterRollingStatus === "completed")
      ) {
        return true;
      }
    }
    return false;
  };

  const heroAccuracyContainerStyle = heroAccuracyFlipping ? styles.cardContainerTransform : styles.cardContainer;

  const monsterAccuracyContainerStyle = monsterAccuracyFlipping ? styles.cardContainerTransform : styles.cardContainer;

  const isVoicelessMonster = voicelessMonsters.indexOf(combatActionResult.monsterResult.monster.monsterSlug) > -1;

  const renderHeroItems = () => {
    const items: JSX.Element[] = [];
    combatActionResult.heroResult.hero.equippedItems.forEach((i) => {
      items.push(
        <Grid item key={i.id}>
          <Grid container direction="row" className={styles.itemContainer}>
            <DCXItem item={i} rarity={i.rarity} height={20} width={20} containerWidth={20} top={-4} />
            <Typography component="span" className={styles.itemName}>
              {i.name}
            </Typography>
          </Grid>
        </Grid>
      );
    });
    return items;
  };

  const renderDice = (character: string, diceResults: Array<DieResultDto>) => {
    let rollingStatus = "";
    let displayResults = false;
    if (character === "hero") {
      rollingStatus = heroRollingStatus;
      displayResults = displayHeroResults;
    } else {
      rollingStatus = monsterRollingStatus;
      displayResults = displayMonsterResults;
    }

    if (diceResults) {
      const dice: JSX.Element[] = [];
      let key = 0;
      diceResults.forEach((d) => {
        key++;
        dice.push(
          <Grid container className={styles.die} key={key}>
            <Image src="/img/unity-assets/combat/dice_bg.png" height={90} width={90} quality={100} />
            <Grid container className={styles.diePlayerContainer}>
              <ReactPlayer
                playing={!playerSettings.autoRoll && rollingStatus === "rolling"}
                url={`video/${d.sides}.mp4`}
                controls={false}
                playsinline={true}
                muted={true}
                onEnded={() => handleRollEnded(character)}
              />
            </Grid>
            {displayResults && (
              <Typography component="span" className={styles.dieDamage}>
                {d.result}
              </Typography>
            )}
          </Grid>
        );
      });
      return dice;
    } else {
      return null;
    }
  };

  const renderSecondaryDice = (character: string, diceResults: Array<DieResultDto>) => {
    const rollingStatus = secondaryDiceRollingStatus;
    let displayResults = displaySecondaryResults;

    if (diceResults) {
      const dice: JSX.Element[] = [];
      let key = 0;
      diceResults.forEach((d) => {
        key++;
        let toolTipDescription = ``;
        if (isPersuadeAttempt) {
          if (character === "hero") {
            toolTipDescription = `${d.result} + ${combatActionResult.heroResult.hero.calculatedStats.charisma} = ${
              combatActionResult.heroResult.hero.calculatedStats.charisma + d.result
            } (ROLL + CHARISMA = TOTAL)`;
          } else {
            toolTipDescription = `${d.result} + ${combatActionResult.monsterResult.monster.charisma} = ${
              combatActionResult.monsterResult.monster.charisma + d.result
            } (ROLL + CHARISMA = TOTAL)`;
          }
        } else {
          if (character === "hero") {
            toolTipDescription = `${d.result} + ${combatActionResult.heroResult.hero.calculatedStats.quickness / 4} = ${
              combatActionResult.heroResult.hero.calculatedStats.quickness / 4 + d.result
            } (ROLL + QUICKNESS/4 = TOTAL)`;
          } else {
            toolTipDescription = `${d.result} + ${combatActionResult.monsterResult.monster.quickness / 4} = ${
              combatActionResult.monsterResult.monster.quickness / 4 + d.result
            } (ROLL + QUICKNESS/4 = TOTAL)`;
          }
        }
        dice.push(
          <Grid container className={styles.die} key={key}>
            <Image src="/img/unity-assets/combat/dice_bg.png" height={90} width={90} quality={100} />
            <Grid container className={styles.diePlayerContainer}>
              <ReactPlayer
                playing={!playerSettings.autoRoll && rollingStatus === "rolling"}
                url={`video/${d.sides}.mp4`}
                controls={false}
                playsinline={true}
                muted={true}
                onEnded={() => handleSecondaryRollEnded()}
              />
            </Grid>
            {displayResults && (
              <Typography component="span" className={styles.dieDamage}>
                {d.result}
              </Typography>
            )}

            {displayResults && (
              <ThemeProvider theme={tooltipTheme}>
                <Tooltip
                  disableFocusListener
                  placement="top"
                  arrow
                  enterTouchDelay={0}
                  TransitionComponent={Zoom}
                  title={toolTipDescription}
                >
                  <Grid container className={styles.secondaryActionDescriptionContainer}>
                    <PriorityHighIcon className={styles.secondaryDescriptionIcon} />
                  </Grid>
                </Tooltip>
              </ThemeProvider>
            )}
          </Grid>
        );
      });
      return dice;
    } else {
      return null;
    }
  };

  return (
    <Grid container direction="row">
      {(heroRollingStatus === "rolling" ||
        monsterRollingStatus === "rolling" ||
        secondaryDiceRollingStatus === "rolling") && (
        <DCXAudioPlayer audioUrl={"/audio/sound-effects/combat/dice-roll"} soundType={SoundType.SOUND_EFFECT} />
      )}
      {heroVoiceSlug !== "" && !introsPlaying && !outrosPlaying && (
        <DCXAudioPlayer
          audioUrl={`/audio/voice/heroes/${heroVoiceSlug}`}
          soundType={SoundType.VOICE}
          onEnded={() => clearHeroVoice()}
        />
      )}
      {monsterVoiceSlug !== "" && !introsPlaying && !outrosPlaying && (
        <DCXAudioPlayer
          audioUrl={`/audio/voice/monsters/${monsterVoiceSlug}`}
          soundType={SoundType.VOICE}
          onEnded={() => clearMonsterVoice()}
        />
      )}
      <Modal open={displayCombatPhases === "yes"} onClose={handleCloseModal} className={styles.modalMain}>
        <Grid container direction="row-reverse" className={styles.modalContainer}>
          <Grid container item lg={6} xs={12} direction="column" className={styles.heroPhaseContainer}>
            <Grid container className={styles.actionPhaseBackground}>
              <Grid container className={styles.combatActionBackground}>
                <Image
                  src="/img/unity-assets/shared/action_bg.png"
                  height={350}
                  width={width > xsScreenWidth ? 610 : 400}
                  quality={100}
                />
              </Grid>
              <Grid container className={styles.characterNameBackground}>
                <Image src="/img/unity-assets/combat/combat_result_name_bg.png" height={70} width={243} />
              </Grid>
              <Typography component="span" className={styles.heroResultName}>
                {combatActionResult.heroResult.hero.name}
              </Typography>
              <Typography component="span" className={styles.phaseText}>
                PHASE I
              </Typography>
              <Grid
                container
                className={styles.closeButtonContainer}
                style={{
                  cursor:
                    heroRollingStatus === "completed" && monsterRollingStatus === "completed" ? "pointer" : "default",
                }}
              >
                <CloseButton handleClose={handleCloseModal} />
              </Grid>
              <Grid container direction="row" className={styles.characterAttributes}>
                <Grid container className={styles.characterSymbol}>
                  <Image
                    src={`/img/unity-assets/card/${combatActionResult.heroResult.hero.heroClass.toLowerCase()}_symbol.png`}
                    height={18}
                    width={18}
                  />
                </Grid>
                <Typography component="span" className={styles.characterLevel}>
                  LEVEL {combatActionResult.heroResult.hero.level}
                </Typography>
                <div className={styles.attributeDivider} />
                <Grid container className={styles.itemsContainer}>
                  {renderHeroItems()}
                </Grid>
              </Grid>
              {/* Check isPersuadeAttempt or isFleeAttempt and have dice roll first, at the same time */}
              {(isFleeAttempt || isPersuadeAttempt) && (
                <Grid
                  container
                  className={
                    secondaryDiceRollingStatus === "completed"
                      ? styles.secondaryDiceContainerCompleted
                      : styles.secondaryDiceContainer
                  }
                >
                  {renderSecondaryDice(
                    "hero",
                    combatActionResult.heroResult.attackResult.dice!.filter(
                      (d) =>
                        d.rollFor ===
                        (isPersuadeAttempt ? DiceRollReason.HeroCharismaRoll : DiceRollReason.HeroFleeRoll)
                    )
                  )}
                  <Grid container className={styles.secondaryDiceButtonContainer}>
                    <DCXButton
                      title={getSecondaryButtonTitle()}
                      height={42}
                      width={144}
                      color="blue"
                      disabled={secondaryDiceRollingStatus !== "waiting"}
                      onClick={() => (secondaryDiceRollingStatus === "waiting" ? handleSecondaryRoll() : undefined)}
                    />
                  </Grid>
                </Grid>
              )}
              {checkHeroPhaseOne() ? (
                <Grid
                  container
                  direction="column"
                  className={
                    isFleeAttempt || isPersuadeAttempt
                      ? secondaryDiceRollingStatus === "completed"
                        ? styles.cardMainCompleted
                        : styles.cardMain
                      : styles.cardMain
                  }
                >
                  <Grid item xs={12} className={heroAccuracyContainerStyle}>
                    <Grid item xs={12} className={styles.cardFront}>
                      <Grid container className={styles.containerHitMiss}>
                        <Grid container className={styles.backgroundHitMiss}>
                          <Image
                            src="/img/unity-assets/combat/action_result_bg.png"
                            height={110}
                            width={110}
                            quality={100}
                          />
                        </Grid>
                      </Grid>
                    </Grid>
                    <Grid item xs={12} className={styles.cardBack}>
                      <Grid container className={styles.containerHitMiss}>
                        <Grid container className={styles.backgroundHitMiss}>
                          <Image
                            src="/img/unity-assets/combat/action_result_bg.png"
                            height={110}
                            width={110}
                            quality={100}
                          />
                        </Grid>
                        {combatActionResult.heroResult.attackResult.isHit ? (
                          <Grid container className={styles.imageHitMiss}>
                            <Image src="/img/unity-assets/combat/hit.png" height={65} width={65} quality={100} />
                          </Grid>
                        ) : isFleeAttempt ? (
                          <Grid container className={styles.imageflee}>
                            <Image src="/img/unity-assets/combat/flee.png" height={90} width={89} quality={100} />
                          </Grid>
                        ) : isPersuadeAttempt ? (
                          <Grid container className={styles.imagePersuade}>
                            <Image src="/img/unity-assets/combat/persuade.png" height={59} width={91} quality={100} />
                          </Grid>
                        ) : (
                          <Grid container className={styles.imageHitMiss}>
                            <Image src="/img/unity-assets/combat/miss.png" height={65} width={65} quality={100} />
                          </Grid>
                        )}
                        <Typography component="span" className={styles.messageHitMiss}>
                          {isFleeAttempt
                            ? combatActionResult.didHeroFlee
                              ? "SUCCESS"
                              : "FAIL"
                            : isPersuadeAttempt
                            ? combatActionResult.charismaOpportunityResultType
                            : combatActionResult.heroResult.attackResult.isHit
                            ? "HIT"
                            : "MISS"}
                        </Typography>
                      </Grid>
                    </Grid>
                  </Grid>
                </Grid>
              ) : (
                <Grid container className={styles.rollingStatus}>
                  {!isPersuadeAttempt && !isFleeAttempt && <Typography component="span">ENEMY ROLLING...</Typography>}
                </Grid>
              )}
              <Grid
                container
                className={displayHeroParryEvent ? styles.heroParryFadeInContainer : styles.heroParryFadeOutContainer}
              >
                <Grid container className={styles.eventBackground}>
                  <Image src="/img/unity-assets/combat/event-splash.png" height={57} width={135} quality={100} />
                </Grid>
                <Typography component="span" className={styles.heroParryEventText}>
                  {`PARRY`}
                </Typography>
              </Grid>
              <Grid
                container
                className={displayHeroArmorEvent ? styles.heroArmorFadeInContainer : styles.heroArmorFadeOutContainer}
              >
                <Grid container className={styles.eventBackground}>
                  <Image src="/img/unity-assets/combat/event-splash.png" height={57} width={135} quality={100} />
                </Grid>
                <Typography component="span" className={styles.heroParryEventText}>
                  {`CLANK`}
                </Typography>
              </Grid>
            </Grid>
            <Grid container className={styles.actionPhaseBackground}>
              <Grid container className={styles.combatActionBackground}>
                <Image
                  src="/img/unity-assets/shared/action_bg.png"
                  height={350}
                  width={width > xsScreenWidth ? 610 : 400}
                  quality={100}
                />
              </Grid>
              <Typography component="span" className={styles.phaseText}>
                PHASE II
              </Typography>
              {(combatActionResult.initiative === CombatantType.Hero || monsterRollingStatus === "completed") &&
                combatActionResult.heroResult.attackResult.isHit &&
                heroAccuracyFlipped && (
                  <Grid container direction="row" className={styles.attackResultsContainer}>
                    <Grid container className={styles.buttonDiceContainer}>
                      <Grid
                        container
                        className={
                          combatActionResult.heroResult.attackResult.dice &&
                          combatActionResult.heroResult.attackResult.dice.length > 2
                            ? styles.manyDiceContainer
                            : styles.diceContainer
                        }
                      >
                        {combatActionResult.heroResult.attackResult.dice && renderDice("hero", getDiceType())}
                      </Grid>
                      <Grid container className={styles.buttonContainer}>
                        <DCXButton
                          title={getHeroButtonTitle()}
                          height={42}
                          width={144}
                          color="blue"
                          disabled={
                            heroRollingStatus !== "waiting" &&
                            (heroRollingStatus !== "completed" || monsterRollingStatus !== "completed")
                          }
                          onClick={() => (heroRollingStatus === "waiting" ? handleHeroRollToHit() : handleCloseModal())}
                        />
                      </Grid>
                    </Grid>
                    {displayHeroResults && (
                      <Grid container direction="column" className={styles.damageContainer}>
                        <Grid container className={styles.totalDamageContainer} onClick={handleHeroDmgBreakdown}>
                          <Grid container className={styles.backgroundHitMiss}>
                            <Image
                              src="/img/unity-assets/combat/action_result_bg.png"
                              height={110}
                              width={110}
                              quality={100}
                              className={styles.totalDamageImage}
                            />
                          </Grid>
                          <Typography component="span" className={styles.heroTotalDamage}>
                            {combatActionResult.heroResult.attackResult.totalDamage}
                          </Typography>
                        </Grid>
                        <Typography component="span" className={styles.heroDamageText}>
                          {`DAMAGE`}
                        </Typography>
                        {displayHeroDmgBreakdown && (
                          <Grid container className={styles.damageBreakdownContainer}>
                            <Image
                              src="/img/unity-assets/shared/tooltip_bg.png"
                              height={120}
                              width={150}
                              quality={100}
                            />
                            <Grid container direction="row" className={styles.damageBreakdown}>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`DICE DAMAGE`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownBonus}>
                                {heroTotalDiceDamage}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`BONUS DAMAGE`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownBonus}>
                                {combatActionResult.heroResult.attackResult.bonusDamage}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`CRIT DAMAGE`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownBonus}>
                                {combatActionResult.heroResult.attackResult.critDamage}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`STATUS EFFECT BONUS`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownBonus}>
                                {combatActionResult.heroResult.attackResult.statusEffectBonus}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`ARMOR MITIGATION`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownMitigation}>
                                {combatActionResult.heroResult.attackResult.armorMitigation}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`PARRY MITIGATION`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownMitigation}>
                                {combatActionResult.heroResult.attackResult.parryMitigation}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`STATUS EFFECT MIT`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownMitigation}>
                                {combatActionResult.heroResult.attackResult.statusEffectMitigation}
                              </Typography>
                            </Grid>
                          </Grid>
                        )}
                      </Grid>
                    )}
                  </Grid>
                )}
              {(!combatActionResult.heroResult.attackResult.isHit || !heroAccuracyFlipped) && (
                <Grid container className={styles.phaseTwoContainer}>
                  {!isFleeAttempt && !isPersuadeAttempt && (
                    <Grid container className={styles.actionPhaseBackgroundDisabled} />
                  )}
                  {isFleeAttempt && heroAccuracyFlipped && (
                    <Grid container className={styles.fleeResultContainer}>
                      <Typography component="span" className={styles.fleeResult}>
                        {combatActionResult.didHeroFlee
                          ? `YOUR QUICKNESS ALLOWS YOU TO NARROWLY ESCAPE YOUR ENEMY'S GRASP!`
                          : `YOU ARE TOO SLOW AND FAIL TO ESCAPE, YOUR FOE ATTEMPTS A HIT AS YOU STUMBLE!`}
                      </Typography>
                    </Grid>
                  )}
                  {isPersuadeAttempt && heroAccuracyFlipped && (
                    <Grid container className={styles.fleeResultContainer}>
                      <Typography component="span" className={styles.fleeResult}>
                        {getCharismaOpportunityResultDescription()}
                      </Typography>
                    </Grid>
                  )}
                  {heroRollingStatus === "completed" && monsterRollingStatus === "completed" && (
                    <Grid container className={styles.buttonContainer}>
                      <DCXButton
                        title={"RESOLVE"}
                        height={42}
                        width={144}
                        color="blue"
                        onClick={() => handleCloseModal()}
                      />
                    </Grid>
                  )}
                </Grid>
              )}
              <Grid
                container
                className={displayHeroCritEvent ? styles.heroCritFadeInContainer : styles.heroCritFadeOutContainer}
              >
                <Grid container className={styles.eventBackground}>
                  <Image src="/img/unity-assets/combat/event-splash.png" height={57} width={100} quality={100} />
                </Grid>
                <Typography component="span" className={styles.heroEventText}>
                  {`CRIT`}
                </Typography>
              </Grid>
            </Grid>
          </Grid>
          <Grid container item lg={6} xs={12} direction="column" className={styles.monsterPhaseContainer}>
            <Grid container className={styles.actionPhaseBackground}>
              <Grid container className={styles.combatActionBackground}>
                <Image
                  src="/img/unity-assets/shared/action_bg.png"
                  height={350}
                  width={width > xsScreenWidth ? 610 : 400}
                  quality={100}
                />
              </Grid>
              <Grid container className={styles.characterNameBackground}>
                <Image src="/img/unity-assets/combat/combat_result_name_bg.png" height={70} width={243} />
              </Grid>
              {/* <Typography component="span" className={styles.monsterResultName}>
                {`${
                  combatActionResult.monsterResult.monster.personalityType !== MonsterPersonalityTypeDto.None
                    ? "(" + combatActionResult.monsterResult.monster.personalityType + ")"
                    : ""
                } ${combatActionResult.monsterResult.monster.name}`}
              </Typography> */}
              <Grid container className={styles.characterNameContainer}>
                {combatActionResult.monsterResult.monster.personalityType &&
                  combatActionResult.monsterResult.monster.personalityType !== MonsterPersonalityTypeDto.None && (
                    <ThemeProvider theme={tooltipTheme}>
                      <Tooltip
                        disableFocusListener
                        placement="top"
                        arrow
                        TransitionComponent={Zoom}
                        enterTouchDelay={0}
                        title={combatActionResult.monsterResult.monster.personalityTypeDesc}
                      >
                        <Typography component="span" className={styles.monsterResultName}>
                          {`(${combatActionResult.monsterResult.monster.personalityType})`}
                        </Typography>
                      </Tooltip>
                    </ThemeProvider>
                  )}
                <Typography component="span" className={styles.monsterResultName}>
                  {`${combatActionResult.monsterResult.monster.name}`}
                </Typography>
              </Grid>
              <Typography component="span" className={styles.phaseText}>
                PHASE I
              </Typography>
              <Grid container direction="row" className={styles.characterAttributes}>
                <Typography component="span" className={styles.characterLevel}>
                  LEVEL {combatActionResult.monsterResult.monster.level}
                </Typography>
                <div className={styles.attributeDivider} />
                <Grid container className={styles.itemsContainer}>
                  <Typography component="span" className={styles.itemName}>
                    {combatActionResult.monsterResult.monster.monsterItems.join(", ")}
                  </Typography>
                </Grid>
              </Grid>
              {(isFleeAttempt || isPersuadeAttempt) && (
                <Grid
                  container
                  className={
                    secondaryDiceRollingStatus === "completed"
                      ? styles.secondaryDiceContainerCompleted
                      : styles.secondaryDiceContainer
                  }
                >
                  {renderSecondaryDice(
                    "monster",
                    combatActionResult.monsterResult.attackResult.dice!.filter(
                      (d) =>
                        d.rollFor ===
                        (isPersuadeAttempt ? DiceRollReason.MonsterCharismaRoll : DiceRollReason.MonsterFleeRoll)
                    )
                  )}
                </Grid>
              )}
              <Grid container>
                {combatActionResult.initiative === CombatantType.Monster ||
                (secondaryDiceRollingStatus === "completed" && (isPersuadeAttempt || isFleeAttempt)) ||
                (combatActionResult.initiative === CombatantType.Hero && heroRollingStatus === "completed") ? (
                  <Grid
                    container
                    direction="column"
                    className={
                      isFleeAttempt || isPersuadeAttempt
                        ? secondaryDiceRollingStatus === "completed"
                          ? styles.cardMainCompleted
                          : styles.cardMain
                        : styles.cardMain
                    }
                  >
                    <Grid item xs={12} className={monsterAccuracyContainerStyle}>
                      <Grid item xs={12} className={styles.cardFront}>
                        <Grid container className={styles.containerHitMiss}>
                          <Grid container className={styles.backgroundHitMiss}>
                            <Image
                              src="/img/unity-assets/combat/action_result_bg.png"
                              height={110}
                              width={110}
                              quality={100}
                            />
                          </Grid>
                        </Grid>
                      </Grid>
                      <Grid item xs={12} className={styles.cardBack}>
                        <Grid container className={styles.containerHitMiss}>
                          <Grid container className={styles.backgroundHitMiss}>
                            <Image
                              src="/img/unity-assets/combat/action_result_bg.png"
                              height={110}
                              width={110}
                              quality={100}
                            />
                          </Grid>
                          {combatActionResult.monsterResult.attackResult.isHit ? (
                            <Grid container className={styles.imageHitMiss}>
                              <Image src="/img/unity-assets/combat/hit.png" height={65} width={65} quality={100} />
                            </Grid>
                          ) : (
                            <Grid container className={styles.imageHitMiss}>
                              <Image src="/img/unity-assets/combat/miss.png" height={65} width={65} quality={100} />
                            </Grid>
                          )}
                          <Typography component="span" className={styles.messageHitMiss}>
                            {combatActionResult.monsterResult.attackResult.isHit ? "HIT" : "MISS"}
                          </Typography>
                        </Grid>
                      </Grid>
                    </Grid>
                  </Grid>
                ) : (
                  <Grid container className={styles.rollingStatus}>
                    {!isPersuadeAttempt && !isFleeAttempt && <Typography component="span">HERO ROLLING...</Typography>}
                  </Grid>
                )}
              </Grid>
              <Grid
                container
                className={
                  displayMonsterParryEvent ? styles.monsterParryFadeInContainer : styles.monsterParryFadeOutContainer
                }
              >
                <Grid container className={styles.eventBackground}>
                  <Image src="/img/unity-assets/combat/event-splash.png" height={57} width={135} quality={100} />
                </Grid>
                <Typography component="span" className={styles.monsterParryEventText}>
                  {`PARRY`}
                </Typography>
              </Grid>
              <Grid
                container
                className={
                  displayMonsterArmorEvent ? styles.monsterArmorFadeInContainer : styles.monsterArmorFadeOutContainer
                }
              >
                <Grid container className={styles.eventBackground}>
                  <Image src="/img/unity-assets/combat/event-splash.png" height={57} width={135} quality={100} />
                </Grid>
                <Typography component="span" className={styles.monsterParryEventText}>
                  {`CLANK`}
                </Typography>
              </Grid>
              {!combatActionResult.didHeroFlee &&
                combatActionResult.monsterCastedSpecialAbility &&
                (combatActionResult.initiative === CombatantType.Monster ||
                  (combatActionResult.initiative === CombatantType.Hero && heroRollingStatus === "completed")) && (
                  <Grid container className={styles.monsterSpecialAbilityContainer}>
                    {!isVoicelessMonster && !introsPlaying && !outrosPlaying && (
                      <DCXAudioPlayer
                        audioUrl={`/audio/voice/monsters/${gameState.slug}/${combatActionResult.monsterResult.monster.monsterSlug}/${combatActionResult.monsterCastedSpecialAbility.slug}`}
                        soundType={SoundType.VOICE}
                      />
                    )}
                    <ThemeProvider theme={tooltipTheme}>
                      <Tooltip
                        disableFocusListener
                        placement="top"
                        arrow
                        enterTouchDelay={0}
                        TransitionComponent={Zoom}
                        title={combatActionResult.monsterCastedSpecialAbility.description}
                      >
                        <span className={styles.specialAbilityText}>
                          {`${combatActionResult.monsterResult.monster.name} CAST `}
                          <span
                            className={styles.specialAbilityName}
                          >{`${combatActionResult.monsterCastedSpecialAbility.name}`}</span>
                        </span>
                      </Tooltip>
                    </ThemeProvider>
                  </Grid>
                )}
            </Grid>
            <Grid container className={styles.actionPhaseBackground}>
              <Grid container className={styles.combatActionBackground}>
                <Image
                  src="/img/unity-assets/shared/action_bg.png"
                  height={350}
                  width={width > xsScreenWidth ? 610 : 400}
                  quality={100}
                />
              </Grid>
              <Typography component="span" className={styles.phaseText}>
                PHASE II
              </Typography>
              {(combatActionResult.initiative === CombatantType.Monster || heroRollingStatus === "completed") &&
                combatActionResult.monsterResult.attackResult.isHit &&
                monsterAccuracyFlipped && (
                  <Grid container direction="row" className={styles.attackResultsContainer}>
                    <Grid container className={styles.buttonDiceContainer}>
                      <Grid
                        container
                        className={
                          combatActionResult.monsterResult.attackResult.dice &&
                          combatActionResult.monsterResult.attackResult.dice.length > 2
                            ? styles.manyDiceContainer
                            : styles.diceContainer
                        }
                      >
                        {combatActionResult.monsterResult.attackResult.dice &&
                          renderDice(
                            "monster",
                            combatActionResult.monsterResult.attackResult.dice.filter(
                              (d) => d.rollFor === DiceRollReason.MonsterDieDamage
                            )
                          )}
                      </Grid>
                      <Grid container className={styles.buttonContainer}>
                        <DCXButton
                          title={getMonsterButtonTitle()}
                          height={42}
                          width={144}
                          color="blue"
                          disabled={
                            monsterRollingStatus !== "completed" ||
                            heroRollingStatus !== "completed" ||
                            width > mdScreenWidth
                          }
                          onClick={() => handleCloseModal()}
                        />
                      </Grid>
                    </Grid>
                    {displayMonsterResults && (
                      <Grid container direction="column" className={styles.damageContainer}>
                        <Grid container className={styles.totalDamageContainer} onClick={handleMonsterDmgBreakdown}>
                          <Grid container className={styles.backgroundHitMiss}>
                            <Image
                              src="/img/unity-assets/combat/action_result_bg.png"
                              height={110}
                              width={110}
                              quality={100}
                              className={styles.totalDamageImage}
                            />
                          </Grid>
                          <Typography component="span" className={styles.monsterTotalDamage}>
                            {combatActionResult.monsterResult.attackResult.totalDamage}
                          </Typography>
                        </Grid>
                        <Typography component="span" className={styles.monsterDamageText}>
                          {`DAMAGE`}
                        </Typography>
                        {displayMonsterDmgBreakdown && (
                          <Grid container className={styles.damageBreakdownContainer}>
                            <Image
                              src="/img/unity-assets/shared/tooltip_bg.png"
                              height={137}
                              width={150}
                              quality={100}
                            />
                            <Grid container direction="row" className={styles.damageBreakdown}>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`DICE DAMAGE`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownBonus}>
                                {monsterTotalDiceDamage}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`BONUS DAMAGE`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownBonus}>
                                {combatActionResult.monsterResult.attackResult.bonusDamage}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`CRIT DAMAGE`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownBonus}>
                                {combatActionResult.monsterResult.attackResult.critDamage}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`STATUS EFFECT BONUS`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownBonus}>
                                {combatActionResult.monsterResult.attackResult.statusEffectBonus}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`ARMOR MITIGATION`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownMitigation}>
                                {combatActionResult.monsterResult.attackResult.armorMitigation}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`PARRY MITIGATION`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownMitigation}>
                                {combatActionResult.monsterResult.attackResult.parryMitigation}
                              </Typography>
                              <Typography
                                component="span"
                                className={styles.damageBreakdownHeader}
                              >{`STATUS EFFECT MIT`}</Typography>
                              <Typography component="span" className={styles.damageBreakdownMitigation}>
                                {combatActionResult.monsterResult.attackResult.statusEffectMitigation}
                              </Typography>
                            </Grid>
                          </Grid>
                        )}
                      </Grid>
                    )}
                    <Grid
                      container
                      className={
                        displayMonsterCritEvent ? styles.monsterCritFadeInContainer : styles.monsterCritFadeOutContainer
                      }
                    >
                      <Grid container className={styles.eventBackground}>
                        <Image src="/img/unity-assets/combat/event-splash.png" height={57} width={100} quality={100} />
                      </Grid>
                      <Typography component="span" className={styles.monsterEventText}>
                        {`CRIT`}
                      </Typography>
                    </Grid>
                  </Grid>
                )}
              {((combatActionResult.initiative === CombatantType.Hero && heroRollingStatus !== "completed") ||
                !combatActionResult.monsterResult.attackResult.isHit ||
                !monsterAccuracyFlipped) && (
                <Grid container>
                  <Grid container className={styles.actionPhaseBackgroundDisabled} />
                  {heroRollingStatus === "completed" &&
                    monsterRollingStatus === "completed" &&
                    width <= mdScreenWidth && (
                      <Grid container className={styles.buttonContainer}>
                        <DCXButton
                          title={"RESOLVE"}
                          disabled={width > mdScreenWidth}
                          height={42}
                          width={144}
                          color="blue"
                          onClick={() => handleCloseModal()}
                        />
                      </Grid>
                    )}
                </Grid>
              )}
            </Grid>
          </Grid>
        </Grid>
      </Modal>
    </Grid>
  );
};

export default CombatPhases;
