import {
  ActionResponseDto,
  DieDto,
  HeroResultDto,
  MonsterLootDto,
  MonsterResultDto,
  OpportunityResultTypeDto,
  SkillResultDto,
  StatusEffectDto,
} from "@dcx/dcx-backend";

export enum ActionType {
  START_ROUND = "Start Round",
  ATTACK = "Attack",
  SKILL = "Skill",
  INTIMIDATE = "Intimidate",
  PERSUADE = "Persuade",
  FLEE = "Flee",
  UNKNOWN = "Unknown",
}

export type Die = DieDto & {};

export type OpportunityResultType = OpportunityResultTypeDto & {};

export type StatusEffect = StatusEffectDto & {};

export type SkillResult = SkillResultDto & {};

export type MonsterResult = MonsterResultDto & {};

export type HeroResult = HeroResultDto & {};

export type ActionResponse = ActionResponseDto & {};

export type MonsterLoot = MonsterLootDto & {};

export interface NonCombatActionResponse {
  variableName: string;
}
