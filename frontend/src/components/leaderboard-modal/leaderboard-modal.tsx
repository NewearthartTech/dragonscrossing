import Grid from "@mui/material/Grid";
import Modal from "@mui/material/Modal";
import styles from "./leaderboard-modal.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Image from "next/image";
import CloseButton from "../close-button/close-button";
import {
  selectLeaderboard,
  selectShowLeaderboardModal,
  setShowLeaderboardModal,
} from "@/state-mgmt/season/seasonSlice";
import { selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import useWindowDimensions from "@/helpers/window-dimensions";
import { smScreenWidth, tooltipTheme } from "@/helpers/global-constants";
import Typography from "@mui/material/Typography";
import { ThemeProvider } from "@mui/material/styles";
import Tooltip from "@mui/material/Tooltip";
import Zoom from "@mui/material/Zoom";
import InfoIcon from "@mui/icons-material/Info";

interface Props {}

const LeaderboardModal: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const showLeaderboardModal = useAppSelector(selectShowLeaderboardModal);
  const hero = useAppSelector(selectSelectedHero).hero;
  const leaderboard = useAppSelector(selectLeaderboard);
  const { width, height } = useWindowDimensions();

  const handleClose = () => {
    dispatch(setShowLeaderboardModal(false));
  };

  const renderLeaderboardRows = () => {
    const rows: JSX.Element[] = [];
    let count = 0;

    leaderboard.heroRanks.forEach((hr) => {
      const cellTextStyle =
        hr.heroId === hero.id ? styles.selectedHeroText : count % 2 === 0 ? styles.cellText : styles.cellTextGray;
      rows.push(
        <Grid container direction="row" className={styles.row} key={hr.heroId}>
          <Grid container className={styles.rankCol}>
            <Typography component="span" className={cellTextStyle}>
              {hr.rank}
            </Typography>
          </Grid>
          <Grid container className={styles.heroIdCol}>
            <Typography component="span" className={cellTextStyle}>
              {hr.heroId}
            </Typography>
          </Grid>
          <Grid container className={styles.levelCol}>
            <Typography component="span" className={cellTextStyle}>
              {hr.level}
            </Typography>
          </Grid>
          <Grid container className={styles.finalBossDefeatedCol}>
            <Typography component="span" className={cellTextStyle}>
              {hr.isFinalBossDefeated ? "YES" : "NO"}
            </Typography>
          </Grid>
          <Grid container className={styles.furthestZoneDiscoveredCol}>
            <Typography component="span" className={cellTextStyle}>
              {hr.farthestZoneDiscovered}
            </Typography>
          </Grid>
          <Grid container className={styles.benchmarkQuestsUsedCol}>
            <Typography component="span" className={cellTextStyle}>
              {hr.benchmarkQuestsUsed}
            </Typography>
          </Grid>
          <Grid container className={styles.totalQuestsUsedCol}>
            <Typography component="span" className={cellTextStyle}>
              {hr.currentTotalUsedQuests}
            </Typography>
          </Grid>
        </Grid>
      );
      count++;
    });

    return rows;
  };

  return (
    <Modal open={showLeaderboardModal} onClose={handleClose} className={styles.modalMain}>
      <Grid container className={styles.container}>
        <Image
          src="/img/unity-assets/shared/action_bg.png"
          height={335}
          width={width <= smScreenWidth ? 390 : 810}
          quality={100}
        />
        <Grid container className={styles.closeButtonContainer}>
          <CloseButton handleClose={handleClose} />
        </Grid>
        <Grid container className={styles.headerTitleContainer}>
          <Image src="/img/unity-assets/shared/header_gold.png" height={45} width={197} quality={100} />
          <Typography component="span" className={styles.headerTitle}>
            LEADERBOARD
          </Typography>
        </Grid>
        <Grid container className={styles.infoIconContainer}>
          <ThemeProvider theme={tooltipTheme}>
            <Tooltip
              disableFocusListener
              placement="top"
              arrow
              TransitionComponent={Zoom}
              enterTouchDelay={0}
              title={"LEADERBOARD IS UPDATED EVERY MINUTE"}
            >
              <span className={styles.refreshDescription}>
                <InfoIcon fontSize="small" />
              </span>
            </Tooltip>
          </ThemeProvider>
        </Grid>
        <Grid container className={styles.headerContainer}>
          <Grid container className={styles.seasonNameContainer}>
            <Typography component="span" className={styles.titleText}>
              {width <= smScreenWidth ? "SEASON:" : "SEASON NAME:"}
            </Typography>
            <Typography component="span" className={styles.titleTextData}>
              {leaderboard.seasonName}
            </Typography>
          </Grid>
          <Grid container className={styles.registedHeroesContainer}>
            <Typography component="span" className={styles.titleText}>
              {width <= smScreenWidth ? "REGISTERED:" : "REGISTERED HEROES:"}
            </Typography>
            <Typography component="span" className={styles.titleTextData}>
              {leaderboard.heroesInSeason}
            </Typography>
          </Grid>
        </Grid>
        <Grid container className={styles.contentContainer}>
          <Grid container direction="row" className={styles.row}>
            <Grid container className={styles.rankCol}>
              <Typography component="span" className={styles.headerText}>
                {width <= smScreenWidth ? `RNK` : `RANK`}
              </Typography>
            </Grid>
            <Grid container className={styles.heroIdCol}>
              <Grid container className={width <= smScreenWidth ? styles.hidColumnDivider : styles.columnDivider} />
              <Typography component="span" className={styles.headerText}>
                {width <= smScreenWidth ? `HID` : `HERO ID`}
              </Typography>
            </Grid>
            <Grid container className={styles.levelCol}>
              <Grid container className={styles.columnDivider} />
              <Typography component="span" className={styles.headerText}>
                {width <= smScreenWidth ? `LVL` : `LEVEL`}
              </Typography>
            </Grid>
            <Grid container className={styles.finalBossDefeatedCol}>
              <Grid container className={styles.columnDivider} />
              <Typography component="span" className={styles.headerText}>
                {width <= smScreenWidth ? `FBD` : `FINAL BOSS DEFEATED`}
              </Typography>
            </Grid>
            <Grid container className={styles.furthestZoneDiscoveredCol}>
              <Grid container className={width <= smScreenWidth ? styles.fzdColumnDivider : styles.columnDivider} />
              <Typography component="span" className={styles.headerText}>
                {width <= smScreenWidth ? `FZD` : `FURTHEST ZONE DISCOVERED`}
              </Typography>
            </Grid>
            <Grid container className={styles.benchmarkQuestsUsedCol}>
              <Grid container className={styles.columnDivider} />
              <Typography component="span" className={styles.headerText}>
                {width <= smScreenWidth ? `BQU` : `BENCHMARK QUESTS USED`}
              </Typography>
            </Grid>
            <Grid container className={styles.totalQuestsUsedCol}>
              <Grid container className={styles.columnDivider} />
              <Typography component="span" className={styles.headerText}>
                {width <= smScreenWidth ? `TQU` : `TOTAL QUESTS USED`}
              </Typography>
            </Grid>
          </Grid>
          <Grid container className={styles.headerDivider} />
          {renderLeaderboardRows()}
        </Grid>
      </Grid>
    </Modal>
  );
};

export default LeaderboardModal;
