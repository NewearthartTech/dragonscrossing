export enum Status {
  NotStarted,
  Loading,
  Loaded,
  Failed,
}

export interface LoadingStatus {
  status?: Status;
  error?: string;
}

export enum VendorType {
  ADVENTURING_GUILD = "Adventuring Guild",
  BLACKSMITH = "Blacksmith",
  HERBALIST = "Herbalist",
  LIBRARY = "Library",
}

export enum SoundType {
  MUSIC = "Music",
  VOICE = "Voice",
  SOUND_EFFECT = "SoundEffect",
}

export enum TileBorderType {
  WOODEN = "wooden",
  ARMORED = "armored",
}

export interface HeroToken {
  ClaimWalletVarified: string;
  SelectedHeroId: string;
  exp: number;
}

export interface AppMessage {
  message: string;
  isClearToken: boolean;
  buttonTitle?: string;
  isCamp?: boolean;
}

export interface SnackbarMessage {
  isOpen: boolean;
  message: string;
}

// export interface HeroRank {
//   id: string;
//   seasonId: string;
//   heroId: number;
//   rank: number;
//   level: number;
//   isFinalBossDefeated: boolean;
//   farthestZoneDiscovered: string;
//   numberOfQuestsUsed: number;
// }

// export interface SeasonLeaderboard {
//   id: string;
//   seasonId: string;
//   seasonName: string;
//   heroesInSeason: number;
//   heroRanks: Array<HeroRank>;
// }

export interface SeasonClaimOrder {
  orderId: string;
  rank: number;
  heroesInSeason: number;
  prizeMultiplier: number;
  claimableDcx: number;
}
