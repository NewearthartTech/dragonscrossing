import {
  CalculatedCharacterStats,
  CharacterClassDto,
  DcxTiles,
  Gender,
  HeroDto,
  HeroRarityDto,
  ItemDto,
  ItemSlotTypeDto,
  MonsterDto,
  MonsterPersonalityTypeDto,
  ZoneBackgroundTypeDto,
} from "@dcx/dcx-backend";
import { createTheme } from "@mui/material/styles";
import { default as _ReactPlayer } from "react-player/lazy";
import { ReactPlayerProps } from "react-player/types/lib";

export const ReactPlayer = _ReactPlayer as unknown as React.FC<ReactPlayerProps>;

export const xsScreenWidth = 599;
export const smScreenWidth = 899;
export const mdScreenWidth = 1199;
export const lgScreenWidth = 1535;
export const xlScreenWidth = 1669;
export const xxlScreenWidth = 1920;
export const unexpectedErrorMessage = "An unexpected error has occurred, please try again.";
export const noHeroesMessage = "There are no heroes associated with this wallet.";
export const fetchGameStateErrorMessage = "Failed to retrieve the game state for this hero, please try again.";
export const linkWalletMessage = "Please link your metamask wallet.";
export const noAccountMessage = "Failed to find wallet account.";
export const strengthDescription =
  "Attribute used by the Warrior class for heavy melee weapons. Provides a small increase in chance to double attack.";
export const wisdomDescription =
  "Attribute used by the Mage class for magical weapons. Provides a small increase in chance to parry and dodge.";
export const agilityDescription =
  "Attribute used by the Ranger class for ranged weapons and lighter melee weapons. Provides a small increase in chance to critical strike.";
export const quicknessDescription = "Attribute governing dodge and initiative in combat.";
export const charismaDescription =
  " Attribute governing player interaction in noncombat situations. Also governs the combat-related Charisma opportunity, which allows the player to potentially avoid combat altogether, while still obtaining experience.";
export const walletAddressMismatch = "ERROR: CONNECTED WALLET ADDRESS DOES NOT MATCH";
export const encounterCompletionFailed = "FAILED TO COMPLETE THE ENCOUNTER, PLEASE TRY AGAIN";
export const getQuestionResponseFailed = "FAILED TO RETRIEVE QUESTION RESPONSE, PLEASE TRY AGAIN";
export const getChoiceResponseFailed = "FAILED TO RETRIEVE CHOICE RESPONSE, PLEASE TRY AGAIN";
export const getHerbalistOptionsFailed = "FAILED TO RETRIEVE HERBALIST OPTIONS, PLEASE TRY AGAIN";
export const herbalistHealFailed = "AN UNEXPECTED ERROR OCCURRED WHILE TRYING TO HEAL YOUR HERO, PLEASE TRY AGAIN";
export const retrievePlayerFailed =
  "AN UNEXPECTED ERROR OCCURRED WHILE TRYING TO RETRIEVE PLAYER DETAILS, PLEASE TRY AGAIN";
export const getCombatStateFailed = "FAILED TO RETRIEVE COMBAT STATE, PLEASE TRY AGAIN";

export const initializedHero = {
  id: -1,
  isDefaultChain:true,
  dcxRewards:0,
  playerId: "",
  seasonId: -1,
  lastDailyResetAt: "",
  name: "",
  heroClass: CharacterClassDto.Warrior,
  gender: Gender.Male,
  generation: -1,
  rarity: HeroRarityDto.Unknown,
  isHearthstoneAvailable: false,
  remainingHitPointsPercentage: -1,
  remainingHitPoints: -1,
  totalHitPoints: -1,
  baseHitPoints: -1,
  difficultyToHit: -1,
  strength: -1,
  agility: -1,
  wisdom: -1,
  quickness: -1,
  charisma: -1,
  isAscended: false,
  prestigeLevel: -1,
  level: -1,
  experiencePoints: -1,
  maxExperiencePoints: -1,
  remainingQuests: -1,
  maxDailyQuests: -1,
  dailyQuestsUsed: -1,
  extraDailyQuestGiven: -1,
  learnedSkillCapacity: -1,
  usedUpSkillPoints: -1,
  baseChanceToHit: -1,
  baseSkillPoints: -1,
  zoneBackgroundType: ZoneBackgroundTypeDto.Unknown,
  
  unsecuredDCXValue: {
    amount: 0,
  },
  gold: -1,
  skills: [],
  equippedItems: [],
  inventory: [],
  calculatedStats: {
    strength: -1,
    agility: -1,
    wisdom: -1,
    quickness: -1,
    charisma: -1,
    chanceToCrit: -1,
    chanceToParry: -1,
    chanceToHit: -1,
    difficultyToHit: -1,
    chanceToDodge: -1,
    armorMitigation: -1,
    armorMitigationAmount: -1,
    itemBonusDamage: -1,
    statsBonusDamage: -1,
    damageRange: -1,
  } as CalculatedCharacterStats,
  alteredStats: [],
} as HeroDto;

export const initializedMonster = {
  id: "",
  name: "",
  monsterSlug: "",
  locationTile: DcxTiles.EnchantedFields,
  personalityType: MonsterPersonalityTypeDto.None,
  personalityTypeDesc: "",
  description: "",
  power: -1,
  chanceToHit: -1,
  difficultyToHit: -1,
  chanceToDodge: -1,
  armorMitigation: -1,
  armorMitigationAmount: -1,
  bonusDamage: -1,
  critChance: -1,
  parryChance: -1,
  appearChance: -1,
  charisma: -1,
  quickness: -1,
  chanceOfUsingSpecialAbility: -1,
  charismaOpportunityChance: -1,
  dieDamage: [],
  hitPoints: -1,
  maxHitPoints: -1,
  level: -1,
  monsterItems: [],
  alteredStats: [],
} as MonsterDto;

export const initializedItem = {
  isDefaultChain:true,
  type: "",
  id: "",
  name: "",
  slug: "",
  itemDropSound: "",
  slot: ItemSlotTypeDto.Unknown,
  levelRequirement: -1,
  dieDamage: [],
  affectedAttributes: { [""]: -1 },
  allowedHeroClassList: [],
  heroStatComplianceDictionary: { [""]: -1 },
} as ItemDto;

export const tooltipTheme = createTheme({
  components: {
    MuiTooltip: {
      styleOverrides: {
        tooltip: {
          fontSize: 16,
          textAlign: "center",
          color: "rgb(230, 230, 230)",
          backgroundColor: "rgb(30, 30, 30)",
          fontFamily: "Whatacolour",
          border: "1px solid rgb(230, 230, 230)",
          cursor: "default",
        },
        arrow: {
          color: "rgb(230, 230, 230)",
        },
      },
    },
  },
});

export const inputTheme = createTheme({
  components: {
    MuiInputBase: {
      styleOverrides: {
        root: {
          "&.MuiInputBase-root": {
            height: "25px",
            backgroundColor: "rgb(40, 40, 40)",
            color: "rgb(230, 230, 230)",
            fontFamily: "Whatacolour",
            fontSize: "18px",
          },
        },
      },
    },
    MuiTextField: {
      styleOverrides: {
        root: {
          "& label.Mui-focused": {
            color: "rgb(185, 143, 36)",
          },
          "& .MuiInput-underline:after": {
            borderBottomColor: "rgb(185, 143, 36)",
          },
          "& .MuiOutlinedInput-root": {
            "& fieldset": {
              borderColor: "rgb(163, 124, 27)",
            },
            "&:hover fieldset": {
              borderColor: "rgb(163, 124, 27)",
              borderWidth: "0.15rem",
            },
            "&.Mui-focused fieldset": {
              borderColor: "rgb(163, 124, 27)",
            },
          },
        },
      },
    },
  },
});

export const tileDescriptionsMap: { [key: string]: string } = {
  aedos: "TOWN",
  wildPrairie: "TRAVEL TILE",
  enchantedFields: "ADVENTURING QUEST",
  blacksmith: "VENDOR",
  camp_mysteriousForest: "VENDOR",
  camp_treacherousPeaks: "VENDOR",
  herbalist_aedos: "VENDOR",
  herbalist_foulWastes: "VENDOR",
  herbalist_darkTower: "VENDOR",
  adventuringGuild: "VENDOR",
  adventuringGuild_foulWastes: "VENDOR",
  sharedStash: "VENDOR",
  camp: "VENDOR",
  mysteriousForest: "TRAVEL TILE",
  sylvanWoodlands: "ADVENTURING QUEST",
  pilgrimsClearing: "DAILY QUEST",
  foulWastes: "TRAVEL TILE",
  odorousBog: "ADVENTURING QUEST",
  ancientBattlefield: "DAILY QUEST",
  terrorswamp: "BOSS QUEST",
  treacherousPeaks: "TRAVEL TILE",
  mountainFortress: "ADVENTURING QUEST",
  griffonsNest: "DAILY QUEST",
  summonersSummit: "BOSS QUEST",
  darkTower: "TRAVEL TILE",
  labyrinthianDungeon: "ADVENTURING QUEST",
  barracks: "DAILY QUEST",
  slaversRow: "BOSS QUEST",
  libraryOfTheArchmage: "BOSS QUEST",

  wondrousThicket: "TRAVEL TILE",
  feyClearing: "ADVENTURING QUEST",
  shatteredStable: "DAILY QUEST",
  forebodingDale: "BOSS QUEST",
  herbalist_wondrousThicket: "VENDOR",
  adventuringGuild_wondrousThicket: "VENDOR",

  fallenTemples: "TRAVEL TILE",
  pillaredRuins: "ADVENTURING QUEST",
  acropolis: "DAILY QUEST",
  destroyedPantheon: "BOSS QUEST",

  blacksmith_fallenTemples: "VENDOR",
};

export const voicelessMonsters = ["mighty-stag", "giant-wolf", "winter-grizzly", "griffon", "stone-golem"];
