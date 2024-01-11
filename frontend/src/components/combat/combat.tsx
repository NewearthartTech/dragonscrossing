import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./combat.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useEffect, useState } from "react";
import HealthBar from "../health-bar/health-bar";
import { SoundType, Status } from "@/state-mgmt/app/appTypes";
import DCXButton from "../dcx-button/dcx-button";
import useWindowDimensions from "../../helpers/window-dimensions";
import HeroCard from "../hero-card/hero-card";
import CombatPhases from "./combat-phases/combat-phases";
import {
  selectDisplayCombatPhases,
  setDisplayCombatSkills,
  selectDisplayCombatSkills,
  setDisplayCombatPhases,
} from "@/state-mgmt/app/appSlice";
import {
  clearCombatActionResult,
  clearCombatActionStatus,
  clearCombatActionType,
  getCombatAttack,
  getCombatFlee,
  getCombatPersuade,
  getCombatStartRound,
  selectCombatActionResult,
  selectCombatActionResultStatus,
  selectCombatActionType,
  setCombatActionResult,
  setFleeAttempt,
  setPersuadeAttempt,
} from "@/state-mgmt/combat/combatSlice";
import Image from "next/image";
import {
  initializedMonster,
  mdScreenWidth,
  smScreenWidth,
  tooltipTheme,
  voicelessMonsters,
  xlScreenWidth,
} from "@/helpers/global-constants";
import CombatResolution from "@/components/combat-resolution/combat-resolution";
import CombatSkills from "./combat-skills/combat-skills";
import {
  ActionResponseDto,
  DcxTiles,
  DcxZones,
  DiceRollReason,
  LingeringStatusEffects,
  MonsterDto,
  MonsterPersonalityTypeDto,
} from "@dcx/dcx-backend";
import { selectGameState, updateGameState } from "@/state-mgmt/game-state/gameStateSlice";
import { selectSelectedHero, setSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { ThemeProvider } from "@mui/material/styles";
import Tooltip from "@mui/material/Tooltip";
import Zoom from "@mui/material/Zoom";
import { ActionType } from "@/state-mgmt/combat/combatTypes";
import DCXAudioPlayer from "../dcx-audio-player/dcx-audio-player";
import { rng } from "@/helpers/shared-functions";
import { selectPlayerSettings } from "@/state-mgmt/player/playerSlice";
import PriorityHighIcon from "@mui/icons-material/PriorityHigh";
import { calculateDamageRange } from "@/helpers/helper-functions";

interface Props {}

const Combat: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const combatActionResult = useAppSelector(selectCombatActionResult);
  const combatActionStatus = useAppSelector(selectCombatActionResultStatus);
  const displayCombatPhases = useAppSelector(selectDisplayCombatPhases);
  const displayCombatSkills = useAppSelector(selectDisplayCombatSkills);
  const gameState = useAppSelector(selectGameState);
  const combatActionType = useAppSelector(selectCombatActionType);
  const hero = useAppSelector(selectSelectedHero).hero;
  const playerSettings = useAppSelector(selectPlayerSettings);
  const { width, height } = useWindowDimensions();

  const [isCombatInitialized, setCombatInitialized] = useState(false);
  const [monster, setMonster] = useState<MonsterDto>(initializedMonster);
  const [displayVictoryModal, setDisplayVictoryModal] = useState(false);
  const [displayDefeatModal, setDisplayDefeatModal] = useState(false);
  const [positiveStatusEffects, setPositiveStatusEffects] = useState<Array<LingeringStatusEffects>>([]);
  const [negativeStatusEffects, setNegativeStatusEffects] = useState<Array<LingeringStatusEffects>>([]);
  const [voiceSlug, setVoiceSlug] = useState("");
  const [voiceCharacterType, setVoiceCharacterType] = useState("");
  const [playIntros, setPlayIntros] = useState(false);
  const [playOutros, setPlayOutros] = useState(false);
  const [monsterIntroPlayed, setMonsterIntroPlayed] = useState(false);
  const [monsterOutroPlayed, setMonsterOutroPlayed] = useState(false);
  const [introsCompleted, setIntrosCompleted] = useState(false);
  const [outrosCompleted, setOutrosCompleted] = useState(false);
  const [displayLingeringDamageToMonster, setDisplayLingeringDamageToMonster] = useState(false);
  const [lingeringDamageToMonster, setLingeringDamageToMonster] = useState(0);
  const [warningMessage, setWarningMessage] = useState("");
  const [isLevelDifferentialChecked, setLevelDifferentialChecked] = useState(false);
  const [previousHeroHp, setPreviousHeroHp] = useState(-1);
  const [previousMonsterHp, setPreviousMonsterHp] = useState(-1);
  const [checkForHealing, setCheckForHealing] = useState(false);
  const [displayHeroHealing, setDisplayHeroHealing] = useState(false);
  const [displayMonsterHealing, setDisplayMonsterHealing] = useState(false);
  const [displayHeroHealingAmount, setDisplayHeroHealingAmount] = useState(-1);
  const [displayMonsterHealingAmount, setDisplayMonsterHealingAmount] = useState(-1);

  useEffect(() => {
    if (gameState.encounters![0].type === "ActionResponseDto") {
      setCombatInitialized(true);
      dispatch(getCombatStartRound());
    }
  }, []);

  // When a combat encounter begins, set the monster and hero action results
  useEffect(() => {
    if (combatActionStatus === Status.Loaded && isCombatInitialized) {
      if (!isLevelDifferentialChecked) {
        checkLevelDifferential();
        setLevelDifferentialChecked(true);
      }
      if (
        combatActionResult.round === 0 &&
        !combatActionResult.heroResult.isDead &&
        !combatActionResult.monsterResult.isDead
      ) {
        setPlayIntros(true);
      } else {
        setPlayIntros(false);
        setIntrosCompleted(true);
      }
      if (combatActionResult.didHeroFlee) {
        dispatch(updateGameState(gameState.zone.slug as DcxTiles));
      }
      dispatch(setSelectedHero(combatActionResult.heroResult.hero));
      setMonster(combatActionResult.monsterResult.monster);
      combatActionResult.heroSkillStatusEffects
        ? setPositiveStatusEffects(combatActionResult.heroSkillStatusEffects)
        : setPositiveStatusEffects([]);
      combatActionResult.monsterSpecialAbilityStatusEffects
        ? setNegativeStatusEffects(combatActionResult.monsterSpecialAbilityStatusEffects)
        : setNegativeStatusEffects([]);
      setPreviousHeroHp(combatActionResult.heroResult.hero.remainingHitPoints);
      setPreviousMonsterHp(combatActionResult.monsterResult.monster.hitPoints);
      setCheckForHealing(true);
      setCombatInitialized(false);
    }
  }, [combatActionStatus, isCombatInitialized]);

  useEffect(() => {
    if (combatActionStatus === Status.Loaded) {
      if (combatActionType === ActionType.START_ROUND) {
        dispatch(setSelectedHero(combatActionResult.heroResult.hero));
        setMonster(combatActionResult.monsterResult.monster);
        dispatch(clearCombatActionStatus());
        if (combatActionResult.heroResult.attackResult.dice) {
          const lingeringDamageDice = combatActionResult.heroResult.attackResult.dice.filter(
            (d) => d.rollFor === DiceRollReason.SkillLingeringDamage
          );
          if (lingeringDamageDice && lingeringDamageDice.length > 0) {
            let totalLingeringDamage = 0;
            lingeringDamageDice.forEach((ldd) => {
              totalLingeringDamage += Number(ldd.finalResult);
            });
            setLingeringDamageToMonster(totalLingeringDamage);
            setDisplayLingeringDamageToMonster(true);
          }
        }
        if (combatActionResult.monsterResult.isDead) {
          setPlayOutros(true);
          setDisplayVictoryModal(true);
        }
        if (combatActionResult.heroResult.isDead) {
          setPlayOutros(true);
          setDisplayDefeatModal(true);
        }
      }
      if (combatActionType === ActionType.ATTACK) {
        dispatch(setDisplayCombatPhases("yes"));
      }
      if (combatActionType === ActionType.FLEE) {
        dispatch(setFleeAttempt(true));
        dispatch(setDisplayCombatPhases("yes"));
      }
      if (combatActionType === ActionType.PERSUADE) {
        dispatch(setPersuadeAttempt(true));
        dispatch(setDisplayCombatPhases("yes"));
      }

      // Check for healing
      if (checkForHealing) {
        if (combatActionResult.heroResult.hero.remainingHitPoints > previousHeroHp) {
          setDisplayHeroHealingAmount(combatActionResult.heroResult.hero.remainingHitPoints - previousHeroHp);
          setDisplayHeroHealing(true);
        }
        if (combatActionResult.monsterResult.monster.hitPoints > previousMonsterHp) {
          setDisplayMonsterHealingAmount(combatActionResult.monsterResult.monster.hitPoints - previousMonsterHp);
          setDisplayMonsterHealing(true);
        }
        setPreviousHeroHp(combatActionResult.heroResult.hero.remainingHitPoints);
        setPreviousMonsterHp(combatActionResult.monsterResult.monster.hitPoints);
      }

      dispatch(clearCombatActionType());
    }
    if (combatActionStatus === Status.Failed) {
      dispatch(clearCombatActionStatus());
      dispatch(clearCombatActionType());
    }
  }, [combatActionStatus]);

  // When a combat modal closes, set the monster and hero action results
  useEffect(() => {
    if ((displayCombatPhases === "no" && combatActionStatus === Status.Loaded) || displayCombatSkills === "no") {
      dispatch(setSelectedHero(combatActionResult.heroResult.hero));
      setMonster(combatActionResult.monsterResult.monster);
      dispatch(clearCombatActionStatus());

      combatActionResult.heroSkillStatusEffects
        ? setPositiveStatusEffects(combatActionResult.heroSkillStatusEffects)
        : setPositiveStatusEffects([]);
      combatActionResult.monsterSpecialAbilityStatusEffects
        ? setNegativeStatusEffects(combatActionResult.monsterSpecialAbilityStatusEffects)
        : setNegativeStatusEffects([]);

      if (combatActionResult.monsterResult.isDead) {
        setPlayOutros(true);
        setDisplayVictoryModal(true);
      }
      if (combatActionResult.heroResult.isDead) {
        setPlayOutros(true);
        setDisplayDefeatModal(true);
      }
      if (displayCombatPhases === "no") {
        dispatch(setDisplayCombatPhases(""));
        if (!combatActionResult.monsterResult.isDead && !combatActionResult.heroResult.isDead) {
          dispatch(getCombatStartRound());
        }
      }
      if (displayCombatSkills === "no") {
        dispatch(setDisplayCombatSkills(""));
      }
      if (combatActionResult.didHeroFlee) {
        dispatch(clearCombatActionType());
        dispatch(clearCombatActionResult());
        dispatch(updateGameState(gameState.zone.slug as DcxTiles));
      }
    }
  }, [displayCombatPhases, displayCombatSkills]);

  useEffect(() => {
    if (playIntros) {
      if (playerSettings.playVoice) {
        if (!isVoicelessMonster) {
          setMonsterIntro();
        } else {
          setHeroIntro();
        }
      } else {
        setIntrosCompleted(true);
        setPlayIntros(false);
      }
    }
  }, [playIntros]);

  useEffect(() => {
    if (playOutros) {
      if (playerSettings.playVoice) {
        if (!isVoicelessMonster) {
          setMonsterOutro();
        } else {
          setHeroOutro();
        }
      } else {
        setOutrosCompleted(true);
        setPlayOutros(false);
      }
    }
  }, [playOutros]);

  // useEffect(() => {
  //   if (outrosCompleted) {
  //     if (combatActionResult.monsterResult.isDead) {
  //       setDisplayVictoryModal(true);
  //     } else {
  //       setDisplayDefeatModal(true);
  //     }
  //   }
  // }, [outrosCompleted]);

  useEffect(() => {
    if (displayLingeringDamageToMonster) {
      const timer2 = setTimeout(() => {
        setDisplayLingeringDamageToMonster((prevVal) => !prevVal);
      }, 2000);
      // clear the timeout if user closes modal before timeout length (750ms) completes
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [displayLingeringDamageToMonster]);

  useEffect(() => {
    if (displayHeroHealing) {
      const timer2 = setTimeout(() => {
        setDisplayHeroHealing((prevVal) => !prevVal);
      }, 2000);
      // clear the timeout if user closes modal before timeout length (750ms) completes
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [displayHeroHealing]);

  useEffect(() => {
    if (displayMonsterHealing) {
      const timer2 = setTimeout(() => {
        setDisplayMonsterHealing((prevVal) => !prevVal);
      }, 2000);
      // clear the timeout if user closes modal before timeout length (750ms) completes
      return () => {
        clearTimeout(timer2);
      };
    }
  }, [displayMonsterHealing]);

  const checkLevelDifferential = () => {
    if (combatActionResult) {
      const differential = combatActionResult.heroResult.hero.level - combatActionResult.monsterResult.monster.level;
      if (differential > 3) {
        setWarningMessage(`THIS MONSTER IS TOO WEAK FOR YOU! YOU WILL NOT GAIN ANY XP.`);
      }
      if (differential < -3) {
        setWarningMessage(`BEWARE, HERO! THIS FOE APPEARS TO BE BEYOND YOUR ABILITIES. TREAD CAREFULLY.`);
      }
    }
  };

  const handleAttackClick = () => {
    dispatch(getCombatAttack());
  };

  const handleUseSkillClick = () => {
    dispatch(setDisplayCombatSkills("yes"));
  };

  const handlePersuadeClick = () => {
    dispatch(getCombatPersuade());
  };

  const handleFleeClick = () => {
    dispatch(getCombatFlee());
  };

  const getBackgroundBarWidth = (): number => {
    if (width > xlScreenWidth) {
      return 520;
    } else if (width <= xlScreenWidth && width > smScreenWidth) {
      return 418;
    } else {
      return 375;
    }
  };

  const getTrimmedWidth = (): number => {
    if (width > xlScreenWidth) {
      return 486;
    } else if (width <= xlScreenWidth && width > smScreenWidth) {
      return 390;
    } else {
      return 352;
    }
  };

  const getMonsterImageSlug = (): string => {
    const personality = combatActionResult.monsterResult.monster.personalityType;
    if (
      personality === MonsterPersonalityTypeDto.Arcane ||
      personality === MonsterPersonalityTypeDto.Brutal ||
      personality === MonsterPersonalityTypeDto.Deadly ||
      personality === MonsterPersonalityTypeDto.Inspired ||
      personality === MonsterPersonalityTypeDto.Lean ||
      personality === MonsterPersonalityTypeDto.Reckless
    ) {
      return `${combatActionResult.monsterResult.monster.monsterSlug}-deadly`;
    } else {
      return `${combatActionResult.monsterResult.monster.monsterSlug}`;
    }
  };

  const isVoicelessMonster = voicelessMonsters.indexOf(combatActionResult.monsterResult.monster.monsterSlug) > -1;

  const setHeroIntro = () => {
    setVoiceCharacterType(`heroes`);
    setVoiceSlug(hero.gender.toLowerCase() + "-" + hero.heroClass.toLowerCase() + "/intro-" + rng(1, 3));
  };

  const setMonsterIntro = () => {
    setVoiceCharacterType(`monsters`);
    setVoiceSlug(gameState.slug + "/" + combatActionResult.monsterResult.monster.monsterSlug + "/intro");
    setMonsterIntroPlayed(true);
  };

  const setHeroOutro = () => {
    setVoiceCharacterType(`heroes`);
    if (combatActionResult.monsterResult.isDead) {
      let soundSlug = "";
      if (hero.remainingHitPoints / hero.totalHitPoints <= 0.15) {
        soundSlug = "narrow";
      } else {
        soundSlug = rng(1, 2).toString();
      }
      setVoiceSlug(hero.gender.toLowerCase() + "-" + hero.heroClass.toLowerCase() + "/victory-" + soundSlug);
    }
    if (combatActionResult.heroResult.isDead) {
      let soundSlug = "";
      soundSlug = rng(1, 2).toString();
      setVoiceSlug(hero.gender.toLowerCase() + "-" + hero.heroClass.toLowerCase() + "/defeat-" + soundSlug);
    }
  };

  const setMonsterOutro = () => {
    setVoiceCharacterType(`monsters`);
    let slug = gameState.slug + "/" + combatActionResult.monsterResult.monster.monsterSlug + "/";
    let result = combatActionResult.monsterResult.isDead ? "defeat" : "victory";
    slug += result;
    setVoiceSlug(slug);
    setMonsterOutroPlayed(true);
  };

  const clearVoiceLine = () => {
    if (playIntros) {
      if (monsterIntroPlayed) {
        setHeroIntro();
        setMonsterIntroPlayed(false);
      } else {
        setVoiceSlug("");
        setVoiceCharacterType("");
        setIntrosCompleted(true);
        setPlayIntros(false);
      }
    } else if (playOutros) {
      if (monsterOutroPlayed) {
        setHeroOutro();
        setMonsterOutroPlayed(false);
      } else {
        setVoiceSlug("");
        setVoiceCharacterType("");
        setOutrosCompleted(true);
      }
    } else {
      setVoiceSlug("");
      setVoiceCharacterType("");
    }
  };

  const onVoiceLineError = () => {
    if (playIntros) {
      if (voiceCharacterType === "monsters") {
        clearVoiceLine();
      } else {
        setPlayIntros(false);
        setIntrosCompleted(true);
      }
    } else if (playOutros) {
      if (voiceCharacterType === "monsters") {
        clearVoiceLine();
      } else {
        setPlayOutros(false);
        setOutrosCompleted(true);
      }
    }
  };

  const renderStatusEffects = () => {
    const statusEffects: JSX.Element[] = [];
    positiveStatusEffects.forEach((pse) => {
      statusEffects.push(
        <Grid container key={pse.name} className={styles.statusEffectContainer}>
          <ThemeProvider theme={tooltipTheme}>
            <Tooltip
              disableFocusListener
              placement="top"
              arrow
              TransitionComponent={Zoom}
              enterTouchDelay={0}
              title={pse.description}
            >
              <span className={styles.skill}>{`${pse.name}`}</span>
            </Tooltip>
          </ThemeProvider>
        </Grid>
      );
    });
    negativeStatusEffects.forEach((nse) => {
      statusEffects.push(
        <Grid container key={nse.name} className={styles.statusEffectContainer}>
          <ThemeProvider theme={tooltipTheme}>
            <Tooltip
              disableFocusListener
              placement="top"
              arrow
              TransitionComponent={Zoom}
              enterTouchDelay={0}
              title={nse.description}
            >
              <span className={styles.specialAbility}>{`${nse.name}`}</span>
            </Tooltip>
          </ThemeProvider>
        </Grid>
      );
    });
    return statusEffects;
  };

  return (
    <Grid container direction="row" className={styles.main}>
      {voiceSlug !== "" && voiceCharacterType !== "" && (
        <DCXAudioPlayer
          audioUrl={`/audio/voice/${voiceCharacterType}/${voiceSlug}`}
          soundType={SoundType.VOICE}
          onEnded={() => clearVoiceLine()}
          onError={() => onVoiceLineError()}
        />
      )}
      {monster.id !== "" && hero.id !== -1 && (
        <Grid container className={styles.parentContainer}>
          <Grid container direction="row" className={styles.container}>
            {width > mdScreenWidth && (
              <Grid container className={styles.descriptionContainer}>
                <Typography component="span" variant="h6" className={styles.descriptionText}>
                  {monster.description}
                </Typography>
              </Grid>
            )}
            <Grid container className={styles.subContainer}>
              <Grid container direction="column" className={styles.heroContainer} rowGap={1}>
                {warningMessage !== "" && (
                  <ThemeProvider theme={tooltipTheme}>
                    <Tooltip
                      disableFocusListener
                      placement="top"
                      arrow
                      enterTouchDelay={0}
                      TransitionComponent={Zoom}
                      title={warningMessage}
                    >
                      <Grid container className={styles.warningMessageContainer}>
                        <Image
                          src="/img/unity-assets/shared/small_circle_bg.png"
                          height={40}
                          width={40}
                          quality={100}
                        />
                        <PriorityHighIcon className={styles.warningIcon} />
                      </Grid>
                    </Tooltip>
                  </ThemeProvider>
                )}
                <HeroCard hero={hero} disableButtons={true} />
                <Grid container className={styles.heroHealthBarContainer}>
                  <HealthBar
                    hero={hero}
                    backgroundBarHeight={80}
                    backgroundBarWidth={getBackgroundBarWidth()}
                    trimmedWidth={getTrimmedWidth()}
                    barHeight={28}
                  />
                  <Grid
                    container
                    className={displayHeroHealing ? styles.healthBarFadeInContainer : styles.healthBarFadeOutContainer}
                  >
                    <Grid container className={styles.eventBackground}>
                      <Image src="/img/unity-assets/combat/event-splash.png" height={57} width={80} quality={100} />
                    </Grid>
                    <Typography component="span" className={styles.healingText}>
                      {displayHeroHealingAmount}
                    </Typography>
                  </Grid>
                </Grid>
                <Grid container className={styles.characterStatContainer}>
                  <Grid container className={styles.heroStatShadowContainer} />
                  <Image
                    src="/img/unity-assets/combat/combat_stat_background.png"
                    height={width > xlScreenWidth ? 65 : 60}
                    width={width > xlScreenWidth ? 389 : 349}
                    className={styles.characterStatBackground}
                    quality={100}
                  />
                  <Grid container className={styles.statContainer}>
                    <Grid container direction="row" className={styles.statRow}>
                      <Grid item xs={6}>
                        <Typography component="span" className={styles.statHeader}>
                          DAMAGE:
                        </Typography>
                      </Grid>
                      <Grid item xs={6} className={styles.stat}>
                        <Typography component="span" className={styles.statText}>
                          {`${hero.calculatedStats.damageRange.lower}-${hero.calculatedStats.damageRange.upper}`}
                        </Typography>
                      </Grid>
                    </Grid>
                    <Grid container direction="row" className={styles.statRow}>
                      <Grid item xs={6}>
                        <Typography component="span" className={styles.statHeader}>
                          CHARISMA:
                        </Typography>
                      </Grid>
                      <Grid item xs={6} className={styles.stat}>
                        <Typography component="span" className={styles.statText}>
                          {hero.calculatedStats.charisma}
                        </Typography>
                      </Grid>
                    </Grid>
                    <Grid container direction="row" className={styles.statRow}>
                      <Grid item xs={6}>
                        <Typography component="span" className={styles.statHeader}>
                          QUICKNESS:
                        </Typography>
                      </Grid>
                      <Grid item xs={6} className={styles.stat}>
                        <Typography component="span" className={styles.statText}>
                          {hero.calculatedStats.quickness}
                        </Typography>
                      </Grid>
                    </Grid>
                  </Grid>
                </Grid>
              </Grid>
              <Grid container className={styles.centerContainer}>
                <Grid container className={styles.statusEffectsContainer}>
                  <Grid container className={styles.effectsShadowContainer} />
                  <Image src="/img/unity-assets/shared/tooltip_bg.png" height={130} width={180} quality={100} />
                  <Grid container className={styles.statusEffectsHeaderContainer}>
                    <Image src="/img/unity-assets/shared/header_red.png" height={35} width={180} quality={100} />
                  </Grid>
                  <Typography component="span" className={styles.statusEffectsHeader}>
                    {`ROUND ${combatActionResult.round + 1} EFFECTS`}
                  </Typography>
                  <Grid container className={styles.statusEffects}>
                    {renderStatusEffects()}
                  </Grid>
                </Grid>
                <Grid container direction="row" className={styles.buttonsContainer}>
                  <Grid container className={styles.buttonContainer}>
                    <DCXButton
                      title="ATTACK"
                      height={42}
                      width={144}
                      color="red"
                      disabled={
                        combatActionResult.heroResult.isDead ||
                        combatActionResult.monsterResult.isDead ||
                        combatActionStatus !== Status.NotStarted
                        // || !introsCompleted
                      }
                      disabledLayerHeightAdjustment={11}
                      disabledLayerWidthAdjustment={20}
                      onClick={handleAttackClick}
                    />
                  </Grid>
                  <Grid container className={styles.buttonContainer}>
                    <DCXButton
                      title="USE SKILL"
                      height={42}
                      width={144}
                      color="blue"
                      disabled={
                        combatActionResult.heroResult.isDead ||
                        combatActionResult.monsterResult.isDead ||
                        !combatActionResult.isSkillUseAvailable ||
                        combatActionStatus !== Status.NotStarted
                        // || !introsCompleted
                      }
                      disabledLayerHeightAdjustment={11}
                      disabledLayerWidthAdjustment={20}
                      onClick={handleUseSkillClick}
                    />
                  </Grid>
                  <Grid container className={styles.buttonContainer}>
                    <DCXButton
                      title="PERSUADE"
                      height={42}
                      width={144}
                      color={combatActionResult.isCharismaOpportunityAvailable ? "red" : "blue"}
                      disabled={
                        combatActionResult.heroResult.isDead ||
                        combatActionResult.monsterResult.isDead ||
                        !combatActionResult.isCharismaOpportunityAvailable ||
                        combatActionStatus !== Status.NotStarted
                        // || !introsCompleted
                      }
                      disabledLayerHeightAdjustment={11}
                      disabledLayerWidthAdjustment={20}
                      onClick={handlePersuadeClick}
                    />
                  </Grid>
                  <Grid container className={styles.buttonContainer}>
                    <DCXButton
                      title="FLEE"
                      height={42}
                      width={144}
                      color="blue"
                      disabled={
                        combatActionResult.heroResult.isDead ||
                        combatActionResult.monsterResult.isDead ||
                        combatActionStatus !== Status.NotStarted
                        // || !introsCompleted
                      }
                      disabledLayerHeightAdjustment={11}
                      disabledLayerWidthAdjustment={20}
                      onClick={handleFleeClick}
                      marginBottom={5}
                    />
                  </Grid>
                </Grid>
              </Grid>
              <Grid container direction="column" className={styles.monsterContainer} rowGap={1}>
                <Grid container className={styles.shadowContainer} />
                <Grid container className={styles.characterImageContainer}>
                  <Grid container className={styles.cardBgContainer}>
                    <Image
                      src={`/img/unity-assets/shared/window_bg_vertical.png`}
                      height={width > xlScreenWidth ? 512 : 460}
                      width={width > xlScreenWidth ? 398 : 358}
                      quality={100}
                    />
                  </Grid>
                  <Grid container className={styles.bottomLeftOrnamentContainer}>
                    <Image
                      src={`/img/unity-assets/shared/window_bottom_left_angled.png`}
                      height={27}
                      width={27}
                      quality={100}
                    />
                  </Grid>
                  <Grid container className={styles.bottomRightOrnamentContainer}>
                    <Image
                      src={`/img/unity-assets/shared/window_bottom_right_angled.png`}
                      height={27}
                      width={27}
                      quality={100}
                    />
                  </Grid>
                  <Grid
                    container
                    className={styles.cardHeaderContainer}
                    style={{ top: width < xlScreenWidth ? -3 : 0 }}
                  >
                    <Image
                      src={`/img/unity-assets/card/character_border_header.png`}
                      height={width > xlScreenWidth ? 58 : 58}
                      width={width > xlScreenWidth ? 397 : 358.65}
                      quality={100}
                    />
                  </Grid>
                  <Grid container className={styles.characterLevelBackground}>
                    <Image
                      src={`/img/unity-assets/card/level_monster.png`}
                      height={width > xlScreenWidth ? 80 : 72}
                      width={width > xlScreenWidth ? 80 : 72}
                      quality={100}
                    />
                    <Typography component="span" className={styles.monsterLevel}>
                      {monster.level}
                    </Typography>
                  </Grid>
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
                            <Typography component="span" className={styles.characterPersonality}>
                              {`(${combatActionResult.monsterResult.monster.personalityType})`}
                            </Typography>
                          </Tooltip>
                        </ThemeProvider>
                      )}
                    <Typography component="span" className={styles.characterName}>
                      {`${combatActionResult.monsterResult.monster.name}`}
                    </Typography>
                  </Grid>
                  <Grid container className={styles.monsterImage}>
                    <Image
                      src={`/img/api/monsters/${monster.locationTile}/${getMonsterImageSlug()}.png`}
                      height={width > xlScreenWidth ? 475 : 425}
                      width={width > xlScreenWidth ? 371 : 333.9}
                      quality={100}
                    />
                  </Grid>
                </Grid>
                <Grid container className={styles.healthBarContainer}>
                  <HealthBar
                    monster={monster}
                    backgroundBarHeight={80}
                    backgroundBarWidth={getBackgroundBarWidth()}
                    trimmedWidth={getTrimmedWidth()}
                    barHeight={28}
                  />
                  <Grid
                    container
                    className={
                      displayLingeringDamageToMonster
                        ? styles.healthBarFadeInContainer
                        : styles.healthBarFadeOutContainer
                    }
                  >
                    <Grid container className={styles.eventBackground}>
                      <Image src="/img/unity-assets/combat/event-splash.png" height={57} width={80} quality={100} />
                    </Grid>
                    <Typography component="span" className={styles.monsterEventText}>
                      {lingeringDamageToMonster}
                    </Typography>
                  </Grid>
                  <Grid
                    container
                    className={
                      displayMonsterHealing ? styles.healthBarFadeInContainer : styles.healthBarFadeOutContainer
                    }
                  >
                    <Grid container className={styles.eventBackground}>
                      <Image src="/img/unity-assets/combat/event-splash.png" height={57} width={80} quality={100} />
                    </Grid>
                    <Typography component="span" className={styles.healingText}>
                      {displayMonsterHealingAmount}
                    </Typography>
                  </Grid>
                </Grid>
                <Grid container className={styles.characterStatContainer}>
                  <Grid container className={styles.monsterStatShadowContainer} />
                  <Image
                    src="/img/unity-assets/combat/combat_stat_background.png"
                    height={width > xlScreenWidth ? 75 : 70}
                    width={width > xlScreenWidth ? 398.5 : 358.65}
                    quality={100}
                  />
                  <Grid container className={styles.statContainer}>
                    <Grid container direction="row" className={styles.statRow}>
                      <Grid item xs={6}>
                        <Typography component="span" className={styles.statHeader}>
                          DAMAGE:
                        </Typography>
                      </Grid>
                      <Grid item xs={6} className={styles.stat}>
                        <Typography component="span" className={styles.statText}>{`${calculateDamageRange(
                          monster.dieDamage,
                          monster.bonusDamage
                        )}`}</Typography>
                      </Grid>
                    </Grid>
                    <Grid container direction="row" className={styles.statRow}>
                      <Grid item xs={6}>
                        <Typography component="span" className={styles.statHeader}>
                          CHARISMA:
                        </Typography>
                      </Grid>
                      <Grid item xs={6} className={styles.stat}>
                        <Typography component="span" className={styles.statText}>
                          {monster.charisma}
                        </Typography>
                      </Grid>
                    </Grid>
                    <Grid container direction="row" className={styles.statRow}>
                      <Grid item xs={6}>
                        <Typography component="span" className={styles.statHeader}>
                          QUICKNESS:
                        </Typography>
                      </Grid>
                      <Grid item xs={6} className={styles.stat}>
                        <Typography component="span" className={styles.statText}>
                          {monster.quickness}
                        </Typography>
                      </Grid>
                    </Grid>
                  </Grid>
                </Grid>
              </Grid>
            </Grid>
          </Grid>
          {monster.id === "" && (
            <Grid item>
              <Typography component="span" variant="h6">
                Exploring...
              </Typography>
            </Grid>
          )}
          {/* Attack Modal */}
          <CombatPhases combatActionResult={combatActionResult} introsPlaying={playIntros} outrosPlaying={playOutros} />
          {/* Skills Modal */}
          <CombatSkills combatActionResult={combatActionResult} introsPlaying={playIntros} outrosPlaying={playOutros} />
          {/* Combat Resolution Modal */}
          {(displayVictoryModal || displayDefeatModal) && (
            <CombatResolution isVictory={displayVictoryModal ? true : false} />
          )}
        </Grid>
      )}
    </Grid>
  );
};

export default Combat;
