import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import styles from "./dcx-nft-item.module.scss";
import { Item } from "@/state-mgmt/item/itemTypes";
import { useEffect, useRef, useState } from "react";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import {
  selectItemDetailsOpen,
  selectItemMenuOpen,
  setItemDetailsOpen,
  setItemMenuOpen,
} from "@/state-mgmt/item/itemSlice";
import { Status } from "@/state-mgmt/app/appTypes";
import Image from "next/image";
import useWindowDimensions from "@/helpers/window-dimensions";
import { mdScreenWidth, xsScreenWidth } from "@/helpers/global-constants";
import {
  resetInventoryItemMovedStatus,
  selectDraggedItem,
  selectInventoryItemMovedStatus,
  selectSelectedHero,
} from "@/state-mgmt/hero/heroSlice";
import { ItemDto, ItemRarityDto, ItemSlotTypeDto, SkillItem, UnlearnedHeroSkill } from "@dcx/dcx-backend";
import { pickUpLootRequest } from "@/state-mgmt/combat/combatSlice";
import { setDroppedItemSoundSlug } from "@/state-mgmt/app/appSlice";

interface Props {
  item: ItemDto;
  isWalletItem?: boolean;
  isLootItem?: boolean;
  detailsLeft?: boolean;
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

const DCXNftItem: React.FC<Props> = (props: Props) => {
  const {
    item,
    isWalletItem,
    isLootItem,
    detailsLeft,
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
  const { width: windowWidth, height: windowHeight } = useWindowDimensions();
  const ref = useRef<any>(null);

  const [nftUrl, setNftUrl] = useState(`/img/api/items/unidentified-skill.png`);
  const [detailsHeight, setDetailsHeight] = useState(35);
  const [displayDetails, setDisplayDetails] = useState(false);
  const [x, setX] = useState(0);
  const [y, setY] = useState(0);
  const [unlearnedHeroSkill, setUnlearnedHeroSkill] = useState<UnlearnedHeroSkill>();

  useEffect(() => {
    
    if (item.slot === ItemSlotTypeDto.UnidentifiedSkill) {
      setNftUrl(`/img/api/items/unidentified-skill.png`);
      setDetailsHeight(35);
    }else if (item.slot === ItemSlotTypeDto.UnlearnedSkill) {
      const skillItem = item as SkillItem;
      const unlearnedSkill = skillItem.skill as UnlearnedHeroSkill;
      setUnlearnedHeroSkill(unlearnedSkill);
      setNftUrl(`/img/api/items/${unlearnedSkill.skillClass.toLowerCase()}-skill.png`);
      setDetailsHeight(73);
    }else if (item.slot === ItemSlotTypeDto.Shard) {
      setNftUrl(`/img/api/items/shard.png`);
      setDetailsHeight(35);
    }else if (item.slot === ItemSlotTypeDto.NftAction) {
      setNftUrl(`/img/api/items/${(item.imageSlug||item.slug).toLowerCase()}.png`);
      setDetailsHeight(35);
    }
  }, []);

  useEffect(() => {
    if (inventoryItemMovedStatus.status === Status.Loaded) {
      if (item.slot === ItemSlotTypeDto.UnidentifiedSkill) {
        setNftUrl(`/img/api/items/unidentified-skill.png`);
        setDetailsHeight(35);
      }else if (item.slot === ItemSlotTypeDto.UnlearnedSkill) {
        setDetailsHeight(73);
      }else if (item.slot === ItemSlotTypeDto.Shard) {
        setNftUrl(`/img/api/items/shard.png`);
        setDetailsHeight(35);
      }else if (item.slot === ItemSlotTypeDto.NftAction) {
        setNftUrl(`/img/api/items/${(item.imageSlug||item.slug).toLowerCase()}.png`);
        setDetailsHeight(35);
      }
      dispatch(resetInventoryItemMovedStatus());
    }
  }, [inventoryItemMovedStatus]);

  useEffect(() => {
    if (itemDetailsOpen.item.id === item?.id) {
      if (itemDetailsOpen.open) {
        if (item.slot === ItemSlotTypeDto.UnlearnedSkill) {
          setDetailsHeight(73);
        } else {
          setDetailsHeight(35);
        }
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
  };

  const handleDoubleClick = () => {
    if (item) {
      if (isLootItem) {
        const itemIds: Array<string> = [];
        itemIds.push(item.id);
        dispatch(pickUpLootRequest(itemIds));
        dispatch(setDroppedItemSoundSlug(item.itemDropSound));
      }
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

  return (
    <Grid container style={{ width: containerWidth }}>
      {draggedItem.id === item?.id && inventoryItemMovedStatus.status === Status.Loading ? null : (
        <Grid
          container
          className={styles.imageContainer}
          onMouseEnter={() => windowWidth > mdScreenWidth && onHover()}
          onMouseLeave={() => windowWidth > mdScreenWidth && onLeave()}
          onMouseMove={(e) => onMove(e)}
          onContextMenu={(e) => windowWidth <= mdScreenWidth && handleItemMenuOpen(e)}
          onClick={(e) => windowWidth <= mdScreenWidth && handleItemMenuOpen(e)}
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
          <Grid container className={showOpaque ? styles.imageOpaque : styles.image}>
            {draggedItem.id === item?.id && inventoryItemMovedStatus.status === Status.Loading ? null : (
              <Image
                src={`/img/items/item_mythic_bg.png`}
                height={height}
                width={width}
                quality={100}
                className={styles.hoverPointer}
              />
            )}
          </Grid>
          <Grid container className={showOpaque ? styles.imageOpaque : styles.image}>
            {draggedItem.id === item?.id && inventoryItemMovedStatus.status === Status.Loading ? null : (
              <Image src={nftUrl} height={height} width={width} quality={100} className={styles.hoverPointer} />
            )}
          </Grid>
          {displayDetails && !disableDetails && detailsHeight > 0 && item && (
            <Grid
              container
              className={getItemDetailsStyles()}
              style={{
                height: detailsHeight,
                left: x,
                top: y,
                marginLeft: detailsMarginLeft,
              }}
              ref={ref}
            >
              <Grid container className={styles.itemDetails} style={{ height: detailsHeight }}>
                <Image
                  src={
                    detailsHeight < 70
                      ? `/img/unity-assets/shared/tooltip_bg_short.png`
                      : detailsHeight < 210
                      ? `/img/unity-assets/shared/tooltip_bg.png`
                      : `/img/unity-assets/shared/tooltip_bg_vertical.png`
                  }
                  height={detailsHeight}
                  width={210}
                  quality={100}
                />
                <Typography component="span" className={styles.itemName}>
                  {(()=>{
                    switch(item.slot){
                      case ItemSlotTypeDto.UnidentifiedSkill:
                        return "UNIDENTIFIED SKILL";
                      case ItemSlotTypeDto.UnlearnedSkill:
                        return "UNLEARNED SKILL";
                      case ItemSlotTypeDto.Shard:
                        return "SHARD";
                      case ItemSlotTypeDto.NftAction:
                        return item.name;
                      default:
                        return "UNKNOWN";
                    }
                  })()}
                  
                </Typography>
                {item.slot === ItemSlotTypeDto.UnlearnedSkill && unlearnedHeroSkill && (
                  <Grid container direction="column" className={styles.itemStats}>
                    <Grid container className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`CLASS`}
                      </Typography>
                      <Typography component="span" className={styles.itemStat}>
                        {unlearnedHeroSkill.skillClass}
                      </Typography>
                    </Grid>
                    <Grid container className={styles.statRow}>
                      <Typography component="span" className={styles.itemStat}>
                        {`NAME`}
                      </Typography>
                      <Typography component="span" className={styles.itemStat}>
                        {unlearnedHeroSkill.name}
                      </Typography>
                    </Grid>
                  </Grid>
                )}
              </Grid>
            </Grid>
          )}
        </Grid>
      )}
    </Grid>
  );
};

export default DCXNftItem;
