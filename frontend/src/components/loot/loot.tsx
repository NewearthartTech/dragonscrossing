import Modal from "@mui/material/Modal";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./loot.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useEffect, useState } from "react";
import DCXButton from "@/components/dcx-button/dcx-button";
import Image from "next/image";
import {
  clearCombatActionResult,
  clearCombatActionType,
  clearMonsterLootUpdatedStatus,
  pickUpLootRequest,
  selectCombatActionResult,
  selectMonsterLootUpdatedStatus,
} from "@/state-mgmt/combat/combatSlice";
import ItemDND from "../inventory/item-dnd";
import { getSelectedHero, selectInventory, selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { useRouter } from "next/router";
import { selectGameState, updateGameState } from "@/state-mgmt/game-state/gameStateSlice";
import { DcxTiles, ItemSlotTypeDto } from "@dcx/dcx-backend";
import { Status } from "@/state-mgmt/app/appTypes";
import DcxLogo from "../dcx-logo/dcx-logo";

interface Props {}

const Loot: React.FC<Props> = (props: Props) => {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const loot = useAppSelector(selectCombatActionResult).monsterResult.loot;
  const monsterLootUpdatedStatus = useAppSelector(selectMonsterLootUpdatedStatus);
  const inventory = useAppSelector(selectInventory);
  const gameState = useAppSelector(selectGameState);

  const [lootingConfirmation, setLootingConfirmation] = useState(false);
  const [lootingMessage, setLootingMessage] = useState(``);

  useEffect(() => {
    if (!loot || loot.items.length === 0) {
      setLootingMessage(`YOU HAVE WRETCHED LUCK AND FIND NOTHING WORTH LOOTING`);
    }
  }, []);

  useEffect(() => {
    if (monsterLootUpdatedStatus.status === Status.Loaded) {
      dispatch(getSelectedHero());
      if (!loot || loot.items.length === 0) {
        setLootingMessage(`THERE'S NOTHING LEFT TO LOOT`);
      }
    }
  }, [monsterLootUpdatedStatus]);

  const handleTakeAll = () => {
    const itemIds = loot?.items.map((i) => i.id);
    if (itemIds && itemIds.length > 0) {
      dispatch(pickUpLootRequest(itemIds));
    }
  };

  const handleDoneLooting = () => {
    if (loot && loot.items.length > 0) {
      setLootingConfirmation(true);
    } else {
      handleDoneLootingConfirmation();
    }
  };

  const handleDoneLootingConfirmation = () => {
    setLootingConfirmation(false);
    dispatch(clearMonsterLootUpdatedStatus());
    dispatch(clearCombatActionType());
    dispatch(clearCombatActionResult());
    // redirect player to the game state current zone
    dispatch(updateGameState(gameState.zone.slug as DcxTiles));
  };

  const handleClose = () => {
    setLootingConfirmation(false);
  };

  const renderLootItems = () => {
    const items: JSX.Element[] = [];
    let i = 0;
    if (loot) {
      loot.items.forEach((item) => {
        let detailsLeft = false;
        const itemPosition = i % 3;
        if (itemPosition === 2) {
          detailsLeft = true;
        }
        items.push(
          <Grid container key={item.id} className={styles.itemContainer}>
            <ItemDND
              item={item}
              itemIndex={i}
              isNftItem={item?.slot === ItemSlotTypeDto.UnidentifiedSkill || item?.slot === ItemSlotTypeDto.Shard}
              isLootItem={true}
              detailsLeft={detailsLeft}
            />
          </Grid>
        );
        i++;
      });
    }

    return items;
  };

  return (
    <Grid container direction="row" className={styles.main}>
      {loot && (
        <Grid container direction="row" className={styles.container}>
          <Image src="/img/unity-assets/shared/rectangle_vertical_bg.png" height={500} width={385} quality={100} />
          <Grid item className={styles.topLeft}>
            <Image src="/img/unity-assets/shared/window_top_left.png" height={50} width={70} quality={100} />
          </Grid>
          <Grid item className={styles.topRight}>
            <Image src="/img/unity-assets/shared/window_top_right.png" height={50} width={70} quality={100} />
          </Grid>
          <Grid item className={styles.bottomLeft}>
            <Image src="/img/unity-assets/shared/window_bottom_left.png" height={30} width={43} quality={100} />
          </Grid>
          <Grid item className={styles.bottomRight}>
            <Image src="/img/unity-assets/shared/window_bottom_right.png" height={30} width={43} quality={100} />
          </Grid>
          <Grid item className={styles.header}>
            <Image src="/img/unity-assets/shared/header_gold.png" height={39} width={188} quality={100} />
          </Grid>
          <Typography component="span" className={styles.headerText}>
            MONSTER LOOT
          </Typography>
          {/* <Grid container className={styles.lootedDcxContainer}>
            <Typography component="span" className={styles.dcx}>
              {`${loot.dcx}`}
            </Typography>
            <Grid container className={styles.dcxLogoContainer}>
              <DcxLogo />
            </Grid>
          </Grid> */}
          <Grid container className={styles.topSeparator} />
          <Grid container direction="row" className={styles.itemsContainer}>
            {loot.items.length > 0 && renderLootItems()}
          </Grid>
          {loot.items.length === 0 && (
            <Grid container className={styles.noItemsMessageContainer}>
              <Typography component="span" className={styles.noItemsMessage}>
                {lootingMessage}
              </Typography>
            </Grid>
          )}
          <Grid container className={styles.bottomSeparator} />
          <Grid container className={styles.takeAll}>
            <DCXButton
              title="TAKE ALL"
              height={32}
              width={120}
              color="blue"
              disabled={
                60 - inventory.length < loot.items.length || monsterLootUpdatedStatus.status !== Status.NotStarted
              }
              onClick={() => handleTakeAll()}
            />
          </Grid>
          <Grid container className={styles.doneLooting}>
            <DCXButton title="DONE LOOTING" height={32} width={120} color="red" onClick={() => handleDoneLooting()} />
          </Grid>
        </Grid>
      )}
      <Modal open={lootingConfirmation} onClose={handleClose} className={styles.modalMain}>
        <Grid container className={styles.modalContainer}>
          <Grid item className={styles.confirmationBackground}>
            <Image src="/img/unity-assets/shared/tooltip_bg.png" height={110} width={210} quality={100} />
          </Grid>
          <Typography component="span" className={styles.confirmText}>
            ARE YOU SURE YOU WANT TO LEAVE THE SPOILS OF YOUR VICTORY?
          </Typography>
          <Grid container className={styles.confirmDone}>
            <DCXButton
              title="LEAVE IT"
              height={32}
              width={120}
              color="red"
              onClick={() => handleDoneLootingConfirmation()}
            />
          </Grid>
        </Grid>
      </Modal>
    </Grid>
  );
};

export default Loot;
