import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./vendor.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useEffect, useState } from "react";
import { selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import Image from "next/image";
import { mdScreenWidth, xsScreenWidth } from "@/helpers/global-constants";
import useWindowDimensions from "@/helpers/window-dimensions";
import { SoundType, VendorType } from "@/state-mgmt/app/appTypes";
import { useRouter } from "next/router";
import DCXButton from "../dcx-button/dcx-button";
import Skills from "../skills/skills";
import LevelUp from "../level-up/level-up";
import {
	selectGameState,
	updateGameState,
} from "@/state-mgmt/game-state/gameStateSlice";
import BlacksmithItems from "../blacksmith-items/blacksmith-items";
import InventorySell from "../inventory-sell/inventory-sell";
import { setDisableInventory } from "@/state-mgmt/item/itemSlice";
import Summoning from "../summoning/summoning";
import { UpdateGameStateRequest } from "@/state-mgmt/game-state/gameStateTypes";
import DCXAudioPlayer from "../dcx-audio-player/dcx-audio-player";
import Herbalist from "../herbalist/herbalist";
import { DcxTiles } from "@dcx/dcx-backend";
import {
	selectAdventuringGuildTab,
	setAdventuringGuildTab,
	setPlayButtonClickSound,
} from "@/state-mgmt/app/appSlice";
import Scrollbars from "react-custom-scrollbars";
import NftActions from "../nftActions/nftActions";

interface Props {
	vendorType: VendorType;
}

const Vendor: React.FC<Props> = (props: Props) => {
	const router = useRouter();
	const { vendorType } = props;
	const { width, height } = useWindowDimensions();
	const dispatch = useAppDispatch();
	const { hero } = useAppSelector(selectSelectedHero);
	const gameState = useAppSelector(selectGameState);
	const adventuringGuildTab = useAppSelector(selectAdventuringGuildTab);

	const [tab, setTab] = useState("skills");
	const [page, setPage] = useState("equip");
	const [isShowBuy, setShowBuy] = useState(true);
	const [isShowSell, setShowSell] = useState(false);
	const [isShowHeal, setShowHeal] = useState(true);

	const skillsIntro = `HERE WE CAN TEACH YOU ABOUT SPECIAL SKILLS THAT WILL SERVE YOU IN COMBAT.`;
	const summoningIntro = `IF YOU'VE MANAGED TO FIND ONE OF THE RARE SUMMONING SHARDS, HERE YOU CAN USE IT TO PULL ANOTHER HERO THROUGH THE RIFT AND INTO AEDOS TO HELP US TAME HORIZON.`;
	const levelUpIntro = `YOU'VE CLEARLY LEARNED A LOT IN YOUR ADVENTURES. LET'S TURN THAT KNOWLEDGE INTO SOMETHING THAT WILL MAKE YOU MORE POWERFUL.`;
	const blacksmithIntro = `I MAKE THE FINEST WEAPONS AND ARMOR IN AEDOS. SEE ANYTHING YOU LIKE?`;
	const herbalistIntro = `HAVE ANY WOUNDS THAT NEED ATTENTION?`;
	const librarianIntro = `WELCOME, I HAVE MANY BOOKS THAT I THINK WOULD INTEREST A FELLOW SCHOLAR SUCH AS YOURSELF.`;
	const actionsIntro = `WELCOME, TRAVELER! HERE, THE POWER OF MANDRAKE ROOTS AWAITS YOU – A FORCE THAT CAN BREATHE LIFE BACK INTO YOUR DAILY QUESTS.`;
	useEffect(() => {
		if (adventuringGuildTab !== "") {
			setTab(adventuringGuildTab);
			dispatch(setAdventuringGuildTab(""));
		}
	}, [adventuringGuildTab]);

	const handleSkillsTabClick = () => {
		setTab("skills");
	};

	const handleSummoningTabClick = () => {
		setTab("summoning");
	};

	const handleLevelUpTabClick = () => {
		setTab("levelup");
	};
	const handleActionsTabClick = () => {
		setTab("actions");
	};
	const handleIdentifySkillsClick = () => {
		dispatch(setPlayButtonClickSound(true));
		setPage("identify");
	};

	const handleLearnSkillsClick = () => {
		dispatch(setPlayButtonClickSound(true));
		setPage("learn");
	};

	const handleEquipSkillsClick = () => {
		dispatch(setPlayButtonClickSound(true));
		setPage("equip");
	};

	const handleBuyGearClick = () => {
		dispatch(setPlayButtonClickSound(true));
		dispatch(setDisableInventory(false));
		setShowSell(false);
		setShowBuy(true);
	};

	const handleSellGearClick = () => {
		dispatch(setPlayButtonClickSound(true));
		dispatch(setDisableInventory(true));
		setShowBuy(false);
		setShowSell(true);
	};

	const handleHealClick = () => {
		dispatch(setPlayButtonClickSound(true));
		setShowHeal(true);
	};

	const handleLeaveClick = () => {
		dispatch(setPlayButtonClickSound(true));
		dispatch(setDisableInventory(false));
		setShowBuy(false);
		setShowSell(false);
		setShowHeal(false);
		setPage("leave");
		dispatch(updateGameState(gameState.zone.slug as DcxTiles));
	};

	return (
		<Grid container direction="row" className={styles.container}>
			<Grid container direction="row" className={styles.vendorContainer}>
				<Grid container item className={styles.portraitContainer}>
					<Image
						src="/img/unity-assets/shared/action_bg_vertical.png"
						height={width > xsScreenWidth ? 367 : 340}
						width={width > xsScreenWidth ? 277 : 375}
						quality={100}
					/>
					<Grid container className={styles.portrait}>
						<Image
							src={
								vendorType === VendorType.ADVENTURING_GUILD
									? `/img/npc/guild_master.jpg`
									: vendorType === VendorType.BLACKSMITH
									? `/img/npc/blacksmith.jpg`
									: vendorType === VendorType.HERBALIST
									? `/img/npc/herbalist.jpg`
									: `/img/npc/librarian.jpg`
							}
							height={width > xsScreenWidth ? 349 : 323}
							width={width > xsScreenWidth ? 251 : 342}
							quality={100}
						/>
					</Grid>
				</Grid>
				<Grid container item className={styles.interactionContainer}>
					<Image
						src={
							width > xsScreenWidth
								? "/img/unity-assets/shared/action_bg_vertical_small.png"
								: "/img/unity-assets/shared/action_bg.png"
						}
						height={
							width > mdScreenWidth ? 318 : width > xsScreenWidth ? 367 : 265
						}
						width={width > xsScreenWidth ? 277 : 362}
						quality={100}
					/>
					<Grid container className={styles.guildMasterText}>
						<Typography component="span">
							{vendorType === VendorType.ADVENTURING_GUILD
								? tab === "skills"
									? skillsIntro
									: tab === "summoning"
									? summoningIntro
									: tab === "actions"
									? actionsIntro
									: levelUpIntro
								: vendorType === VendorType.BLACKSMITH
								? blacksmithIntro
								: vendorType === VendorType.HERBALIST
								? herbalistIntro
								: librarianIntro}
						</Typography>
					</Grid>
					{vendorType === VendorType.ADVENTURING_GUILD && (
						<Grid container className={styles.optionContainer}>
							{tab === "skills" && (
								<Grid container className={styles.skillsButtonContainer}>
									<Grid
										container
										className={styles.option}
										onClick={handleIdentifySkillsClick}
									>
										<Grid container className={styles.divider} />
										{page === "identify" && (
											<Grid container className={styles.activeOption} />
										)}
										<Typography component="span" className={styles.optionText}>
											IDENTIFY SKILLS
										</Typography>
									</Grid>
									<Grid
										container
										className={styles.option}
										onClick={handleLearnSkillsClick}
									>
										<Grid container className={styles.divider} />
										{page === "learn" && (
											<Grid container className={styles.activeOption} />
										)}
										<Typography component="span" className={styles.optionText}>
											LEARN SKILLS
										</Typography>
									</Grid>
									<Grid
										container
										className={styles.option}
										onClick={handleEquipSkillsClick}
									>
										<Grid container className={styles.divider} />
										{page === "equip" && (
											<Grid container className={styles.activeOption} />
										)}
										<Typography component="span" className={styles.optionText}>
											EQUIP SKILLS
										</Typography>
									</Grid>
									<Grid
										container
										className={styles.option}
										onClick={handleLeaveClick}
									>
										<Grid container className={styles.divider} />
										{page === "leave" && (
											<Grid container className={styles.activeOption} />
										)}
										<Typography component="span" className={styles.optionText}>
											LEAVE
										</Typography>
									</Grid>
								</Grid>
							)}
							{tab === "summoning" && (
								<Grid container className={styles.levelUpButtonContainer}>
									<Grid container className={styles.option}>
										<Grid container className={styles.divider} />
										{page !== "leave" && (
											<Grid container className={styles.activeOption} />
										)}
										<Typography component="span" className={styles.optionText}>
											SUMMON
										</Typography>
									</Grid>
									<Grid
										container
										className={styles.option}
										onClick={handleLeaveClick}
									>
										<Grid container className={styles.divider} />
										{page === "leave" && (
											<Grid container className={styles.activeOption} />
										)}
										<Typography component="span" className={styles.optionText}>
											LEAVE
										</Typography>
									</Grid>
								</Grid>
							)}
							{tab === "levelup" && (
								<Grid container className={styles.levelUpButtonContainer}>
									<Grid container className={styles.option}>
										<Grid container className={styles.divider} />
										{page !== "leave" && (
											<Grid container className={styles.activeOption} />
										)}
										<Typography component="span" className={styles.optionText}>
											LEVEL UP
										</Typography>
									</Grid>
									<Grid
										container
										className={styles.option}
										onClick={handleLeaveClick}
									>
										<Grid container className={styles.divider} />
										{page === "leave" && (
											<Grid container className={styles.activeOption} />
										)}
										<Typography component="span" className={styles.optionText}>
											LEAVE
										</Typography>
									</Grid>
								</Grid>
							)}
						</Grid>
					)}
					{vendorType === VendorType.BLACKSMITH && (
						<Grid container className={styles.optionContainer}>
							<Grid container className={styles.blacksmithButtonContainer}>
								<Grid
									container
									className={styles.option}
									onClick={handleBuyGearClick}
								>
									<Grid container className={styles.divider} />
									{isShowBuy && (
										<Grid container className={styles.activeOption} />
									)}
									<Typography component="span" className={styles.optionText}>
										BUY GEAR
									</Typography>
								</Grid>
								{
									<Grid
										container
										className={styles.option}
										onClick={handleSellGearClick}
									>
										<Grid container className={styles.divider} />
										{isShowSell && (
											<Grid container className={styles.activeOption} />
										)}
										<Typography component="span" className={styles.optionText}>
											SMELT GEAR
										</Typography>
									</Grid>
								}
								<Grid
									container
									className={styles.option}
									onClick={handleLeaveClick}
								>
									<Grid container className={styles.divider} />
									{page === "leave" && (
										<Grid container className={styles.activeOption} />
									)}
									<Typography component="span" className={styles.optionText}>
										LEAVE
									</Typography>
								</Grid>
							</Grid>
						</Grid>
					)}
					{vendorType === VendorType.HERBALIST && (
						<Grid container className={styles.optionContainer}>
							<Grid container className={styles.herbalistButtonContainer}>
								<Grid
									container
									className={styles.option}
									onClick={handleHealClick}
								>
									<Grid container className={styles.divider} />
									{isShowHeal && (
										<Grid container className={styles.activeOption} />
									)}
									<Typography component="span" className={styles.optionText}>
										HEAL
									</Typography>
								</Grid>
								<Grid
									container
									className={styles.option}
									onClick={handleLeaveClick}
								>
									<Grid container className={styles.divider} />
									{page === "leave" && (
										<Grid container className={styles.activeOption} />
									)}
									<Typography component="span" className={styles.optionText}>
										LEAVE
									</Typography>
								</Grid>
							</Grid>
						</Grid>
					)}
				</Grid>
			</Grid>
			<Grid container className={styles.guildRoomContainer}>
				<Grid container>
					<Image
						src={
							width > xsScreenWidth
								? "/img/unity-assets/shared/action_bg.png"
								: "/img/unity-assets/shared/action_bg_vertical_small.png"
						}
						height={width > mdScreenWidth ? 711 : 340}
						width={
							width > mdScreenWidth ? 953 : width > xsScreenWidth ? 538 : 377
						}
						quality={100}
					/>
				</Grid>
				{vendorType === VendorType.ADVENTURING_GUILD && (
					<Grid
						container
						direction="row"
						className={styles.topContainer}
						style={{
							justifyContent:
								tab === "skills" && page === "equip" && width <= mdScreenWidth
									? "space-between"
									: "center",
							left:
								tab === "skills" && page === "equip" && width <= xsScreenWidth
									? 22
									: undefined,
						}}
					>
						<Grid
							container
							direction="row"
							className={styles.tabButtonsContainer}
						>
							<DCXButton
								title="SKILLS"
								height={30}
								width={108}
								color="blue"
								inactive={tab !== "skills"}
								arrowTopAdjustment={8}
								onClick={() => handleSkillsTabClick()}
							/>
							<DCXButton
								title="SUMMONING"
								height={30}
								width={108}
								color="blue"
								inactive={tab !== "summoning"}
								arrowTopAdjustment={8}
								onClick={() => handleSummoningTabClick()}
							/>
							<DCXButton
								title="LEVEL UP"
								height={30}
								width={108}
								color="blue"
								inactive={tab !== "levelup"}
								disabled={
									hero.experiencePoints < hero.maxExperiencePoints ||
									hero.level === 20
								}
								arrowTopAdjustment={8}
								onClick={() => handleLevelUpTabClick()}
							/>
							<DCXButton
								title="ACTIONS"
								height={30}
								width={108}
								color="blue"
								inactive={tab !== "actions"}
								disabled={hero.level < 1}
								arrowTopAdjustment={8}
								onClick={() => handleActionsTabClick()}
							/>
						</Grid>
						{tab === "skills" && page === "equip" && (
							<Grid container className={styles.skillPointsBackgroundContainer}>
								<Grid container className={styles.skillPointsContainer}>
									<Image
										src="/img/unity-assets/shared/text_bg.png"
										height={35}
										width={154}
										quality={100}
									/>
									<Typography
										component="span"
										className={styles.skillPointsCount}
									>
										{`SKILL POINTS: ${
											hero.baseSkillPoints - hero.usedUpSkillPoints
										}/${hero.baseSkillPoints}`}
									</Typography>
								</Grid>
							</Grid>
						)}
					</Grid>
				)}
				{vendorType === VendorType.ADVENTURING_GUILD && (
					<Grid container className={styles.test}>
						<DCXAudioPlayer
							audioUrl={"/audio/sound-effects/vendor/adventuring-guild"}
							soundType={SoundType.SOUND_EFFECT}
						/>

						{tab === "skills" && page !== "equip" && (
							<Grid container className={styles.skillsIdentifyContainer}>
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
									<Skills page={page} />
								</Scrollbars>
							</Grid>
						)}
						{tab === "skills" && page === "equip" && (
							<Grid container className={styles.skillsEquipContainer}>
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
									<Skills page={page} />
								</Scrollbars>
							</Grid>
						)}
						{tab === "summoning" && (
							<Grid container className={styles.contentContainer}>
								<Summoning />
							</Grid>
						)}
						{tab === "actions" && (
							<Grid container className={styles.contentContainer}>
								<NftActions />
							</Grid>
						)}
						{tab === "levelup" && (
							<Grid container className={styles.levelUpContainer}>
								<LevelUp hero={hero} />
							</Grid>
						)}
					</Grid>
				)}
				{vendorType === VendorType.BLACKSMITH && (
					<Grid container>
						<DCXAudioPlayer
							audioUrl={"/audio/sound-effects/vendor/blacksmith"}
							soundType={SoundType.SOUND_EFFECT}
						/>
						{isShowBuy && (
							<Grid container className={styles.blacksmithContentContainer}>
								<BlacksmithItems hero={hero} />
							</Grid>
						)}
						{isShowSell && (
							<Grid container className={styles.blacksmithContentContainer}>
								<InventorySell />
							</Grid>
						)}
					</Grid>
				)}
				{vendorType === VendorType.HERBALIST && (
					<Grid container>
						<DCXAudioPlayer
							audioUrl={"/audio/sound-effects/vendor/herbalist"}
							soundType={SoundType.SOUND_EFFECT}
						/>
						<Grid container className={styles.herbalistContentContainer}>
							<Herbalist />
						</Grid>
					</Grid>
				)}
			</Grid>
		</Grid>
	);
};

export default Vendor;
