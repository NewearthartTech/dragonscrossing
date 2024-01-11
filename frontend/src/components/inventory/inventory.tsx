import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import Container from "@mui/material/Container";
import styles from "./inventory.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import {
  selectDisplayInventory,
  selectDisplayLoadout,
  selectDisplayWallet,
  setDisplayInventory,
  setDisplayLoadout,
  setDisplayWallet,
} from "@/state-mgmt/app/appSlice";
import { useEffect, useState } from "react";
import { equipItemRequest, selectInventory, selectSelectedHero } from "@/state-mgmt/hero/heroSlice";
import { DropTarget } from "react-drag-drop-container";
import { SoundType, Status } from "@/state-mgmt/app/appTypes";
import { Item } from "@/state-mgmt/item/itemTypes";
import ItemDND from "./item-dnd";
import DCXButton from "../dcx-button/dcx-button";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { lgScreenWidth, mdScreenWidth, smScreenWidth, xsScreenWidth } from "@/helpers/global-constants";
import { selectDcxBalance, selectGetPlayerStatus, selectWalletItems } from "@/state-mgmt/player/playerSlice";
import { ItemSlotTypeDto } from "@dcx/dcx-backend";
import CloseButton from "../close-button/close-button";
import DCXAudioPlayer from "../dcx-audio-player/dcx-audio-player";
import DcxLogo from "../dcx-logo/dcx-logo";
import { selectDisableInventory } from "@/state-mgmt/item/itemSlice";

interface Props {}

const Inventory: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const displayWallet = useAppSelector(selectDisplayWallet);
  const displayInventory = useAppSelector(selectDisplayInventory);
  const displayLoadout = useAppSelector(selectDisplayLoadout);
  const hero = useAppSelector(selectSelectedHero).hero;
  const heroInventory = useAppSelector(selectInventory);
  const walletItems = useAppSelector(selectWalletItems);
  const getPlayerStatus = useAppSelector(selectGetPlayerStatus);
  const isDisableInventory = useAppSelector(selectDisableInventory);
  const dcxBalance = useAppSelector(selectDcxBalance);
  const { width, height } = useWindowDimensions();

  const [isHeadEquipped, setHeadEquipped] = useState(false);
  const [isMainHandEquipped, setMainHandEquipped] = useState(false);
  const [isTwoHandEquipped, setTwoHandEquipped] = useState(false);
  const [isOffHandEquipped, setOffHandEquipped] = useState(false);
  const [isChestEquipped, setChestEquipped] = useState(false);
  const [isRingEquipped, setRingEquipped] = useState(false);
  const [isFeetEquipped, setFeetEquipped] = useState(false);
  const [walletPage, setWalletPage] = useState(1);
  const [walletPageCount, setWalletPageCount] = useState(1);
  const [playDropSound, setPlayDropSound] = useState(false);
  const [dropSound, setDropSound] = useState("");

  useEffect(() => {
    setEquippedItems();
    renderInventory();
  }, [hero]);

  useEffect(() => {
    renderWalletInventory();
  }, [walletItems]);

  useEffect(() => {
    if (getPlayerStatus.status === Status.Loaded) {
      // const itemCount = player.wallet.length;
      const itemCount = 0;
      const pageCount = Math.floor(itemCount / 60) + 1;
      setWalletPageCount(pageCount);
    }
  }, [getPlayerStatus]);

  const handleClose = () => {
    dispatch(setDisplayInventory(false));
  };

  const handleInventoryClick = () => {
    dispatch(setDisplayInventory(true));
  };

  const handleWalletClick = () => {
    dispatch(setDisplayWallet(true));
  };

  const handleLoadoutClick = () => {
    dispatch(setDisplayLoadout(true));
  };

  const incrementPage = () => {
    if (walletPage !== walletPageCount) {
      setWalletPage(walletPage + 1);
    }
  };

  const decrementPage = () => {
    if (walletPage !== 1) {
      setWalletPage(walletPage - 1);
    }
  };

  const dropped = (e: any) => {
    setDropSound(e.dragData.data.itemDropSound);
    setPlayDropSound(true);
    // Get the index of the dragged item
    const draggedItemIndex = heroInventory.findIndex((item: Item) => item.id === e.dragData.data.id);

    if (draggedItemIndex !== -1) {
      dispatch(
        equipItemRequest({
          itemId: e.dragData.data.id,
          inventoryIndex: draggedItemIndex,
        })
      );
    }
  };

  const setEquippedItems = () => {
    if (hero && hero.id !== -1 && hero.equippedItems) {
      hero.equippedItems.forEach((item: Item) => {
        if (item.slot === ItemSlotTypeDto.Head) {
          setHeadEquipped(true);
        } else if (item.slot === ItemSlotTypeDto.MainHand) {
          setMainHandEquipped(true);
        } else if (item.slot === ItemSlotTypeDto.TwoHand) {
          setTwoHandEquipped(true);
        } else if (item.slot === ItemSlotTypeDto.OffHand) {
          setOffHandEquipped(true);
        } else if (item.slot === ItemSlotTypeDto.Chest) {
          setChestEquipped(true);
        } else if (item.slot === ItemSlotTypeDto.Ring) {
          setRingEquipped(true);
        } else if (item.slot === ItemSlotTypeDto.Feet) {
          setFeetEquipped(true);
        }
      });
    }
  };

  const getEquippedItem = (slot: ItemSlotTypeDto) => {
    if (hero && hero.id !== -1 && hero.equippedItems) {
      const item = hero.equippedItems.find((i) => i.slot === slot);
      if (item) {
        return item;
      } else {
        if (slot === ItemSlotTypeDto.Head) {
          setHeadEquipped(false);
        } else if (slot === ItemSlotTypeDto.MainHand) {
          setMainHandEquipped(false);
        } else if (slot === ItemSlotTypeDto.TwoHand) {
          setTwoHandEquipped(false);
        } else if (slot === ItemSlotTypeDto.OffHand) {
          setOffHandEquipped(false);
        } else if (slot === ItemSlotTypeDto.Chest) {
          setChestEquipped(false);
        } else if (slot === ItemSlotTypeDto.Ring) {
          setRingEquipped(false);
        } else {
          setFeetEquipped(false);
        }
      }
    }
  };

  const renderInventory = () => {
    const items: JSX.Element[] = [];
    for (let i = 0; i < 60; i++) {
      let inventoryItem;
      let detailsLeft = false;
      if (typeof heroInventory[i] !== "undefined") {
        inventoryItem = heroInventory[i];
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
            inventoryItem?.slot === ItemSlotTypeDto.UnidentifiedSkill || inventoryItem?.slot === ItemSlotTypeDto.Shard || inventoryItem?.slot === ItemSlotTypeDto.NftAction 
          }
          disableContextMenu={
            width > mdScreenWidth &&
            (inventoryItem?.slot === ItemSlotTypeDto.UnidentifiedSkill || inventoryItem?.slot === ItemSlotTypeDto.Shard || inventoryItem?.slot === ItemSlotTypeDto.NftAction)
          }
          detailsLeft={detailsLeft}
          key={i}
        />
      );
    }
    return items;
  };

  const renderWalletInventory = () => {
    const items: JSX.Element[] = [];

    const indexStart = (walletPage - 1) * 60;
    const indexEnd = walletPage * 60;

    for (let i = indexStart; i < indexEnd; i++) {
      let inventoryItem;
      let detailsLeft = false;
      if (typeof walletItems[i] !== "undefined") {
        inventoryItem = walletItems[i];

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
          isNftItem={true}
          isWalletItem={true}
          disableContextMenu={false}
          disableDND={true}
          detailsLeft={detailsLeft}
          key={i}
        />
      );
    }
    return items;
  };

  return (
    <Modal
      open={displayInventory || displayWallet || displayLoadout}
      onClose={handleClose}
      className={styles.modalMain}
    >
      {heroInventory && (
        <Container maxWidth="lg" className={styles.container}>
          {playDropSound && (
            <DCXAudioPlayer
              audioUrl={`/audio/sound-effects/item/${dropSound.toLowerCase()}`}
              soundType={SoundType.SOUND_EFFECT}
              onEnded={() => setPlayDropSound(false)}
            />
          )}
          <Grid container direction="row" className={styles.secondaryContainer}>
            {(displayWallet || displayInventory) && (
              <Grid container className={styles.inventoryContainer}>
                <Grid container className={styles.inventoryBackground}>
                  <Image src="/img/unity-assets/shared/action_bg.png" height={550} width={765} quality={100} />
                </Grid>
                {width <= mdScreenWidth && (
                  <Grid container className={styles.closeButtonContainerInventory}>
                    <CloseButton handleClose={handleClose} />
                  </Grid>
                )}
                {width <= mdScreenWidth && (
                  <Grid container className={styles.viewToggleContainer}>
                    {displayInventory && (
                      <Grid container direction="row" className={styles.buttons}>
                        <DCXButton
                          title="WALLET"
                          height={32}
                          width={120}
                          color="blue"
                          arrowTopAdjustment={10}
                          onClick={() => handleWalletClick()}
                        />
                        <DCXButton
                          title="LOADOUT"
                          height={32}
                          width={120}
                          color="blue"
                          arrowTopAdjustment={10}
                          onClick={() => handleLoadoutClick()}
                        />
                      </Grid>
                    )}
                    {displayWallet && (
                      <Grid container direction="row" className={styles.buttons}>
                        <DCXButton
                          title="INVENTORY"
                          height={32}
                          width={120}
                          color="blue"
                          arrowTopAdjustment={10}
                          disabled={isDisableInventory}
                          onClick={() => handleInventoryClick()}
                        />
                        <DCXButton
                          title="LOADOUT"
                          height={32}
                          width={120}
                          color="blue"
                          arrowTopAdjustment={10}
                          onClick={() => handleLoadoutClick()}
                        />
                      </Grid>
                    )}
                  </Grid>
                )}
                <Grid container direction="row" className={styles.itemsContainer}>
                  {displayInventory ? renderInventory() : renderWalletInventory()}
                </Grid>
                <Grid container className={styles.bottomContainerBackground} />
                {displayInventory && (
                  <Grid container className={styles.inventoryFooterContainer}>
                    <Grid container className={styles.footerLeftContainerInventory}>
                      <Typography component="span" className={styles.unsecuredDCX}>
                        <Grid container direction="row">
                          <p className={styles.dcxAmount}>{`${dcxBalance} USDC`}</p>
                        </Grid>
                      </Typography>
                      {/* <Grid container className={styles.dcxLogoContainer}>
                        <DcxLogo />
                      </Grid> */}
                    </Grid>
                    <Grid container className={styles.footerRightContainerInventory}>
                      <Typography component="span" className={styles.itemTotal}>
                        <Grid container direction="row">
                          <p className={styles.wordSpacing}>{width > xsScreenWidth ? `TOTAL ITEMS` : `ITEMS`}</p>
                          <p>{`${heroInventory.length}/60`}</p>
                        </Grid>
                      </Typography>
                    </Grid>
                  </Grid>
                )}
                {displayWallet && (
                  <Grid container className={styles.inventoryFooterContainer}>
                    <Grid container className={styles.footerLeftContainer}>
                      <Typography component="span" className={styles.securedDCX}>
                        <Grid container direction="row">
                          <p className={styles.dcxAmount}>{`${dcxBalance} USDC`}</p>
                        </Grid>
                      </Typography>
                      {/* <Grid container className={styles.dcxLogoContainer}>
                        <DcxLogo />
                      </Grid> */}
                    </Grid>
                    <Grid container className={styles.footerCenterContainerWallet}>
                      <Grid container direction="row" className={styles.pagination}>
                        <Typography
                          component="span"
                          className={walletPage === 1 ? styles.leftArrowDisabled : styles.leftArrow}
                          onClick={decrementPage}
                        >
                          {`<`}
                        </Typography>
                        <Typography component="span" className={styles.pageNumber}>
                          {walletPage}
                        </Typography>
                        <Typography
                          component="span"
                          className={walletPage === walletPageCount ? styles.rightArrowDisabled : styles.rightArrow}
                          onClick={incrementPage}
                        >
                          {`>`}
                        </Typography>
                      </Grid>
                    </Grid>
                    <Grid container className={styles.footerRightContainer}>
                      <Typography component="span" className={styles.itemTotal}>
                        <Grid container direction="row">
                          <p className={styles.wordSpacing}>{width > xsScreenWidth ? `TOTAL ITEMS` : `ITEMS`}</p>
                          <p>{`${walletItems.length}`}</p>
                        </Grid>
                      </Typography>
                    </Grid>
                  </Grid>
                )}
              </Grid>
            )}
            {(displayLoadout || width > mdScreenWidth) && (
              <Grid container direction="row" className={styles.equippedItemsContainer}>
                <Grid container className={styles.characterBackground}>
                  <Image
                    src="/img/unity-assets/inventory/inventory_character_bg.png"
                    height={561}
                    width={326}
                    quality={100}
                  />
                </Grid>
                <Typography component="span" className={styles.inventoryHeader}>
                  LOADOUT
                </Typography>
                <Grid container className={styles.closeButtonContainer}>
                  <CloseButton handleClose={handleClose} />
                </Grid>
                <Grid container className={styles.buttonContainer}>
                  <Grid container direction="row" className={styles.buttons}>
                    <DCXButton
                      title="INVENTORY"
                      height={32}
                      width={120}
                      color="blue"
                      arrowTopAdjustment={10}
                      inactive={width > mdScreenWidth && !displayInventory}
                      disabled={isDisableInventory}
                      onClick={() => handleInventoryClick()}
                    />
                    <DCXButton
                      title="WALLET"
                      height={32}
                      width={120}
                      color="blue"
                      arrowTopAdjustment={10}
                      inactive={width > mdScreenWidth && !displayWallet}
                      onClick={() => handleWalletClick()}
                    />
                  </Grid>
                </Grid>
                <Grid container className={styles.characterShadow}>
                  <Image
                    src="/img/unity-assets/inventory/character_shadow.png"
                    height={380}
                    width={186}
                    quality={100}
                  />
                </Grid>
                <Grid container className={styles.headSlotContainer}>
                  <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.Head} dropData={{}}>
                    <Grid container className={styles.slotImageContainer}>
                      <Grid container className={styles.equippedSlot}>
                        <Image src="/img/unity-assets/inventory/equipped_item_slot.png" height={70} width={70} />
                      </Grid>
                      <Grid container className={styles.slotIcon}>
                        <Image src="/img/unity-assets/inventory/head_slot_icon.png" height={48} width={48} />
                      </Grid>
                      {isHeadEquipped && (
                        <ItemDND item={getEquippedItem(ItemSlotTypeDto.Head)} itemIndex={0} isEquippedItem={true} />
                      )}
                    </Grid>
                  </DropTarget>
                </Grid>
                <Grid container className={styles.chestSlotContainer}>
                  <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.Chest} dropData={{}}>
                    <Grid container className={styles.slotImageContainer}>
                      <Grid container className={styles.equippedSlot}>
                        <Image src="/img/unity-assets/inventory/equipped_item_slot.png" height={70} width={70} />
                      </Grid>
                      <Grid container className={styles.slotIcon}>
                        <Image src="/img/unity-assets/inventory/chest_slot_icon.png" height={48} width={48} />
                      </Grid>
                      {isChestEquipped && (
                        <ItemDND item={getEquippedItem(ItemSlotTypeDto.Chest)} itemIndex={1} isEquippedItem={true} />
                      )}
                    </Grid>
                  </DropTarget>
                </Grid>
                <Grid container className={styles.mainHandSlotContainer}>
                  <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.MainHand} dropData={{}}>
                    <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.TwoHand} dropData={{}}>
                      <Grid container className={styles.slotImageContainer}>
                        <Grid container className={styles.equippedSlot}>
                          <Image src="/img/unity-assets/inventory/equipped_item_slot.png" height={70} width={70} />
                        </Grid>
                        <Grid container className={styles.slotIcon}>
                          <Image src="/img/unity-assets/inventory/main_hand_slot_icon.png" height={48} width={48} />
                        </Grid>
                        {isMainHandEquipped && (
                          <ItemDND
                            item={getEquippedItem(ItemSlotTypeDto.MainHand)}
                            itemIndex={2}
                            isEquippedItem={true}
                          />
                        )}
                        {isTwoHandEquipped && (
                          <ItemDND
                            item={getEquippedItem(ItemSlotTypeDto.TwoHand)}
                            itemIndex={2}
                            isEquippedItem={true}
                          />
                        )}
                      </Grid>
                    </DropTarget>
                  </DropTarget>
                </Grid>
                <Grid container className={styles.offHandSlotContainer}>
                  <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.OffHand} dropData={{}}>
                    <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.TwoHand} dropData={{}}>
                      <Grid container className={styles.slotImageContainer}>
                        <Grid container className={styles.equippedSlot}>
                          <Image src="/img/unity-assets/inventory/equipped_item_slot.png" height={70} width={70} />
                        </Grid>
                        <Grid container className={styles.slotIcon}>
                          <Image src="/img/unity-assets/inventory/off_hand_slot_icon.png" height={48} width={48} />
                        </Grid>
                        {isOffHandEquipped && (
                          <ItemDND
                            item={getEquippedItem(ItemSlotTypeDto.OffHand)}
                            itemIndex={3}
                            isEquippedItem={true}
                            detailsLeft={width <= xsScreenWidth}
                          />
                        )}
                        {isTwoHandEquipped && <Grid container className={styles.disabledContainer} />}
                      </Grid>
                    </DropTarget>
                  </DropTarget>
                </Grid>
                <Grid container className={styles.ringSlotContainer}>
                  <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.Ring} dropData={{}}>
                    <Grid container className={styles.slotImageContainer}>
                      <Grid container className={styles.equippedSlot}>
                        <Image src="/img/unity-assets/inventory/equipped_item_slot.png" height={70} width={70} />
                      </Grid>
                      <Grid container className={styles.slotIcon}>
                        <Image src="/img/unity-assets/inventory/ring_slot_icon.png" height={48} width={48} />
                      </Grid>
                      {isRingEquipped && (
                        <ItemDND item={getEquippedItem(ItemSlotTypeDto.Ring)} itemIndex={4} isEquippedItem={true} />
                      )}
                    </Grid>
                  </DropTarget>
                </Grid>
                <Grid container className={styles.feetSlotContainer}>
                  <DropTarget onHit={dropped} targetKey={ItemSlotTypeDto.Feet} dropData={{}}>
                    <Grid container className={styles.slotImageContainer}>
                      <Grid container className={styles.equippedSlot}>
                        <Image src="/img/unity-assets/inventory/equipped_item_slot.png" height={70} width={70} />
                      </Grid>
                      <Grid container className={styles.slotIcon}>
                        <Image src="/img/unity-assets/inventory/feet_slot_icon.png" height={48} width={48} />
                      </Grid>
                      {isFeetEquipped && (
                        <ItemDND item={getEquippedItem(ItemSlotTypeDto.Feet)} itemIndex={5} isEquippedItem={true} />
                      )}
                    </Grid>
                  </DropTarget>
                </Grid>
              </Grid>
            )}
          </Grid>
        </Container>
      )}
    </Modal>
  );
};

export default Inventory;
