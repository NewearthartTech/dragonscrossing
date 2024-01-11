import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import styles from "./combat-skills.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { selectDisplayCombatSkills, setDisplayCombatSkills } from "@/state-mgmt/app/appSlice";
import { useEffect, useState } from "react";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { ReactPlayer, xsScreenWidth } from "@/helpers/global-constants";
import { selectPlayerSettings } from "@/state-mgmt/player/playerSlice";
import { Scrollbars } from "react-custom-scrollbars";
import { ActionResponseDto, DiceRollReason, DieResultDto, LearnedHeroSkill } from "@dcx/dcx-backend";
import { selectGameState } from "@/state-mgmt/game-state/gameStateSlice";
import CloseButton from "@/components/close-button/close-button";
import {
  getCombatSkill,
  selectCombatActionResultStatus,
  selectCombatActionType,
} from "@/state-mgmt/combat/combatSlice";
import DCXButton from "@/components/dcx-button/dcx-button";
import { SoundType, Status } from "@/state-mgmt/app/appTypes";
import { ActionType } from "@/state-mgmt/combat/combatTypes";
import DCXLearnedSkill from "@/components/dcx-skill/dcx-learned-skill/dcx-learned-skill";
import DCXAudioPlayer from "@/components/dcx-audio-player/dcx-audio-player";

interface Props {
  combatActionResult: ActionResponseDto;
  introsPlaying: boolean;
  outrosPlaying: boolean;
}

const CombatSkills: React.FC<Props> = (props: Props) => {
  const { combatActionResult, introsPlaying, outrosPlaying } = props;
  const dispatch = useAppDispatch();
  const displayCombatSkills = useAppSelector(selectDisplayCombatSkills);
  const playerSettings = useAppSelector(selectPlayerSettings);
  const gameState = useAppSelector(selectGameState);
  const combatActionStatus = useAppSelector(selectCombatActionResultStatus);
  const combatActionType = useAppSelector(selectCombatActionType);
  const { width, height } = useWindowDimensions();

  const [selectedSkillName, setSelectedSkillName] = useState("");
  const [selectedSkillSlug, setSelectedSkillSlug] = useState("");
  const [showConfirmModal, setShowConfirmModal] = useState(false);
  const [showDiceModal, setShowDiceModal] = useState(false);
  const [diceRollingStatus, setDiceRollingStatus] = useState("waiting");
  const [showDiceResults, setShowDiceResults] = useState(false);
  const [playSkillSound, setPlaySkillSound] = useState(false);

  useEffect(() => {
    if (displayCombatSkills == "yes") {
      if (playerSettings.autoRoll) {
        setDiceRollingStatus("completed");
        setShowDiceResults(true);
      }
    }
  }, [displayCombatSkills]);

  useEffect(() => {
    if (combatActionStatus === Status.Loaded && combatActionType === ActionType.SKILL) {
      setPlaySkillSound(true);
      let skillDiceCount = 0;
      if (combatActionResult.heroResult.attackResult.dice) {
        skillDiceCount = combatActionResult.heroResult.attackResult.dice?.filter(
          (d) => d.rollFor === DiceRollReason.SkillDamageRightAway || DiceRollReason.SkillHealing
        ).length;
      }

      if (skillDiceCount > 0) {
        setShowConfirmModal(false);
        setShowDiceModal(true);
      } else {
        setShowConfirmModal(false);
        handleCloseModal();
      }
    }
  }, [combatActionStatus]);

  const handleCloseModal = () => {
    dispatch(setDisplayCombatSkills("no"));
  };

  const handleSkillSelected = (skill: LearnedHeroSkill) => {
    setSelectedSkillName(skill.name);
    setSelectedSkillSlug(skill.slug);
    setShowConfirmModal(true);
  };

  const handleConfirmClick = () => {
    dispatch(getCombatSkill(selectedSkillSlug));
  };

  const handleCloseConfirmModal = () => {
    setSelectedSkillName("");
    setSelectedSkillSlug("");
    setShowConfirmModal(false);
  };

  const handleCloseDiceModal = () => {
    if (diceRollingStatus === "completed") {
      setShowDiceResults(false);
      setDiceRollingStatus("waiting");
      setShowDiceModal(false);
      handleCloseModal();
    }
  };

  const handleRollClick = () => {
    setDiceRollingStatus("rolling");
  };

  const handleRollEnded = () => {
    setShowDiceResults(true);
    setDiceRollingStatus("completed");
  };

  const isSkillUsable = (skill: LearnedHeroSkill): boolean => {
    if (
      !skill.skillUseInstance ||
      skill.skillUseInstance.length === 0 ||
      skill.skillUseInstance.findIndex((sui) => !sui.alreadyUsed) === -1 ||
      skill.levelRequirement > combatActionResult.heroResult.hero.level
    ) {
      return false;
    } else {
      return true;
    }
  };

  const sortSkills = () => {
    const unsortedSkills = combatActionResult.heroResult.hero.skills;
    const sortedSkills: Array<LearnedHeroSkill> = [];
    unsortedSkills.forEach((s) => {
      if (!isSkillUsable(s)) {
        sortedSkills.push(s);
      } else {
        sortedSkills.unshift(s);
      }
    });
    return sortedSkills;
  };

  const renderHeroSkills = () => {
    const skills: JSX.Element[] = [];
    const sortedCombatSkills = sortSkills();
    sortedCombatSkills.forEach((s) => {
      skills.push(
        <Grid
          container
          direction="row"
          className={isSkillUsable(s) ? styles.skillContainer : styles.skillContainerDisabled}
          key={s.id}
          onClick={() => (isSkillUsable(s) ? handleSkillSelected(s) : undefined)}
        >
          <Grid container className={styles.skillBackgroundContainer}>
            <Image
              src="/img/unity-assets/shared/tooltip_bg.png"
              height={100}
              width={width > xsScreenWidth ? 280 : 350}
              quality={100}
              className={isSkillUsable(s) ? styles.hoverPointer : undefined}
            />
          </Grid>
          <Grid container className={styles.skillCompContainer}>
            <DCXLearnedSkill
              skill={s}
              small={true}
              hideDetails={true}
              disableOverlays={true}
              page="combat"
              isClickable={isSkillUsable(s)}
            />
          </Grid>
          <Grid container className={styles.skillEffect}>
            <Typography component="span" className={styles.effectText}>
              {
                <span className={isSkillUsable(s) ? styles.hoverPointer : undefined}>
                  <span className={isSkillUsable(s) ? styles.skillName : styles.skillNameDisabled}>{s.name}</span>
                  {` - ${s.description}`}
                </span>
              }
            </Typography>
          </Grid>
        </Grid>
      );
    });
    return skills;
  };

  const renderDice = (diceResults: Array<DieResultDto>) => {
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
                playing={!playerSettings.autoRoll && diceRollingStatus === "rolling"}
                url={`video/${d.sides}.mp4`}
                controls={false}
                playsinline={true}
                muted={true}
                onEnded={() => handleRollEnded()}
              />
            </Grid>
            {showDiceResults && (
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

  return (
    <Grid container direction="row">
      {playSkillSound && !introsPlaying && !outrosPlaying && (
        <DCXAudioPlayer
          audioUrl={`/audio/voice/heroes/${combatActionResult.heroResult.hero.gender.toLowerCase()}-${combatActionResult.heroResult.hero.heroClass.toLowerCase()}/${selectedSkillSlug}`}
          soundType={SoundType.VOICE}
          onEnded={() => setPlaySkillSound(false)}
        />
      )}
      {diceRollingStatus === "rolling" && (
        <DCXAudioPlayer audioUrl={"/audio/sound-effects/combat/dice-roll"} soundType={SoundType.SOUND_EFFECT} />
      )}
      <Modal open={displayCombatSkills === "yes"} onClose={handleCloseModal} className={styles.modalMain}>
        <Grid container direction="row" className={styles.modalContainer}>
          <Grid container item lg={6} xs={12} direction="column">
            <Grid container className={styles.actionPhaseBackground}>
              <Grid container className={styles.combatActionBackground}>
                <Image
                  src="/img/unity-assets/shared/action_bg.png"
                  height={width > xsScreenWidth ? 435 : 440}
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
                SKILLS
              </Typography>
              <Grid container className={styles.closeButtonContainer}>
                <CloseButton handleClose={handleCloseModal} />
              </Grid>
              <Grid container className={styles.skillSelectionContainer}>
                <Scrollbars
                  renderThumbVertical={() => (
                    <Grid
                      container
                      style={{
                        width: "5px",
                        borderRadius: "4px",
                        backgroundColor: "rgb(230, 230, 230)",
                      }}
                    />
                  )}
                >
                  <Grid container className={styles.skillSelectionSubContainer}>
                    {combatActionResult.heroResult.hero.skills.length > 0 ? (
                      renderHeroSkills()
                    ) : (
                      <Grid container className={styles.noSkillsMessageContainer}>
                        <Typography
                          component="span"
                          className={styles.noSkillsMessage}
                        >{`YOU HAVE NOT LEARNED ANY SKILLS`}</Typography>
                      </Grid>
                    )}
                  </Grid>
                </Scrollbars>
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </Modal>
      <Modal open={showConfirmModal} onClose={handleCloseConfirmModal} className={styles.confirmModalMain}>
        <Grid container className={styles.confirmModalContainer}>
          <Image src="/img/unity-assets/shared/tooltip_bg.png" height={100} width={220} quality={100} />
          <Grid container className={styles.confirmContainer}>
            <Typography component="span" className={styles.confirmSkillText}>
              ARE YOU SURE YOU WANT TO CAST <span className={styles.selectedSkillName}>{selectedSkillName}</span>
              <span className={styles.questionMark}>?</span>
            </Typography>
          </Grid>
          <Grid container className={styles.confirmCastButtonContainer}>
            <DCXButton title="CONFIRM" height={32} width={120} color="red" onClick={() => handleConfirmClick()} />
          </Grid>
        </Grid>
      </Modal>
      <Modal open={showDiceModal} onClose={handleCloseDiceModal} className={styles.diceModalMain}>
        <Grid container className={styles.diceModalContainer}>
          <Image src="/img/unity-assets/shared/window_bg_rounded_corner.png" height={260} width={230} quality={100} />
          <Grid container className={styles.diceContainer}>
            {renderDice(
              combatActionResult.heroResult.attackResult.dice!.filter(
                (d) => d.rollFor === DiceRollReason.SkillDamageRightAway || DiceRollReason.SkillHealing
              )
            )}
          </Grid>
          <Grid container className={styles.confirmButtonContainer}>
            <DCXButton
              title={diceRollingStatus !== "completed" ? "ROLL" : "RESOLVE"}
              height={32}
              width={120}
              color={diceRollingStatus !== "completed" ? "blue" : "red"}
              onClick={() => (diceRollingStatus !== "completed" ? handleRollClick() : handleCloseDiceModal())}
            />
          </Grid>
        </Grid>
      </Modal>
    </Grid>
  );
};

export default CombatSkills;
