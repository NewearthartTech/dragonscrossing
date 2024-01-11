import Grid from "@mui/material/Grid";
import Modal from "@mui/material/Modal";
import styles from "./achievement-modal.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { selectDisplayAchievementModal, setDisplayAchievementModal } from "@/state-mgmt/app/appSlice";
import Image from "next/image";
import CloseButton from "../close-button/close-button";
import Typography from "@mui/material/Typography";
import { selectSelectedHero } from "@/state-mgmt/hero/heroSlice";

interface Props {}

const AchievementModal: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();

  const displayAchievementModal = useAppSelector(selectDisplayAchievementModal);
  const selectedHero = useAppSelector(selectSelectedHero).hero;

  const handleClose = () => {
    dispatch(setDisplayAchievementModal(false));
  };

  return (
    <Modal open={displayAchievementModal} onClose={handleClose} className={styles.modalMain}>
      <Grid container className={styles.container}>
        <Image src="/img/unity-assets/shared/action_bg.png" height={260} width={390} quality={100} />
        <Grid container className={styles.headerContainer}>
          <Image src="/img/unity-assets/shared/header_gold.png" height={57} width={247} quality={100} />
        </Grid>
        <Grid container className={styles.topLeftOrnament}>
          <Image src="/img/unity-assets/shared/window_top_left.png" height={50} width={70} quality={100} />
        </Grid>
        <Grid container className={styles.topRightOrnament}>
          <Image src="/img/unity-assets/shared/window_top_right.png" height={50} width={70} quality={100} />
        </Grid>
        <Grid container className={styles.bottomLeftOrnament}>
          <Image src="/img/unity-assets/shared/window_bottom_left.png" height={30} width={43} quality={100} />
        </Grid>
        <Grid container className={styles.bottomRightOrnament}>
          <Image src="/img/unity-assets/shared/window_bottom_right.png" height={30} width={43} quality={100} />
        </Grid>
        <Typography component="span" className={styles.headerText}>
          ACHIEVEMENT
        </Typography>
        <Grid container className={styles.closeButtonContainer}>
          <CloseButton handleClose={handleClose} />
        </Grid>
        <Grid container className={styles.messageContentContainer}>
          <Grid container className={styles.achievementRow}>
            <Typography component="span" className={styles.achievementTitle}>{`ABADDON THE DESTROYER`}</Typography>
            <Typography component="span" className={styles.achievementSeparator}>{`-`}</Typography>
            <Typography component="span" className={styles.achievementText}>{`DEFEATED`}</Typography>
          </Grid>
          <Grid container className={styles.messageContainer}>
            <Typography component="span" className={styles.achievementMessage}>
              {`CONGRATULATIONS `}
              <span className={styles.championName}>{selectedHero.name}</span>
              {` YOUR HEROICS HAVE BROUGHT PEACE FROM THE THREAT OF ABADDON TO ALL RESIDENTS OF HORIZON! HOWEVER, THERE IS STILL EVIL TO VANQUISH AND RARE ARTIFACTS TO FIND. SO GOOD LUCK CHAMPION AND MAY FORTUNE FOLLOW YOU WHEREVER YOU GO!`}
            </Typography>
          </Grid>

          <Typography component="span" className={styles.questMessage}>
            {`ANY QUESTS SPENT FOR THE REMAINDER OF THE SEASON WILL NOT AFFECT YOUR SEASON LEADERBOARD STATUS`}
          </Typography>
        </Grid>
      </Grid>
    </Modal>
  );
};

export default AchievementModal;
