import Grid from "@mui/material/Grid";
import styles from "./nftActions.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Image from "next/image";
import { ItemDto, ItemSlotTypeDto } from "@dcx/dcx-backend";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import { selectConnectedWalletAddress, selectWalletItems } from "@/state-mgmt/player/playerSlice";
import DCXButton from "@/components/dcx-button/dcx-button";
import { useEffect, useState } from "react";
import {
  submitNftActionUseTransaction,
  getNftActionUse,
  selectNftActionUseSubmittedStatus,
  selectConsumeNFTMessage,
  addToConsumedNFTTokenIds,
  selectConsumedNFTTokenIds,
  selectGetNftActionUseStatus,
  resetGetNFTActionUseStatus,
  setNFtConsumptionMessage,
  setNFTActionUseSubmittedStatus,
  selectNftActionUseResponse,
} from "@/state-mgmt/vendor/vendorSlice";
import { SoundType, Status } from "@/state-mgmt/app/appTypes";
import HeroCard from "../hero-card/hero-card";
import useWindowDimensions from "@/helpers/window-dimensions";
import {
  lgScreenWidth,
  mdScreenWidth,
  ReactPlayer,
  walletAddressMismatch,
  xsScreenWidth,
} from "@/helpers/global-constants";
import CloseButton from "../close-button/close-button";
import { useConnectCalls } from "../web3";
import { setAppMessage, setRefreshItemNFTsDelayed } from "@/state-mgmt/app/appSlice";
import CircularProgress from "@mui/material/CircularProgress";
import DcxLogo from "../dcx-logo/dcx-logo";
import DCXAudioPlayer from "../dcx-audio-player/dcx-audio-player";
import Scrollbars from "react-custom-scrollbars";
import { getSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { _USE_DEFAULT_CHAIN, _USE_SHADOW_CHAIN } from "../web3/contractCalls";

interface Props {}

const NftActions: React.FC<Props> = (props: Props) => {
  const { width, height } = useWindowDimensions();
  const dispatch = useAppDispatch();
  const { connect } = useConnectCalls();
  const connectedWalletAddress = useAppSelector(selectConnectedWalletAddress);
  const walletItems = useAppSelector(selectWalletItems);

  const nftActionUseResponse = useAppSelector(selectNftActionUseResponse);
  const nftActionUseSubmittedStatus = useAppSelector(selectNftActionUseSubmittedStatus);
  const consumeNFTMessage = useAppSelector(selectConsumeNFTMessage);

  const getNftActionUseStatus = useAppSelector(selectGetNftActionUseStatus);

  const consumedNFTTokenIds = useAppSelector(selectConsumedNFTTokenIds);
  /*
  
  
  
  const getSummonedHeroByIdStatus = useAppSelector(selectGetSummonedHeroByIdStatus);
  const summonedHero = useAppSelector(selectSummonedHero);
  
  
  const summonedHeroId = useAppSelector(selectSummonedHeroId);
*/
  const [showConfirmation, setShowConfirmation] = useState(false);
  const [showStatusModal, setShowStatusModal] = useState(false);
  const [spinnerModalText, setSpinnerModalText] = useState("");
  const [actionNfts, setActionNfts] = useState<Array<ItemDto>>();
  const [playNftConsume, setPlayNftConsume] = useState(false);

  const [firstActionNft] = actionNfts||[];


  useEffect(() => {
    if (walletItems) {
      setActionNfts(walletItems.filter((wi) => wi.slot === ItemSlotTypeDto.NftAction));
    }
  }, []);

  useEffect(() => {
    if (walletItems) {
      setActionNfts(walletItems.filter((wi) =>  wi.slot === ItemSlotTypeDto.NftAction));
    }
  }, [walletItems]);

  
  useEffect(() => {
    if (getNftActionUseStatus.status === Status.Loaded) {
      submitActionUse(firstActionNft.isDefaultChain?_USE_DEFAULT_CHAIN:_USE_SHADOW_CHAIN);
      dispatch(resetGetNFTActionUseStatus());
    }
  }, [getNftActionUseStatus]);

  
  useEffect(() => {
    if (nftActionUseSubmittedStatus === "processing") {
      setSpinnerModalText(consumeNFTMessage);
      setShowStatusModal(true);
    }
    if (nftActionUseSubmittedStatus === "failed") {
      setSpinnerModalText(consumeNFTMessage);
      setShowStatusModal(true);
    }
    if (nftActionUseSubmittedStatus === "completed") {
      dispatch(addToConsumedNFTTokenIds(nftActionUseResponse.nftTokenId!));
      setSpinnerModalText(consumeNFTMessage);
      setShowStatusModal(true);
      dispatch(setRefreshItemNFTsDelayed(true));
    }
  }, [nftActionUseSubmittedStatus, consumeNFTMessage]);


  const handleActionClick = () => {
    setShowConfirmation(true);
  };

  const handleUseConfirmation = () => {
    setShowConfirmation(false);
    dispatch(getNftActionUse(getNextTokenId()));
  };

  const getNextTokenId = (): number => {
    if (actionNfts) {
      const useableShardItems = actionNfts.filter((s) => !consumedNFTTokenIds.includes(s.nftTokenId!));
      
      if (useableShardItems.length > 0) {
        return useableShardItems[0].nftTokenId!;
      }
    }
    return -1;
  };

  const submitActionUse = async (chainId:number) => {
    (await connect(chainId)).getAddress().then(async (address) => {
      
      if (address.toLowerCase() === connectedWalletAddress.toLowerCase()) {
        dispatch(
          submitNftActionUseTransaction({
            ...nftActionUseResponse,
            connect: await connect(chainId),
          })
        );
      } else {
        dispatch(setAppMessage({ message: walletAddressMismatch, isClearToken: true }));
      }
    });
  };

  
  const handelQuestsResetDone = () => {
    setPlayNftConsume(true);
    handleStatusModalClose();
    dispatch(getSelectedHero());
  };


  const handleCloseConfirmation = () => {
    setShowConfirmation(false);
  };

  
  const handleStatusModalClose = () => {
    setSpinnerModalText("");
    setShowStatusModal(false);
    dispatch(setNFtConsumptionMessage(""));
    dispatch(setNFTActionUseSubmittedStatus(""));
  };
  

  /*
  const handleCloseSummonedHero = () => {
    dispatch(resetGetSummonedHeroByIdStatus());
    dispatch(setSummonedHeroId(-1));
  };
  */

  const clearSoundResponse = () => {
    setPlayNftConsume(false);
  };

  
  return (
    <Grid container className={styles.container}>
      {playNftConsume && (
        <DCXAudioPlayer
          audioUrl={`/audio/sound-effects/item/shard-consume`}
          soundType={SoundType.SOUND_EFFECT}
          onEnded={() => clearSoundResponse()}
        />
      )}
      <Grid container className={styles.containerBackground}>
        <Image
          src="/img/unity-assets/shared/rectangle_vertical_bg.png"
          height={width < mdScreenWidth ? 246 : 360}
          width={width < mdScreenWidth ? 360 : 540}
          quality={100}
        />
      </Grid>
      <Grid container className={styles.shadowContainer} />
      {/* Will need to get shard count from the players wallet */}
      <Typography component="span" className={styles.header}>
        {firstActionNft?`${Number(actionNfts?.length)} NFT${Number(actionNfts?.length) > 1?"S":""}
        AVAILABLE TO RESET QUESTS `:'NO ACTION NFTS AVAILABLE'}
        
      </Typography>
      <Grid container className={styles.separator} />

        {/* We will have to re do this when we have NFTS that can do other things then reset quests*/}
      {firstActionNft && <>
        <Grid container className={styles.shard}>
          <Image src={`/img/api/items/${(firstActionNft.imageSlug||firstActionNft.slug).toLowerCase()}.png`} height={279} width={241} quality={100} />
        </Grid>
        <Grid container className={styles.buttonContainer}>
          <DCXButton
            title="RESET QUESTS"
            height={40}
            width={180}
            color="red"
            arrowTopAdjustment={13}
            disabled={Number(actionNfts?.length) === 0}
            onClick={handleActionClick}
          />
        </Grid>
      </>}


      <Modal open={showConfirmation} onClose={handleCloseConfirmation} className={styles.modalMain}>
        <Grid container className={styles.summonConfirmationBackground}>
          <Grid item>
            <Image src="/img/unity-assets/shared/tooltip_bg.png" height={125} width={262} quality={100} />
          </Grid>

          
          <Grid container className={styles.summonCostContainer}>
            <Grid container className={styles.lineContainer}>
              <Typography component="span" className={styles.confirmText}>
                COST TO RESET
              </Typography>
              <Grid container className={styles.costContainer}>
                <Typography component="span" className={styles.confirmText}>
                  {/* TODO: GET SUMMONING AMOUNT FROM BACKEND */}
                  {`0 USDC`}
                </Typography>
              </Grid>
            </Grid>
          </Grid>
          <Grid container className={styles.summonConfirmButton}>
            <DCXButton
              title="CONFIRM"
              height={32}
              width={120}
              color="blue"
              onClick={() => handleUseConfirmation()}
            />
          </Grid>
        </Grid>
      </Modal>
      <Modal
        open={showStatusModal}
        onClose={nftActionUseSubmittedStatus === "failed" ? handleStatusModalClose : undefined}
        className={styles.modalMain}
      >
        <Grid container className={styles.pendingTransactionModalContainer}>
          <Image src="/img/unity-assets/shared/tooltip_bg.png" height={220} width={320} quality={100} />
          <Grid container direction="column" className={styles.summonHeroStatusContainer}>
            {nftActionUseSubmittedStatus !== "failed" && nftActionUseSubmittedStatus !== "completed" && (
              <Grid container className={styles.spinnerContainer}>
                <CircularProgress className={styles.spinner} />
              </Grid>
            )}
            <Typography component="span" className={styles.message}>
              {spinnerModalText.length > 270 ? spinnerModalText.substring(0, 270) + "..." : spinnerModalText}
            </Typography>
            {nftActionUseSubmittedStatus === "completed" && (
              <Grid container className={styles.viewHeroButtonContainer}>
                <DCXButton title="QUESTS HAVE BEEN RESET" height={40} width={170} color="blue" onClick={handelQuestsResetDone} />
                
              </Grid>
            )}
          </Grid>
        </Grid>
      </Modal>
      
    </Grid>
  );
};

export default NftActions;
