import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./dcx-context-menu.module.scss";
import { Item } from "@/state-mgmt/item/itemTypes";
import { useEffect, useState } from "react";
import {
  destroyItemRequest,
  equipItemRequest,
  selectEquippedItems,
  selectInventory,
  selectSelectedHero,
  unequipItemRequest,
} from "@/state-mgmt/hero/heroSlice";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { setItemDetailsOpen, setItemMenuOpen, setItemToKeep } from "@/state-mgmt/item/itemSlice";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { mdScreenWidth, xsScreenWidth } from "@/helpers/global-constants";
import { toggleItemToSell } from "@/state-mgmt/vendor/vendorSlice";
import { ItemDto } from "@dcx/dcx-backend";
import DCXButton from "../dcx-button/dcx-button";
import { pickUpLootRequest } from "@/state-mgmt/combat/combatSlice";
import { selectDisplaySharedStash, setDroppedItemSoundSlug } from "@/state-mgmt/app/appSlice";
import { grabItemFromStash, selectStashItemMovedStatus, storeItemInStash } from "@/state-mgmt/player/playerSlice";
import { Status } from "@/state-mgmt/app/appTypes";

interface Props {
  item?: Item;
  isWalletItem?: boolean;
  isNftItem?: boolean;
  isLootItem?: boolean;
  isDeathItem?: boolean;
  isMerchantItem?: boolean;
  isSellableItem?: boolean;
  isStashItem?: boolean;
  menuX: number;
  menuY: number;
}

const DCXContextMenu: React.FC<Props> = (props: Props) => {
  const {
    item,
    menuX,
    menuY,
    isWalletItem,
    isNftItem,
    isLootItem,
    isDeathItem,
    isMerchantItem,
    isSellableItem,
    isStashItem,
  } = props;

  const dispatch = useAppDispatch();
  const equippedItems = useAppSelector(selectEquippedItems);
  const inventory = useAppSelector(selectInventory);
  const hero = useAppSelector(selectSelectedHero).hero;
  const displaySharedStash = useAppSelector(selectDisplaySharedStash);
  const stashItemMovedStatus = useAppSelector(selectStashItemMovedStatus);
  const { width, height } = useWindowDimensions();

  const [isEquipped, setEquipped] = useState(false);
  const [confirmDestroy, setConfirmDestroy] = useState(false);
  const [menuHeight, setMenuHeight] = useState(35);

  useEffect(() => {
    if (equippedItems && equippedItems.length > 0) {
      const equippedItem = equippedItems.find((i: ItemDto) => i.id === item?.id);
      if (equippedItem && equippedItem.id !== "") {
        setEquipped(true);
      }
    }
    if (width <= mdScreenWidth) {
      if (isWalletItem || isMerchantItem || isNftItem) {
        if (isLootItem) {
          setMenuHeight(70);
        } else {
          setMenuHeight(35);
        }
      } else if (isLootItem || isDeathItem || isSellableItem || isStashItem) {
        setMenuHeight(70);
      } else {
        setMenuHeight(105);
      }
    } else {
      if (isWalletItem || isMerchantItem) {
        setMenuHeight(0);
      } else if (isLootItem || isDeathItem || isSellableItem || isStashItem) {
        setMenuHeight(35);
      } else {
        setMenuHeight(70);
      }
    }
  }, []);

  const handleEquip = () => {
    if (item) {
      dispatch(setDroppedItemSoundSlug(item.itemDropSound));
      // Get the index of the dragged item
      const index = inventory.findIndex((i) => i.id === item?.id);
      dispatch(
        equipItemRequest({
          itemId: item.id,
          inventoryIndex: index,
        })
      );
      dispatch(setItemMenuOpen({ item: item as Item, open: false }));
    }
  };

  const handleUnequip = () => {
    if (item) {
      dispatch(setDroppedItemSoundSlug(item.itemDropSound));
      dispatch(unequipItemRequest({ itemId: item.id }));
      dispatch(setItemMenuOpen({ item: item as Item, open: false }));
    }
  };

  const handleTake = () => {
    if (item) {
      const itemIds: Array<string> = [];
      itemIds.push(item.id);
      dispatch(pickUpLootRequest(itemIds));
      dispatch(setItemMenuOpen({ item: item as Item, open: false }));
    }
  };

  const handleKeep = () => {
    dispatch(setItemToKeep(item as Item));
    dispatch(setItemMenuOpen({ item: item as Item, open: false }));
  };

  const handleDetails = () => {
    dispatch(setItemMenuOpen({ item: item as Item, open: false }));
    dispatch(setItemDetailsOpen({ item: item as Item, open: true }));
  };

  const handleSell = () => {
    dispatch(setItemMenuOpen({ item: item as Item, open: false }));
    dispatch(toggleItemToSell({ item: item, showConfirmation: true }));
  };

  const handleMoveToStash = () => {
    if (item && stashItemMovedStatus.status === Status.NotStarted) {
      dispatch(storeItemInStash(item.id));
      dispatch(setItemMenuOpen({ item: item as Item, open: false }));
    }
  };

  const handleMoveToInventory = () => {
    if (item && stashItemMovedStatus.status === Status.NotStarted) {
      dispatch(grabItemFromStash(item.id));
      dispatch(setItemMenuOpen({ item: item as Item, open: false }));
    }
  };

  const handleDestroy = () => {
    setMenuHeight(105);
    setConfirmDestroy(true);
  };

  const handleConfirmDestroy = () => {
    if (item && !isWalletItem) {
      dispatch(
        destroyItemRequest({
          itemId: item.id,
        })
      );
      dispatch(setItemMenuOpen({ item: item as Item, open: false }));
    }
  };

  return (
    <Grid container className={styles.main}>
      {((isWalletItem && width <= mdScreenWidth) || !isWalletItem) && (
        <Grid container className={styles.contextMenuContainer} style={{ height: menuHeight, left: menuX, top: menuY }}>
          <Grid container className={width <= xsScreenWidth ? styles.menuContainerMobile : styles.menuContainer}>
            <Image
              src={`/img/unity-assets/shared/tooltip_bg_vertical.png`}
              height={menuHeight}
              width={120}
              quality={100}
            />
            {!confirmDestroy && (
              <Grid container>
                <Grid container direction="column" className={styles.optionsContainer}>
                  <Grid container className={styles.topDivider} />
                  {!isEquipped &&
                    !isWalletItem &&
                    !isNftItem &&
                    !isLootItem &&
                    !isDeathItem &&
                    !isMerchantItem &&
                    !isSellableItem &&
                    !isStashItem &&
                    !displaySharedStash && (
                      <Grid container onClick={handleEquip} className={styles.optionContainer}>
                        <Typography component="span" className={styles.optionText}>{`EQUIP`}</Typography>
                        <Grid container className={styles.opacityContainer} />
                        <Grid container className={styles.divider} />
                      </Grid>
                    )}
                  {isEquipped && !isDeathItem && (
                    <Grid container onClick={handleUnequip} className={styles.optionContainer}>
                      <Typography component="span" className={styles.optionText}>{`UNEQUIP`}</Typography>
                      <Grid container className={styles.opacityContainer} />
                      <Grid container className={styles.divider} />
                    </Grid>
                  )}
                  {isLootItem && (
                    <Grid container onClick={handleTake} className={styles.optionContainer}>
                      <Typography component="span" className={styles.optionText}>{`TAKE`}</Typography>
                      <Grid container className={styles.opacityContainer} />
                      <Grid container className={styles.divider} />
                    </Grid>
                  )}
                  {isDeathItem && (
                    <Grid container onClick={handleKeep} className={styles.optionContainer}>
                      <Typography component="span" className={styles.optionText}>{`KEEP`}</Typography>
                      <Grid container className={styles.opacityContainer} />
                      <Grid container className={styles.divider} />
                    </Grid>
                  )}
                  {width <= mdScreenWidth && (
                    <Grid container onClick={handleDetails} className={styles.optionContainer}>
                      <Typography component="span" className={styles.optionText}>{`DETAILS`}</Typography>
                      <Grid container className={styles.opacityContainer} />
                      <Grid container className={styles.divider} />
                    </Grid>
                  )}
                  {isSellableItem && (
                    <Grid container onClick={handleSell} className={styles.optionContainer}>
                      <Typography component="span" className={styles.optionText}>{`SELL`}</Typography>
                      <Grid container className={styles.opacityContainer} />
                      <Grid container className={styles.divider} />
                    </Grid>
                  )}
                  {displaySharedStash && !isStashItem && !isNftItem && (
                    <Grid container onClick={handleMoveToStash} className={styles.optionContainer}>
                      <Typography component="span" className={styles.optionText}>{`MOVE TO STASH`}</Typography>
                      <Grid container className={styles.opacityContainer} />
                      <Grid container className={styles.divider} />
                    </Grid>
                  )}
                  {isStashItem && (
                    <Grid container onClick={handleMoveToInventory} className={styles.optionContainer}>
                      <Typography
                        component="span"
                        className={styles.moveToInventoryOptionText}
                      >{`MOVE TO INVENTORY`}</Typography>
                      <Grid container className={styles.opacityContainer} />
                      <Grid container className={styles.divider} />
                    </Grid>
                  )}
                  {!isWalletItem &&
                    !isNftItem &&
                    !isLootItem &&
                    !isDeathItem &&
                    !isMerchantItem &&
                    !isSellableItem &&
                    !isStashItem && (
                      <Grid container onClick={handleDestroy} className={styles.optionContainer}>
                        <Typography component="span" className={styles.destroyText}>{`DESTROY`}</Typography>
                        <Grid container className={styles.opacityContainer} />
                        <Grid container className={styles.divider} />
                      </Grid>
                    )}
                </Grid>
              </Grid>
            )}
            {confirmDestroy && (
              <Grid container>
                <Typography component="span" className={styles.confirmDestroyMessage}>
                  Are you sure you want to destroy this item?
                </Typography>
                <Grid container className={styles.confirmDestroyButtonContainer}>
                  <DCXButton
                    title="CONFIRM"
                    height={28}
                    width={100}
                    arrowTopAdjustment={7}
                    color="red"
                    onClick={() => handleConfirmDestroy()}
                  />
                </Grid>
              </Grid>
            )}
          </Grid>
        </Grid>
      )}
    </Grid>
  );
};

export default DCXContextMenu;
