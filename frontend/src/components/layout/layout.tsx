/* eslint-disable  @next/next/no-sync-scripts */
// we are disabling the external script rule cause we want to load the ENV variables using a script
import Grid from "@mui/material/Grid";
import Head from "next/head";
import React, { ReactNode, useEffect, useState } from "react";
import Footer from "@/components/footer/footer";
import styles from "./layout.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { getSelectedHero, selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { selectDisplayFooter } from "@/state-mgmt/app/appSlice";
import CharacterSelectFooter from "../character-select-footer/character-select-footer";
import Image from "next/image";
import Typography from "@mui/material/Typography";
import { selectConnectedWalletAddress } from "@/state-mgmt/player/playerSlice";
import useWindowDimensions from "@/helpers/window-dimensions";
import { tooltipTheme, xsScreenWidth } from "@/helpers/global-constants";
import { useRouter } from "next/router";
import { AppVersion } from "../../version";
import { ThemeProvider } from "@mui/material/styles";
import Tooltip from "@mui/material/Tooltip";
import Zoom from "@mui/material/Zoom";
import LogoutIcon from "@mui/icons-material/Logout";

interface Props {
  children: ReactNode | any;
}

const Layout = ({ children }: Props) => {
  const dispatch = useAppDispatch();
  const selectedHero = useAppSelector(selectSelectedHero).hero;
  const displayFooter = useAppSelector(selectDisplayFooter);
  const connectedWalletAddress = useAppSelector(selectConnectedWalletAddress);

  const { width, height } = useWindowDimensions();
  const router = useRouter();

  const [startTimer, setStartTimer] = useState(false);
  const [timeUntilReset, setTimeUntilReset] = useState("");
  const [heroSlowPolling, setHeroSlowPolling] = useState(false);
  const [heroFastPolling, setHeroFastPolling] = useState(false);

  useEffect(() => {
    if (selectedHero && selectedHero.id !== -1 && selectedHero.timeTillNextReset) {
      setStartTimer(true);
      setTimeUntilReset(selectedHero.timeTillNextReset.toString().substring(0, 8));
    }
  }, [selectedHero]);

  useEffect(() => {
    if (selectedHero && selectedHero.id !== -1) {
      if (heroSlowPolling) {
        let timer2 = setInterval(function () {
          dispatch(getSelectedHero());
        }, 60000);
        return () => {
          clearInterval(timer2);
        };
      }
    } else {
      setHeroSlowPolling(false);
    }
  }, [heroSlowPolling, selectedHero]);

  useEffect(() => {
    if (selectedHero && selectedHero.id !== -1) {
      if (heroFastPolling) {
        let timer2 = setInterval(function () {
          dispatch(getSelectedHero());
        }, 10000);
        return () => {
          clearInterval(timer2);
        };
      }
    } else {
      setHeroFastPolling(false);
    }
  }, [heroFastPolling, selectedHero]);

  useEffect(() => {
    if (selectedHero && selectedHero.id !== -1) {
      if (startTimer && selectedHero.timeTillNextReset) {
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

          if (hour === 0 && minute <= 20 && minute >= 1) {
            setHeroSlowPolling(true);
          } else {
            if (heroSlowPolling === true) {
              setHeroSlowPolling(false);
            }
          }

          if ((hour === 0 && minute < 1) || (hour === 22 && minute >= 59 && second >= 30)) {
            setHeroFastPolling(true);
          } else {
            if (heroFastPolling === true) {
              setHeroFastPolling(false);
            }
          }
        }, 1000);
        return () => {
          clearInterval(timer1);
        };
      }
    } else {
      setStartTimer(false);
    }
  }, [startTimer, timeUntilReset]);

  const handleDisconnectWallet = () => {
    localStorage.removeItem("login_key");
    router.reload();
  };

  let showFullFooter = false;
  if (selectedHero && selectedHero.id !== -1 && displayFooter === true) {
    showFullFooter = true;
  }

  return (
    <Grid container className={styles.main}>
      <Head>
        <title>{`Dragon's Crossing`}</title>
        <meta name="viewport" content="initial-scale=1, width=device-width" />
        <link rel="icon" href="/img/brand-assets/dcx-logo.png" />
      </Head>

      {router.pathname !== "/mint" && router.pathname !== "/boosted-mint" && (
        <Grid container direction="column" className={styles.container}>
          {selectedHero && selectedHero.id !== -1 && (
            <Grid container className={styles.dailyResetTimeContainer}>
              <Image src="/img/unity-assets/shared/text_bg_cropped.png" height={34} width={80} quality={100} />
              <Typography component="span" className={styles.timeText}>
                {timeUntilReset}
              </Typography>
            </Grid>
          )}
          {selectedHero && selectedHero.id !== -1 && (
            <ThemeProvider theme={tooltipTheme}>
              <Tooltip
                disableFocusListener
                placement="right"
                arrow
                enterTouchDelay={0}
                TransitionComponent={Zoom}
                title={"TIME UNTIL DAILY RESET"}
              >
                <Grid container className={styles.tooltipContainer} />
              </Tooltip>
            </ThemeProvider>
          )}
          <Grid container className={styles.headerContainer}>
            <Image
              src="/img/unity-assets/shared/info_text_bg.png"
              height={24}
              width={width <= xsScreenWidth ? 120 : 340}
              quality={100}
            />
            <Typography component="span" className={styles.addressText}>
              {width <= xsScreenWidth
                ? connectedWalletAddress.substring(0, 5) +
                  "..." +
                  connectedWalletAddress.substring(connectedWalletAddress.length - 5)
                : connectedWalletAddress}
            </Typography>
          </Grid>
          <Grid container className={styles.appVersionContainer}>
            <Image src="/img/unity-assets/shared/text_bg_cropped.png" height={34} width={100} quality={100} />
            <Typography component="span" className={styles.timeText}>
              {`V. ${AppVersion}`}
            </Typography>
          </Grid>
          <ThemeProvider theme={tooltipTheme}>
            <Tooltip
              disableFocusListener
              placement="right"
              arrow
              enterTouchDelay={0}
              TransitionComponent={Zoom}
              title={"DISCONNECT WALLET"}
            >
              <Grid container className={styles.disconnectWalletContainer} onClick={handleDisconnectWallet}>
                <LogoutIcon />
              </Grid>
            </Tooltip>
          </ThemeProvider>
          <Grid item xs={12} className={showFullFooter ? styles.page : styles.pageNoFooter}>
            {children}
          </Grid>
          {showFullFooter ? (
            <Grid item xs={12}>
              <Footer />
            </Grid>
          ) : (
            <Grid item xs={12}>
              <CharacterSelectFooter />
            </Grid>
          )}
        </Grid>
      )}
      {(router.pathname === "/mint" || router.pathname === "/boosted-mint") && <Grid container>{children} </Grid>}
    </Grid>
  );
};

export default Layout;
