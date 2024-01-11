import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import styles from "./blacksmith-items.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import useWindowDimensions from "@/helpers/window-dimensions";
import { Hero } from "@/state-mgmt/hero/heroTypes";
import Image from "next/image";
import { useEffect, useState } from "react";
import { getBlacksmithItems, selectBlacksmithItems } from "@/state-mgmt/vendor/vendorSlice";
import ItemDND from "../inventory/item-dnd";
import Scrollbars from "react-custom-scrollbars";
import { mdScreenWidth, xsScreenWidth } from "@/helpers/global-constants";
import DCXButton from "../dcx-button/dcx-button";
import { ItemDto } from "@dcx/dcx-backend";
import { buyBlacksmithItem, resetBuyItemStatus, selectBuyItemStatus } from "@/state-mgmt/hero/heroSlice";
import { Status } from "@/state-mgmt/app/appTypes";

interface Props {
  hero: Hero;
}

const BlacksmithItems: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const blacksmithItems = useAppSelector(selectBlacksmithItems);
  const buyItemStatus = useAppSelector(selectBuyItemStatus);
  const { width, height } = useWindowDimensions();
  const { hero } = props;

  const [selectedItem, setSelectedItem] = useState<ItemDto>();
  const [showConfirmation, setShowConfirmation] = useState(false);

  useEffect(() => {
    dispatch(getBlacksmithItems());
  }, []);

  useEffect(() => {
    if (buyItemStatus.status === Status.Loaded) {
      setShowConfirmation(false);
      dispatch(resetBuyItemStatus());
    }
    if (buyItemStatus.status === Status.Failed) {
      setShowConfirmation(false);
      dispatch(resetBuyItemStatus());
    }
  }, [buyItemStatus]);

  const handleBuyClick = (item: ItemDto) => {
    setSelectedItem(item);
    setShowConfirmation(true);
  };

  const handleConfirmClick = () => {
    if (selectedItem) {
      dispatch(buyBlacksmithItem(selectedItem.slug));
    }
  };

  const handleConfirmClose = () => {
    setShowConfirmation(false);
  };

  const renderBlacksmithItems = () => {
    const items: JSX.Element[] = [];
    let detailsLeft = false;
    let i = 0;
    blacksmithItems.forEach((item: ItemDto) => {
      // Determine if details should show on left or right of item
      if (width > mdScreenWidth) {
        const itemPosition = i % 4;
        if (itemPosition === 2 || itemPosition === 3) {
          detailsLeft = true;
        } else {
          detailsLeft = false;
        }
      } else if (width > xsScreenWidth && width <= mdScreenWidth) {
        const itemPosition = i % 2;
        if (itemPosition === 1) {
          detailsLeft = true;
        } else {
          detailsLeft = false;
        }
      }

      items.push(
        <Grid item className={styles.itemContainer} key={item.id}>
          <ItemDND
            item={item}
            itemIndex={i}
            isMerchantItem={true}
            disableDND={true}
            disableContextMenu={width > mdScreenWidth ? true : false}
            detailsLeft={detailsLeft}
            detailsMarginLeft={width < xsScreenWidth ? -25 : undefined}
            backgroundHeight={175}
            backgroundWidth={175}
            itemHeight={149}
            itemWidth={151}
            itemMarginLeft={7}
            itemMarginTop={7}
          />
          <Grid container className={styles.priceContainer}>
            <Typography component="span" className={styles.price}>
              {`1 QUEST`}
            </Typography>
          </Grid>
          <Grid container className={styles.buyButton}>
            <DCXButton
              title="BUY"
              height={30}
              width={105}
              color="blue"
              arrowTopAdjustment={8}
              disabled={hero.remainingQuests < 1}
              onClick={() => handleBuyClick(item)}
            />
          </Grid>
        </Grid>
      );
      i++;
    });
    return items;
  };

  return (
    <Grid container className={styles.main}>
      <Grid container className={styles.container}>
        {blacksmithItems.length > 0 ? (
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
            renderThumbHorizontal={() => <Grid container />}
          >
            <Grid container className={styles.itemsContainer}>
              {renderBlacksmithItems()}
            </Grid>
          </Scrollbars>
        ) : (
          <Grid container className={styles.noItemsMessageContainer}>
            <Typography component="span" className={styles.noItemsMessage}>
              {`YOU'VE CLEANED ME OUT! TRY BACK TOMORROW.`}
            </Typography>
          </Grid>
        )}
      </Grid>
      <Modal open={showConfirmation} onClose={handleConfirmClose} className={styles.modalMain}>
        <Grid container className={styles.confirmModalContainer}>
          <Grid item>
            <Image src="/img/unity-assets/shared/tooltip_bg.png" height={125} width={262.5} quality={100} />
          </Grid>
          <Grid container className={styles.confirmContainer}>
            <Typography component="span" className={styles.headerText}>
              BUY {<span className={styles.itemName}>{selectedItem?.name}</span>} FOR{" "}
              {<span className={styles.goldText}>{"1"}</span>} QUEST?
            </Typography>
          </Grid>
          <Grid container className={styles.confirmButton}>
            <DCXButton
              title="CONFIRM"
              height={32}
              width={120}
              color="blue"
              disabled={buyItemStatus.status !== Status.NotStarted}
              onClick={() => handleConfirmClick()}
            />
          </Grid>
        </Grid>
      </Modal>
    </Grid>
  );
};

export default BlacksmithItems;
