import { GameStateDto } from "@dcx/dcx-backend";

export interface EncounterResponseRequest {
  id: string;
  slug: string;
}

// export enum EncounterEnum {
//   COMBAT = "Combat",
//   LORE = "Lore",
//   LOCATION = "Location",
//   FOREIGN_BERRIES = "Foreign Berries",
//   FRESHWATER_ORB = "Freshwater Orb",
//   GAMBLER = "Gambler",
// }

// export interface GetEncountersRequest {
//   heroId: number;
//   slug: string;
// }

// export interface Dialogue {
//   slug: string;
//   questionText: string;
//   answerText: string;
// }

// export interface Choice {
//   id: string;
//   choiceText: string;
//   goodOutcomeChance: number;
// }

// export interface ChanceEncounterType extends NonCombatEncounterType {
//   choices: Array<Choice>;
// }

// export interface LoreEncounterType extends NonCombatEncounterType {
//   dialogues: Array<Dialogue>;
//   narratedText: string;
// }

// export interface LocationEncounterType extends NonCombatEncounterType {
//   narratedText: string;
// }

// export interface NonCombatEncounterType extends EncounterType {
//   encounterName: string; // ie. Mysterious Forest, Foul Wastes, Foreign Berries, Gambler, etc.
//   introText: string;
// }

// export interface EncounterType {
//   slug: string;
//   encounterType: EncounterEnum;
// }

// export interface EncounterChoiceResultRequest {
//   heroId: number;
//   choiceId: string;
//   gold?: number;
// }

// export interface EncounterChoiceResultResponse {
//   description: string;
//   gameState: GameStateDto;
// }
