import {
  HeroDto,
  SelectedHeroDto,
  HeroListDto,
  EquipItemRequestDto,
  LevelUpResponseDto,
  AllocateStatsRequestDto,
  AddItemToInventoryRequestDto,
  MoveItemRequestDto,
} from "@dcx/dcx-backend";

export type Hero = HeroDto & {};

export type SelectedHero = SelectedHeroDto & {};

export type HeroList = HeroListDto & {};

export type EquipUnequipItemRequest = EquipItemRequestDto & {
  inventoryIndex?: number;
};

export type LevelUpResponse = LevelUpResponseDto & {};

export type AllocateStatsRequest = AllocateStatsRequestDto & {};

export type AddItemToInventoryRequest = AddItemToInventoryRequestDto & {};

export type MoveItemRequest = MoveItemRequestDto & {
  heroId: number;
};
