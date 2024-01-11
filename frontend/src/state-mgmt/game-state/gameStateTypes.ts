import { DiceRollReason, GameStateDto } from "@dcx/dcx-backend";
import { ZoneDto, TileDto } from "@dcx/dcx-backend";

export type Zone = ZoneDto & {};

export type Tile = TileDto & {};

export type GameState = GameStateDto & {};

export interface UpdateGameStateRequest {
  heroId: number;
  slug: string;
}
