import { ItemDto } from "@dcx/dcx-backend";

export interface BlacksmithItemsRequest {
  heroId: number;
}

export interface BuyBlacksmithItemRequest {
  heroId: number;
  itemId: number;
}

export interface ItemsToSell {
  items: ItemDto[];
  showConfirmation: boolean;
}

export interface ToggleItemToSell {
  item?: ItemDto;
  showConfirmation: boolean;
  unselectedAll?: boolean;
}

export interface SummonHeroResponse {
  summonedHeroId: number;
  txHash: string;
}
