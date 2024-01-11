import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./mint.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useConnectCalls } from "@/components/web3";
import {
  getMintPrice,
  mint,
  resetMintStatus,
  selectCallBackHash,
  selectHeroMintPrice,
  selectHeroMintPriceStatus,
  selectMintStatus,
  selectMintedHeroIds,
  setCallBackHash,
} from "@/state-mgmt/mint/mintSlice";
import { useEffect, useState } from "react";
import { HeroMintOrderReq } from "@dcx/dcx-backend";
import { Status } from "@/state-mgmt/app/appTypes";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import CircularProgress from "@mui/material/CircularProgress";
import CloseButton from "@/components/close-button/close-button";
import { useRouter } from "next/router";
import { setAppMessage } from "@/state-mgmt/app/appSlice";
import { _USE_DEFAULT_CHAIN } from "@/components/web3/contractCalls";

interface Props {}

const Mint: NextPage<Props> = (props: Props) => {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const { connect } = useConnectCalls();
  const heroMintPrice = useAppSelector(selectHeroMintPrice);
  const heroMintPriceStatus = useAppSelector(selectHeroMintPriceStatus);
  const mintStatus = useAppSelector(selectMintStatus);
  const callBackHash = useAppSelector(selectCallBackHash);
  const mintedHeroIds = useAppSelector(selectMintedHeroIds);

  const [quantity, setQuantity] = useState(0);
  const [showMintStatusModal, setShowMintStatusModal] = useState(false);
  const [heroSupply, setHeroSupply] = useState(0);

  useEffect(() => {
    if (router.isReady) {
      retrieveMintPrice();
      handleGetHeroSupply();
    }
  }, [router]);

  useEffect(() => {
    if (mintStatus.includes("SUCCESSFULLY MINTED!")) {
      if (mintedHeroIds.length > 0) {
        updateNftOwner();
      }
    }
  }, [mintStatus]);

  const retrieveMintPrice = async () => {
    (await connect(undefined)).getAddress().then(async (address) => {
      if (address && address !== "") {
        dispatch(getMintPrice({ walletAddress: address, queryParams: JSON.stringify(router.query) }));
      } else {
        dispatch(setAppMessage({ message: "Failed to get wallet address", isClearToken: false }));
      }
    });
  };

  const updateNftOwner = async () => {
    (await connect(_USE_DEFAULT_CHAIN)).getNfts("heroes", mintedHeroIds);
  };

  const handleGetHeroSupply = async () => {
    (await connect(_USE_DEFAULT_CHAIN)).getMintedHeroesSupply().then(async (hs) => {
      setHeroSupply(hs);
    });
  };

  const handleQuantityClick = (selectedQuantity: number) => {
    setQuantity(selectedQuantity);
  };

  const handleMintClick = async () => {
    setShowMintStatusModal(true);
    const mintRequest: HeroMintOrderReq = {
      quantity: quantity,
      walletAddress: "",
      queryParams: JSON.stringify(router.query),
    };
    dispatch(mint({ mintRequest: mintRequest, connect: await connect(_USE_DEFAULT_CHAIN) }));
  };

  const handleGoToApp = () => {
    window.open("https://game.dragonscrossing.com/", "_blank");
  };

  const handleStatusModalClose = () => {
    setShowMintStatusModal(false);
    dispatch(resetMintStatus());
    dispatch(setCallBackHash(""));
  };

  return (
    <Grid container className={styles.container}>
      <Grid container className={styles.backgroundContainer}>
        <img src="img/unity-assets/shared/action_bg.png" width={380} height={290} />
        <Grid container className={styles.rarityBreakdownContainer}>
          <Grid container className={styles.row}>
            <Typography component="span" className={styles.rarityText}>{`MYTHIC`}</Typography>
            <Grid container className={styles.mythicLine} />
            <Typography component="span" className={styles.rarityText}>{`5%`}</Typography>
          </Grid>
          <Grid container className={styles.row}>
            <Typography component="span" className={styles.rarityText}>{`LEGENDARY`}</Typography>
            <Grid container className={styles.legendaryLine} />
            <Typography component="span" className={styles.rarityText}>{`20%`}</Typography>
          </Grid>
          <Grid container className={styles.row}>
            <Typography component="span" className={styles.rarityText}>{`RARE`}</Typography>
            <Grid container className={styles.rareLine} />
            <Typography component="span" className={styles.rarityText}>{`75%`}</Typography>
          </Grid>
        </Grid>
        <Grid container direction="row" className={styles.quantityContainer}>
          <Typography component="span" className={styles.quantityHeader}>
            Quantity:
          </Typography>
          <Grid
            container
            className={
              heroMintPrice.maxQuantity < 1
                ? styles.numberContainerDisabled
                : quantity === 1
                ? styles.numberContainerSelected
                : styles.numberContainer
            }
            onClick={() => (heroMintPrice.maxQuantity < 1 ? undefined : handleQuantityClick(1))}
          >
            <Typography component="span" className={styles.quantityText}>
              1
            </Typography>
          </Grid>
          <Grid
            container
            className={
              heroMintPrice.maxQuantity < 2
                ? styles.numberContainerDisabled
                : quantity === 2
                ? styles.numberContainerSelected
                : styles.numberContainer
            }
            onClick={() => (heroMintPrice.maxQuantity < 2 ? undefined : handleQuantityClick(2))}
          >
            <Typography component="span" className={styles.quantityText}>
              2
            </Typography>
          </Grid>
          <Grid
            container
            className={
              heroMintPrice.maxQuantity < 3
                ? styles.numberContainerDisabled
                : quantity === 3
                ? styles.numberContainerSelected
                : styles.numberContainer
            }
            onClick={() => (heroMintPrice.maxQuantity < 3 ? undefined : handleQuantityClick(3))}
          >
            <Typography component="span" className={styles.quantityText}>
              3
            </Typography>
          </Grid>
          <Grid
            container
            className={
              heroMintPrice.maxQuantity < 4
                ? styles.numberContainerDisabled
                : quantity === 4
                ? styles.numberContainerSelected
                : styles.numberContainer
            }
            onClick={() => (heroMintPrice.maxQuantity < 4 ? undefined : handleQuantityClick(4))}
          >
            <Typography component="span" className={styles.quantityText}>
              4
            </Typography>
          </Grid>
          <Grid
            container
            className={
              heroMintPrice.maxQuantity < 5
                ? styles.numberContainerDisabled
                : quantity === 5
                ? styles.numberContainerSelected
                : styles.numberContainer
            }
            onClick={() => (heroMintPrice.maxQuantity < 5 ? undefined : handleQuantityClick(5))}
          >
            <Typography component="span" className={styles.quantityText}>
              5
            </Typography>
          </Grid>
        </Grid>
        <Grid
          container
          className={
            quantity === 0 || heroMintPriceStatus.status === Status.Failed
              ? styles.buttonContainerDisabled
              : styles.buttonContainer
          }
          onClick={quantity === 0 || heroMintPriceStatus.status === Status.Failed ? undefined : handleMintClick}
        >
          <Typography component="span" className={styles.buttonText}>
            {`MINT`}
          </Typography>
        </Grid>
        <Grid container className={styles.costContainer}>
          <Typography
            component="span"
            className={styles.rarityText}
          >{`COST PER HERO: ${heroMintPrice.basePrice} USDC`}</Typography>
        </Grid>
        <Grid container className={styles.heroSupplyContainer}>
          <Typography component="span" className={styles.rarityText}>{`HEROES MINTED: ${heroSupply}/2800`}</Typography>
        </Grid>
      </Grid>
      <Modal
        open={showMintStatusModal}
        onClose={mintStatus !== "SUBMITTING MINT HERO TRANSACTION..." ? handleStatusModalClose : undefined}
        className={styles.modalMain}
      >
        <Grid container className={styles.pendingTransactionModalContainer}>
          <img src="/img/unity-assets/shared/tooltip_bg.png" height={180} width={300} />
          {mintStatus !== "SUBMITTING MINT HERO TRANSACTION..." && (
            <Grid container className={styles.closeButtonContainer}>
              <CloseButton handleClose={handleStatusModalClose} />
            </Grid>
          )}
          <Grid container direction="column" className={styles.mintHeroStatusContainer}>
            {mintStatus === "SUBMITTING MINT HERO TRANSACTION..." && (
              <Grid container className={styles.spinnerContainer}>
                <CircularProgress className={styles.spinner} />
              </Grid>
            )}
            <Typography component="p" className={styles.message}>
              {callBackHash !== ""
                ? mintStatus === "SUBMITTING MINT HERO TRANSACTION..."
                  ? "CONFIRMING TRANSACTION... TX HASH: " + callBackHash
                  : mintStatus + " TX HASH: " + callBackHash
                : mintStatus}
            </Typography>
            {mintStatus.includes("SUCCESSFULLY MINTED") && (
              <Grid container className={styles.navigateButtonContainer} onClick={handleGoToApp}>
                <Typography component="span" className={styles.buttonText}>
                  {`VIEW HERO(S) IN APP`}
                </Typography>
              </Grid>
            )}
          </Grid>
        </Grid>
      </Modal>
    </Grid>
  );
};

export default Mint;
