import { PlayerDto } from "@dcx/dcx-backend";

export interface PlayerSettings {
  autoRoll: boolean;
  playMusic: boolean;
  musicVolume: number;
  playVoice: boolean;
  voiceVolume: number;
  playSoundEffects: boolean;
  soundEffectsVolume: number;
}

export type Player = PlayerDto & {
  playerSettings: PlayerSettings;
};

export interface LoginRequest {
  username: string;
  password: string;
}
