import Grid from "@mui/material/Grid";
import styles from "./camp-modal.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Modal from "@mui/material/Modal";
import { useEffect, useState } from "react";
import {
  selectDisplayCampModal,
  setAppMessage,
  setDisplayCampModal,
  setRefreshHeroNFTs,
  setRefreshItemNFTsDelayed,
} from "@/state-mgmt/app/appSlice";
import Image from "next/image";
import { walletAddressMismatch } from "@/helpers/global-constants";
import {
  getCampOrdersStatus,
  selectCampOrders,
  selectCampOrdersStatus,
  selectClaimDcxSubmittedStatus,
  selectClaimNftSubmittedStatus,
  selectSubmittedTransactionOrderId,
  setClaimDcxSubmittedStatus,
  setClaimNftSubmittedStatus,
  setSubmittedTransactionOrderId,
  submitClaimDcxTransaction,
  submitClaimNftTransaction,
} from "@/state-mgmt/camp/campSlice";
import { AuthApi, ClaimDcxOrder, SecuredNFTsOrder } from "@dcx/dcx-backend";
import { apiConfig } from "../hoc/verification";
import { resetSelectedHeroStatus } from "@/state-mgmt/hero/heroSlice";
import { clearGameState } from "@/state-mgmt/game-state/gameStateSlice";
import { useAuthentication } from "@/components/auth";
import { Status } from "@/state-mgmt/app/appTypes";
import { selectConnectedWalletAddress } from "@/state-mgmt/player/playerSlice";
import { CircularProgress, Typography } from "@mui/material";
import { useConnectCalls } from "../web3";

interface Props {}

const CampModal: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const { updateJWT } = useAuthentication();
  const displayCampModal = useAppSelector(selectDisplayCampModal);
  const campOrders = useAppSelector(selectCampOrders);
  const campOrdersStatus = useAppSelector(selectCampOrdersStatus);
  const connectedWalletAddress = useAppSelector(selectConnectedWalletAddress);
  const submittedTransactionOrderId = useAppSelector(selectSubmittedTransactionOrderId);
  const claimDcxSubmittedStatus = useAppSelector(selectClaimDcxSubmittedStatus);
  const claimNftSubmittedStatus = useAppSelector(selectClaimNftSubmittedStatus);
  const { connect } = useConnectCalls();

  const [isPolling, setPolling] = useState(false);
  const [showStatusModal, setShowStatusModal] = useState(false);
  const [spinnerModalText, setSpinnerModalText] = useState("");
  const [isProcessingOrders, setProcessingOrders] = useState(false);

  useEffect(() => {
    if (displayCampModal) {
      dispatch(getCampOrdersStatus());
    }
  }, []);

  useEffect(() => {
    let isOrderSubmitted = false;
    // Grab the first order that is not complete and use that.
    // When the order completes this useEffect should be triggered and we get the next order that is incomplete
    if (campOrdersStatus.status === Status.Loaded) {
      if (campOrders.claimDcxOrders && campOrders.claimDcxOrders.length > 0) {
        isOrderSubmitted = true;
        setProcessingOrders(true);
        if (submittedTransactionOrderId !== "") {
          const submittedOrder = campOrders.claimDcxOrders.find((o) => o.id === submittedTransactionOrderId);
          if (!submittedOrder) {
            const order = campOrders.claimDcxOrders.find((o) => !o.isCompleted);
            if (order) {
              dispatch(setSubmittedTransactionOrderId(order.id));
              handleSubmitClaimDcxTransaction(order);
              setPolling(true);
            } else {
              dispatch(setSubmittedTransactionOrderId(""));
            }
          }
        } else {
          const order = campOrders.claimDcxOrders.find((o) => !o.isCompleted);
          if (order) {
            dispatch(setSubmittedTransactionOrderId(order.id));
            handleSubmitClaimDcxTransaction(order);
            setPolling(true);
          }
        }
      }
      if (campOrders.nftOrders && campOrders.nftOrders.length > 0 && !isOrderSubmitted) {
        isOrderSubmitted = true;
        setProcessingOrders(true);
        if (submittedTransactionOrderId !== "") {
          const submittedOrder = campOrders.nftOrders.find((o) => o.id === submittedTransactionOrderId);
          if (!submittedOrder) {
            const order = campOrders.nftOrders.find((o) => !o.isCompleted);
            if (order) {
              dispatch(setSubmittedTransactionOrderId(order.id));
              handleSubmitClaimNftTransaction(order);
              setPolling(true);
            } else {
              dispatch(setSubmittedTransactionOrderId(""));
            }
          }
        } else {
          const order = campOrders.nftOrders.find((o) => !o.isCompleted);
          if (order) {
            dispatch(setSubmittedTransactionOrderId(order.id));
            handleSubmitClaimNftTransaction(order);
            setPolling(true);
          }
        }
      }
      if (!isOrderSubmitted) {
        // Stop polling and close transaction modal because there are no incompleted orders
        setPolling(false);
        setProcessingOrders(false);
        if (displayCampModal) {
          dispatch(setClaimDcxSubmittedStatus(""));
          dispatch(setClaimNftSubmittedStatus(""));
          dispatch(setRefreshItemNFTsDelayed(true));
          dispatch(setDisplayCampModal(false));
          handleGoToHeroSelect();
        }
        setSpinnerModalText("");
        setShowStatusModal(false);
      }
    }
  }, [campOrdersStatus]);

  useEffect(() => {
    if (isPolling) {
      let timer1 = setInterval(function () {
        dispatch(getCampOrdersStatus());
      }, 2000);
      return () => {
        clearInterval(timer1);
      };
    }
  }, [isPolling]);

  useEffect(() => {
    if (claimDcxSubmittedStatus === "submitting") {
      setSpinnerModalText("SUBMITTING CLAIM DCX TRANSACTION");
      setShowStatusModal(true);
    }
    if (claimDcxSubmittedStatus === "processing") {
      setSpinnerModalText("PROCESSING CLAIM DCX TRANSACTION");
      setShowStatusModal(true);
      setPolling(true);
    }
    if (claimDcxSubmittedStatus === "failed") {
      setSpinnerModalText("FAILED TO COMPLETE CLAIM DCX TRANSACTION");
      setShowStatusModal(true);
      setProcessingOrders(false);
      setPolling(false);
    }
    if (claimDcxSubmittedStatus === "completed") {
      setSpinnerModalText("SUCCESSFULLY CLAIMED DCX");
      setShowStatusModal(true);
      setProcessingOrders(false);
      setPolling(false);
    }
  }, [claimDcxSubmittedStatus]);

  useEffect(() => {
    if (claimNftSubmittedStatus === "submitting") {
      setSpinnerModalText("SUBMITTING CLAIM NFT TRANSACTION");
      setShowStatusModal(true);
    }
    if (claimNftSubmittedStatus === "processing") {
      setSpinnerModalText("PROCESSING CLAIM NFT TRANSACTION");
      setShowStatusModal(true);
      setPolling(true);
    }
    if (claimNftSubmittedStatus === "failed") {
      setSpinnerModalText("FAILED TO COMPLETE CLAIM NFT TRANSACTION");
      setProcessingOrders(false);
      setPolling(false);
      setShowStatusModal(true);
    }
    if (claimNftSubmittedStatus === "completed") {
      setSpinnerModalText("SUCCESSFULLY CLAIMED NFT");
      setShowStatusModal(true);
      setProcessingOrders(false);
      setPolling(false);
    }
  }, [claimNftSubmittedStatus]);

  const handleGoToHeroSelect = async () => {
    // Call on endpoint to get a new token without the selectedHeroId
    try {
      const { data: userWithoutHero } = await new AuthApi(apiConfig).apiAuthRemoveHeroClaimGet();
      updateJWT(userWithoutHero);
      dispatch(resetSelectedHeroStatus());
      dispatch(clearGameState());
      dispatch(setRefreshHeroNFTs(true));
    } catch (err) {
      //brandon-todo: Please handle the error
      console.log(`failed to select Hero :${err}`);
    }
  };

  const handleSubmitClaimDcxTransaction = async (order: ClaimDcxOrder) => {
    (await connect(order.chainId!.toString())).getAddress().then(async (address) => {
      if (address.toLowerCase() === connectedWalletAddress.toLowerCase()) {
        dispatch(submitClaimDcxTransaction({ ...order, connect: await connect(order.chainId) }));
      } else {
        dispatch(setAppMessage({ message: walletAddressMismatch, isClearToken: true }));
      }
    });
  };

  const handleSubmitClaimNftTransaction = async (order: SecuredNFTsOrder) => {
    (await connect(order.chainId!.toString())).getAddress().then(async (address) => {
      if (address.toLowerCase() === connectedWalletAddress.toLowerCase()) {
        dispatch(submitClaimNftTransaction({ ...order, connect: await connect(order.chainId) }));
      } else {
        dispatch(setAppMessage({ message: walletAddressMismatch, isClearToken: true }));
      }
    });
  };

  const handleStatusModalClose = () => {
    dispatch(setClaimDcxSubmittedStatus(""));
    dispatch(setClaimNftSubmittedStatus(""));
    setShowStatusModal(false);
  };

  return (
    <Modal
      open={showStatusModal}
      onClose={isProcessingOrders ? undefined : handleStatusModalClose}
      className={styles.modalMain}
    >
      <Grid container className={styles.pendingTransactionModalContainer}>
        <Image src="/img/unity-assets/shared/tooltip_bg.png" height={110} width={168} quality={100} />
        <Grid container direction="column" className={styles.claimStatusContainer}>
          {(claimDcxSubmittedStatus === "submitting" ||
            claimDcxSubmittedStatus === "processing" ||
            claimNftSubmittedStatus === "submitting" ||
            claimNftSubmittedStatus === "processing") &&
            campOrdersStatus.status !== Status.Failed && (
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
  );
};

export default CampModal;
