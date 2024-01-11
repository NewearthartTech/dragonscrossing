import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import styles from "./dcx-unlearned-skill.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import useWindowDimensions from "@/helpers/window-dimensions";
import Image from "next/image";
import { useEffect, useRef, useState } from "react";
import { mdScreenWidth, tooltipTheme, walletAddressMismatch } from "@/helpers/global-constants";
import { getSelectedHero, selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { selectConnectedWalletAddress, selectDcxBalance } from "@/state-mgmt/player/playerSlice";
import { UnlearnedHeroSkill } from "@dcx/dcx-backend";
import DCXButton from "@/components/dcx-button/dcx-button";
import { SoundType, Status } from "@/state-mgmt/app/appTypes";
import CircularProgress from "@mui/material/CircularProgress";
import {
  getLearnSkill,
  resetGetLearnSkillStatus,
  selectGetLearnSkillStatus,
  selectLearnSkillResponse,
  selectLearnSkillSubmittedStatus,
  selectSkillLearnMessage,
  selectSkillToLearnId,
  setLearnSkillMessage,
  setLearnSkillSubmittedStatus,
  setSkillToLearnId,
  submitLearnSkillTransaction,
} from "@/state-mgmt/vendor/vendorSlice";
import { useConnectCalls } from "@/components/web3";
import { setAppMessage, setRefreshItemNFTsDelayed } from "@/state-mgmt/app/appSlice";
import DcxLogo from "@/components/dcx-logo/dcx-logo";
import DCXAudioPlayer from "@/components/dcx-audio-player/dcx-audio-player";
import { ThemeProvider } from "@mui/material/styles";
import Tooltip from "@mui/material/Tooltip";
import Zoom from "@mui/material/Zoom";

interface Props {
  skill: UnlearnedHeroSkill;
  skillId: string;
  nftTokenId: number;
  page?: string;
  small?: boolean;
  hideDetails?: boolean;
  disableActions?: boolean;
  disableOverlays?: boolean;
  isClickable?: boolean;
  chainId: number;
}

const DCXUnlearnedSkill: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const { connect } = useConnectCalls();
  const hero = useAppSelector(selectSelectedHero).hero;
  const connectedWalletAddress = useAppSelector(selectConnectedWalletAddress);
  const learnSkillResponse = useAppSelector(selectLearnSkillResponse);
  const getLearnSkillStatus = useAppSelector(selectGetLearnSkillStatus);
  const learnSkillSubmittedStatus = useAppSelector(selectLearnSkillSubmittedStatus);
  const skillToLearnId = useAppSelector(selectSkillToLearnId);
  const skillLearnMessage = useAppSelector(selectSkillLearnMessage);

  const { width, height } = useWindowDimensions();
  const { skill, skillId, nftTokenId, page, small, hideDetails, disableActions, disableOverlays, isClickable, chainId } = props;
  const ref = useRef<any>(null);

  const [displayActions, setDisplayActions] = useState(false);
  const [learnConfirmation, setLearnConfirmation] = useState(false);
  const [learnedSkillsCount, setLearnedSkillsCount] = useState(0);
  const [showStatusModal, setShowStatusModal] = useState(false);
  const [spinnerModalText, setSpinnerModalText] = useState("");
  const [playSkillbookConsume, setPlaySkillbookConsume] = useState(false);

  const containerWidth = small ? 74.2 : 215;
  const skillImageWidth = small ? 71.5 : 207;
  const skillIconWidth = small ? 9 : 30;
  const detailsContainerWidth = small ? 63.3 : 211;
  let mainContainerHeight = small ? 84.5 : 265;
  const imageContainerHeight = small ? 63 : 180;
  const skillImageHeight = small ? 60.5 : 172;
  const skillIconHeight = small ? 9 : 30;
  const detailsContainerHeight = small ? 19.5 : 85;

  if (hideDetails) {
    mainContainerHeight = mainContainerHeight - detailsContainerHeight;
  }

  useEffect(() => {
    if (hero.skills) {
      setLearnedSkillsCount(hero.skills.length);
    }
  }, []);

  // If a player clicks outside of the Skill Image then close the menu
  useEffect(() => {
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      // Unbind the event listener on clean up
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [ref]);

  useEffect(() => {
    if (skillId === skillToLearnId) {
      if (getLearnSkillStatus.status === Status.Loaded) {
        submitLearnSkill();
        dispatch(resetGetLearnSkillStatus());
      }
    }
  }, [getLearnSkillStatus]);

  useEffect(() => {
    if (skillId === skillToLearnId) {
      if (learnSkillSubmittedStatus === "processing") {
        setSpinnerModalText(skillLearnMessage);
        setShowStatusModal(true);
      }
      if (learnSkillSubmittedStatus === "failed") {
        setSpinnerModalText(skillLearnMessage);
        setShowStatusModal(true);
      }
      if (learnSkillSubmittedStatus === "completed") {
        setPlaySkillbookConsume(true);
        setSpinnerModalText(skillLearnMessage);
        setShowStatusModal(true);
        dispatch(getSelectedHero());
        dispatch(setRefreshItemNFTsDelayed(true));
      }
    }
  }, [learnSkillSubmittedStatus, skillLearnMessage]);

  const handleStatusModalClose = () => {
    setSpinnerModalText("");
    setShowStatusModal(false);
    dispatch(setLearnSkillMessage(""));
    dispatch(setLearnSkillSubmittedStatus(""));
    dispatch(setSkillToLearnId(""));
  };

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

  const handleLearn = () => {
    setLearnConfirmation(true);
  };

  const handleLearnConfirmation = () => {
    dispatch(setSkillToLearnId(skillId));
    dispatch(getLearnSkill(Number(nftTokenId)));
    setLearnConfirmation(false);
  };

  const submitLearnSkill = async () => {
    (await connect(chainId)).getAddress().then(async (address) => {
      if (address.toLowerCase() === connectedWalletAddress.toLowerCase()) {
        setLearnConfirmation(false);
        dispatch(
          submitLearnSkillTransaction({
            ...learnSkillResponse,
            connect: await connect(chainId),
          })
        );
      } else {
        dispatch(setAppMessage({ message: walletAddressMismatch, isClearToken: true }));
      }
    });
  };

  const handleClose = () => {
    setLearnConfirmation(false);
  };

  const clearSoundResponse = () => {
    setPlaySkillbookConsume(false);
  };

  const isSkillAlreadyLearned = hero.skills.some((s) => s.slug.toLowerCase() === skill.slug.toLowerCase());

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
      {playSkillbookConsume && (
        <DCXAudioPlayer
          audioUrl={`/audio/sound-effects/item/skillbook-consume`}
          soundType={SoundType.SOUND_EFFECT}
          onEnded={() => clearSoundResponse()}
        />
      )}
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
            src={`/img/api/skills/${skill.slug.toLowerCase()}.png`}
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
                  src={`/img/api/skills/icons/${skill.slug.toLowerCase()}.png`}
                  height={skillIconHeight}
                  width={skillIconWidth}
                  quality={100}
                />
              </Grid>
            </Tooltip>
          </ThemeProvider>
        )}
        {!disableOverlays && (
          <ThemeProvider theme={tooltipTheme}>
            <Tooltip
              disableFocusListener
              placement="top"
              arrow
              TransitionComponent={Zoom}
              enterTouchDelay={0}
              title={skill.skillClass}
            >
              <Grid container className={styles.skillClassSymbolContainer}>
                <Image
                  src={`/img/unity-assets/card/${skill.skillClass.toLowerCase()}_symbol.png`}
                  height={25}
                  width={25}
                  quality={100}
                />
              </Grid>
            </Tooltip>
          </ThemeProvider>
        )}
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
              {page === "learn" && (
                <Grid container direction="column" className={styles.learnActionButton}>
                  <DCXButton
                    title="LEARN"
                    height={30}
                    width={90}
                    color="blue"
                    disabled={
                      hero.learnedSkillCapacity <= learnedSkillsCount ||
                      isSkillAlreadyLearned ||
                      hero.heroClass !== skill.skillClass
                    }
                    hideArrows={true}
                    disabledLayerWidthAdjustment={10}
                    disabledLayerHeightAdjustment={6}
                    onClick={() => handleLearn()}
                  />
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
      <Modal open={learnConfirmation} onClose={handleClose} className={styles.modalMain}>
        <Grid container className={styles.modalContainer}>
          {learnConfirmation && (
            <Grid container className={styles.learnConfirmationBackground}>
              <Grid item>
                <Image src="/img/unity-assets/shared/tooltip_bg.png" height={160} width={290} quality={100} />
              </Grid>
              <Grid container className={styles.learnCostContainer}>
                <Grid container className={styles.lineContainer}>
                  <Typography component="span" className={styles.confirmText}>
                    COST TO LEARN
                  </Typography>
                  <Grid container className={styles.costContainer}>
                    <Typography component="span" className={styles.confirmText}>
                      {`${skill.dcxToLearn} USDC`}
                    </Typography>
                    {/* <Grid container className={styles.dcxLogoContainer}>
                      <DcxLogo />
                    </Grid> */}
                  </Grid>
                </Grid>
                <Grid container className={styles.helperTextContainer}>
                  <Typography component="span" className={styles.helperText}>
                    {`WHEN YOU LEARN A SKILL, THE NFT IS BURNED AND THE SKILL CAN ONLY BE USED BY THE HERO THAT
                    LEARNED IT.`}
                  </Typography>
                </Grid>
              </Grid>
              <Grid container className={styles.learnConfirmButton}>
                <DCXButton
                  title="CONFIRM"
                  height={32}
                  width={120}
                  color="blue"
                  onClick={() => handleLearnConfirmation()}
                />
              </Grid>
            </Grid>
          )}
        </Grid>
      </Modal>
      <Modal
        open={showStatusModal}
        onClose={
          learnSkillSubmittedStatus === "completed" || learnSkillSubmittedStatus === "failed"
            ? handleStatusModalClose
            : undefined
        }
        className={styles.modalMain}
      >
        <Grid container className={styles.pendingTransactionModalContainer}>
          <Image src={`/img/unity-assets/shared/tooltip_bg.png`} height={220} width={320} quality={100} />
          <Grid container direction="column" className={styles.learnSkillStatusContainer}>
            {learnSkillSubmittedStatus !== "failed" && learnSkillSubmittedStatus !== "completed" && (
              <Grid container className={styles.spinnerContainer}>
                <CircularProgress className={styles.spinner} />
              </Grid>
            )}
            <Typography component="span" className={styles.message}>
              {spinnerModalText.length > 270 ? spinnerModalText.substring(0, 270) + "..." : spinnerModalText}
            </Typography>
          </Grid>
        </Grid>
      </Modal>
    </Grid>
  );
};

export default DCXUnlearnedSkill;
