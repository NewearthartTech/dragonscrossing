import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import styles from "./keep-item-choice.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { useEffect, useState } from "react";
import DCXButton from "@/components/dcx-button/dcx-button";
import Image from "next/image";
import { clearItemToKeep, selectItemToKeep } from "@/state-mgmt/item/itemSlice";
import {
  keepItemAtDeath,
  resetKeepItemAtDeathStatus,
  selectInventory,
  selectKeepItemAtDeathStatus,
  selectSelectedHero,
} from "@/state-mgmt/hero/heroSlice";
import { useRouter } from "next/router";
import ItemDND from "../inventory/item-dnd";
import { clearCombatActionResult, selectCombatActionResult } from "@/state-mgmt/combat/combatSlice";
import { Item } from "@/state-mgmt/item/itemTypes";
import useWindowDimensions from "@/helpers/window-dimensions";
import { xsScreenWidth } from "@/helpers/global-constants";
import { selectGameState, updateGameState } from "@/state-mgmt/game-state/gameStateSlice";
import { ItemSlotTypeDto } from "@dcx/dcx-backend";
import { Status } from "@/state-mgmt/app/appTypes";
import Scrollbars from "react-custom-scrollbars";

interface Props {}

const KeepItemChoice: React.FC<Props> = (props: Props) => {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const inventory = useAppSelector(selectInventory);
  const itemToKeep = useAppSelector(selectItemToKeep);
  const { hero } = useAppSelector(selectSelectedHero);
  const combatResult = useAppSelector(selectCombatActionResult);
  const keepItemAtDeathStatus = useAppSelector(selectKeepItemAtDeathStatus);
  const { width, height } = useWindowDimensions();

  const [keepConfirmation, setKeepConfirmation] = useState(false);

  useEffect(() => {
    if (keepItemAtDeathStatus.status === Status.Loaded) {
      setKeepConfirmation(false);
      dispatch(clearCombatActionResult());
      dispatch(clearItemToKeep());
      dispatch(resetKeepItemAtDeathStatus());
      dispatch(updateGameState("aedos"));
    }
    if (keepItemAtDeathStatus.status === Status.Failed) {
      setKeepConfirmation(false);
      dispatch(resetKeepItemAtDeathStatus());
    }
  }, [keepItemAtDeathStatus]);

  const handleContinue = () => {
    setKeepConfirmation(true);
  };

  const handleKeepConfirmation = () => {
    if (itemToKeep && itemToKeep.id !== "") {
      dispatch(keepItemAtDeath(itemToKeep.id));
    } else {
      setKeepConfirmation(false);
      dispatch(clearCombatActionResult());
      dispatch(clearItemToKeep());
      dispatch(resetKeepItemAtDeathStatus());
      dispatch(updateGameState("aedos"));
    }
  };

  const handleClose = () => {
    setKeepConfirmation(false);
  };

  const renderAllItems = () => {
    const combinedItems: Item[] = [];
    combinedItems.push(...inventory);
    combinedItems.push(...hero.equippedItems);
    const items: JSX.Element[] = [];
    let i = 0;
    combinedItems.forEach((item) => {
      if (
        item.slot !== ItemSlotTypeDto.UnidentifiedSkill &&
        item.slot !== ItemSlotTypeDto.UnlearnedSkill &&
        item.slot !== ItemSlotTypeDto.Shard
      ) {
        const itemIndex = combinedItems.findIndex((i) => i.id === item.id);
        let itemPosition;
        let detailsLeft = false;
        if (width > xsScreenWidth) {
          itemPosition = (itemIndex as number) % 6;
          if (itemPosition === 3 || itemPosition === 4 || itemPosition === 5) {
            detailsLeft = true;
          }
        } else {
          itemPosition = (itemIndex as number) % 4;
          if (itemPosition === 2 || itemPosition === 3) {
            detailsLeft = true;
          }
        }
        items.push(
          <Grid container key={item.id} className={styles.itemContainer}>
            <ItemDND
              item={item}
              itemIndex={i}
              disableDND={true}
              isDeathItem={true}
              detailsLeft={detailsLeft}
              showOpaque={item.id !== itemToKeep.id ? true : false}
            />
          </Grid>
        );
        i++;
      }
    });

    return items;
  };

  return (
    <Grid container direction="column" className={styles.main}>
      <Grid container className={styles.deathTollMain}>
        <Grid container className={styles.descriptionBackground}>
          <Image src="/img/unity-assets/shared/action_bg.png" height={150} width={496} quality={100} />
        </Grid>
        <Grid item className={styles.header}>
          <Image src="/img/unity-assets/shared/header_red.png" height={39} width={188} quality={100} />
        </Grid>
        <Typography component="span" className={styles.headerText}>
          {`DEATH'S TOLL`}
        </Typography>
        <Grid container direction="column" className={styles.descriptionContainer}>
          {!hero.isAscended && (
            <Typography component="span" className={styles.lossText}>
              {`YOU HAVE LOST:`}
            </Typography>
          )}
          <Typography component="span" className={styles.lossText}>
            ALL REMAINING QUESTS FOR THE DAY
          </Typography>
          <Typography component="span" className={styles.lossText}>
            ALL ITEMS EQUIPPED AND STORED IN INVENTORY
          </Typography>
          <Grid container className={styles.separator} />
          <Typography component="span" className={styles.keepText}>
            YOU MAY CHOOSE ONE ITEM TO TAKE WITH YOU
          </Typography>
        </Grid>
      </Grid>
      <Grid container className={styles.itemsMain}>
        <Grid container className={styles.itemsBackground}>
          <Image src="/img/unity-assets/shared/action_bg.png" height={485} width={496} quality={100} />
        </Grid>
        <Grid container className={styles.itemsContainer}>
          <Scrollbars
            renderThumbVertical={() => (
              <Grid
                container
                style={{
                  width: "5px",
                  borderRadius: "4px",
                  backgroundColor: "rgb(230, 230, 230)",
                }}
              />
            )}
          >
            <Grid item container className={styles.itemHeightContainer}>
              {renderAllItems()}
            </Grid>
          </Scrollbars>
        </Grid>
        <Grid container direction="row" className={styles.buttonContainer}>
          <DCXButton title="CONTINUE" height={32} width={120} color="blue" onClick={() => handleContinue()} />
        </Grid>
      </Grid>
      <Modal open={keepConfirmation} onClose={handleClose} className={styles.modalMain}>
        <Grid container className={styles.modalContainer}>
          <Grid item className={styles.confirmationBackground}>
            <Image src="/img/unity-assets/shared/tooltip_bg.png" height={100} width={210} quality={100} />
          </Grid>
          <Grid container className={styles.confirmTextContainer}>
            <Typography component="span" className={styles.confirmText}>
              {itemToKeep && itemToKeep.id !== ""
                ? `ARE YOU SURE ABOUT YOUR SELECTION?`
                : `YOU HAVEN'T SELECTED AN ITEM TO KEEP, ARE YOU SURE YOU WANT TO LEAVE?`}
            </Typography>
          </Grid>
          <Grid container className={styles.confirmDone}>
            <DCXButton title="CONFIRM" height={32} width={120} color="red" onClick={() => handleKeepConfirmation()} />
          </Grid>
        </Grid>
      </Modal>
    </Grid>
  );
};

export default KeepItemChoice;
