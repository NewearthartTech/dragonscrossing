import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./claimRewards.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useConnectCalls } from "@/components/web3";
import {
  claimDCX,
  claimItems,
  getAvailableRewards,

  //resetclaimRewardStatus,
  selectAvailableReward,
  selectAvailableRewardStatus,
  selectCallBackHash,
  selectClaimRewardStatus,
  setCallBackHash,
} from "@/state-mgmt/rewards/rewardsSlice";
import { useEffect, useState } from "react";
import { HeroMintOrderReq, ItemRewardType } from "@dcx/dcx-backend";
import { Status } from "@/state-mgmt/app/appTypes";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import CircularProgress from "@mui/material/CircularProgress";
import CloseButton from "@/components/close-button/close-button";
import { useRouter } from "next/router";
import { setAppMessage } from "@/state-mgmt/app/appSlice";
import { _USE_DEFAULT_CHAIN } from "@/components/web3/contractCalls";

interface Props {}

const ClaimReward: NextPage<Props> = (props: Props) => {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const { connect } = useConnectCalls();
  const availableReward = useAppSelector(selectAvailableReward);
  const availableRewardStatus = useAppSelector(selectAvailableRewardStatus);
  const callBackHash = useAppSelector(selectCallBackHash);

  const claimRewardStatus = useAppSelector(selectClaimRewardStatus);
  const [showclaimRewardStatusModal, setShowclaimRewardStatusModal] =
    useState(false);

  useEffect(() => {
    if (router.isReady) {
      retrieveAvailableReward();
    }
  }, [router]);

  useEffect(() => {
    if (claimRewardStatus.includes("SUCCESSFULLY MINTED!")) {
      /*
      if (mintedHeroIds.length > 0) {
        updateNftOwner();
      }
      */
    }
  }, [claimRewardStatus]);

  const retrieveAvailableReward = async () => {
    const address = await (await connect(_USE_DEFAULT_CHAIN)).getAddress();
    if (address && address !== "") {
      dispatch(
        getAvailableRewards({ walletAddress: address, rewardType: "dcxReward" })
      );
    } else {
      dispatch(
        setAppMessage({
          message: "Failed to get wallet address",
          isClearToken: false,
        })
      );
    }
  };

  const handleDCXClaimClick = async () => {
    setShowclaimRewardStatusModal(true);

    dispatch(
      claimDCX({ mintRequest: availableReward, connect: await connect(_USE_DEFAULT_CHAIN) })
    );
  };

  const handleItemsClaimClick = async () => {
    setShowclaimRewardStatusModal(true);

    dispatch(
      claimItems({ mintRequest: availableReward, connect: await connect(_USE_DEFAULT_CHAIN) })
    );
  };

  const handleStatusModalClose = () => {
    setShowclaimRewardStatusModal(false);
    //dispatch(resetclaimRewardStatus());
    dispatch(setCallBackHash(""));
  };

  const itemsCount ={
    shards : (availableReward.itemClaims||[]).filter(i=>i.slot === ItemRewardType.Shard).length,
    skills: (availableReward.itemClaims||[]).filter(i=>i.slot === ItemRewardType.Skill).length,
  };

  return (
    <Grid container className={styles.container}>
      <Grid>
        <Grid
          container
          onClick={() => /*handleGoToApp*/ {}}
          className={styles.homeContainer}
        >
          <Grid
            container
            className={styles.navigateButtonContainer}
            onClick={() => router.push("/")}
          >
            <Typography component="span" className={styles.buttonText}>
              {`BACK TO THE GAME`}
            </Typography>
          </Grid>
        </Grid>

        <Grid container direction="row">
          <Grid container className={styles.backgroundContainer}>
            <img
              src="img/unity-assets/shared/action_bg.png"
              width={380}
              height={290}
            />

            <Grid
              container
              direction="row"
              className={styles.quantityContainer}
            >
              <Typography component="span" className={styles.quantityHeader}>
                Available reward:
              </Typography>
              <Grid container className={styles.numberContainer}>
                <Typography component="span" className={styles.quantityText}>
                  ${availableReward.dcxClaim?.amount}
                </Typography>
              </Grid>
            </Grid>
            <Grid
              container
              className={
                availableReward.dcxClaim?.amount &&
                availableRewardStatus.status !== Status.Failed
                  ? styles.buttonContainer
                  : styles.buttonContainerDisabled
              }
              onClick={() =>
                availableReward.dcxClaim?.amount &&
                availableRewardStatus.status !== Status.Failed &&
                handleDCXClaimClick()
              }
            >
              <Typography component="span" className={styles.buttonText}>
                {`CLAIM USDC`}
              </Typography>
            </Grid>
          </Grid>
          <Grid container className={styles.backgroundContainer}>
            <img
              src="img/unity-assets/shared/action_bg.png"
              width={380}
              height={290}
            />

            <Grid
              container
              direction="row"
              className={styles.quantityContainer}
            >
              <Typography component="span" className={styles.quantityHeader}>
                Shards:
              </Typography>
              <Grid container className={styles.numberContainer}>
                <Typography component="span" className={styles.quantityText}>
                  {availableReward.itemClaims && itemsCount.shards}
                </Typography>
              </Grid>
            </Grid>

            <Grid
              container
              direction="row"
              className={styles.quantityContainer2}
            >
              <Typography component="span" className={styles.quantityHeader}>
                Skill books:
              </Typography>
              <Grid container className={styles.numberContainer}>
                <Typography component="span" className={styles.quantityText}>
                {availableReward.itemClaims && itemsCount.skills}
                </Typography>
              </Grid>
            </Grid>

            <Grid
              container
              className={
                (availableReward.itemClaims||[]).length > 0  && availableRewardStatus.status !== Status.Failed
                  ? styles.buttonContainer
                  : styles.buttonContainerDisabled
              }
              onClick={() =>
                (availableReward.itemClaims||[]).length > 0  &&
                availableRewardStatus.status !== Status.Failed &&
                handleItemsClaimClick()
              }
            >
              <Typography component="span" className={styles.buttonText}>
                {`CLAIM ITEMS`}
              </Typography>
            </Grid>
          </Grid>
        </Grid>
      </Grid>

      <Modal
        open={showclaimRewardStatusModal}
        onClose={
          claimRewardStatus !== "SUBMITTING CLAIM TRANSACTION..."
            ? handleStatusModalClose
            : undefined
        }
        className={styles.modalMain}
      >
        <Grid container className={styles.pendingTransactionModalContainer}>
          <img
            src="/img/unity-assets/shared/tooltip_bg.png"
            height={180}
            width={300}
          />
          {claimRewardStatus !== "SUBMITTING CLAIMS TRANSACTION..." && (
            <Grid container className={styles.closeButtonContainer}>
              <CloseButton handleClose={handleStatusModalClose} />
            </Grid>
          )}
          <Grid
            container
            direction="column"
            className={styles.mintHeroStatusContainer}
          >
            {claimRewardStatus === "SUBMITTING CLAIMS TRANSACTION..." && (
              <Grid container className={styles.spinnerContainer}>
                <CircularProgress className={styles.spinner} />
              </Grid>
            )}
            <Typography component="p" className={styles.message}>
              {callBackHash !== ""
                ? claimRewardStatus === "SUBMITTING CLAIMS TRANSACTION..."
                  ? "CONFIRMING TRANSACTION... TX HASH: " + callBackHash
                  : claimRewardStatus + " TX HASH: " + callBackHash
                : claimRewardStatus}
            </Typography>
            
          </Grid>
        </Grid>
      </Modal>
    </Grid>
  );
};

export default ClaimReward;
