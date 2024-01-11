import { CharacterClassDto, LearnedHeroSkill, LoanerHeroType } from "@dcx/dcx-backend";
import { selectGetHeroesStatus, selectSelectedHero, setSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { Hero } from "@/state-mgmt/hero/heroTypes";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Image from "next/image";
import React, { useEffect, useState } from "react";
import DCXButton from "../dcx-button/dcx-button";
import { tooltipTheme, walletAddressMismatch, xlScreenWidth } from "@/helpers/global-constants";
import useWindowDimensions from "@/helpers/window-dimensions";
import styles from "./hero-card.module.scss";
import { AuthApi } from "@dcx/dcx-backend";
import { apiConfig } from "@/components/hoc/verification";
import { useAuthentication } from "../auth";
import { ThemeProvider } from "@mui/material/styles";
import Tooltip from "@mui/material/Tooltip";
import Zoom from "@mui/material/Zoom";
import { selectOpenSeasons, setHeroToRegister, setShowRegistrationModal } from "@/state-mgmt/season/seasonSlice";
import { Status } from "@/state-mgmt/app/appTypes";
import { useConnectCalls } from "../web3";
import { selectConnectedWalletAddress } from "@/state-mgmt/player/playerSlice";
import { setAppMessage } from "@/state-mgmt/app/appSlice";
//import { getClaimDcxRewards } from "@/state-mgmt/camp/campSlice";
import { useRouter } from "next/router";

interface Props {
  hero: Hero;
  disableButtons?: boolean;
  disableSelectHeroButton?: boolean;
}

const HeroCard = (props: Props) => {
  const router = useRouter();
  const { hero, disableButtons, disableSelectHeroButton } = props;
  const { connect } = useConnectCalls();
  const dispatch = useAppDispatch();
  const openSeasons = useAppSelector(selectOpenSeasons);
  const selectedHero = useAppSelector(selectSelectedHero).hero;
  const getHeroesStatus = useAppSelector(selectGetHeroesStatus);
  const connectedWalletAddress = useAppSelector(selectConnectedWalletAddress);

  const { updateJWT } = useAuthentication();
  const { width, height } = useWindowDimensions();

  const [flipCard, setFlipCard] = useState(false);
  const [startTimer, setStartTimer] = useState(false);
  const [timeUntilReset, setTimeUntilReset] = useState("");

  useEffect(() => {
    if (hero && hero.id !== -1 && hero.timeTillNextReset) {
      setStartTimer(true);
      setTimeUntilReset(hero.timeTillNextReset.toString().substring(0, 8));
    }
  }, []);

  useEffect(() => {
    if (getHeroesStatus.status === Status.Loaded) {
      if (hero && hero.id !== -1 && hero.timeTillNextReset) {
        setStartTimer(true);
        setTimeUntilReset(hero.timeTillNextReset.toString().substring(0, 8));
      }
    }
  }, [getHeroesStatus]);

  useEffect(() => {
    if (hero && hero.id !== -1) {
      if (startTimer && hero.timeTillNextReset) {
        let timer1 = setTimeout(function () {
          const timeArray = timeUntilReset.split(":");
          let hour = Number(timeArray[0]);
          let minute = Number(timeArray[1]);
          let second = Number(timeArray[2]);

          if (timeUntilReset !== "00:00:00") {
            if (second > 0) {
              second = second - 1;
            } else {
              second = 59;
              if (minute > 0) {
                minute = minute - 1;
              } else {
                minute = 59;
                if (hour > 0) {
                  hour = hour - 1;
                } else {
                  hour = 22;
                }
              }
            }
          }

          const formattedHour = hour.toString().length === 1 ? "0" + hour.toString() : hour.toString();
          const formattedMinute = minute.toString().length === 1 ? "0" + minute.toString() : minute.toString();
          const formattedSecond = second.toString().length === 1 ? "0" + second.toString() : second.toString();
          setTimeUntilReset(formattedHour + ":" + formattedMinute + ":" + formattedSecond);
        }, 1000);
        return () => {
          clearInterval(timer1);
        };
      }
    } else {
      setStartTimer(false);
    }
  }, [startTimer, timeUntilReset]);

  const handleSelectClick = async () => {
    try {
      const { data: userWithHero } = await new AuthApi(apiConfig).apiAuthClaimHeroHeroIdGet(hero.id);
      updateJWT(userWithHero);
      dispatch(setSelectedHero(hero));
    } catch (err) {
      //brandon-todo: Please handle the error
      console.log(`failed to select Hero :${err}`);
    }
  };

  const handleRegisterClick = async () => {
    const { data: userWithHero } = await new AuthApi(apiConfig).apiAuthClaimHeroHeroIdGet(hero.id);
    updateJWT(userWithHero);
    dispatch(setHeroToRegister(hero));
    dispatch(setShowRegistrationModal(true));
  };

  const handleClaimRewardsClick = async () => {
    (await connect(undefined)).getAddress().then(async (address) => {
      if (address.toLowerCase() === connectedWalletAddress.toLowerCase()) {
        //dispatch(getClaimDcxRewards(address));
        router.push("/claimRewards");
      } else {
        dispatch(setAppMessage({ message: walletAddressMismatch, isClearToken: true }));
      }
    });
  };

  const handleFlipClick = () => {
    flipCard ? setFlipCard(false) : setFlipCard(true);
  };

  const getImageHeight = () => {
    if (width > xlScreenWidth) {
      return "450px";
    } else {
      return "400px";
    }
  };

  const getImageWidth = () => {
    if (width > xlScreenWidth) {
      return "373px";
    } else {
      return "331px";
    }
  };

  const getImageStyles = () => {
    if (hero.gender === "Male") {
      if (hero.heroClass === CharacterClassDto.Warrior) {
        return styles.maleWarrior;
      } else if (hero.heroClass === CharacterClassDto.Mage) {
        if (hero.generation === 0) {
          return styles.maleMage;
        } else {
          return styles.maleMageGen1;
        }
      } else {
        return styles.maleArcher;
      }
    } else {
      if (hero.heroClass === CharacterClassDto.Warrior) {
        return styles.femaleWarrior;
      } else if (hero.heroClass === CharacterClassDto.Mage) {
        return styles.femaleMage;
      } else {
        return styles.femaleArcher;
      }
    }
  };

  const isButtonDisabled = (): boolean => {
    let isDisabled = false;
    if (hero.seasonId) {
      if (openSeasons.filter((os) => os.seasonId === hero.seasonId).length === 0) {
        isDisabled = true;
      }
    } else {
      if (openSeasons.filter((os) => os.isRegistrationOpen).length === 0) {
        isDisabled = true;
      }
      openSeasons.filter((os) => os.seasonId === hero.seasonId).length === 0;
    }
    return isDisabled;
  };

  const cardContainerStyle = flipCard ? styles.cardContainerTransform : styles.cardContainer;

  const renderSkillSymbols = () => {
    const symbolRows: JSX.Element[] = [];
    let symbolRow: JSX.Element[] = [];
    let count = 0;
    hero.skills.forEach((skill: LearnedHeroSkill) => {
      count++;
      // Add symbol to symbolRow
      symbolRow.push(
        <ThemeProvider theme={tooltipTheme} key={skill.id}>
          <Tooltip
            disableFocusListener
            placement="top"
            arrow
            TransitionComponent={Zoom}
            enterTouchDelay={0}
            title={skill.name}
          >
            <Grid container className={styles.skillIconContainer}>
              <Image
                src={`/img/api/skills/icons/${skill.slug.toLowerCase()}.png`}
                height={40}
                width={40}
                quality={100}
              />
            </Grid>
          </Tooltip>
        </ThemeProvider>
      );
      // Each symbolRow should contain 1 or 2 symbols
      if (count % 2 === 0) {
        // Add the 2 symbols to a symbolRow container
        symbolRows.push(
          <Grid item xs={12} className={styles.symbolRow} key={skill.id}>
            {symbolRow}
          </Grid>
        );
        symbolRow = [];
      } else if (count === hero.skills.length) {
        // If there is an odd amount of symbols and this is the last one, add it to its own row
        symbolRows.push(
          <Grid item xs={12} className={styles.symbolRow} key={skill.id}>
            {symbolRow}
          </Grid>
        );
        symbolRow = [];
      }
    });
    return symbolRows;
  };

  const renderHeroTypeSymbol = () => {
    if (hero.heroClass.includes(CharacterClassDto.Warrior)) {
      return <Image src={`/img/unity-assets/card/warrior_symbol.png`} height={80} width={75} quality={100} />;
    } else if (hero.heroClass.includes(CharacterClassDto.Mage)) {
      return <Image src={`/img/unity-assets/card/mage_symbol.png`} height={99} width={105} quality={100} />;
    } else {
      return <Image src={`/img/unity-assets/card/ranger_symbol.png`} height={104} width={82} quality={100} />;
    }
  };

  let heroBegin = "";

  const isDFkDemo = hero.isLoanedHero?.loanerType == LoanerHeroType.ClaimDfk && !hero.isLoanedHero.loanedToUserId;
  
  if(isDFkDemo){
    heroBegin = "Claim ";
  }

  const demoModeNotYetMinted = hero.id === 999999991 || isDFkDemo;

  const registerForFree = hero.id === 999999991;

  let heroEnd = "";

  if(hero.isLoanedHero?.loanerType === LoanerHeroType.Demo){
    heroEnd = "- DEMO";
  }

  
  return (
    <Grid item container direction="column" xs={12} className={styles.cardMain}>
      <Grid container direction="column" className={styles.container}>
        <Grid item xs={12} className={cardContainerStyle}>
          <Grid item xs={12} className={styles.cardFront}>
            {!selectedHero ||
              (selectedHero.id === -1 && (
                <Grid container className={styles.dailyResetTimeContainer}>
                  <Image src="/img/unity-assets/shared/text_bg_cropped.png" height={26} width={58} quality={100} />
                  <Typography component="span" className={styles.timeText}>
                    {timeUntilReset}
                  </Typography>
                </Grid>
              ))}
            {!selectedHero ||
              (selectedHero.id === -1 && (
                <ThemeProvider theme={tooltipTheme}>
                  <Tooltip
                    disableFocusListener
                    placement="top"
                    arrow
                    enterTouchDelay={0}
                    TransitionComponent={Zoom}
                    title={"TIME UNTIL DAILY RESET"}
                  >
                    <Grid container className={styles.tooltipContainer} />
                  </Tooltip>
                </ThemeProvider>
              ))}
            <Grid container className={styles.shadowContainer} />
            <Grid container className={styles.characterImageContainer}>
              <Grid container className={styles.cardBgContainer}>
                <Image
                  src={`/img/unity-assets/shared/window_bg_vertical.png`}
                  height={width > xlScreenWidth ? 512 : 460}
                  width={width > xlScreenWidth ? 386 : 346}
                  quality={100}
                />
              </Grid>
              <Grid container className={styles.bottomLeftOrnamentContainer}>
                <Image
                  src={`/img/unity-assets/shared/window_bottom_left_angled.png`}
                  height={27}
                  width={27}
                  quality={100}
                />
              </Grid>
              <Grid container className={styles.bottomRightOrnamentContainer}>
                <Image
                  src={`/img/unity-assets/shared/window_bottom_right_angled.png`}
                  height={27}
                  width={27}
                  quality={100}
                />
              </Grid>
              <Grid
                container
                className={styles.cardHeaderContainer}
                style={{
                  left: hero.rarity.toLowerCase() === "common" || hero.rarity.toLowerCase() === "rare" ? 3 : 0,
                  top:
                    width > xlScreenWidth
                      ? hero.rarity.toLowerCase() === "common" || hero.rarity.toLowerCase() === "rare"
                        ? 4
                        : 0
                      : hero.rarity.toLowerCase() === "common" || hero.rarity.toLowerCase() === "rare"
                      ? 1
                      : -3,
                }}
              >
                <Image
                  src={`/img/unity-assets/card/card_bg_${hero.rarity.toLowerCase()}_header.png`}
                  height={width > xlScreenWidth ? 58 : 58}
                  width={width > xlScreenWidth ? 397 : 358.65}
                  quality={100}
                />
              </Grid>
              <Grid container className={styles.characterFrame}>
                <Image
                  src={`/img/unity-assets/card/rarity_${hero.rarity.toLowerCase()}.png`}
                  height={width > xlScreenWidth ? 531.3 : 478.17}
                  width={width > xlScreenWidth ? 385 : 346.5}
                  quality={100}
                />
              </Grid>
              <Grid container className={styles.characterLevelBackground}>
                <Image
                  src={`/img/unity-assets/card/level_${hero.rarity.toLowerCase()}.png`}
                  height={width > xlScreenWidth ? 80 : 72}
                  width={width > xlScreenWidth ? 80 : 72}
                  quality={100}
                />
              </Grid>
              <Typography component="span" className={styles.characterName}>
                {`${heroBegin}${hero.name} ${heroEnd}`}
              </Typography>
              <Grid container className={styles.levelContainer}>
                <Typography component="span" className={styles.characterLevel}>
                  {hero.level}
                </Typography>
              </Grid>
              <Grid container className={styles.characterBackground}>
              {!demoModeNotYetMinted && <Image
                  src={`/img/api/heroes/backgrounds/${hero.zoneBackgroundType.toLowerCase()}.jpg`}
                  height={width > xlScreenWidth ? 480 : 419}
                  width={width > xlScreenWidth ? 360 : 323}
                />
              }
              </Grid>
              <Grid container className={styles.opaqueContainer} />
              {/* Couldn't get next/image to render the hero images with high quality. Reverted to img tag. Revisit later. */}
              {!demoModeNotYetMinted && <img
                src={hero.image?`img/api/heroes/${hero.image}`:  `img/api/heroes/${hero.gender.toLowerCase()}-${hero.heroClass.toLowerCase()}-gen${
                  hero.generation
                }.png`}
                className={getImageStyles()}
                style={{ height: getImageHeight(), width: getImageWidth() }}
              />
              }
            </Grid>
          </Grid>
          <Grid item xs={12} className={styles.cardBack}>
            <Grid container className={styles.shadowContainer} />
            <Grid container className={styles.characterImageContainer}>
              <Grid container className={styles.cardBgContainer}>
                <Image
                  src={`/img/unity-assets/shared/window_bg_vertical.png`}
                  height={width > xlScreenWidth ? 512 : 460}
                  width={width > xlScreenWidth ? 386 : 346}
                  quality={100}
                />
              </Grid>
              <Grid container className={styles.bottomLeftOrnamentContainer}>
                <Image
                  src={`/img/unity-assets/shared/window_bottom_left_angled.png`}
                  height={27}
                  width={27}
                  quality={100}
                />
              </Grid>
              <Grid container className={styles.bottomRightOrnamentContainer}>
                <Image
                  src={`/img/unity-assets/shared/window_bottom_right_angled.png`}
                  height={27}
                  width={27}
                  quality={100}
                />
              </Grid>
              <Grid
                container
                className={styles.cardHeaderContainer}
                style={{
                  left: hero.rarity.toLowerCase() === "common" || hero.rarity.toLowerCase() === "rare" ? 3 : 0,
                  top:
                    width > xlScreenWidth
                      ? hero.rarity.toLowerCase() === "common" || hero.rarity.toLowerCase() === "rare"
                        ? 4
                        : 0
                      : hero.rarity.toLowerCase() === "common" || hero.rarity.toLowerCase() === "rare"
                      ? 1
                      : -3,
                }}
              >
                <Image
                  src={`/img/unity-assets/card/card_bg_${hero.rarity.toLowerCase()}_header.png`}
                  height={width > xlScreenWidth ? 58 : 58}
                  width={width > xlScreenWidth ? 397 : 358.65}
                  quality={100}
                />
              </Grid>
              <Grid container className={styles.characterFrame}>
                <Image
                  src={`/img/unity-assets/card/rarity_${hero.rarity.toLowerCase()}.png`}
                  height={width > xlScreenWidth ? 531.3 : 478.17}
                  width={width > xlScreenWidth ? 385 : 346.5}
                  quality={100}
                />
              </Grid>
              <Typography component="span" className={styles.characterName}>
                {`${hero.name}`}
              </Typography>
              <Grid container className={styles.symbolContainer}>
                {hero.skills && renderSkillSymbols()}
              </Grid>
              <Grid
                container
                className={
                  hero.heroClass === CharacterClassDto.Warrior
                    ? styles.warriorTypeSymbolContainer
                    : hero.heroClass === CharacterClassDto.Ranger
                    ? styles.rangerTypeSymbolContainer
                    : styles.mageTypeSymbolContainer
                }
              >
                {renderHeroTypeSymbol()}
              </Grid>
              <Grid container className={styles.statsBackground} />
              <Grid container className={styles.statsContainer}>
                <Grid item xs={5} className={styles.leftColumn}>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      STRENGTH
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      {hero.strength}
                    </Typography>
                  </Grid>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      WISDOM
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      {hero.wisdom}
                    </Typography>
                  </Grid>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      AGILITY
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      {hero.agility}
                    </Typography>
                  </Grid>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      SKILLPOINTS
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      {hero.baseSkillPoints}
                    </Typography>
                  </Grid>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      QUESTS REM.
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      {hero.remainingQuests}
                    </Typography>
                  </Grid>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      RARITY
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      {hero.rarity}
                    </Typography>
                  </Grid>
                </Grid>
                <Grid item xs={5} className={styles.rightColumn}>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      {hero.quickness}
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      QUICKNESS
                    </Typography>
                  </Grid>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      {hero.charisma}
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      CHARISMA
                    </Typography>
                  </Grid>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      {hero.baseHitPoints}
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      TOTAL HP
                    </Typography>
                  </Grid>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      {hero.experiencePoints}
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      CURRENT XP
                    </Typography>
                  </Grid>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      {hero.maxDailyQuests}
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      TOTAL QUESTS
                    </Typography>
                  </Grid>
                  <Grid item xs={12} className={styles.statRow}>
                    <Typography component="span" className={styles.statsText}>
                      {hero.id}
                    </Typography>
                    <Typography component="span" className={styles.statsText}>
                      HERO ID
                    </Typography>
                  </Grid>
                </Grid>
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
      {!disableButtons && (
        <Grid container direction="row" className={styles.buttonContainer}>
          {!disableSelectHeroButton && (
            <Grid item xs={4}>
              <DCXButton
                height={37.5}
                width={demoModeNotYetMinted?150:110}
                color="blue"
                title={hero.seasonId ? "PLAY" : ("REGISTER" + (registerForFree?" FOR FREE":""))}
                onClick={hero.seasonId ? handleSelectClick : handleRegisterClick}
                disabled={isButtonDisabled()}
                hideArrows={true}
                marginRight={5}
              />
            </Grid>
          )}
          {hero.dcxRewards >0 && !demoModeNotYetMinted && (
            <Grid item xs={4}>
              <DCXButton
                height={37.5}
                width={110}
                color="red"
                title={"CLAIM REWARDS"}
                onClick={handleClaimRewardsClick}
                hideArrows={true}
                marginRight={5}
              />
            </Grid>
          )}
          <Grid item xs={4}>
            {!demoModeNotYetMinted && <DCXButton
              height={37.5}
              width={110}
              color="blue"
              title="FLIP CARD"
              onClick={handleFlipClick}
              hideArrows={true}
              // marginLeft={5}
            />
            }
          </Grid>
        </Grid>
      )}
    </Grid>
  );
};

export default HeroCard;
