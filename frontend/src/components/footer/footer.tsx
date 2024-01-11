import {
  setDisplayCharacter,
  setDisplayFixDice,
  setDisplayGuildModal,
  setDisplayInventory,
  setDisplayLearnSkill,
  setDisplaySettings,
  setDisplayStaking,
  setPlayButtonClickSound,
  setRefreshHeroNFTs,
} from "@/state-mgmt/app/appSlice";
import { resetSelectedHeroStatus, selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Grid from "@mui/material/Grid";
import useWindowDimensions from "@/helpers/window-dimensions";
import styles from "./footer.module.scss";
import Image from "next/image";
import { lgScreenWidth, mdScreenWidth, tooltipTheme, xlScreenWidth } from "@/helpers/global-constants";
import DCXMenu from "../dcx-menu/dcx-menu";
import Typography from "@mui/material/Typography";
import { selectDisableInventory } from "@/state-mgmt/item/itemSlice";
import { ThemeProvider } from "@mui/material/styles";
import Tooltip from "@mui/material/Tooltip";
import Zoom from "@mui/material/Zoom";
import { useRouter } from "next/router";
import { AuthApi, DcxZones } from "@dcx/dcx-backend";
import { apiConfig } from "../hoc/verification";
import { useAuthentication } from "../auth";
import { clearGameState, selectGameState } from "@/state-mgmt/game-state/gameStateSlice";
import HealthBar from "../health-bar/health-bar";
import DCXButton from "../dcx-button/dcx-button";
import { resetQuests, selectIsTestEndpointsAvailable } from "@/state-mgmt/testing/testingSlice";
import FlareIcon from "@mui/icons-material/Flare";
import { getSeasonLeaderboard, setShowLeaderboardModal } from "@/state-mgmt/season/seasonSlice";
import { LeaderboardRequest } from "@/state-mgmt/season/seasonTypes";
import XPBar from "../xp-bar/xp-bar";

interface Props {}

const Footer: React.FC<Props> = (props: Props) => {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const selectedHero = useAppSelector(selectSelectedHero);
  const isDisableInventory = useAppSelector(selectDisableInventory);
  const gameState = useAppSelector(selectGameState);
  const isTestEndpointsAvailable = useAppSelector(selectIsTestEndpointsAvailable);

  const { width, height } = useWindowDimensions();
  const { updateJWT } = useAuthentication();

  const handleCharacterClick = () => {
    dispatch(setPlayButtonClickSound(true));
    dispatch(setDisplayCharacter(true));
  };

  const handleInventoryClick = () => {
    dispatch(setPlayButtonClickSound(true));
    dispatch(setDisplayInventory(true));
  };

  const handleStakingClick = () => {
    dispatch(setPlayButtonClickSound(true));
    dispatch(setDisplayStaking(true));
  };

  const handleAdventuringGuildClick = () => {
    dispatch(setPlayButtonClickSound(true));
    dispatch(setDisplayGuildModal(true));
  };

  const handleLeaderboardClick = () => {
    dispatch(
      getSeasonLeaderboard({ seasonId: selectedHero.hero.seasonId, heroId: selectedHero.hero.id } as LeaderboardRequest)
    );
    dispatch(setPlayButtonClickSound(true));
    dispatch(setShowLeaderboardModal(true));
  };

  const handleSettingsClick = () => {
    dispatch(setPlayButtonClickSound(true));
    dispatch(setDisplaySettings(true));
  };

  const handleGoToHeroSelectClick = async () => {
    // Call on endpoint to get a new token without the selectedHeroId
    try {
      dispatch(setPlayButtonClickSound(true));
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

  const handleFixDiceClick = () => {
    dispatch(setDisplayFixDice(true));
  };

  const handleResetQuestsClick = () => {
    dispatch(resetQuests(selectedHero.hero.id));
  };

  const handleLearnSkillClick = () => {
    dispatch(setDisplayLearnSkill(true));
  };

  // const handleGiveAllLootClick = (event: React.ChangeEvent<HTMLInputElement>) => {
  //   dispatch(giveAllLoot(event.target.checked));
  // };

  const getBackgroundBarWidth = (): number => {
    if (width > lgScreenWidth) {
      return 450;
    } else {
      return 450;
    }
  };

  const getTrimmedWidth = (): number => {
    if (width > lgScreenWidth) {
      return 421;
    } else {
      return 421;
    }
  };

  return (
    <Grid container direction="row" className={styles.container}>
      <Grid container className={styles.background} />
      <Grid container direction="row" className={styles.dividerContainerLeft}>
        <Image
          src="/img/unity-assets/shared/footer-divider.png"
          height={7}
          width={width}
          quality={100}
          onClick={handleCharacterClick}
        />
      </Grid>
      <Grid container direction="row" className={styles.dividerContainerRight}>
        <Image
          src="/img/unity-assets/shared/footer-divider.png"
          height={7}
          width={width}
          quality={100}
          onClick={handleCharacterClick}
        />
      </Grid>
      {width > mdScreenWidth ? (
        <Grid container className={styles.contentContainer}>
          <Grid item xs={5} className={styles.experienceContainer}>
            {/* <HealthBar
              hero={selectedHero.hero}
              backgroundBarHeight={80}
              backgroundBarWidth={getBackgroundBarWidth()}
              trimmedWidth={getTrimmedWidth()}
              barHeight={28}
            /> */}
            <Grid container direction="column" className={styles.barsContainer}>
              <Grid container className={styles.hpContainer}>
                <HealthBar
                  hero={selectedHero.hero}
                  backgroundBarHeight={65}
                  backgroundBarWidth={475}
                  trimmedWidth={444}
                  barHeight={22}
                />
              </Grid>
              <Grid container className={styles.xpContainer}>
                <XPBar
                  hero={selectedHero.hero}
                  backgroundBarHeight={65}
                  backgroundBarWidth={475}
                  trimmedWidth={444}
                  barHeight={22}
                />
              </Grid>
            </Grid>
          </Grid>
          <Grid item xs={2} className={styles.centerContainer}>
            <Grid container className={styles.questContainer}>
              <Grid container direction="row" className={styles.questFrameContainer}>
                <Image src="/img/unity-assets/shared/oval_frame.png" height={44} width={70} quality={100} />
              </Grid>
              <Grid container direction="row" className={styles.questTextContainer}>
                <Image src="/img/brand-assets/quests-text.png" height={25} width={66} quality={100} />
              </Grid>
              <Typography
                component="span"
                className={styles.questText}
              >{`${selectedHero.hero.remainingQuests}/${selectedHero.hero.maxDailyQuests}`}</Typography>
            </Grid>
            {selectedHero.hero.experiencePoints >= selectedHero.hero.maxExperiencePoints && (
              <ThemeProvider theme={tooltipTheme}>
                <Tooltip
                  disableFocusListener
                  placement="top"
                  arrow
                  enterTouchDelay={0}
                  TransitionComponent={Zoom}
                  title={`LEVEL UP AVAILABLE`}
                >
                  <Grid container direction="row" className={styles.levelUpIndicatorContainer}>
                    <FlareIcon className={styles.levelUpIcon} />
                  </Grid>
                </Tooltip>
              </ThemeProvider>
            )}
          </Grid>
          <Grid item xs={5} className={styles.buttonContainer}>
            <Grid container direction="row" className={styles.buttonIconContainer}>
              <ThemeProvider theme={tooltipTheme}>
                <Tooltip
                  disableFocusListener
                  placement="top"
                  enterTouchDelay={0}
                  arrow
                  TransitionComponent={Zoom}
                  title={`CHARACTER`}
                >
                  <Grid container className={[styles.iconButton, styles.buttonPadding].join(" ")}>
                    <Image
                      src="/img/unity-assets/shared/character_button.png"
                      height={51}
                      width={60}
                      quality={100}
                      onClick={handleCharacterClick}
                      className={styles.footerButtonImage}
                    />
                  </Grid>
                </Tooltip>
              </ThemeProvider>
              <ThemeProvider theme={tooltipTheme}>
                <Tooltip
                  disableFocusListener
                  placement="top"
                  arrow
                  TransitionComponent={Zoom}
                  enterTouchDelay={0}
                  title={`INVENTORY`}
                >
                  <Grid container className={[styles.iconButton, styles.buttonPadding].join(" ")}>
                    <Image
                      src="/img/unity-assets/shared/inventory_button.png"
                      height={51}
                      width={60}
                      quality={100}
                      onClick={!isDisableInventory ? handleInventoryClick : undefined}
                      className={styles.footerButtonImage}
                    />
                    {isDisableInventory && <Grid container className={styles.disableInventory} />}
                  </Grid>
                </Tooltip>
              </ThemeProvider>
              {/* <ThemeProvider theme={tooltipTheme}>
                <Tooltip
                  disableFocusListener
                  placement="top"
                  arrow
                  enterTouchDelay={0}
                  TransitionComponent={Zoom}
                  title={`SINGLE STAKING`}
                >
                  <Grid
                    container
                    className={[styles.iconButton, styles.buttonPadding].join(
                      " "
                    )}
                  >
                    <Image
                      src="/img/unity-assets/shared/staking_button.png"
                      height={51}
                      width={60}
                      quality={100}
                      enterTouchDelay={0}
                      onClick={handleStakingClick}
                      className={styles.footerButtonImage}
                    />
                  </Grid>
                </Tooltip>
              </ThemeProvider> */}
              {gameState.zone.slug !== DcxZones.Aedos && (
                <ThemeProvider theme={tooltipTheme}>
                  <Tooltip
                    disableFocusListener
                    placement="top"
                    arrow
                    enterTouchDelay={0}
                    TransitionComponent={Zoom}
                    title={`ADVENTURING GUILD`}
                  >
                    <Grid container className={[styles.iconButton, styles.buttonPadding].join(" ")}>
                      <Image
                        src="/img/unity-assets/shared/adventuring_guild_button.png"
                        height={51}
                        width={60}
                        quality={100}
                        onClick={handleAdventuringGuildClick}
                        className={styles.footerButtonImage}
                      />
                    </Grid>
                  </Tooltip>
                </ThemeProvider>
              )}
              {selectedHero.hero.seasonId && (
                <ThemeProvider theme={tooltipTheme}>
                  <Tooltip
                    disableFocusListener
                    placement="top"
                    arrow
                    TransitionComponent={Zoom}
                    enterTouchDelay={0}
                    title={`LEADERBOARD`}
                  >
                    <Grid container className={[styles.iconButton, styles.buttonPadding].join(" ")}>
                      <Image
                        src="/img/unity-assets/shared/leaderboard_button.png"
                        height={51}
                        width={60}
                        quality={100}
                        onClick={handleLeaderboardClick}
                        className={styles.footerButtonImage}
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
                  title={`HERO SELECT`}
                >
                  <Grid container className={[styles.iconButton, styles.buttonPadding].join(" ")}>
                    <Image
                      src="/img/unity-assets/shared/hero_select_button.png"
                      height={51}
                      width={60}
                      quality={100}
                      onClick={handleGoToHeroSelectClick}
                      className={styles.footerButtonImage}
                    />
                  </Grid>
                </Tooltip>
              </ThemeProvider>
              <ThemeProvider theme={tooltipTheme}>
                <Tooltip
                  disableFocusListener
                  placement="top"
                  arrow
                  TransitionComponent={Zoom}
                  enterTouchDelay={0}
                  title={`SETTINGS`}
                >
                  <Grid container className={styles.iconButton}>
                    <Image
                      src="/img/unity-assets/shared/settings_button.png"
                      height={51}
                      width={60}
                      quality={100}
                      onClick={handleSettingsClick}
                      className={styles.footerButtonImage}
                    />
                  </Grid>
                </Tooltip>
              </ThemeProvider>
            </Grid>
          </Grid>
        </Grid>
      ) : (
        <Grid container className={styles.contentContainer}>
          <Grid container direction="row" className={styles.mobileContentContainer}>
            <Grid container className={styles.mobileQuestContainer}>
              <Grid container direction="row" className={styles.questFrameContainer}>
                <Image src="/img/unity-assets/shared/oval_frame.png" height={44} width={70} quality={100} />
              </Grid>
              <Grid container direction="row" className={styles.mobileQuestTextContainer}>
                <Image src="/img/brand-assets/quests-text.png" height={25} width={66} quality={100} />
              </Grid>
              <Typography
                component="span"
                className={styles.mobileQuestText}
              >{`${selectedHero.hero.remainingQuests}/${selectedHero.hero.maxDailyQuests}`}</Typography>
            </Grid>
            <Grid container className={styles.menuContainer}>
              <DCXMenu hero={selectedHero.hero} />
            </Grid>
            {selectedHero.hero.experiencePoints >= selectedHero.hero.maxExperiencePoints &&
              selectedHero.hero.level < 20 && (
                <Grid container direction="row" className={styles.mobileLevelUpIndicatorContainer}>
                  <ThemeProvider theme={tooltipTheme}>
                    <Tooltip
                      disableFocusListener
                      placement="top"
                      arrow
                      enterTouchDelay={0}
                      TransitionComponent={Zoom}
                      title={`LEVEL UP AVAILABLE`}
                    >
                      <FlareIcon className={styles.levelUpIcon} />
                    </Tooltip>
                  </ThemeProvider>
                </Grid>
              )}
          </Grid>
        </Grid>
      )}
      {/* Temp */}
      {isTestEndpointsAvailable && (
        <Grid container>
          <Grid container className={styles.fixDiceContainer}>
            <DCXButton
              title={"FIX DICE"}
              height={30}
              width={80}
              arrowTopAdjustment={8}
              color="red"
              onClick={() => handleFixDiceClick()}
            />
          </Grid>
          <Grid container className={styles.resetQuestsContainer}>
            <DCXButton
              title={"DAILY RESET"}
              height={30}
              width={80}
              arrowTopAdjustment={8}
              color="red"
              onClick={() => handleResetQuestsClick()}
            />
          </Grid>
          <Grid container className={styles.learnSkillContainer}>
            <DCXButton
              title={"LEARN SKILL"}
              height={30}
              width={80}
              arrowTopAdjustment={8}
              color="red"
              onClick={() => handleLearnSkillClick()}
            />
          </Grid>
        </Grid>
      )}
    </Grid>
  );
};

export default Footer;
