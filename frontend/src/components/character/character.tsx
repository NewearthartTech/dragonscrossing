import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import styles from "./character.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { selectDisplayCharacter, setDisplayCharacter } from "@/state-mgmt/app/appSlice";
import HeroCard from "../hero-card/hero-card";
import HealthBar from "../health-bar/health-bar";
import XPBar from "../xp-bar/xp-bar";
import { selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import Image from "next/image";
import useWindowDimensions from "../../helpers/window-dimensions";
import {
  agilityDescription,
  charismaDescription,
  quicknessDescription,
  smScreenWidth,
  strengthDescription,
  tooltipTheme,
  wisdomDescription,
  xlScreenWidth,
} from "@/helpers/global-constants";
import { useState } from "react";
import { LearnedHeroSkill } from "@dcx/dcx-backend";
import CloseButton from "../close-button/close-button";
import { calculateTotalChanceToDodge } from "@/helpers/helper-functions";
import { ThemeProvider } from "@mui/material/styles";
import Tooltip from "@mui/material/Tooltip";
import Zoom from "@mui/material/Zoom";
import InfoIcon from "@mui/icons-material/Info";
import Scrollbars from "react-custom-scrollbars";

interface Props {}

const Character: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const displayCharacter = useAppSelector(selectDisplayCharacter);
  const hero = useAppSelector(selectSelectedHero).hero;
  const { width, height } = useWindowDimensions();

  const [showStatDescription, setShowStatDescription] = useState(false);

  const handleToggleStatModal = () => {
    showStatDescription ? setShowStatDescription(false) : setShowStatDescription(true);
  };

  const handleClose = () => dispatch(setDisplayCharacter(false));

  const renderLearnedSkills = () => {
    const skills: JSX.Element[] = [];
    hero.skills.forEach((skill: LearnedHeroSkill) => {
      skills.push(
        <Grid container direction="row" className={styles.skillRow} key={skill.id}>
          <Grid item xs={7}>{`${skill.name}`}</Grid>
          <Grid item xs={3}>
            <ThemeProvider theme={tooltipTheme}>
              <Tooltip
                disableFocusListener
                placement="top"
                arrow
                TransitionComponent={Zoom}
                enterTouchDelay={0}
                title={skill.description}
              >
                <span className={styles.skillDescription}>
                  <InfoIcon fontSize="small" />
                </span>
              </Tooltip>
            </ThemeProvider>
          </Grid>
          <Grid item xs={2} className={styles.uses}>{`${
            skill.skillUseInstance.filter((sui) => !sui.alreadyUsed).length
          }`}</Grid>
        </Grid>
      );
    });
    return skills;
  };

  return (
    <Modal open={displayCharacter} onClose={handleClose} className={styles.modalMain}>
      <Grid container direction="row" className={styles.characterContainer}>
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
          <Grid container direction="row" className={styles.subContainer}>
            <Grid container className={styles.cardContainer}>
              <Grid container className={styles.windowLeftBackground}>
                <Image
                  src="/img/unity-assets/shared/window_bg_left.png"
                  height={width > xlScreenWidth ? 680 : 620}
                  width={width > xlScreenWidth ? 444 : 385}
                  quality={100}
                />
              </Grid>
              {width <= smScreenWidth && (
                <Grid container className={styles.closeButtonContainerSmall}>
                  <CloseButton handleClose={handleClose} />
                </Grid>
              )}
              <Grid container className={styles.heroCardContainer}>
                <HeroCard hero={hero} disableButtons={true} />
              </Grid>
              <Grid container className={styles.hpBar}>
                <HealthBar
                  hero={hero}
                  backgroundBarHeight={65}
                  backgroundBarWidth={width > xlScreenWidth ? 375 : 340}
                  trimmedWidth={width > xlScreenWidth ? 351 : 318}
                  barHeight={22}
                />
              </Grid>
              <Grid container className={styles.xpBar}>
                <XPBar
                  hero={hero}
                  backgroundBarHeight={65}
                  backgroundBarWidth={width > xlScreenWidth ? 375 : 340}
                  trimmedWidth={width > xlScreenWidth ? 351 : 318}
                  barHeight={22}
                />
              </Grid>
            </Grid>
            <Grid container direction="column" className={styles.attributesContainer}>
              <Grid container className={styles.statsContainer}>
                <Grid container className={styles.windowRightBackgroundStats}>
                  <Image
                    src="/img/unity-assets/character/stats_bg.png"
                    height={width > xlScreenWidth ? 381 : 350}
                    width={width > xlScreenWidth ? 444 : 376}
                    quality={100}
                  />
                </Grid>
                {width > smScreenWidth && (
                  <Grid container className={styles.closeButtonContainer}>
                    <CloseButton handleClose={handleClose} />
                  </Grid>
                )}
                <Grid container className={styles.statsHeaderContainer}>
                  <Grid container className={styles.statsHeaderBackground}>
                    <Image src="/img/unity-assets/shared/header.png" height={35} width={240} quality={100} />
                  </Grid>
                  <Typography component="span" className={styles.statsHeader}>
                    STATS
                  </Typography>
                  {/* Commenting out stat descriptions */}
                  {/* {showStatDescription && (
                    <Grid container className={styles.statDescriptionButtonMinusContainer} onClick={handleToggleStatModal}>
                      <Image
                        src="/img/unity-assets/shared/minus_icon.png"
                        height={10}
                        width={18}
                        quality={100}
                        className={styles.clickableImage}
                      />
                    </Grid>
                  )}
                  {!showStatDescription && (
                    <Grid container className={styles.statDescriptionButtonPlusContainer} onClick={handleToggleStatModal}>
                      <Image
                        src="/img/unity-assets/shared/plus_icon.png"
                        height={18}
                        width={18}
                        quality={100}
                        className={styles.clickableImage}
                      />
                    </Grid>
                  )} */}
                </Grid>
                {showStatDescription && (
                  <Grid container className={styles.statDescriptionContainer}>
                    <Grid container className={styles.statDescriptionShadowContainer} />
                    <Image src="/img/unity-assets/shared/window_bg_right.png" height={260} width={374} quality={100} />
                    <Grid container direction="column" className={styles.descriptionsContainer}>
                      <Grid container direction="row" className={styles.descriptionRow}>
                        <Typography component="span" className={styles.descriptionText}>
                          <span className={styles.descriptionHeader}>STRENGTH</span>
                          {` - ${strengthDescription}`}
                        </Typography>
                      </Grid>
                      <Grid container direction="row" className={styles.descriptionRow}>
                        <Typography component="span" className={styles.descriptionText}>
                          <span className={styles.descriptionHeader}>WISDOM</span>
                          {` - ${wisdomDescription}`}
                        </Typography>
                      </Grid>
                      <Grid container direction="row" className={styles.descriptionRow}>
                        <Typography component="span" className={styles.descriptionText}>
                          <span className={styles.descriptionHeader}>AGILITY</span>
                          {` - ${agilityDescription}`}
                        </Typography>
                      </Grid>
                      <Grid container direction="row" className={styles.descriptionRow}>
                        <Typography component="span" className={styles.descriptionText}>
                          <span className={styles.descriptionHeader}>QUICKNESS</span>
                          {` - ${quicknessDescription}`}
                        </Typography>
                      </Grid>
                      <Grid container direction="row" className={styles.descriptionRow}>
                        <Typography component="span" className={styles.descriptionText}>
                          <span className={styles.descriptionHeader}>CHARISMA</span>
                          {` - ${charismaDescription}`}
                        </Typography>
                      </Grid>
                    </Grid>
                  </Grid>
                )}
                <Grid container direction="row" className={styles.statContainer}>
                  <Grid container direction="row" className={styles.categoryRow}>
                    <Typography component="span" variant="body1" className={styles.categoryHeader}>
                      {`ATTRIBUTES`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`STRENGTH`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`${hero.calculatedStats.strength}/${hero.strength}`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`WISDOM`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`${hero.calculatedStats.wisdom}/${hero.wisdom}`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`AGILITY`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`${hero.calculatedStats.agility}/${hero.agility}`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`QUICKNESS`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`${hero.calculatedStats.quickness}/${hero.quickness}`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`CHARISMA`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`${hero.calculatedStats.charisma}/${hero.charisma}`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`SKILL POINTS`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`${hero.baseSkillPoints - hero.usedUpSkillPoints}/${hero.baseSkillPoints}`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.categoryRow}>
                    <Typography component="span" variant="body1" className={styles.categoryHeader}>
                      {`ATTACK`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`DAMAGE`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`${hero.calculatedStats.damageRange.lower}-${hero.calculatedStats.damageRange.upper}`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`CHANCE TO HIT BONUS`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`${hero.calculatedStats.chanceToHit / 100}%`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`CHANCE TO CRIT`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`${hero.calculatedStats.chanceToCrit / 100}%`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.categoryRow}>
                    <Typography component="span" variant="body1" className={styles.categoryHeader}>
                      {`DEFENSE`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`ARMOR MITIGATION AMOUNT`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`${hero.calculatedStats.armorMitigationAmount}`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`ARMOR MITIGATION PROC CHANCE`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`${hero.calculatedStats.armorMitigation / 100}%`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`CHANCE TO DODGE BONUS`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.stat}>
                      {`${calculateTotalChanceToDodge(hero)}%`}
                    </Typography>
                  </Grid>
                  <Grid container direction="row" className={styles.statRow}>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`CHANCE TO PARRY`}
                    </Typography>
                    <Typography component="span" variant="body1" className={styles.statWhite}>
                      {`${hero.calculatedStats.chanceToParry / 100}%`}
                    </Typography>
                  </Grid>
                </Grid>
              </Grid>
              <Grid container className={styles.skillsContainer}>
                <Grid container className={styles.windowRightBackgroundSkills}>
                  <Image
                    src="/img/unity-assets/character/skills_bg.png"
                    height={width > xlScreenWidth ? 265 : 238}
                    width={width > xlScreenWidth ? 444 : 376}
                    quality={100}
                  />
                </Grid>
                <Grid container className={styles.skillsHeaderContainer}>
                  <Grid container className={styles.skillsHeaderBackground}>
                    <Image src="/img/unity-assets/shared/header.png" height={35} width={240} quality={100} />
                  </Grid>
                  <Typography component="span" className={styles.skillsHeader}>
                    SKILLS
                  </Typography>
                </Grid>
                <Grid container className={styles.skillCategoryHeaders}>
                  <Grid item xs={7}>{`NAME`}</Grid>
                  <Grid item xs={3}>{`DESCRIPTION`}</Grid>
                  <Grid item xs={2} className={styles.uses}>{`USES`}</Grid>
                </Grid>
                <Grid container className={styles.skillDivider} />
                <Grid container className={styles.skills}>
                  {hero.skills && renderLearnedSkills()}
                </Grid>
              </Grid>
            </Grid>
          </Grid>
        </Scrollbars>
      </Grid>
    </Modal>
  );
};

export default Character;
