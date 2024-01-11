import Grid from "@mui/material/Grid";
import styles from "./summoning.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Image from "next/image";
import { ItemDto, ItemSlotTypeDto } from "@dcx/dcx-backend";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import {
	selectConnectedWalletAddress,
	selectWalletItems,
} from "@/state-mgmt/player/playerSlice";
import DCXButton from "@/components/dcx-button/dcx-button";
import { useEffect, useState } from "react";
import {
	getSummonHero,
	resetGetSummonHeroStatus,
	selectSummonedHero,
	selectGetSummonHeroStatus,
	selectSummonHeroSubmittedStatus,
	setSummonHeroSubmittedStatus,
	submitSummonHeroTransaction,
	selectSummonHeroResponse,
	resetGetSummonedHeroByIdStatus,
	selectGetSummonedHeroByIdStatus,
	selectConsumedNFTTokenIds,
	addToConsumedNFTTokenIds,
	getHeroById,
	selectSummonedHeroId,
	setSummonedHeroId,
	selectConsumeNFTMessage,
	setNFtConsumptionMessage,
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
import {
	setAppMessage,
	setRefreshItemNFTsDelayed,
} from "@/state-mgmt/app/appSlice";
import CircularProgress from "@mui/material/CircularProgress";
import DcxLogo from "../dcx-logo/dcx-logo";
import DCXAudioPlayer from "../dcx-audio-player/dcx-audio-player";
import Scrollbars from "react-custom-scrollbars";
import { _USE_DEFAULT_CHAIN, _USE_SHADOW_CHAIN } from "../web3/contractCalls";

interface Props {}

const Summoning: React.FC<Props> = (props: Props) => {
	const { width, height } = useWindowDimensions();
	const dispatch = useAppDispatch();
	const { connect } = useConnectCalls();
	const connectedWalletAddress = useAppSelector(selectConnectedWalletAddress);
	const walletItems = useAppSelector(selectWalletItems);
	const getSummonHeroStatus = useAppSelector(selectGetSummonHeroStatus);
	const summonHeroSubmittedStatus = useAppSelector(
		selectSummonHeroSubmittedStatus
	);
	const summonHeroResponse = useAppSelector(selectSummonHeroResponse);
	const getSummonedHeroByIdStatus = useAppSelector(
		selectGetSummonedHeroByIdStatus
	);
	const summonedHero = useAppSelector(selectSummonedHero);
	const summonedShardTokenIds = useAppSelector(selectConsumedNFTTokenIds);
	const summonHeroMessage = useAppSelector(selectConsumeNFTMessage);
	const summonedHeroId = useAppSelector(selectSummonedHeroId);

	const [showConfirmation, setShowConfirmation] = useState(false);
	const [showStatusModal, setShowStatusModal] = useState(false);
	const [spinnerModalText, setSpinnerModalText] = useState("");
	const [shards, setShards] = useState<Array<ItemDto>>();
	const [playShardConsume, setPlayShardConsume] = useState(false);

	useEffect(() => {
		if (walletItems) {
			setShards(walletItems.filter((wi) => wi.slot === ItemSlotTypeDto.Shard));
		}
	}, []);

	useEffect(() => {
		if (walletItems) {
			setShards(walletItems.filter((wi) => wi.slot === ItemSlotTypeDto.Shard));
		}
	}, [walletItems]);

	useEffect(() => {
		if (getSummonHeroStatus.status === Status.Loaded) {
			submitSummonHero(
				summonedHero.isDefaultChain ? _USE_DEFAULT_CHAIN : _USE_SHADOW_CHAIN
			);
			dispatch(resetGetSummonHeroStatus());
		}
	}, [getSummonHeroStatus]);

	useEffect(() => {
		if (summonHeroSubmittedStatus === "processing") {
			setSpinnerModalText(summonHeroMessage);
			setShowStatusModal(true);
		}
		if (summonHeroSubmittedStatus === "failed") {
			setSpinnerModalText(summonHeroMessage);
			setShowStatusModal(true);
		}
		if (summonHeroSubmittedStatus === "completed") {
			dispatch(addToConsumedNFTTokenIds(summonHeroResponse.nftTokenId!));
			setSpinnerModalText(summonHeroMessage);
			setShowStatusModal(true);
			dispatch(setRefreshItemNFTsDelayed(true));
		}
	}, [summonHeroSubmittedStatus, summonHeroMessage]);

	const handleSummonClick = () => {
		setShowConfirmation(true);
	};

	const handleSummonConfirmation = () => {
		setShowConfirmation(false);
		dispatch(getSummonHero(getNextShardId()));
	};

	const getNextShardId = (): number => {
		if (shards) {
			const useableShardItems = shards.filter(
				(s) => !summonedShardTokenIds.includes(s.nftTokenId!)
			);
			if (useableShardItems.length > 0) {
				return useableShardItems[0].nftTokenId!;
			}
		}
		return -1;
	};

	const submitSummonHero = async (chainId: string | number) => {
		(await connect(chainId)).getAddress().then(async (address) => {
			if (address.toLowerCase() === connectedWalletAddress.toLowerCase()) {
				dispatch(
					submitSummonHeroTransaction({
						...summonHeroResponse,
						connect: await connect(chainId),
					})
				);
			} else {
				dispatch(
					setAppMessage({ message: walletAddressMismatch, isClearToken: true })
				);
			}
		});
	};

	const handleGetSummonedHeroById = () => {
		setPlayShardConsume(true);
		handleStatusModalClose();
		if (summonedHeroId !== -1) {
			dispatch(getHeroById(summonedHeroId));
		}
	};

	const handleCloseConfirmation = () => {
		setShowConfirmation(false);
	};

	const handleStatusModalClose = () => {
		setSpinnerModalText("");
		setShowStatusModal(false);
		dispatch(setNFtConsumptionMessage(""));
		dispatch(setSummonHeroSubmittedStatus(""));
	};

	const handleCloseSummonedHero = () => {
		dispatch(resetGetSummonedHeroByIdStatus());
		dispatch(setSummonedHeroId(-1));
	};

	const clearSoundResponse = () => {
		setPlayShardConsume(false);
	};

	return (
		<Grid container className={styles.container}>
			{playShardConsume && (
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
				{`${Number(shards?.length)} ${
					Number(shards?.length) > 1 || Number(shards?.length) === 0
						? "SHARDS"
						: "SHARD"
				} AVAILABLE FOR SUMMONING`}
			</Typography>
			<Grid container className={styles.separator} />

			<Grid container className={styles.shard}>
				<Image
					src="/img/miscellaneous/hero-shard.png"
					height={279}
					width={241}
					quality={100}
					className={Number(shards?.length) === 0 ? styles.disabledShard : ""}
				/>
			</Grid>
			{Number(shards?.length) > 0 && (
				<Grid container className={styles.buttonContainer}>
					<DCXButton
						title="SUMMON"
						height={40}
						width={180}
						color="red"
						arrowTopAdjustment={13}
						disabled={Number(shards?.length) === 0}
						onClick={handleSummonClick}
					/>
				</Grid>
			)}
			{Number(shards?.length) === 0 && (
				<Typography component="span" className={styles.noShards}>
					NO SHARDS AVAILABLE
				</Typography>
			)}
			<Modal
				open={showConfirmation}
				onClose={handleCloseConfirmation}
				className={styles.modalMain}
			>
				<Grid container className={styles.summonConfirmationBackground}>
					<Grid item>
						<Image
							src="/img/unity-assets/shared/tooltip_bg.png"
							height={125}
							width={262}
							quality={100}
						/>
					</Grid>
					<Grid container className={styles.summonCostContainer}>
						<Grid container className={styles.lineContainer}>
							<Typography component="span" className={styles.confirmText}>
								COST TO SUMMON
							</Typography>
							<Grid container className={styles.costContainer}>
								<Typography component="span" className={styles.confirmText}>
									{/* TODO: GET SUMMONING AMOUNT FROM BACKEND */}
									{`10 USDC`}
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
							onClick={() => handleSummonConfirmation()}
						/>
					</Grid>
				</Grid>
			</Modal>
			<Modal
				open={showStatusModal}
				onClose={
					summonHeroSubmittedStatus === "failed"
						? handleStatusModalClose
						: undefined
				}
				className={styles.modalMain}
			>
				<Grid container className={styles.pendingTransactionModalContainer}>
					<Image
						src="/img/unity-assets/shared/tooltip_bg.png"
						height={220}
						width={320}
						quality={100}
					/>
					<Grid
						container
						direction="column"
						className={styles.summonHeroStatusContainer}
					>
						{summonHeroSubmittedStatus !== "failed" &&
							summonHeroSubmittedStatus !== "completed" && (
								<Grid container className={styles.spinnerContainer}>
									<CircularProgress className={styles.spinner} />
								</Grid>
							)}
						<Typography component="span" className={styles.message}>
							{spinnerModalText.length > 270
								? spinnerModalText.substring(0, 270) + "..."
								: spinnerModalText}
						</Typography>
						{summonHeroSubmittedStatus === "completed" && (
							<Grid container className={styles.viewHeroButtonContainer}>
								<DCXButton
									title="VIEW HERO"
									height={40}
									width={140}
									color="blue"
									onClick={handleGetSummonedHeroById}
								/>
							</Grid>
						)}
					</Grid>
				</Grid>
			</Modal>
			<Modal
				open={getSummonedHeroByIdStatus.status === Status.Loaded}
				onClose={handleCloseSummonedHero}
				className={styles.modalMain}
			>
				<Grid container className={styles.summonedHeroModalParentContainer}>
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
						<Grid container className={styles.summonedHeroModalContainer}>
							{width > xsScreenWidth && (
								<Grid container className={styles.summonedHeroBg}>
									<Image
										src="/img/unity-assets/shared/action_bg_vertical.png"
										height={
											width <= xsScreenWidth
												? 498
												: width <= lgScreenWidth
												? 765
												: 892
										}
										width={
											width <= xsScreenWidth
												? 390
												: width <= lgScreenWidth
												? 599
												: 698
										}
										quality={100}
									/>
								</Grid>
							)}
							{width > xsScreenWidth && (
								<Grid container className={styles.summonedHeroBgVideo}>
									<ReactPlayer
										playing={true}
										url={`video/summon-backgrounds/${summonedHero.rarity.toLowerCase()}-rotation.mp4`}
										controls={false}
										playsinline={true}
										muted={true}
										loop={true}
										height={width <= lgScreenWidth ? 731 : 854}
									/>
								</Grid>
							)}
							<Grid container className={styles.closeButtonContainer}>
								<CloseButton handleClose={handleCloseSummonedHero} />
							</Grid>
							<Grid container className={styles.cardContainer}>
								<HeroCard hero={summonedHero} disableSelectHeroButton={true} />
							</Grid>
						</Grid>
					</Scrollbars>
				</Grid>
			</Modal>
		</Grid>
	);
};

export default Summoning;
