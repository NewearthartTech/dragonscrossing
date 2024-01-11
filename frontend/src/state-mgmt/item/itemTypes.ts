import {
  AffectedHeroStatTypeDto,
  DisplayItemDetailsDto,
  DisplayItemMenuDto,
  UpdateInventoryRequestDto,
} from "@dcx/dcx-backend";
import { ItemDto, ItemListDto } from "@dcx/dcx-backend";

export type Item = ItemDto & {};

export type ItemList = ItemListDto & {};

export type UpdateInventoryRequest = UpdateInventoryRequestDto & {};

export type DisplayItemMenu = DisplayItemMenuDto & {};

export type DisplayItemDetails = DisplayItemDetailsDto & {};

export enum ItemStatType {
  Strength = "Strength",
  Agility = "Agility",
  Wisdom = "Wisdom",
  Charisma = "Charisma",
  Quickness = "Quickness",
  HitPoints = "HitPoints",
  DifficultyToHit = "DifficultyToHit",
  ChanceToHit = "ChanceToHit",
  ArmorMitigationAmount = "ArmorMitigationAmount",
  ArmorMitigation = "ArmorMitigation",
  ParryRate = "ParryRate",
  DodgeRate = "DodgeRate",
  CriticalHitRate = "CriticalHitRate",
  BonusDamage = "BonusDamage",
}
