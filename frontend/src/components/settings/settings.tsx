import Grid from "@mui/material/Grid";
import Modal from "@mui/material/Modal";
import styles from "./settings.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { selectDisplaySettings, setDisplaySettings } from "@/state-mgmt/app/appSlice";
import Image from "next/image";
import {
  selectPlayer,
  selectPlayerSettings,
  setAutoRoll,
  setMusic,
  setMusicVolume,
  setPlayerSettingsInLocalStorage,
  setSoundEffects,
  setSoundEffectsVolume,
  setVoice,
  setVoiceVolume,
} from "@/state-mgmt/player/playerSlice";
import CloseButton from "../close-button/close-button";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import Typography from "@mui/material/Typography";
import Switch from "@mui/material/Switch";
import Slider from "@mui/material/Slider";
import { useEffect, useState } from "react";
import DCXButton from "../dcx-button/dcx-button";

interface Props {}

const Settings: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const displaySettings = useAppSelector(selectDisplaySettings);
  // const player = useAppSelector(selectPlayer);
  const playerSettings = useAppSelector(selectPlayerSettings);

  const [autoRollLocal, setAutoRollLocal] = useState(false);
  const [musicLocal, setMusicLocal] = useState(false);
  const [voiceLocal, setVoiceLocal] = useState(false);
  const [soundEffectsLocal, setSoundEffectsLocal] = useState(false);
  const [musicVolLocal, setMusicVolLocal] = useState(0);
  const [voiceVolLocal, setVoiceVolLocal] = useState(0);
  const [soundEffectsVolLocal, setSoundEffectsVolLocal] = useState(0);

  const switchTheme = createTheme({
    components: {
      MuiSwitch: {
        styleOverrides: {
          switchBase: {
            color: "rgb(230, 230, 230)",
          },
          colorPrimary: {
            "&.Mui-checked": {
              color: "rgb(230, 230, 230)",
            },
          },
          track: {
            opacity: 0.3,
            backgroundColor: "rgb(230, 230, 230)",
            ".Mui-checked.Mui-checked + &": {
              opacity: 0.7,
              backgroundColor: "rgb(185, 143, 36)",
            },
          },
        },
      },
    },
  });

  const musicSliderTheme = createTheme({
    components: {
      MuiSlider: {
        styleOverrides: {
          thumb: {
            "&.MuiSlider-thumb": {
              color: musicLocal ? "rgb(230, 230, 230)" : "rgb(175, 175, 175)",
              pointerEvents: "none",
            },
          },
          track: {
            "&.MuiSlider-track": {
              color: musicLocal ? "rgb(185, 143, 36)" : "rgb(135, 103, 28)",
              pointerEvents: "none",
            },
          },
          rail: {
            "&.MuiSlider-rail": {
              color: musicLocal ? "rgb(230, 230, 230)" : "rgb(175, 175, 175)",
              pointerEvents: "none",
            },
          },
        },
      },
    },
  });

  const voiceSliderTheme = createTheme({
    components: {
      MuiSlider: {
        styleOverrides: {
          thumb: {
            "&.MuiSlider-thumb": {
              color: voiceLocal ? "rgb(230, 230, 230)" : "rgb(175, 175, 175)",
            },
          },
          track: {
            "&.MuiSlider-track": {
              color: voiceLocal ? "rgb(185, 143, 36)" : "rgb(135, 103, 28)",
            },
          },
          rail: {
            "&.MuiSlider-rail": {
              color: voiceLocal ? "rgb(230, 230, 230)" : "rgb(175, 175, 175)",
            },
          },
        },
      },
    },
  });

  const soundEffectsSliderTheme = createTheme({
    components: {
      MuiSlider: {
        styleOverrides: {
          thumb: {
            "&.MuiSlider-thumb": {
              color: soundEffectsLocal ? "rgb(230, 230, 230)" : "rgb(175, 175, 175)",
            },
          },
          track: {
            "&.MuiSlider-track": {
              color: soundEffectsLocal ? "rgb(185, 143, 36)" : "rgb(135, 103, 28)",
            },
          },
          rail: {
            "&.MuiSlider-rail": {
              color: soundEffectsLocal ? "rgb(230, 230, 230)" : "rgb(175, 175, 175)",
            },
          },
        },
      },
    },
  });

  useEffect(() => {
    if (displaySettings === true) {
      setAutoRollLocal(playerSettings.autoRoll);
      setMusicLocal(playerSettings.playMusic);
      setMusicVolLocal(playerSettings.musicVolume);
      setVoiceLocal(playerSettings.playVoice);
      setVoiceVolLocal(playerSettings.voiceVolume);
      setSoundEffectsLocal(playerSettings.playSoundEffects);
      setSoundEffectsVolLocal(playerSettings.soundEffectsVolume);
    }
  }, [displaySettings]);

  useEffect(() => {
    setAutoRollLocal(playerSettings.autoRoll);
    setMusicLocal(playerSettings.playMusic);
    setMusicVolLocal(playerSettings.musicVolume);
    setVoiceLocal(playerSettings.playVoice);
    setVoiceVolLocal(playerSettings.voiceVolume);
    setSoundEffectsLocal(playerSettings.playSoundEffects);
    setSoundEffectsVolLocal(playerSettings.soundEffectsVolume);
  }, [playerSettings]);

  const handleClose = () => {
    dispatch(setDisplaySettings(false));
  };

  const handleAutoRollChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setAutoRollLocal(event.target.checked);
  };

  const handleMusicChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setMusicLocal(event.target.checked);
  };

  const handleMusicVolChange = (event: Event, value: number | number[]) => {
    if (typeof value === "number") {
      setMusicVolLocal(value);
    }
  };

  const handleVoiceChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setVoiceLocal(event.target.checked);
  };

  const handleVoiceVolChange = (event: Event, value: number | number[]) => {
    if (typeof value === "number") {
      setVoiceVolLocal(value);
    }
  };

  const handleSoundEffectsChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSoundEffectsLocal(event.target.checked);
  };

  const handleSoundEffectsVolChange = (event: Event, value: number | number[]) => {
    if (typeof value === "number") {
      setSoundEffectsVolLocal(value);
    }
  };

  const handleSave = () => {
    dispatch(setAutoRoll(autoRollLocal));
    dispatch(setMusic(musicLocal));
    dispatch(setMusicVolume(musicVolLocal));
    dispatch(setVoice(voiceLocal));
    dispatch(setVoiceVolume(voiceVolLocal));
    dispatch(setSoundEffects(soundEffectsLocal));
    dispatch(setSoundEffectsVolume(soundEffectsVolLocal));
    dispatch(
      setPlayerSettingsInLocalStorage({
        autoRoll: autoRollLocal,
        playMusic: musicLocal,
        musicVolume: musicVolLocal,
        playVoice: voiceLocal,
        voiceVolume: voiceVolLocal,
        playSoundEffects: soundEffectsLocal,
        soundEffectsVolume: soundEffectsVolLocal,
      })
    );
    dispatch(setDisplaySettings(false));
  };

  return (
    <Modal open={displaySettings} onClose={handleClose} className={styles.modalMain}>
      <Grid container className={styles.container}>
        <Grid container className={styles.containerBackground}>
          <Image src="/img/unity-assets/shared/action_bg.png" height={370} width={390} quality={100} />
          <Grid container className={styles.headerContainer}>
            <Typography component="span" className={styles.header}>
              SETTINGS
            </Typography>
          </Grid>
          <Grid container className={styles.closeButtonContainer}>
            <CloseButton handleClose={handleClose} />
          </Grid>
          <Grid container className={styles.divider} />
          <Grid container className={styles.contentContainer}>
            <Grid container className={styles.content}>
              <ThemeProvider theme={switchTheme}>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.text}>
                    AUTOROLL
                  </Typography>
                  <Switch checked={autoRollLocal} onChange={handleAutoRollChange} size="small" />
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.text}>
                    MUSIC
                  </Typography>
                  <Switch checked={musicLocal} onChange={handleMusicChange} size="small" />
                </Grid>
                <ThemeProvider theme={musicSliderTheme}>
                  <Grid container className={styles.row}>
                    <Typography component="span" className={styles.text}>
                      MUSIC VOL
                    </Typography>
                    <Grid container className={styles.sliderContainer}>
                      <Slider
                        value={musicVolLocal}
                        onChange={handleMusicVolChange}
                        valueLabelDisplay="auto"
                        disabled={!musicLocal}
                      />
                    </Grid>
                  </Grid>
                </ThemeProvider>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.text}>
                    VOICE
                  </Typography>
                  <Switch checked={voiceLocal} onChange={handleVoiceChange} size="small" />
                </Grid>
                <ThemeProvider theme={voiceSliderTheme}>
                  <Grid container className={styles.row}>
                    <Typography component="span" className={styles.text}>
                      VOICE VOL
                    </Typography>
                    <Grid container className={styles.sliderContainer}>
                      <Slider
                        value={voiceVolLocal}
                        onChange={handleVoiceVolChange}
                        valueLabelDisplay="auto"
                        disabled={!voiceLocal}
                      />
                    </Grid>
                  </Grid>
                </ThemeProvider>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.text}>
                    SOUND EFFECTS
                  </Typography>
                  <Switch checked={soundEffectsLocal} onChange={handleSoundEffectsChange} size="small" />
                </Grid>
                <ThemeProvider theme={soundEffectsSliderTheme}>
                  <Grid container className={styles.row}>
                    <Typography component="span" className={styles.text}>
                      SOUND EFFECTS VOL
                    </Typography>
                    <Grid container className={styles.sliderContainer}>
                      <Slider
                        value={soundEffectsVolLocal}
                        onChange={handleSoundEffectsVolChange}
                        valueLabelDisplay="auto"
                        disabled={!soundEffectsLocal}
                      />
                    </Grid>
                  </Grid>
                </ThemeProvider>
              </ThemeProvider>
              <Grid container className={styles.saveButtonContainer}>
                <DCXButton
                  title="SAVE"
                  height={42}
                  width={160}
                  color="blue"
                  // disabledLayerHeightAdjustment={11}
                  // disabledLayerWidthAdjustment={20}
                  onClick={handleSave}
                />
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </Modal>
  );
};

export default Settings;
