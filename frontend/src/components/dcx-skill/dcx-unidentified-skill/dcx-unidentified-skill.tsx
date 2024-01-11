import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import styles from "./dcx-unidentified-skill.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import useWindowDimensions from "@/helpers/window-dimensions";
import Image from "next/image";
import { useEffect, useRef, useState } from "react";
import { mdScreenWidth, walletAddressMismatch } from "@/helpers/global-constants";
import { selectConnectedWalletAddress, selectDcxBalance } from "@/state-mgmt/player/playerSlice";
import { SkillItem, UnidentifiedHeroSkill } from "@dcx/dcx-backend";
import DCXButton from "@/components/dcx-button/dcx-button";
import DcxLogo from "@/components/dcx-logo/dcx-logo";
import {
  getIdentifySkill,
  resetGetIdentifySkillStatus,
  selectGetIdentifySkillStatus,
  selectIdentifySkillResponse,
  selectIdentifySkillSubmittedStatus,
  selectSkillIdentifyMessage,
  selectSkillToIdentifyId,
  setIdentifySkillMessage,
  setIdentifySkillSubmittedStatus,
  setSkillToIdentifyId,
  submitIdentifySkillTransaction,
} from "@/state-mgmt/vendor/vendorSlice";
import { useConnectCalls } from "@/components/web3";
import { setAppMessage, setRefreshItemNFTsDelayed } from "@/state-mgmt/app/appSlice";
import { SoundType, Status } from "@/state-mgmt/app/appTypes";
import CircularProgress from "@mui/material/CircularProgress";
import DCXAudioPlayer from "@/components/dcx-audio-player/dcx-audio-player";
import { _USE_DEFAULT_CHAIN, _USE_SHADOW_CHAIN } from "@/components/web3/contractCalls";

interface Props {
  skillItem: SkillItem;
  small?: boolean;
  disableActions?: boolean;
  disableOverlays?: boolean;
  isClickable?: boolean;
}

const DCXUnidentifiedSkill: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const { connect } = useConnectCalls();
  const connectedWalletAddress = useAppSelector(selectConnectedWalletAddress);
  const dcxBalance = useAppSelector(selectDcxBalance);
  const identifySkillResponse = useAppSelector(selectIdentifySkillResponse);
  const getIdentifySkillStatus = useAppSelector(selectGetIdentifySkillStatus);
  const identifySkillSubmittedStatus = useAppSelector(selectIdentifySkillSubmittedStatus);
  const skillToIdentifyId = useAppSelector(selectSkillToIdentifyId);
  const skillIdentifyMessage = useAppSelector(selectSkillIdentifyMessage);

  const { width, height } = useWindowDimensions();
  const { skillItem, small, disableActions, disableOverlays, isClickable } = props;
  const ref = useRef<any>(null);

  const [displayActions, setDisplayActions] = useState(false);
  const [identifyConfirmation, setIdentifyConfirmation] = useState(false);
  const [showStatusModal, setShowStatusModal] = useState(false);
  const [spinnerModalText, setSpinnerModalText] = useState("");
  const [playSkillbookIdentify, setPlaySkillbookIdentify] = useState(false);

  const containerWidth = small ? 74.2 : 215;
  const skillImageWidth = small ? 71.5 : 207;
  let mainContainerHeight = small ? 84.5 : 245;
  const imageContainerHeight = small ? 63 : 180;
  const skillImageHeight = small ? 60.5 : 174;

  // If a player clicks outside of the Skill Image then close the menu
  useEffect(() => {
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      // Unbind the event listener on clean up
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [ref]);

  useEffect(() => {
    if (skillItem.id === skillToIdentifyId) {
      if (getIdentifySkillStatus.status === Status.Loaded) {
        submitIdentifySkill(skillItem.isDefaultChain?_USE_DEFAULT_CHAIN:_USE_SHADOW_CHAIN);
        dispatch(resetGetIdentifySkillStatus());
      }
    }
  }, [getIdentifySkillStatus]);

  useEffect(() => {
    if (skillItem.id === skillToIdentifyId) {
      if (identifySkillSubmittedStatus === "processing") {
        setSpinnerModalText(skillIdentifyMessage);
        setShowStatusModal(true);
      }
      if (identifySkillSubmittedStatus === "failed") {
        setSpinnerModalText(skillIdentifyMessage);
        setShowStatusModal(true);
      }
      if (identifySkillSubmittedStatus === "completed") {
        setPlaySkillbookIdentify(true);
        setSpinnerModalText(skillIdentifyMessage);
        setShowStatusModal(true);
        dispatch(setRefreshItemNFTsDelayed(true));
      }
    }
  }, [identifySkillSubmittedStatus, skillIdentifyMessage]);

  const handleStatusModalClose = () => {
    setSpinnerModalText("");
    setShowStatusModal(false);
    dispatch(setIdentifySkillMessage(""));
    dispatch(setIdentifySkillSubmittedStatus(""));
    dispatch(setSkillToIdentifyId(""));
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

  const handleIdentify = () => {
    setIdentifyConfirmation(true);
  };

  const handleIdentifyConfirmation = () => {
    dispatch(setSkillToIdentifyId(skillItem.id));
    dispatch(getIdentifySkill(Number(skillItem.nftTokenId)));
    setIdentifyConfirmation(false);
  };

  const submitIdentifySkill = async (chainId:number) => {
    (await connect(chainId)).getAddress().then(async (address) => {
      if (address.toLowerCase() === connectedWalletAddress.toLowerCase()) {
        setIdentifyConfirmation(false);
        dispatch(
          submitIdentifySkillTransaction({
            ...identifySkillResponse,
            connect: await connect(chainId),
          })
        );
      } else {
        dispatch(setAppMessage({ message: walletAddressMismatch, isClearToken: true }));
      }
    });
  };

  const handleClose = () => {
    setIdentifyConfirmation(false);
  };

  const getUnidentifiedSkill = () => {
    return skillItem.skill as UnidentifiedHeroSkill;
  };

  const clearSoundResponse = () => {
    setPlaySkillbookIdentify(false);
  };

  return (
    <Grid
      container
      className={styles.main}
      style={{
        cursor: isClickable ? "pointer" : undefined,
      }}
    >
      {playSkillbookIdentify && (
        <DCXAudioPlayer
          audioUrl={`/audio/sound-effects/item/skillbook-identify`}
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
            src={`/img/api/items/unidentified-skill.png`}
            height={skillImageHeight}
            width={skillImageWidth}
            quality={100}
            className={isClickable ? styles.hoverPointer : undefined}
          />
        </Grid>
        {!disableOverlays && (
          <Grid container className={styles.skillNameBackground} style={{ width: skillImageWidth }} />
        )}
        {!disableOverlays && (
          <Typography component="span" className={styles.skillNameText}>
            {"UNIDENTIFIED"}
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
              <Grid container direction="column" className={styles.identifyActionButton}>
                <DCXButton
                  title="IDENTIFY"
                  height={30}
                  width={90}
                  color="blue"
                  hideArrows={true}
                  disabledLayerWidthAdjustment={10}
                  disabledLayerHeightAdjustment={6}
                  onClick={() => handleIdentify()}
                />
              </Grid>
            </Grid>
          </Grid>
        )}
      </Grid>
      <Modal open={identifyConfirmation} onClose={handleClose} className={styles.modalMain}>
        <Grid container className={styles.modalContainer}>
          {identifyConfirmation && (
            <Grid container className={styles.identifyConfirmationBackground}>
              <Grid item>
                <Image src="/img/unity-assets/shared/tooltip_bg.png" height={125} width={262.5} quality={100} />
              </Grid>
              <Grid container className={styles.identifyCostContainer}>
                <Typography component="span" className={styles.confirmText}>
                  COST TO IDENTIFY
                </Typography>
                <Grid container className={styles.costContainer}>
                  <Typography component="span" className={styles.confirmText}>
                    {`${getUnidentifiedSkill().dcxToIdentify} USDC ${skillItem.isDefaultChain?"":"DFK"}`}
                  </Typography>
                  {/* <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid> */}
                </Grid>
              </Grid>
              <Grid container className={styles.identifyConfirmButton}>
                <DCXButton
                  title="CONFIRM"
                  height={32}
                  width={120}
                  color="blue"
                  onClick={() => handleIdentifyConfirmation()}
                />
              </Grid>
            </Grid>
          )}
        </Grid>
      </Modal>
      <Modal
        open={showStatusModal}
        onClose={
          identifySkillSubmittedStatus === "completed" || identifySkillSubmittedStatus === "failed"
            ? handleStatusModalClose
            : undefined
        }
        className={styles.modalMain}
      >
        <Grid container className={styles.pendingTransactionModalContainer}>
          <Image src={`/img/unity-assets/shared/tooltip_bg.png`} height={220} width={320} quality={100} />
          <Grid container direction="column" className={styles.identifySkillStatusContainer}>
            {identifySkillSubmittedStatus !== "failed" && identifySkillSubmittedStatus !== "completed" && (
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

export default DCXUnidentifiedSkill;
