import Grid from "@mui/material/Grid";
import Modal from "@mui/material/Modal";
import styles from "./register.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Image from "next/image";
import CloseButton from "../close-button/close-button";
import Typography from "@mui/material/Typography";
import { unixToLocalDateTime } from "@/helpers/shared-functions";
import InfoIcon from "@mui/icons-material/Info";
import { ThemeProvider } from "@mui/material/styles";
import Tooltip from "@mui/material/Tooltip";
import { tooltipTheme, walletAddressMismatch } from "@/helpers/global-constants";
import Zoom from "@mui/material/Zoom";
import DcxLogo from "../dcx-logo/dcx-logo";
import DCXButton from "../dcx-button/dcx-button";
import {
  getSeasonSignUpOrder,
  resetGetSignUpToSeasonStatus,
  selectGetSignUpToSeasonStatus,
  selectHeroToRegister,
  selectOpenSeasons,
  selectSeasonSignUpSubmittedStatus,
  selectShowRegistrationModal,
  selectSignUpToSeasonResponse,
  setHeroToRegister,
  setShowRegistrationModal,
  submitSeasonSignUpRequest,
} from "@/state-mgmt/season/seasonSlice";
import { useEffect, useState } from "react";
import { AuthApi, Season } from "@dcx/dcx-backend";
import { useConnectCalls } from "../web3";
import { selectConnectedWalletAddress } from "@/state-mgmt/player/playerSlice";
import { setAppMessage, setRefreshHeroNFTs, setRefreshHeroNFTsDelayed } from "@/state-mgmt/app/appSlice";
import { Status } from "@/state-mgmt/app/appTypes";
import CircularProgress from "@mui/material/CircularProgress";
import { apiConfig } from "../hoc/verification";
import { useAuthentication } from "../auth";
import { resetSelectedHeroStatus } from "@/state-mgmt/hero/heroSlice";
import { clearGameState } from "@/state-mgmt/game-state/gameStateSlice";
import { _USE_DEFAULT_CHAIN, _USE_SHADOW_CHAIN } from "../web3/contractCalls";

interface Props {}

const Register: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const { connect } = useConnectCalls();

  const showRegistrationModal = useAppSelector(selectShowRegistrationModal);
  const heroToRegister = useAppSelector(selectHeroToRegister);
  const openSeasons = useAppSelector(selectOpenSeasons);
  const signUpToSeasonResponse = useAppSelector(selectSignUpToSeasonResponse);
  const connectedWalletAddress = useAppSelector(selectConnectedWalletAddress);
  const getSignUpToSeasonStatus = useAppSelector(selectGetSignUpToSeasonStatus);
  const seasonSignUpSubmittedStatus = useAppSelector(selectSeasonSignUpSubmittedStatus);

  const [seasonWithOpenRegistration, setSeasonWithOpenRegistration] = useState<Season>();
  const [isPolling, setPolling] = useState(false);
  const [showStatusModal, setShowStatusModal] = useState(false);
  const [spinnerModalText, setSpinnerModalText] = useState("");

  const { updateJWT } = useAuthentication();

  useEffect(() => {
    if (showRegistrationModal) {
      const openRegistrationSeason = openSeasons.find((s) => s.isRegistrationOpen);
      if (openRegistrationSeason && openRegistrationSeason.isRegistrationOpen) {
        setSeasonWithOpenRegistration(openRegistrationSeason);
        dispatch(getSeasonSignUpOrder(openRegistrationSeason.seasonId));
      }
    }
  }, [showRegistrationModal]);

  useEffect(() => {
    if (isPolling && seasonWithOpenRegistration) {
      let timer1 = setInterval(function () {
        dispatch(getSeasonSignUpOrder(seasonWithOpenRegistration.seasonId));
      }, 2000);
      return () => {
        clearInterval(timer1);
      };
    }
  }, [isPolling]);

  useEffect(() => {
    if (isPolling) {
      if (getSignUpToSeasonStatus.status === Status.Loaded) {
        if (signUpToSeasonResponse.isCompleted) {
          setPolling(false);
          setSpinnerModalText(
            `SUCCESSFULLY REGISTERED HERO ID: ${heroToRegister?.id} TO SEASON "${seasonWithOpenRegistration?.seasonName}" - TX HASH: ${signUpToSeasonResponse.fulfillmentTxnHash}`
          );
          dispatch(setRefreshHeroNFTsDelayed(true));
        } else {
          if (signUpToSeasonResponse.fulfillmentTxnHash && signUpToSeasonResponse.fulfillmentTxnHash !== "") {
            setSpinnerModalText(
              `CONFIRMING SEASON REGISTRATION - TX HASH: ${signUpToSeasonResponse.fulfillmentTxnHash}`
            );
          } else {
            setSpinnerModalText(`PROCESSING SEASON REGISTRATION TRANSACTION`);
          }
        }
      }
      if (getSignUpToSeasonStatus.status === Status.Failed) {
        setPolling(false);
        setSpinnerModalText(
          `FAILED TO REGISTER HERO ID: ${heroToRegister?.id} TO SEASON ${
            seasonWithOpenRegistration?.seasonName
          }. PLEASE TRY AGAIN. ${
            signUpToSeasonResponse.fulfillmentTxnHash ? "TX HASH: " + signUpToSeasonResponse.fulfillmentTxnHash : ""
          }`
        );
      }
    }
  }, [getSignUpToSeasonStatus]);

  useEffect(() => {
    if (seasonSignUpSubmittedStatus.status === Status.Failed) {
      setPolling(false);
      setSpinnerModalText(
        `FAILED TO REGISTER HERO ID: ${heroToRegister?.id} TO SEASON ${
          seasonWithOpenRegistration?.seasonName
        }. PLEASE TRY AGAIN. ${
          signUpToSeasonResponse.fulfillmentTxnHash ? "TX HASH: " + signUpToSeasonResponse.fulfillmentTxnHash : ""
        }`
      );
    }
  }, [seasonSignUpSubmittedStatus]);

  const handleRegisterClick = async () => {
    if (
      seasonWithOpenRegistration &&
      seasonWithOpenRegistration.isRegistrationOpen &&
      getSignUpToSeasonStatus.status === Status.Loaded
    ) {
      (await connect(undefined)).getAddress().then(async (address) => {
        if (address.toLowerCase() === connectedWalletAddress.toLowerCase()) {
          setShowStatusModal(true);
          dispatch(
            submitSeasonSignUpRequest({
              ...signUpToSeasonResponse,
              connect: await connect(heroToRegister!.isDefaultChain?_USE_DEFAULT_CHAIN:_USE_SHADOW_CHAIN),
            })
          );
          setPolling(true);
          setSpinnerModalText(`PROCESSING SEASON REGISTRATION TRANSACTION`);
        } else {
          dispatch(setAppMessage({ message: walletAddressMismatch, isClearToken: true }));
        }
      });
    }
  };

  const handleClose = async () => {
    setPolling(false);
    setShowStatusModal(false);
    dispatch(setHeroToRegister(undefined));
    dispatch(resetGetSignUpToSeasonStatus());
    dispatch(setShowRegistrationModal(false));

    /*
    const { data: userWithoutHero } = await new AuthApi(apiConfig).apiAuthRemoveHeroClaimGet();
    updateJWT(userWithoutHero);
    dispatch(resetSelectedHeroStatus());
    dispatch(clearGameState());
    dispatch(setRefreshHeroNFTs(true));
    */

    location.replace(location.href);

  };

  return (
    <Modal open={showRegistrationModal} onClose={handleClose} className={styles.modalMain}>
      <Grid container className={styles.container}>
        {seasonWithOpenRegistration && seasonWithOpenRegistration.isRegistrationOpen && (
          <Grid container className={styles.containerBackground}>
            <Image src="/img/unity-assets/shared/action_bg.png" height={308} width={390} quality={100} />
            <Grid container className={styles.headerContainer}>
              <Typography component="span" className={styles.header}>
                {`SEASON REGISTRATION`}
              </Typography>
            </Grid>
            <Grid container className={styles.closeButtonContainer}>
              <CloseButton handleClose={handleClose} />
            </Grid>
            <ThemeProvider theme={tooltipTheme}>
              <Tooltip
                disableFocusListener
                placement="top"
                arrow
                enterTouchDelay={0}
                TransitionComponent={Zoom}
                title={"ALL TIMES ARE DISPLAYED IN COORDINATED UNIVERSAL TIME (UTC)"}
              >
                <Grid container className={styles.infoContainer}>
                  <InfoIcon className={styles.infoIcon} />
                </Grid>
              </Tooltip>
            </ThemeProvider>
            <Grid container className={styles.divider} />
            <Grid container className={styles.contentContainer}>
              <Grid container className={styles.row}>
                <Typography component="span" className={styles.rowText}>
                  {`SEASON NAME:`}
                </Typography>
                <Typography component="span" className={styles.rowText}>
                  {`${seasonWithOpenRegistration.seasonName}`}
                </Typography>
              </Grid>
              <Grid container className={styles.row}>
                <Typography component="span" className={styles.rowText}>
                  {`REGISTRATION STATUS:`}
                </Typography>
                <Typography
                  component="span"
                  className={seasonWithOpenRegistration.isRegistrationOpen ? styles.openText : styles.closedText}
                >
                  {`${seasonWithOpenRegistration.isRegistrationOpen ? "OPEN" : "CLOSED"}`}
                </Typography>
              </Grid>
              <Grid container className={styles.row}>
                <Typography component="span" className={styles.rowText}>
                  {`REGISTRATION OPEN:`}
                </Typography>
                <Typography component="span" className={styles.rowText}>
                  {`${unixToLocalDateTime(seasonWithOpenRegistration.registrationOpenDate)}`}
                </Typography>
              </Grid>
              <Grid container className={styles.row}>
                <Typography component="span" className={styles.rowText}>
                  {`REGISTRATION CLOSE:`}
                </Typography>
                <Typography component="span" className={styles.rowText}>
                  {`${unixToLocalDateTime(seasonWithOpenRegistration.registrationCloseDate)}`}
                </Typography>
              </Grid>
              <Grid container className={styles.row}>
                <Typography component="span" className={styles.rowText}>
                  {`SEASON OPEN:`}
                </Typography>
                <Typography component="span" className={styles.rowText}>
                  {`${unixToLocalDateTime(seasonWithOpenRegistration.seasonOpenDate)}`}
                </Typography>
              </Grid>
              <Grid container className={styles.row}>
                <Typography component="span" className={styles.rowText}>
                  {`SEASON CLOSE:`}
                </Typography>
                <Typography component="span" className={styles.rowText}>
                  {`${unixToLocalDateTime(seasonWithOpenRegistration.seasonCloseDate)}`}
                </Typography>
              </Grid>
              <Grid container className={styles.row}>
                <Typography component="span" className={styles.rowText}>
                  {`REGISTRATION COST:`}
                </Typography>
                <Grid container className={styles.costContainer}>
                  <Typography component="span" className={styles.rowText}>
                    {heroToRegister?.id === 999999991? '0 USDC':`${seasonWithOpenRegistration.dcxCostToRegister} USDC`}
                  </Typography>
                  {/* <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid> */}
                </Grid>
              </Grid>
              <Grid container className={styles.buttonContainer}>
                <DCXButton title="REGISTER" height={42} width={144} color="red" onClick={handleRegisterClick} />
              </Grid>
            </Grid>
          </Grid>
        )}
        <Modal
          open={showStatusModal}
          onClose={
            spinnerModalText.indexOf("SUCCESS") !== -1 || spinnerModalText.indexOf("FAILED") !== -1
              ? handleClose
              : undefined
          }
          className={styles.modalMain}
        >
          <Grid container className={styles.pendingTransactionModalContainer}>
            <Image src="/img/unity-assets/shared/tooltip_bg.png" height={170} width={280} quality={100} />
            <Grid container direction="column" className={styles.registrationStatusContainer}>
              {spinnerModalText.indexOf("SUCCESS") === -1 && spinnerModalText.indexOf("FAILED") === -1 && (
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
    </Modal>
  );
};

export default Register;
