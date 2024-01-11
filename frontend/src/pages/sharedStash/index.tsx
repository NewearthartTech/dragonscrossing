import { withHoc } from "@/components/hoc/hoc";
import Grid from "@mui/material/Grid";
import type { NextPage } from "next";
import styles from "./sharedStash.module.scss";
import Image from "next/image";
import DCXButton from "@/components/dcx-button/dcx-button";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { selectGameState, updateGameState } from "@/state-mgmt/game-state/gameStateSlice";
import { getSelectedHero, selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { DcxTiles, ItemSlotTypeDto } from "@dcx/dcx-backend";
import Typography from "@mui/material/Typography";
import { mdScreenWidth, smScreenWidth, xsScreenWidth } from "@/helpers/global-constants";
import ItemDND from "@/components/inventory/item-dnd";
import useWindowDimensions from "@/helpers/window-dimensions";
import { useEffect, useState } from "react";
import { setDisableInventory } from "@/state-mgmt/item/itemSlice";
import {
  resetStashItemMovedStatus,
  retrievePlayer,
  selectPlayer,
  selectStashItemMovedStatus,
} from "@/state-mgmt/player/playerSlice";
import { setDisplaySharedStash } from "@/state-mgmt/app/appSlice";
import { Status } from "@/state-mgmt/app/appTypes";

interface Props {}

const SharedStash: NextPage<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const { hero } = useAppSelector(selectSelectedHero);
  const player = useAppSelector(selectPlayer);
  const gameState = useAppSelector(selectGameState);
  const stashItemMovedStatus = useAppSelector(selectStashItemMovedStatus);
  const { width, height } = useWindowDimensions();

  const [showInventory, setShowInventory] = useState(false);

  useEffect(() => {
    dispatch(setDisplaySharedStash(true));
    dispatch(setDisableInventory(true));
  }, []);

  useEffect(() => {
    if (stashItemMovedStatus.status === Status.Loaded) {
      dispatch(getSelectedHero());
      dispatch(retrievePlayer());
      dispatch(resetStashItemMovedStatus());
    }
    if (stashItemMovedStatus.status === Status.Failed) {
      dispatch(resetStashItemMovedStatus());
    }
  }, [stashItemMovedStatus]);

  const handleInventoryClick = () => {
    setShowInventory(true);
  };

  const handleStashClick = () => {
    setShowInventory(false);
  };

  const handleLeaveClick = () => {
    dispatch(setDisplaySharedStash(false));
    dispatch(setDisableInventory(false));
    dispatch(updateGameState(gameState.zone.slug as DcxTiles));
  };

  const calculateInventoryHeight = () => {
    if (width > smScreenWidth) {
      return 600;
    } else if (width > xsScreenWidth) {
      return 575;
    } else {
      return 550;
    }
  };

  const calculateInventoryWidth = () => {
    if (width > smScreenWidth) {
      return 765;
    } else if (width > xsScreenWidth) {
      return 470;
    } else {
      return 350;
    }
  };

  const renderInventory = () => {
    const items: JSX.Element[] = [];
    for (let i = 0; i < 60; i++) {
      let inventoryItem;
      let detailsLeft = false;
      if (typeof hero.inventory[i] !== "undefined") {
        inventoryItem = hero.inventory[i];
        // Determine if details should show on left or right of item and above or below
        if (width > smScreenWidth) {
          const itemPosition = i % 10;
          if (itemPosition === 7 || itemPosition === 8 || itemPosition === 9) {
            detailsLeft = true;
          }
        } else if (width > xsScreenWidth && width <= smScreenWidth) {
          const itemPosition = i % 6;
          if (itemPosition === 3 || itemPosition === 4 || itemPosition === 5) {
            detailsLeft = true;
          }
        } else {
          const itemPosition = i % 4;
          if (itemPosition === 2 || itemPosition === 3) {
            detailsLeft = true;
          }
        }
      }
      items.push(
        <ItemDND
          item={inventoryItem}
          itemIndex={i}
          isNftItem={
            inventoryItem?.slot === ItemSlotTypeDto.UnidentifiedSkill || inventoryItem?.slot === ItemSlotTypeDto.Shard
          }
          disableContextMenu={
            width > mdScreenWidth &&
            (inventoryItem?.slot === ItemSlotTypeDto.UnidentifiedSkill || inventoryItem?.slot === ItemSlotTypeDto.Shard)
          }
          detailsLeft={detailsLeft}
          key={i}
        />
      );
    }
    return items;
  };

  const renderStashItems = () => {
    const items: JSX.Element[] = [];
    for (let i = 0; i < 4; i++) {
      let stashItem;
      let detailsLeft = false;
      if (typeof player.sharedStash[i] !== "undefined") {
        stashItem = player.sharedStash[i];
        const itemPosition = i % 2;
        if (itemPosition === 1) {
          detailsLeft = true;
        }
      }
      items.push(
        <ItemDND
          item={stashItem}
          isStashItem={true}
          disableDND={true}
          itemIndex={i}
          detailsLeft={detailsLeft}
          key={i}
        />
      );
    }
    return items;
  };

  return (
    <Grid container direction="column" className={styles.main}>
      <Image src="/img/backgrounds/sharedStash-interior.jpg" layout="fill" quality={100} />
      <Grid container className={styles.opaqueContainer} />

      <Grid
        container
        direction="row"
        className={styles.container}
        style={{
          height: showInventory ? calculateInventoryHeight() : 315,
          width: showInventory ? calculateInventoryWidth() : 360,
        }}
      >
        {showInventory ? (
          <Image
            src="/img/unity-assets/shared/action_bg.png"
            height={calculateInventoryHeight()}
            width={calculateInventoryWidth()}
            quality={100}
          />
        ) : (
          <Image src="/img/unity-assets/shared/rectangle_vertical_bg.png" height={315} width={360} quality={100} />
        )}
        <Grid container className={showInventory ? styles.inventoryTopLeftContainer : styles.topLeftContainer}>
          <Image src="/img/unity-assets/shared/window_top_left.png" height={40} width={56} quality={100} />
        </Grid>
        <Grid container className={showInventory ? styles.inventoryTopRightContainer : styles.topRightContainer}>
          <Image src="/img/unity-assets/shared/window_top_right.png" height={40} width={60} quality={100} />
        </Grid>
        <Grid container className={showInventory ? styles.inventoryBottomLeftContainer : styles.bottomLeftContainer}>
          <Image src="/img/unity-assets/shared/window_bottom_left.png" height={30} width={43} quality={100} />
        </Grid>
        <Grid container className={showInventory ? styles.inventoryBottomRightContainer : styles.bottomRightContainer}>
          <Image src="/img/unity-assets/shared/window_bottom_right.png" height={30} width={43} quality={100} />
        </Grid>
        {!showInventory && (
          <Grid container className={styles.stakingHeaderContainer}>
            <Image src="/img/unity-assets/shared/header_gold.png" height={51} width={222} quality={100} />
            <Typography component="span" className={styles.stakingHeaderText}>
              {`SHARED STASH`}
            </Typography>
          </Grid>
        )}
        <Grid
          container
          className={
            showInventory ? styles.inventoryNavigationButtonsContainer : styles.stashNavigationButtonsContainer
          }
        >
          <DCXButton
            title="INVENTORY"
            height={32}
            width={120}
            color="blue"
            inactive={!showInventory}
            arrowTopAdjustment={10}
            onClick={() => handleInventoryClick()}
          />
          <DCXButton
            title="STASH"
            height={32}
            width={120}
            color="blue"
            inactive={showInventory}
            arrowTopAdjustment={10}
            onClick={() => handleStashClick()}
          />
        </Grid>
        <Grid
          container
          className={showInventory ? styles.questMessageContainerInventory : styles.questMessageContainerStash}
        >
          <Typography component="span" className={styles.questMessage}>
            {`MOVING AN ITEM COSTS 2 QUESTS`}
          </Typography>
        </Grid>
        {showInventory ? (
          <Grid container className={styles.stashInventoryItemsContainer}>
            {renderInventory()}
          </Grid>
        ) : (
          <Grid container className={styles.stashItemsContainer}>
            {renderStashItems()}
          </Grid>
        )}
        <Grid container className={showInventory ? styles.inventoryLeaveButtonContainer : styles.leaveButtonContainer}>
          <DCXButton
            title="LEAVE"
            height={32}
            width={120}
            color="blue"
            arrowTopAdjustment={10}
            onClick={handleLeaveClick}
          />
        </Grid>
      </Grid>
    </Grid>
  );
};

export default withHoc(SharedStash);
