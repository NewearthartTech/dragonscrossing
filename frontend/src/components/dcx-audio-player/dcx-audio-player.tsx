import { SoundType } from "@/state-mgmt/app/appTypes";
import { selectPlayerSettings } from "@/state-mgmt/player/playerSlice";
import { useAppSelector } from "@/state-mgmt/store/hooks";
import ReactAudioPlayer from "react-audio-player";

interface Props {
  audioUrl: string;
  soundType: SoundType;
  onEnded?: Function;
  onError?: Function;
  loop?: boolean;
}

const DCXAudioPlayer: React.FC<Props> = (props: Props) => {
  const { audioUrl, soundType, onEnded, onError, loop } = props;

  const playerSettings = useAppSelector(selectPlayerSettings);

  const getAutoPlay = () => {
    if (soundType === SoundType.MUSIC) {
      return playerSettings.playMusic;
    } else if (soundType === SoundType.VOICE) {
      return playerSettings.playVoice;
    } else {
      return playerSettings.playSoundEffects;
    }
  };

  const getVolume = () => {
    if (soundType === SoundType.MUSIC) {
      return playerSettings.musicVolume;
    } else if (soundType === SoundType.VOICE) {
      return playerSettings.voiceVolume;
    } else {
      return playerSettings.soundEffectsVolume;
    }
  };

  return (
    <ReactAudioPlayer
      src={`${audioUrl}.wav`}
      autoPlay={getAutoPlay()}
      muted={!getAutoPlay()}
      volume={getVolume() / 100}
      onEnded={() => (onEnded ? onEnded() : undefined)}
      loop={loop}
      onError={() => (onError ? onError() : undefined)}
    />
  );
};

export default DCXAudioPlayer;
