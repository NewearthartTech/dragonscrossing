import { DragDropContainer, DropTarget } from "react-drag-drop-container";
import { Item } from "@/state-mgmt/item/itemTypes";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { selectItemMenuOpen, setItemMenuOpen } from "@/state-mgmt/item/itemSlice";
import styles from "./inventory.module.scss";
import DCXItem from "../dcx-item/dcx-item";
import Grid from "@mui/material/Grid";
import {
  moveItemRequest,
  selectEquippedItems,
  selectInventory,
  selectSelectedHero,
  setDraggedItem,
  unequipItemRequest,
} from "@/state-mgmt/hero/heroSlice";
import { useEffect, useRef, useState } from "react";
import DCXContextMenu from "../dcx-context-menu/dcx-context-menu";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { mdScreenWidth } from "@/helpers/global-constants";
import { ItemDto, ItemSlotTypeDto } from "@dcx/dcx-backend";
import { MoveItemRequest } from "@/state-mgmt/hero/heroTypes";
import { SoundType } from "@/state-mgmt/app/appTypes";
import DCXAudioPlayer from "@/components/dcx-audio-player/dcx-audio-player";
import DCXNftItem from "../dcx-nft-item/dcx-nft-item";
import { selectSnackbarMessage, setSnackbarMessage } from "@/state-mgmt/app/appSlice";

interface Props {
  item: Item | undefined;
  itemIndex: number;
  disableDND?: boolean;
  disableContextMenu?: boolean;
  backgroundHeight?: number;
  backgroundWidth?: number;
  itemHeight?: number;
  itemWidth?: number;
  isWalletItem?: boolean;
  isNftItem?: boolean;
  isEquippedItem?: boolean;
  isLootItem?: boolean;
  isDeathItem?: boolean;
  isMerchantItem?: boolean;
  isSellableItem?: boolean;
  isStashItem?: boolean;
  itemMarginLeft?: number;
  itemMarginTop?: number;
  detailsLeft?: boolean;
  detailsMarginLeft?: number;
  showOpaque?: boolean;
}

const ItemDND: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const inventory = useAppSelector(selectInventory);
  const equippedItems = useAppSelector(selectEquippedItems);
  const itemMenuOpen = useAppSelector(selectItemMenuOpen);
  const snackbarMessage = useAppSelector(selectSnackbarMessage);
  const hero = useAppSelector(selectSelectedHero).hero;
  const { width, height: windowHeight } = useWindowDimensions();
  const {
    item,
    itemIndex,
    disableDND,
    disableContextMenu,
    backgroundHeight,
    backgroundWidth,
    itemHeight,
    itemWidth,
    isWalletItem,
    isNftItem,
    isEquippedItem,
    isLootItem,
    isDeathItem,
    isMerchantItem,
    isSellableItem,
    isStashItem,
    itemMarginLeft,
    itemMarginTop,
    detailsLeft,
    detailsMarginLeft,
    showOpaque,
  } = props;

  const ref = useRef<any>(null);

  const [disableDetails, setDisableDetails] = useState(false);
  const [playDropSound, setPlayDropSound] = useState(false);
  const [dropSound, setDropSound] = useState("");

  // If a player clicks outside of the DCX Context Menu then close the menu
  useEffect(() => {
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      // Unbind the event listener on clean up
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [ref]);

  const handleClickOutside = (e: any) => {
    if (ref.current && !ref.current.contains(e.target)) {
      if (item) {
        dispatch(setItemMenuOpen({ item: item, open: false }));
      } else {
        dispatch(setItemMenuOpen({ item: inventory[itemIndex], open: false }));
      }
    }
  };

  const dragStart = () => {
    if (snackbarMessage.isOpen) {
      dispatch(setSnackbarMessage({ isOpen: false, message: "" }));
    }
  };

  const dragEnd = () => {
    dispatch(setDraggedItem(item!));
    if (width > mdScreenWidth) {
      setDisableDetails(false);
    }
  };

  const dropped = (e: any) => {
    setDropSound(e.dragData.data.itemDropSound);
    setPlayDropSound(true);
    // If an item is dropped onto an equipped item then the action will be handled by the dropped method in the
    // inventory.tsx component.
    // isEquippedItem means the dragged item is being dropped on an equipped item slot
    if (!isEquippedItem) {
      if (!isWalletItem) {
        const inventoryItemIndex = inventory.findIndex((i: ItemDto) => i.id === e.dragData.data.id);
        // If it is an inventory item
        if (inventoryItemIndex !== -1) {
          dispatch(
            moveItemRequest({
              heroId: hero.id,
              fromIndex: inventoryItemIndex,
              toIndex: itemIndex,
            } as MoveItemRequest)
          );
        } else {
          const equippedItemIndex = equippedItems.findIndex((i: ItemDto) => i.id === e.dragData.data.id);
          // If dropped item is in equippedItems and its not dropped on another equipped item
          if (equippedItemIndex !== -1 && !isEquippedItem) {
            dispatch(
              unequipItemRequest({
                itemId: e.dragData.data.id,
                inventoryIndex: itemIndex,
              })
            );
          }
        }
      }
    }
  };

  return (
    <Grid item>
      {playDropSound && (
        <DCXAudioPlayer
          audioUrl={`/audio/sound-effects/item/${dropSound.toLowerCase()}`}
          soundType={SoundType.SOUND_EFFECT}
          onEnded={() => setPlayDropSound(false)}
        />
      )}
      <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.Head}>
        <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.Chest}>
          <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.MainHand}>
            <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.TwoHand}>
              <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.OffHand}>
                <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.Feet}>
                  <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.Ring}>
                    <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.Shard}>
                      <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.UnidentifiedSkill}>
                        <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.UnlearnedSkill}>
                          <Grid container className={styles.relativeContainer}>
                            {!isEquippedItem && (
                              <Grid
                                container
                                style={{
                                  height: backgroundHeight ? backgroundHeight : 70,
                                  width: backgroundWidth ? backgroundWidth : 70,
                                }}
                              >
                                <Image
                                  src={
                                    backgroundHeight && backgroundHeight > 120
                                      ? `/img/unity-assets/inventory/item_slot_hd.png`
                                      : `/img/unity-assets/inventory/item_slot.png`
                                  }
                                  height={backgroundHeight ? backgroundHeight : 70}
                                  width={backgroundWidth ? backgroundWidth : 70}
                                  quality={100}
                                />
                              </Grid>
                            )}
                            {item && isNftItem && (
                              <Grid container className={styles.itemDNDContainer}>
                                {!disableDND ? (
                                  <DragDropContainer
                                    targetKey={item.slot}
                                    dragClone={false}
                                    dragData={{ data: item }}
                                    onDragStart={dragStart}
                                    onDragEnd={dragEnd}
                                    render={() => {
                                      return (
                                        <DCXNftItem
                                          item={item}
                                          height={itemHeight ? itemHeight : 58}
                                          width={itemWidth ? itemWidth : 60}
                                          marginRight={5}
                                          isWalletItem={isWalletItem}
                                          isLootItem={isLootItem}
                                          detailsLeft={detailsLeft}
                                          showOpaque={showOpaque}
                                          disableDetails={disableDetails}
                                        />
                                      );
                                    }}
                                  />
                                ) : (
                                  <DCXNftItem
                                    item={item}
                                    height={itemHeight ? itemHeight : 58}
                                    width={itemWidth ? itemWidth : 60}
                                    marginRight={5}
                                    isWalletItem={isWalletItem}
                                    isLootItem={isLootItem}
                                    detailsLeft={detailsLeft}
                                    showOpaque={showOpaque}
                                    disableDetails={disableDetails}
                                  />
                                )}
                                {itemMenuOpen.open &&
                                  itemMenuOpen.item.id === item?.id &&
                                  itemMenuOpen.x &&
                                  itemMenuOpen.y &&
                                  !disableContextMenu && (
                                    <Grid container ref={ref}>
                                      <DCXContextMenu
                                        item={item}
                                        isWalletItem={isWalletItem}
                                        isNftItem={isNftItem}
                                        isLootItem={isLootItem}
                                        isDeathItem={isDeathItem}
                                        isMerchantItem={isMerchantItem}
                                        isSellableItem={isSellableItem}
                                        menuX={itemMenuOpen.x}
                                        menuY={itemMenuOpen.y}
                                      />
                                    </Grid>
                                  )}
                              </Grid>
                            )}
                            {item && !isNftItem && (
                              <Grid container className={styles.itemDNDContainer}>
                                {!disableDND ? (
                                  <DragDropContainer
                                    targetKey={item.slot}
                                    dragClone={false}
                                    dragData={{ data: item }}
                                    onDragStart={dragStart}
                                    onDragEnd={dragEnd}
                                    render={() => {
                                      return (
                                        <DCXItem
                                          rarity={item.rarity}
                                          item={item}
                                          itemIndex={itemIndex}
                                          height={itemHeight ? itemHeight : 58}
                                          width={itemWidth ? itemWidth : 60}
                                          marginRight={5}
                                          isWalletItem={isWalletItem}
                                          isMerchantItem={isMerchantItem}
                                          isSellableItem={isSellableItem}
                                          isDeathItem={isDeathItem}
                                          isLootItem={isLootItem}
                                          detailsLeft={detailsLeft}
                                          showOpaque={showOpaque}
                                          disableDetails={disableDetails}
                                          isEquippedItem={isEquippedItem}
                                        />
                                      );
                                    }}
                                  />
                                ) : (
                                  <DCXItem
                                    rarity={item.rarity}
                                    item={item}
                                    itemIndex={itemIndex}
                                    height={itemHeight ? itemHeight : 58}
                                    width={itemWidth ? itemWidth : 60}
                                    marginLeft={itemMarginLeft ? itemMarginLeft : 0}
                                    marginTop={itemMarginTop ? itemMarginTop : 0}
                                    isWalletItem={isWalletItem}
                                    isMerchantItem={isMerchantItem}
                                    isSellableItem={isSellableItem}
                                    isDeathItem={isDeathItem}
                                    isLootItem={isLootItem}
                                    detailsLeft={detailsLeft}
                                    detailsMarginLeft={detailsMarginLeft}
                                    showOpaque={showOpaque}
                                    disableDetails={disableDetails}
                                    isEquippedItem={isEquippedItem}
                                  />
                                )}
                                {itemMenuOpen.open &&
                                  itemMenuOpen.item.id === item?.id &&
                                  itemMenuOpen.x &&
                                  itemMenuOpen.y &&
                                  !disableContextMenu && (
                                    <Grid container ref={ref}>
                                      <DCXContextMenu
                                        item={item}
                                        isWalletItem={isWalletItem}
                                        isNftItem={isNftItem}
                                        isLootItem={isLootItem}
                                        isDeathItem={isDeathItem}
                                        isMerchantItem={isMerchantItem}
                                        isSellableItem={isSellableItem}
                                        isStashItem={isStashItem}
                                        menuX={itemMenuOpen.x}
                                        menuY={itemMenuOpen.y}
                                      />
                                    </Grid>
                                  )}
                              </Grid>
                            )}
                          </Grid>
                        </DropTarget>
                      </DropTarget>
                    </DropTarget>
                  </DropTarget>
                </DropTarget>
              </DropTarget>
            </DropTarget>
          </DropTarget>
        </DropTarget>
      </DropTarget>
    </Grid>
  );
};

export default ItemDND;
