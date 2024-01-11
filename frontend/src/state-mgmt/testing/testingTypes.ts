import { DiceRollReason } from "@dcx/dcx-backend";

export interface FixDiceRequest {
  reason: DiceRollReason;
  value: number;
}
