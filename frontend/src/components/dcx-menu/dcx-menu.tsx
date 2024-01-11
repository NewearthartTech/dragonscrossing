import {
  selectDisplayFooter,
  setDisplayCharacter,
  setDisplayGuildModal,
  setDisplayInventory,
  setDisplayLoadout,
  setDisplaySettings,
  setDisplayStaking,
  setDisplayWallet,
  setPlayButtonClickSound,
  setRefreshHeroNFTs,
} from "@/state-mgmt/app/appSlice";
import { Hero } from "@/state-mgmt/hero/heroTypes";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Grid from "@mui/material/Grid";
import DCXButton from "@/components/dcx-button/dcx-button";
import { useState } from "react";
import styles from "./dcx-menu.module.scss";
import Image from "next/image";
import Modal from "@mui/material/Modal";
import Typography from "@mui/material/Typography";
import { resetSelectedHeroStatus, selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { selectDisableInventory } from "@/state-mgmt/item/itemSlice";
import { apiConfig } from "../hoc/verification";
import { AuthApi, DcxZones } from "@dcx/dcx-backend";
import { useAuthentication } from "../auth";
import { clearGameState, selectGameState } from "@/state-mgmt/game-state/gameStateSlice";
import { getSeasonLeaderboard, setShowLeaderboardModal } from "@/state-mgmt/season/seasonSlice";

interface Props {
  hero: Hero;
}

const DCXMenu: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const selectedHero = useAppSelector(selectSelectedHero);
  const displayFooter = useAppSelector(selectDisplayFooter);
  const isDisableInventory = useAppSelector(selectDisableInventory);
  const gameState = useAppSelector(selectGameState);

  const { updateJWT } = useAuthentication();

  const [showMenu, setShowMenu] = useState(false);

  const handleClick = () => {
    showMenu ? setShowMenu(false) : setShowMenu(true);
  };

  const handleClose = () => {
    setShowMenu(false);
  };

  const handleCharacterClick = () => {
    dispatch(setDisplayCharacter(true));
    dispatch(setPlayButtonClickSound(true));
    handleClose();
  };

  const handleWalletClick = () => {
    dispatch(setDisplayWallet(true));
    dispatch(setPlayButtonClickSound(true));
    handleClose();
  };

  const handleInventoryClick = () => {
    dispatch(setDisplayInventory(true));
    dispatch(setPlayButtonClickSound(true));
    handleClose();
  };

  const handleLoadoutClick = () => {
    dispatch(setDisplayLoadout(true));
    dispatch(setPlayButtonClickSound(true));
    handleClose();
  };

  const handleAdventuringGuildClick = () => {
    dispatch(setDisplayGuildModal(true));
    dispatch(setPlayButtonClickSound(true));
    handleClose();
  };

  const handleLeaderboardClick = () => {
    dispatch(getSeasonLeaderboard({ seasonId: selectedHero.hero.seasonId, heroId: selectedHero.hero.id }));
    dispatch(setPlayButtonClickSound(true));
    dispatch(setShowLeaderboardModal(true));
    handleClose();
  };

  const handleStakingClick = () => {
    dispatch(setDisplayStaking(true));
    dispatch(setPlayButtonClickSound(true));
    handleClose();
  };

  const handleSettingsClick = () => {
    dispatch(setDisplaySettings(true));
    dispatch(setPlayButtonClickSound(true));
    handleClose();
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

  return (
    <Grid container className={styles.container}>
      <DCXButton title="MENU" height={42} width={144} color="red" onClick={handleClick} />
      <Modal open={showMenu} onClose={handleClose} className={styles.modalMain}>
        <Grid container className={styles.modalContainer}>
          {selectedHero && selectedHero.hero.id !== -1 && displayFooter === true ? (
            <Grid container className={styles.modalContainer}>
              <Grid container className={styles.optionsMenuBackground}>
                <Image
                  src="/img/unity-assets/shared/action_bg_vertical.png"
                  height={
                    gameState.zone.slug !== DcxZones.Aedos
                      ? selectedHero.hero.seasonId
                        ? 371
                        : 326
                      : selectedHero.hero.seasonId
                      ? 326
                      : 281
                  }
                  width={240}
                  quality={100}
                />
              </Grid>
              <Grid
                container
                className={
                  gameState.zone.slug !== DcxZones.Aedos
                    ? selectedHero.hero.seasonId
                      ? styles.optionsContainerSeasonLarge
                      : styles.optionsContainerLarge
                    : selectedHero.hero.seasonId
                    ? styles.optionsContainerSeason
                    : styles.optionsContainer
                }
              >
                <Grid container className={styles.option} onClick={handleLoadoutClick}>
                  <Typography component="span" className={styles.firstOptionText}>
                    LOADOUT
                  </Typography>
                </Grid>
                <Grid
                  container
                  className={styles.option}
                  onClick={!isDisableInventory ? handleInventoryClick : undefined}
                >
                  <Grid container className={styles.divider} />
                  <Typography component="span" className={styles.optionText}>
                    INVENTORY
                  </Typography>
                  {isDisableInventory && <Grid container className={styles.disabledContainer} />}
                </Grid>
                <Grid container className={styles.option} onClick={handleWalletClick}>
                  <Grid container className={styles.divider} />
                  <Typography component="span" className={styles.optionText}>
                    WALLET
                  </Typography>
                </Grid>
                <Grid container className={styles.option} onClick={handleCharacterClick}>
                  <Grid container className={styles.divider} />
                  <Typography component="span" className={styles.optionText}>
                    CHARACTER
                  </Typography>
                </Grid>
                {/* <Grid
                  container
                  className={styles.option}
                  onClick={handleStakingClick}
                >
                  <Grid container className={styles.divider} />
                  <Typography component="span" className={styles.optionText}>
                    STAKING
                  </Typography>
                </Grid> */}
                {gameState.zone.slug !== DcxZones.Aedos && (
                  <Grid container className={styles.option} onClick={handleAdventuringGuildClick}>
                    <Grid container className={styles.divider} />
                    <Typography component="span" className={styles.optionText}>
                      ADVENTURING GUILD
                    </Typography>
                  </Grid>
                )}
                {selectedHero.hero.seasonId && (
                  <Grid container className={styles.option} onClick={handleLeaderboardClick}>
                    <Grid container className={styles.divider} />
                    <Typography component="span" className={styles.optionText}>
                      LEADERBOARD
                    </Typography>
                  </Grid>
                )}
                <Grid container className={styles.option} onClick={handleGoToHeroSelectClick}>
                  <Grid container className={styles.divider} />
                  <Typography component="span" className={styles.optionText}>
                    HERO SELECT
                  </Typography>
                </Grid>
                <Grid container className={styles.option} onClick={handleSettingsClick}>
                  <Grid container className={styles.divider} />
                  <Typography component="span" className={styles.optionText}>
                    SETTINGS
                  </Typography>
                </Grid>
              </Grid>
            </Grid>
          ) : (
            <Grid container className={styles.modalContainerSmall}>
              <Grid container className={styles.optionsMenuBackground}>
                <Image src="/img/unity-assets/shared/tooltip_bg.png" height={92} width={240} quality={100} />
              </Grid>
              <Grid container className={styles.optionsContainerSmall}>
                {/* <Grid container className={styles.option} onClick={handleStakingClick}>
                  <Typography component="span" className={styles.firstOptionText}>
                    STAKING
                  </Typography>
                </Grid> */}
                <Grid container className={styles.option} onClick={handleAdventuringGuildClick}>
                  <Typography component="span" className={styles.firstOptionText}>
                    ADVENTURING GUILD
                  </Typography>
                </Grid>
                <Grid container className={styles.option} onClick={handleSettingsClick}>
                  <Grid container className={styles.divider} />
                  <Typography component="span" className={styles.optionText}>
                    SETTINGS
                  </Typography>
                </Grid>
              </Grid>
            </Grid>
          )}
        </Grid>
      </Modal>
    </Grid>
  );
};

export default DCXMenu;
