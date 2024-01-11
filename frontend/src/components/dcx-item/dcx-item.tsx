import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./dcx-item.module.scss";
import { Item, ItemStatType } from "@/state-mgmt/item/itemTypes";
import { useEffect, useRef, useState } from "react";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import {
  selectItemDetailsOpen,
  selectItemMenuOpen,
  setItemDetailsOpen,
  setItemMenuOpen,
  setItemToKeep,
} from "@/state-mgmt/item/itemSlice";
import { Status } from "@/state-mgmt/app/appTypes";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { mdScreenWidth, xsScreenWidth } from "@/helpers/global-constants";
import {
  equipItemRequest,
  resetInventoryItemMovedStatus,
  selectDraggedItem,
  selectEquippedItems,
  selectInventory,
  selectInventoryItemMovedStatus,
  selectSelectedHero,
  unequipItemRequest,
} from "@/state-mgmt/hero/heroSlice";
import { ItemRarityDto, ItemSlotTypeDto } from "@dcx/dcx-backend";
import { setDroppedItemSoundSlug } from "@/state-mgmt/app/appSlice";
import { pickUpLootRequest } from "@/state-mgmt/combat/combatSlice";
import { compareItemStatByType } from "@/helpers/helper-functions";

interface Props {
  rarity?: ItemRarityDto;
  item?: Item;
  isWalletItem?: boolean;
  isMerchantItem?: boolean;
  isSellableItem?: boolean;
  isDeathItem?: boolean;
  isLootItem?: boolean;
  isEquippedItem?: boolean;
  detailsLeft?: boolean;
  itemIndex?: number;
  showOpaque?: boolean;
  disableDetails?: boolean;
  height: number;
  width: number;
  marginLeft?: number;
  marginRight?: number;
  marginTop?: number;
  marginBottom?: number;
  detailsMarginLeft?: number;
  top?: number;
  left?: number;
  containerWidth?: number;
}

const DCXItem: React.FC<Props> = (props: Props) => {
  const {
    rarity,
    item,
    isWalletItem,
    isMerchantItem,
    isSellableItem,
    isDeathItem,
    isLootItem,
    isEquippedItem,
    detailsLeft,
    itemIndex,
    showOpaque,
    disableDetails,
    height,
    width,
    marginLeft,
    marginRight,
    marginTop,
    marginBottom,
    detailsMarginLeft,
    top,
    left,
    containerWidth,
  } = props;

  const dispatch = useAppDispatch();
  const itemMenuOpen = useAppSelector(selectItemMenuOpen);
  const itemDetailsOpen = useAppSelector(selectItemDetailsOpen);
  const inventoryItemMovedStatus = useAppSelector(selectInventoryItemMovedStatus);
  const draggedItem = useAppSelector(selectDraggedItem);
  const { hero } = useAppSelector(selectSelectedHero);
  const inventory = useAppSelector(selectInventory);
  const equippedItems = useAppSelector(selectEquippedItems);
  const { width: windowWidth, height: windowHeight } = useWindowDimensions();
  const ref = useRef<any>(null);

  const [detailsHeight, setDetailsHeight] = useState(0);
  const [displayDetails, setDisplayDetails] = useState(false);
  const [x, setX] = useState(0);
  const [y, setY] = useState(0);
  const [strengthDiff, setStrengthDiff] = useState(0);
  const [agilityDiff, setAgilityDiff] = useState(0);
  const [wisdomDiff, setWisdomDiff] = useState(0);
  const [armorMitDiff, setArmorMitDiff] = useState(0);
  const [armorMitAmountDiff, setArmorMitAmountDiff] = useState(0);
  const [bonusDamageDiff, setBonusDamageDiff] = useState(0);
  const [chanceToHitDiff, setChanceToHitDiff] = useState(0);
  const [charismaDiff, setCharismaDiff] = useState(0);
  const [criticalHitRateDiff, setCriticalHitRateDiff] = useState(0);
  const [dthDiff, setDthDiff] = useState(0);
  const [dodgeRateDiff, setDodgeRateDiff] = useState(0);
  const [hitPointsDiff, setHitPointsDiff] = useState(0);
  const [parryRateDiff, setParryRateDiff] = useState(0);
  const [quicknessDiff, setQuicknessDiff] = useState(0);

  useEffect(() => {
    setStrengthDiff(getStatComparison(ItemStatType.Strength));
    setAgilityDiff(getStatComparison(ItemStatType.Agility));
    setWisdomDiff(getStatComparison(ItemStatType.Wisdom));
    setArmorMitDiff(getStatComparison(ItemStatType.ArmorMitigation));
    setArmorMitAmountDiff(getStatComparison(ItemStatType.ArmorMitigationAmount));
    setBonusDamageDiff(getStatComparison(ItemStatType.BonusDamage));
    setChanceToHitDiff(getStatComparison(ItemStatType.ChanceToHit));
    setCharismaDiff(getStatComparison(ItemStatType.Charisma));
    setCriticalHitRateDiff(getStatComparison(ItemStatType.CriticalHitRate));
    setDthDiff(getStatComparison(ItemStatType.DifficultyToHit));
    setDodgeRateDiff(getStatComparison(ItemStatType.DodgeRate));
    setHitPointsDiff(getStatComparison(ItemStatType.HitPoints));
    setParryRateDiff(getStatComparison(ItemStatType.ParryRate));
    setQuicknessDiff(getStatComparison(ItemStatType.Quickness));
    calculateDetailsHeight();
  }, []);

  useEffect(() => {
    if (inventoryItemMovedStatus.status === Status.Loaded) {
      setStrengthDiff(getStatComparison(ItemStatType.Strength));
      setAgilityDiff(getStatComparison(ItemStatType.Agility));
      setWisdomDiff(getStatComparison(ItemStatType.Wisdom));
      setArmorMitDiff(getStatComparison(ItemStatType.ArmorMitigation));
      setArmorMitAmountDiff(getStatComparison(ItemStatType.ArmorMitigationAmount));
      setBonusDamageDiff(getStatComparison(ItemStatType.BonusDamage));
      setChanceToHitDiff(getStatComparison(ItemStatType.ChanceToHit));
      setCharismaDiff(getStatComparison(ItemStatType.Charisma));
      setCriticalHitRateDiff(getStatComparison(ItemStatType.CriticalHitRate));
      setDthDiff(getStatComparison(ItemStatType.DifficultyToHit));
      setDodgeRateDiff(getStatComparison(ItemStatType.DodgeRate));
      setHitPointsDiff(getStatComparison(ItemStatType.HitPoints));
      setParryRateDiff(getStatComparison(ItemStatType.ParryRate));
      setQuicknessDiff(getStatComparison(ItemStatType.Quickness));
      calculateDetailsHeight();
      dispatch(resetInventoryItemMovedStatus());
    }
  }, [inventoryItemMovedStatus]);

  useEffect(() => {
    if (itemDetailsOpen.item.id === item?.id) {
      if (itemDetailsOpen.open) {
        calculateDetailsHeight();
        setDisplayDetails(true);
      } else {
        setDisplayDetails(false);
      }
    }
  }, [itemDetailsOpen]);

  // If a player clicks outside of the Item Details then close the Item Details
  useEffect(() => {
    document.addEventListener("mousedown", handleClickOutside);
    return () => {
      // Unbind the event listener on clean up
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [ref, inventoryItemMovedStatus]);

  const handleClickOutside = (e: any) => {
    if (ref.current && !ref.current.contains(e.target)) {
      if (item) {
        dispatch(setItemDetailsOpen({ item: item, open: false }));
      }
    }
  };

  const calculateDetailsHeight = () => {
    let count = 0;
    if (item) {
      item.allowedHeroClassList.length > 0 ? count++ : null;
      item.slot ? count++ : null;
      item.rarity ? count++ : null;
      item.bonusDamage ? count++ : null;
      item.dieDamage.length > 0 ? count++ : null;
      item.levelRequirement >= 0 ? count++ : null;
      for (const val of Object.values(item.affectedAttributes)) {
        val > 0 ? count++ : null;
      }
      Object.keys(item.heroStatComplianceDictionary).length > 0
        ? (count += Object.keys(item.heroStatComplianceDictionary).length)
        : null;
    }
    let height = 33;
    const additionalHeight = count * 16.6;
    height += additionalHeight;
    setDetailsHeight(height);
  };

  const onHover = () => {
    itemMenuOpen.open ? setDisplayDetails(false) : setDisplayDetails(true);
  };

  const onLeave = () => {
    setDisplayDetails(false);
  };

  const onMove = (e: any) => {
    const bounds = e.target.getBoundingClientRect();
    const offSetX = bounds.left + 65;
    let offSetY = bounds.top;
    setX(offSetX);
    setY(offSetY);
  };

  const handleItemMenuOpen = (e: any) => {
    if (!isMerchantItem || (isMerchantItem && windowWidth <= mdScreenWidth)) {
      if (!isWalletItem || (isWalletItem && windowWidth <= mdScreenWidth)) {
        const bounds = e.target.getBoundingClientRect();
        let xPosition = bounds.left + 20;
        let yPosition = bounds.top + 10;
        setDisplayDetails(false);
        if (item) {
          dispatch(
            setItemMenuOpen({
              item: item,
              open: true,
              x: xPosition,
              y: yPosition,
            })
          );
        }
      }
    }
  };

  const handleDoubleClick = () => {
    if (item) {
      if (isLootItem) {
        const itemIds: Array<string> = [];
        itemIds.push(item.id);
        dispatch(pickUpLootRequest(itemIds));
      } else if (isDeathItem) {
        dispatch(setItemToKeep(item as Item));
      } else {
        // Get the index of the dragged item
        const index = inventory.findIndex((i) => i.id === item?.id);
        if (hero.equippedItems.some((i) => i.id === item.id)) {
          dispatch(
            unequipItemRequest({
              itemId: item.id,
            })
          );
        } else {
          dispatch(
            equipItemRequest({
              itemId: item.id,
              inventoryIndex: index,
            })
          );
        }
      }
      dispatch(setDroppedItemSoundSlug(item.itemDropSound));
    }
  };

  const getItemDetailsStyles = () => {
    if (detailsLeft) {
      if (windowWidth > mdScreenWidth) {
        return styles.itemDetailsContainerLeftHover;
      } else {
        if (windowWidth <= xsScreenWidth) {
          return styles.itemDetailsContainerLeftDetailsShiftRight;
        } else {
          return styles.itemDetailsContainerLeftDetails;
        }
      }
    } else {
      if (windowWidth <= xsScreenWidth) {
        return styles.itemDetailsContainerShiftLeft;
      } else {
        return styles.itemDetailsContainer;
      }
    }
  };

  const getStatComparison = (statType: ItemStatType): number => {
    if (item) {
      if (item.slot === ItemSlotTypeDto.Chest) {
        const equippedItem = equippedItems.find((i) => i.slot === ItemSlotTypeDto.Chest);
        if (equippedItem) {
          return compareItemStatByType(statType, item, equippedItem);
        }
      }
      if (item.slot === ItemSlotTypeDto.Feet) {
        const equippedItem = equippedItems.find((i) => i.slot === ItemSlotTypeDto.Feet);
        if (equippedItem) {
          return compareItemStatByType(statType, item, equippedItem);
        }
      }
      if (item.slot === ItemSlotTypeDto.Head) {
        const equippedItem = equippedItems.find((i) => i.slot === ItemSlotTypeDto.Head);
        if (equippedItem) {
          return compareItemStatByType(statType, item, equippedItem);
        }
      }
      if (item.slot === ItemSlotTypeDto.Ring) {
        const equippedItem = equippedItems.find((i) => i.slot === ItemSlotTypeDto.Ring);
        if (equippedItem) {
          return compareItemStatByType(statType, item, equippedItem);
        }
      }
      if (item.slot === ItemSlotTypeDto.MainHand) {
        const equippedMainHandItem = equippedItems.find((i) => i.slot === ItemSlotTypeDto.MainHand);
        const equippedTwoHandItem = equippedItems.find((i) => i.slot === ItemSlotTypeDto.TwoHand);
        if (equippedMainHandItem) {
          return compareItemStatByType(statType, item, equippedMainHandItem);
        } else if (equippedTwoHandItem) {
          return compareItemStatByType(statType, item, equippedTwoHandItem);
        }
      }
      if (item.slot === ItemSlotTypeDto.OffHand) {
        const equippedOffHandItem = equippedItems.find((i) => i.slot === ItemSlotTypeDto.OffHand);
        const equippedTwoHandItem = equippedItems.find((i) => i.slot === ItemSlotTypeDto.TwoHand);
        if (equippedOffHandItem) {
          return compareItemStatByType(statType, item, equippedOffHandItem);
        } else if (equippedTwoHandItem) {
          return compareItemStatByType(statType, item, equippedTwoHandItem);
        }
      }
      if (item.slot === ItemSlotTypeDto.TwoHand) {
        const equippedTwoHandItem = equippedItems.find((i) => i.slot === ItemSlotTypeDto.TwoHand);
        const equippedMainHandItem = equippedItems.find((i) => i.slot === ItemSlotTypeDto.MainHand);
        const equippedOffHandItem = equippedItems.find((i) => i.slot === ItemSlotTypeDto.OffHand);
        if (equippedTwoHandItem) {
          return compareItemStatByType(statType, item, equippedTwoHandItem);
        } else if (equippedMainHandItem && equippedOffHandItem) {
          return compareItemStatByType(statType, item, equippedMainHandItem, equippedOffHandItem);
        } else if (equippedMainHandItem) {
          return compareItemStatByType(statType, item, equippedMainHandItem);
        } else if (equippedOffHandItem) {
          return compareItemStatByType(statType, item, equippedOffHandItem);
        }
      }
    }
    return 0;
  };

  const renderDieDamage = () => {
    if (item) {
      const dice = [];
      if (item.dieDamage.filter((d) => d.sides === 4).length > 0) {
        dice.push(item?.dieDamage.filter((d) => d.sides === 4).length + "D4");
      }
      if (item.dieDamage.filter((d) => d.sides === 6).length > 0) {
        dice.push(item?.dieDamage.filter((d) => d.sides === 6).length + "D6");
      }
      if (item.dieDamage.filter((d) => d.sides === 8).length > 0) {
        dice.push(item?.dieDamage.filter((d) => d.sides === 8).length + "D8");
      }
      if (item.dieDamage.filter((d) => d.sides === 10).length > 0) {
        dice.push(item?.dieDamage.filter((d) => d.sides === 10).length + "D10");
      }
      if (item.dieDamage.filter((d) => d.sides === 12).length > 0) {
        dice.push(item?.dieDamage.filter((d) => d.sides === 12).length + "D12");
      }
      if (item.dieDamage.filter((d) => d.sides === 20).length > 0) {
        dice.push(item?.dieDamage.filter((d) => d.sides === 20).length + "D20");
      }
      return dice.join(", ");
    } else {
      return "";
    }
  };

  const renderAffectedAttributes = () => {
    const attributes: JSX.Element[] = [];
    if (item?.affectedAttributes.strength) {
      attributes.push(
        <Grid container direction="row" key={"strength" + item.affectedAttributes.strength} className={styles.statRow}>
          <Typography component="span" className={styles.itemStat}>
            {`STRENGTH`}
          </Typography>
          <Typography component="span" className={styles.itemStat}>
            {strengthDiff !== 0 && (
              <span className={strengthDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}>{`(${
                strengthDiff > 0 ? "+" + strengthDiff : strengthDiff
              })`}</span>
            )}
            {`${item.affectedAttributes.strength}`}
          </Typography>
        </Grid>
      );
    }
    if (item?.affectedAttributes.agility) {
      attributes.push(
        <Grid container direction="row" key={"agility" + item.affectedAttributes.agility} className={styles.statRow}>
          <Typography component="span" className={styles.itemStat}>
            {`AGILITY`}
          </Typography>
          <Typography component="span" className={styles.itemStat}>
            {agilityDiff !== 0 && (
              <span className={agilityDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}>{`(${
                agilityDiff > 0 ? "+" + agilityDiff : agilityDiff
              })`}</span>
            )}
            {`${item.affectedAttributes.agility}`}
          </Typography>
        </Grid>
      );
    }
    if (item?.affectedAttributes.wisdom) {
      attributes.push(
        <Grid container direction="row" key={"wisdom" + item.affectedAttributes.wisdom} className={styles.statRow}>
          <Typography component="span" className={styles.itemStat}>
            {`WISDOM`}
          </Typography>
          <Typography component="span" className={styles.itemStat}>
            {wisdomDiff !== 0 && (
              <span className={wisdomDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}>{`(${
                wisdomDiff > 0 ? "+" + wisdomDiff : wisdomDiff
              })`}</span>
            )}
            {`${item.affectedAttributes.wisdom}`}
          </Typography>
        </Grid>
      );
    }
    if (item?.affectedAttributes.quickness) {
      attributes.push(
        <Grid
          container
          direction="row"
          key={"quickness" + item.affectedAttributes.quickness}
          className={styles.statRow}
        >
          <Typography component="span" className={styles.itemStat}>
            {`QUICKNESS`}
          </Typography>
          <Typography component="span" className={styles.itemStat}>
            {quicknessDiff !== 0 && (
              <span className={quicknessDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}>{`(${
                quicknessDiff > 0 ? "+" + quicknessDiff : quicknessDiff
              })`}</span>
            )}
            {`${item.affectedAttributes.quickness}`}
          </Typography>
        </Grid>
      );
    }
    if (item?.affectedAttributes.charisma) {
      attributes.push(
        <Grid container direction="row" key={"charisma" + item.affectedAttributes.charisma} className={styles.statRow}>
          <Typography component="span" className={styles.itemStat}>
            {`CHARISMA`}
          </Typography>
          <Typography component="span" className={styles.itemStat}>
            {charismaDiff !== 0 && (
              <span className={charismaDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}>{`(${
                charismaDiff > 0 ? "+" + charismaDiff : charismaDiff
              })`}</span>
            )}
            {`${item.affectedAttributes.charisma}`}
          </Typography>
        </Grid>
      );
    }
    if (item?.affectedAttributes.hitPoints) {
      attributes.push(
        <Grid
          container
          direction="row"
          key={"hitpoints" + item.affectedAttributes.hitPoints}
          className={styles.statRow}
        >
          <Typography component="span" className={styles.itemStat}>
            {`HITPOINTS`}
          </Typography>
          <Typography component="span" className={styles.itemStat}>
            {hitPointsDiff !== 0 && (
              <span className={hitPointsDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}>{`(${
                hitPointsDiff > 0 ? "+" + hitPointsDiff : hitPointsDiff
              })`}</span>
            )}
            {`${item.affectedAttributes.hitPoints}`}
          </Typography>
        </Grid>
      );
    }
    if (item?.affectedAttributes.difficultyToHit) {
      attributes.push(
        <Grid
          container
          direction="row"
          key={"dth" + item.affectedAttributes.difficultyToHit}
          className={styles.statRow}
        >
          <Typography component="span" className={styles.itemStat}>
            {`DIFFICULTY TO HIT`}
          </Typography>
          <Typography component="span" className={styles.itemStat}>
            {dthDiff !== 0 && (
              <span className={dthDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}>{`(${
                dthDiff > 0 ? "+" + dthDiff : dthDiff
              }%)`}</span>
            )}
            {`${item.affectedAttributes.difficultyToHit / 10}%`}
          </Typography>
        </Grid>
      );
    }
    if (item?.affectedAttributes.chanceToHit) {
      attributes.push(
        <Grid container direction="row" key={"cth" + item.affectedAttributes.chanceToHit} className={styles.statRow}>
          <Typography component="span" className={styles.itemStat}>
            {`CHANCE TO HIT`}
          </Typography>
          <Typography component="span" className={styles.itemStat}>
            {chanceToHitDiff !== 0 && (
              <span className={chanceToHitDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}>{`(${
                chanceToHitDiff > 0 ? "+" + chanceToHitDiff : chanceToHitDiff
              }%)`}</span>
            )}
            {`${item.affectedAttributes.chanceToHit / 10}%`}
          </Typography>
        </Grid>
      );
    }
    return attributes;
  };

  const renderStatRequirements = () => {
    const requiredStats: JSX.Element[] = [];
    if (item?.heroStatComplianceDictionary.strength) {
      requiredStats.push(
        <Grid
          container
          direction="row"
          key={item.heroStatComplianceDictionary.strength + "stat1"}
          className={styles.statRow}
        >
          <Typography component="span" className={styles.itemStat}>
            {`REQUIRED STRENGTH`}
          </Typography>
          <Typography
            component="span"
            className={
              hero.strength >= item.heroStatComplianceDictionary.strength ? styles.itemStat : styles.itemStatWarning
            }
          >
            {`${item.heroStatComplianceDictionary.strength}`}
          </Typography>
        </Grid>
      );
    }
    if (item?.heroStatComplianceDictionary.agility) {
      requiredStats.push(
        <Grid
          container
          direction="row"
          key={item.heroStatComplianceDictionary.agility + "stat2"}
          className={styles.statRow}
        >
          <Typography component="span" className={styles.itemStat}>
            {`REQUIRED AGILITY`}
          </Typography>
          <Typography
            component="span"
            className={
              hero.agility >= item.heroStatComplianceDictionary.agility ? styles.itemStat : styles.itemStatWarning
            }
          >
            {`${item.heroStatComplianceDictionary.agility}`}
          </Typography>
        </Grid>
      );
    }
    if (item?.heroStatComplianceDictionary.wisdom) {
      requiredStats.push(
        <Grid
          container
          direction="row"
          key={item.heroStatComplianceDictionary.wisdom + "stat3"}
          className={styles.statRow}
        >
          <Typography component="span" className={styles.itemStat}>
            {`REQUIRED WISDOM`}
          </Typography>
          <Typography
            component="span"
            className={
              hero.wisdom >= item.heroStatComplianceDictionary.wisdom ? styles.itemStat : styles.itemStatWarning
            }
          >
            {`${item.heroStatComplianceDictionary.wisdom}`}
          </Typography>
        </Grid>
      );
    }
    if (item?.heroStatComplianceDictionary.charisma) {
      requiredStats.push(
        <Grid
          container
          direction="row"
          key={item.heroStatComplianceDictionary.charisma + "stat4"}
          className={styles.statRow}
        >
          <Typography component="span" className={styles.itemStat}>
            {`REQUIRED CHARISMA`}
          </Typography>
          <Typography
            component="span"
            className={
              hero.charisma >= item.heroStatComplianceDictionary.charisma ? styles.itemStat : styles.itemStatWarning
            }
          >
            {`${item.heroStatComplianceDictionary.charisma}`}
          </Typography>
        </Grid>
      );
    }
    if (item?.heroStatComplianceDictionary.quickness) {
      requiredStats.push(
        <Grid
          container
          direction="row"
          key={item.heroStatComplianceDictionary.quickness + "stat5"}
          className={styles.statRow}
        >
          <Typography component="span" className={styles.itemStat}>
            {`REQUIRED QUICKNESS`}
          </Typography>
          <Typography
            component="span"
            className={
              hero.quickness >= item.heroStatComplianceDictionary.quickness ? styles.itemStat : styles.itemStatWarning
            }
          >
            {`${item.heroStatComplianceDictionary.quickness}`}
          </Typography>
        </Grid>
      );
    }
    return requiredStats;
  };

  return (
    <Grid container style={{ width: containerWidth }}>
      {draggedItem.id === item?.id && inventoryItemMovedStatus.status === Status.Loading ? null : (
        <Grid
          container
          className={styles.imageContainer}
          onMouseEnter={() => windowWidth > mdScreenWidth && onHover()}
          onMouseLeave={() => windowWidth > mdScreenWidth && onLeave()}
          onMouseMove={(e) => onMove(e)}
          onContextMenu={(e) => handleItemMenuOpen(e)}
          onClick={(e) => (windowWidth <= mdScreenWidth || isDeathItem) && handleItemMenuOpen(e)}
          onDoubleClick={() => handleDoubleClick()}
          style={{
            height: height,
            width: width,
            marginLeft: marginLeft,
            marginRight: marginRight,
            marginTop: marginTop,
            marginBottom: marginBottom,
            top: top,
            left: left,
          }}
        >
          {rarity && (
            <Grid container className={showOpaque ? styles.imageOpaque : styles.image}>
              {draggedItem.id === item?.id && inventoryItemMovedStatus.status === Status.Loading ? null : (
                <Image
                  src={`/img/items/item_${rarity.toLowerCase()}_bg.png`}
                  height={height}
                  width={width}
                  quality={100}
                  className={styles.hoverPointer}
                />
              )}
            </Grid>
          )}
          <Grid container className={showOpaque ? styles.imageOpaque : styles.image}>
            {draggedItem.id === item?.id && inventoryItemMovedStatus.status === Status.Loading ? null : (
              <Image
                src={`/img/api/items/${item?.imageSlug!.toLowerCase()}.png`}
                height={height}
                width={width}
                quality={100}
                className={styles.hoverPointer}
              />
            )}
          </Grid>
          {/* {isSellableItem && item?.sellPrice && (
            <Grid container className={styles.sellPriceContainer}>
              <Typography component="span" className={styles.sellPrice}>
                {item.sellPrice}
              </Typography>
            </Grid>
          )} */}
          {displayDetails && !disableDetails && detailsHeight > 0 && item && (
            <Grid
              container
              className={getItemDetailsStyles()}
              style={{
                height: detailsHeight,
                left: x,
                top: y,
                marginLeft: detailsMarginLeft,
                marginTop: item.slot === ItemSlotTypeDto.Feet && isEquippedItem ? -70 : undefined,
              }}
              ref={ref}
            >
              <Grid container className={styles.itemDetails} style={{ height: detailsHeight }}>
                <Image
                  src={
                    detailsHeight < 210
                      ? `/img/unity-assets/shared/tooltip_bg.png`
                      : `/img/unity-assets/shared/tooltip_bg_vertical.png`
                  }
                  height={detailsHeight}
                  width={210}
                  quality={100}
                />
                <Typography component="span" className={styles.itemName}>
                  {item.name}
                </Typography>
                <Grid container direction="column" className={styles.itemStats}>
                  {item.allowedHeroClassList.length > 0 && (
                    <Grid container direction="row" className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`CLASS`}
                      </Typography>
                      <Typography
                        component="span"
                        className={
                          item.allowedHeroClassList.indexOf(hero.heroClass) > -1
                            ? styles.itemStat
                            : styles.itemStatWarning
                        }
                      >
                        {`${item.allowedHeroClassList.join(", ")}`}
                      </Typography>
                    </Grid>
                  )}
                  {(item.slot === ItemSlotTypeDto.Chest ||
                    item.slot === ItemSlotTypeDto.Feet ||
                    item.slot === ItemSlotTypeDto.Head ||
                    item.slot === ItemSlotTypeDto.MainHand ||
                    item.slot === ItemSlotTypeDto.OffHand ||
                    item.slot === ItemSlotTypeDto.Ring ||
                    item.slot === ItemSlotTypeDto.TwoHand) && (
                    <Grid container direction="row" className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`SLOT`}
                      </Typography>
                      <Typography component="span" className={styles.itemStat}>
                        {`${item.slot}`}
                      </Typography>
                    </Grid>
                  )}
                  {item.rarity && (
                    <Grid container direction="row" className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`RARITY`}
                      </Typography>
                      <Typography component="span" className={styles.itemStat}>
                        {`${item.rarity}`}
                      </Typography>
                    </Grid>
                  )}
                  {/* We have to convert numerical attributes to Number because doing a boolean check was causing attributes with a value of 0 to be displayed even though the check would be false. ie item.damage ? returns false but javascript will still display the number 0 in html */}
                  {Number(item.dieDamage.length) > 0 && (
                    <Grid container direction="row" className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`DAMAGE`}
                      </Typography>
                      <Typography component="span" className={styles.itemStat}>
                        {renderDieDamage()}
                      </Typography>
                    </Grid>
                  )}
                  {Number(item.bonusDamage) > 0 && (
                    <Grid container direction="row" className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`BONUS DAMAGE`}
                      </Typography>
                      <Typography component="span" className={styles.itemStat}>
                        {bonusDamageDiff !== 0 && (
                          <span
                            className={bonusDamageDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}
                          >{`(${bonusDamageDiff > 0 ? "+" + bonusDamageDiff : bonusDamageDiff})`}</span>
                        )}
                        {`${item.bonusDamage}`}
                      </Typography>
                    </Grid>
                  )}
                  {Number(item.affectedAttributes.criticalHitRate) > 0 && (
                    <Grid container direction="row" className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`CRIT RATE`}
                      </Typography>
                      <Typography component="span" className={styles.itemStat}>
                        {criticalHitRateDiff !== 0 && (
                          <span
                            className={criticalHitRateDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}
                          >{`(${criticalHitRateDiff > 0 ? "+" + criticalHitRateDiff : criticalHitRateDiff}%)`}</span>
                        )}
                        {`${Number(item.affectedAttributes.criticalHitRate) / 100}%`}
                      </Typography>
                    </Grid>
                  )}
                  {Number(item.affectedAttributes.armorMitigation) > 0 && (
                    <Grid container direction="row" className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`ARMOR MIT PROC %`}
                      </Typography>
                      <Typography component="span" className={styles.itemStat}>
                        {armorMitDiff !== 0 && (
                          <span className={armorMitDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}>{`(${
                            armorMitDiff > 0 ? "+" + armorMitDiff : armorMitDiff
                          }%)`}</span>
                        )}
                        {`${Number(item.affectedAttributes.armorMitigation) / 100}%`}
                      </Typography>
                    </Grid>
                  )}
                  {Number(item.affectedAttributes.armorMitigationAmount) > 0 && (
                    <Grid container direction="row" className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`ARMOR MIT AMOUNT`}
                      </Typography>
                      <Typography component="span" className={styles.itemStat}>
                        {armorMitAmountDiff !== 0 && (
                          <span
                            className={armorMitAmountDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}
                          >{`(${armorMitAmountDiff > 0 ? "+" + armorMitAmountDiff : armorMitAmountDiff})`}</span>
                        )}
                        {`${Number(item.affectedAttributes.armorMitigationAmount)}`}
                      </Typography>
                    </Grid>
                  )}
                  {Number(item.affectedAttributes.parryRate) > 0 && (
                    <Grid container direction="row" className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`PARRY RATE`}
                      </Typography>
                      <Typography component="span" className={styles.itemStat}>
                        {parryRateDiff !== 0 && (
                          <span className={parryRateDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}>{`(${
                            parryRateDiff > 0 ? "+" + parryRateDiff : parryRateDiff
                          }%)`}</span>
                        )}
                        {`${Number(item.affectedAttributes.parryRate) / 100}%`}
                      </Typography>
                    </Grid>
                  )}
                  {Number(item.affectedAttributes.dodgeRate) > 0 && (
                    <Grid container direction="row" className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`DODGE RATE`}
                      </Typography>
                      <Typography component="span" className={styles.itemStat}>
                        {dodgeRateDiff !== 0 && (
                          <span className={dodgeRateDiff > 0 ? styles.itemStatPositive : styles.itemStatNegative}>{`(${
                            dodgeRateDiff > 0 ? "+" + dodgeRateDiff : dodgeRateDiff
                          }%)`}</span>
                        )}
                        {`${Number(item.affectedAttributes.dodgeRate) / 100}%`}
                      </Typography>
                    </Grid>
                  )}
                  {Object.keys(item.affectedAttributes).length > 0 && renderAffectedAttributes()}
                  {Number(item.levelRequirement) >= 0 && (
                    <Grid container direction="row" className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`REQUIRED LEVEL`}
                      </Typography>
                      <Typography
                        component="span"
                        className={hero.level >= item.levelRequirement ? styles.itemStat : styles.itemStatWarning}
                      >
                        {`${Number(item.levelRequirement)}`}
                      </Typography>
                    </Grid>
                  )}
                  {Object.keys(item.heroStatComplianceDictionary).length > 0 && renderStatRequirements()}
                </Grid>
              </Grid>
            </Grid>
          )}
        </Grid>
      )}
    </Grid>
  );
};

export default DCXItem;
