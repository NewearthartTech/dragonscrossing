import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import styles from "./dcx-learned-skill.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import useWindowDimensions from "@/helpers/window-dimensions";
import Image from "next/image";
import { useEffect, useRef, useState } from "react";
import { mdScreenWidth, tooltipTheme } from "@/helpers/global-constants";
import {
  getAllocateSkillPoints,
  getDeAllocateSkillPoints,
  getForgetSkill,
  resetAllocateSkillPointsStatus,
  resetDeAllocateSkillPointsStatus,
  selectAllocateSkillPointsStatus,
  selectDeAllocateSkillPointsStatus,
  selectSelectedHero,
} from "@/state-mgmt/hero/heroSlice";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import { selectPlayer } from "@/state-mgmt/player/playerSlice";
import { LearnedHeroSkill } from "@dcx/dcx-backend";
import DCXButton from "@/components/dcx-button/dcx-button";
import { SkillAllocatePointsRequest } from "@/state-mgmt/skill/skillTypes";
import { ThemeProvider } from "@mui/material/styles";
import Tooltip from "@mui/material/Tooltip";
import Zoom from "@mui/material/Zoom";
import CloseButton from "@/components/close-button/close-button";
import { Status } from "@/state-mgmt/app/appTypes";

interface Props {
  skill: LearnedHeroSkill;
  page?: string;
  small?: boolean;
  hideDetails?: boolean;
  disableActions?: boolean;
  disableOverlays?: boolean;
  isClickable?: boolean;
}

const DCXLearnedSkill: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const hero = useAppSelector(selectSelectedHero).hero;
  const player = useAppSelector(selectPlayer);
  const allocateSkillPointsStatus = useAppSelector(selectAllocateSkillPointsStatus);
  const deAllocateSkillPointsStatus = useAppSelector(selectDeAllocateSkillPointsStatus);

  const { width, height } = useWindowDimensions();
  const { skill, page, small, hideDetails, disableActions, disableOverlays, isClickable } = props;
  const ref = useRef<any>(null);

  const [displayActions, setDisplayActions] = useState(false);
  const [forgetConfirmation, setForgetConfirmation] = useState(false);

  const containerWidth = small ? 74.2 : 215;
  const skillImageWidth = small ? 71.5 : 207;
  const skillIconWidth = small ? 9 : 30;
  const detailsContainerWidth = small ? 63.3 : 211;
  let mainContainerHeight = small ? 84.5 : 277;
  const imageContainerHeight = small ? 63 : 180;
  const skillImageHeight = small ? 60.5 : 172;
  const skillIconHeight = small ? 9 : 30;
  const detailsContainerHeight = small ? 19.5 : 97;
  if (hideDetails) {
    mainContainerHeight = mainContainerHeight - detailsContainerHeight;
  }

  // If a player clicks outside of the Skill Image then close the menu
  useEffect(() => {
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      // Unbind the event listener on clean up
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [ref]);

  useEffect(() => {
    console.log("allocateSkillPointsStatus", allocateSkillPointsStatus);
    if (allocateSkillPointsStatus.status === Status.Loaded || allocateSkillPointsStatus.status === Status.Failed) {
      dispatch(resetAllocateSkillPointsStatus());
    }
  }, [allocateSkillPointsStatus]);

  useEffect(() => {
    if (deAllocateSkillPointsStatus.status === Status.Loaded || deAllocateSkillPointsStatus.status === Status.Failed) {
      dispatch(resetDeAllocateSkillPointsStatus());
    }
  }, [deAllocateSkillPointsStatus]);

  const handleClickOutside = (e: any) => {
    if (ref.current && !ref.current.contains(e.target)) {
      setDisplayActions(false);
    }
  };

  const onHover = () => {
    setDisplayActions(true);
  };

  const onLeave = () => {
    setDisplayActions(false);
  };

  const onClick = () => {
    setDisplayActions(true);
  };

  const handleIncrementUse = () => {
    dispatch(getAllocateSkillPoints(skill.id));
  };

  const handleDecrementUse = () => {
    dispatch(getDeAllocateSkillPoints(skill.id));
  };

  const handleForgetSkill = () => {
    setForgetConfirmation(true);
  };

  const handleForgetConfirmation = () => {
    dispatch(getForgetSkill(skill.id));
    setForgetConfirmation(false);
  };

  const handleClose = () => {
    setForgetConfirmation(false);
  };

  return (
    <Grid
      container
      className={styles.main}
      style={{
        height: mainContainerHeight,
        width: containerWidth,
        cursor: isClickable ? "pointer" : undefined,
      }}
    >
      <Grid
        container
        className={styles.imageContainer}
        style={{ height: imageContainerHeight, width: containerWidth }}
        onMouseEnter={() => width > mdScreenWidth && !disableOverlays && onHover()}
        onMouseLeave={() => width > mdScreenWidth && !disableOverlays && onLeave()}
        onClick={() => width <= mdScreenWidth && !disableOverlays && onClick()}
        ref={ref}
      >
        <Image
          src="/img/unity-assets/skills/skill_bg.png"
          height={imageContainerHeight}
          width={containerWidth}
          quality={100}
        />
        <Grid container className={small ? styles.skillImageSmall : styles.skillImage}>
          <Image
            src={`/img/api/skills/${skill.slug}.png`}
            height={skillImageHeight}
            width={skillImageWidth}
            quality={100}
            className={isClickable ? styles.hoverPointer : undefined}
          />
        </Grid>
        {skill.slug && skill.name && skill.name !== "" && !disableOverlays && (
          <ThemeProvider theme={tooltipTheme}>
            <Tooltip
              disableFocusListener
              placement="top"
              arrow
              TransitionComponent={Zoom}
              enterTouchDelay={0}
              title={skill.name}
            >
              <Grid container className={styles.skillIcon} style={{ height: skillIconHeight, width: skillIconWidth }}>
                <Image
                  src={`/img/api/skills/icons/${skill.slug}.png`}
                  height={skillIconHeight}
                  width={skillIconWidth}
                  quality={100}
                />
              </Grid>
            </Tooltip>
          </ThemeProvider>
        )}
        <ThemeProvider theme={tooltipTheme}>
          <Tooltip
            disableFocusListener
            placement="top"
            arrow
            TransitionComponent={Zoom}
            enterTouchDelay={0}
            title={"USES"}
          >
            <Grid container className={small ? styles.usesCountContainerSmall : styles.usesCountContainer}>
              <Typography
                component="span"
                className={small ? styles.usesCountSmall : styles.usesCount}
                style={{ cursor: isClickable ? "pointer" : undefined }}
              >
                {skill.skillUseInstance.filter((sui) => !sui.alreadyUsed).length}
              </Typography>
            </Grid>
          </Tooltip>
        </ThemeProvider>
        {!disableOverlays && (
          <Grid container className={styles.skillNameBackground} style={{ width: skillImageWidth }} />
        )}
        {!disableOverlays && (
          <Typography component="span" className={styles.skillNameText}>
            {skill.name}
          </Typography>
        )}
        {!disableActions && displayActions && (
          <Grid
            container
            className={styles.actionsContainer}
            style={{ height: skillImageHeight, width: skillImageWidth }}
          >
            <Grid container className={styles.actionsContainerBackground} />
            <Grid container className={styles.actionButtonContainer}>
              {page === "equip" && (
                <Grid container direction="column" className={styles.equipActionButton}>
                  <Grid container className={styles.forgetButtonContainer} onClick={() => handleForgetSkill()}>
                    <DeleteForeverIcon className={styles.forgetButton} />
                  </Grid>
                  <Grid container className={styles.button}>
                    <DCXButton
                      title="+"
                      height={23}
                      width={60}
                      color="blue"
                      disabled={
                        hero.baseSkillPoints - hero.usedUpSkillPoints < Number(skill.requiredSkillPoints) ||
                        hero.level < skill.levelRequirement ||
                        allocateSkillPointsStatus.status !== Status.NotStarted
                      }
                      fontSize={18}
                      hideArrows={true}
                      disabledLayerWidthAdjustment={10}
                      disabledLayerHeightAdjustment={6}
                      onClick={() => handleIncrementUse()}
                    />
                  </Grid>
                  <Grid container>
                    <DCXButton
                      title="-"
                      height={24}
                      width={60}
                      color="red"
                      disabled={
                        skill.skillUseInstance.filter((sui) => !sui.alreadyUsed).length <= 0 ||
                        deAllocateSkillPointsStatus.status !== Status.NotStarted
                      }
                      fontSize={18}
                      hideArrows={true}
                      disabledLayerWidthAdjustment={10}
                      disabledLayerHeightAdjustment={6}
                      onClick={() => handleDecrementUse()}
                    />
                  </Grid>
                </Grid>
              )}
            </Grid>
          </Grid>
        )}
      </Grid>
      {!hideDetails && (
        <Grid
          container
          className={styles.detailsContainer}
          style={{
            height: detailsContainerHeight,
            width: detailsContainerWidth,
          }}
        >
          <Image
            src="/img/unity-assets/shared/tooltip_bg.png"
            height={detailsContainerHeight}
            width={detailsContainerWidth}
            quality={100}
          />
          <Grid container className={styles.requiredStatsContainer}>
            <Grid container className={styles.requiredStatsSection}>
              <Typography component="span" className={styles.requiredPointsText}>
                POINTS
              </Typography>
              <Typography component="span" className={styles.requiredPointsText}>
                {skill.requiredSkillPoints}
              </Typography>
            </Grid>
            <Grid container className={styles.verticalDivider} />
            <Grid container className={styles.requiredStatsSection}>
              <Typography
                component="span"
                className={skill.levelRequirement > hero.level ? styles.warningText : styles.requiredPointsText}
              >
                {skill.levelRequirement}
              </Typography>
              <Typography component="span" className={styles.requiredPointsText}>
                LEVEL
              </Typography>
            </Grid>
          </Grid>
          <Grid container className={styles.skillDescriptionContainer}>
            <Typography component="span" className={styles.skillDescriptionText}>
              {skill.description}
            </Typography>
          </Grid>
        </Grid>
      )}
      <Modal open={forgetConfirmation} onClose={handleClose} className={styles.modalMain}>
        <Grid container className={styles.modalContainer}>
          <Grid container className={styles.forgetConfirmationBackground}>
            <Grid item>
              <Image src="/img/unity-assets/shared/tooltip_bg.png" height={125} width={262.5} quality={100} />
            </Grid>
            <Grid container className={styles.closeButtonContainer}>
              <CloseButton handleClose={handleClose} />
            </Grid>
            <Grid container className={styles.forgetContainer}>
              <Typography component="span" className={styles.confirmText}>
                ARE YOU SURE YOU WANT TO DESTROY THIS SKILL? THIS ACTION IS IRREVERSIBLE!
              </Typography>
            </Grid>
            <Grid container className={styles.forgetConfirmButton}>
              <DCXButton
                title="CONFIRM"
                height={32}
                width={120}
                color="red"
                onClick={() => handleForgetConfirmation()}
              />
            </Grid>
          </Grid>
        </Grid>
      </Modal>
    </Grid>
  );
};

export default DCXLearnedSkill;
