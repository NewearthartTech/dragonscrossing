import { setDisplayGuildModal, setDisplaySettings, setDisplayStaking } from "@/state-mgmt/app/appSlice";
import { selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import Grid from "@mui/material/Grid";
import useWindowDimensions from "@/helpers/window-dimensions";
import styles from "./character-select-footer.module.scss";
import Image from "next/image";
import { mdScreenWidth, tooltipTheme } from "@/helpers/global-constants";
import DCXMenu from "../dcx-menu/dcx-menu";
import { ThemeProvider } from "@mui/material/styles";
import Tooltip from "@mui/material/Tooltip";
import Zoom from "@mui/material/Zoom";

interface Props {}

const CharacterSelectFooter: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const selectedHero = useAppSelector(selectSelectedHero);

  const { width, height } = useWindowDimensions();

  const handleStakingClick = () => {
    dispatch(setDisplayStaking(true));
  };

  const handleAdventuringGuildClick = () => {
    dispatch(setDisplayGuildModal(true));
  };

  const handleSettingsClick = () => {
    dispatch(setDisplaySettings(true));
  };

  return (
    <Grid container direction="row" className={styles.container}>
      <Grid container className={styles.background} />
      <Grid container className={styles.divider} />
      {width > mdScreenWidth ? (
        <Grid container className={styles.contentContainer}>
          <Grid item xs={12} className={styles.buttonContainer}>
            <Grid container direction="row" className={styles.buttonIconContainer}>
              {/* <Grid
                container
                className={[styles.iconButton, styles.buttonPadding].join(" ")}
              >
                <Image
                  src="/img/unity-assets/shared/staking_button.png"
                  height={51}
                  width={60}
                  quality={100}
                  onClick={handleStakingClick}
                  className={styles.footerButtonImage}
                />
              </Grid> */}
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
          <DCXMenu hero={selectedHero.hero} />
        </Grid>
      )}
    </Grid>
  );
};

export default CharacterSelectFooter;
