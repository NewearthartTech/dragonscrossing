import { LearnedHeroSkill, UpdateSkillStateRequestDto } from "@dcx/dcx-backend";

export type Skill = LearnedHeroSkill & {};

export interface SkillAllocatePointsRequest {
  skillId: string;
  skillPoints: number;
}
