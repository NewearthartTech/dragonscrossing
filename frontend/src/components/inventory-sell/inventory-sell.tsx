import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import Modal from "@mui/material/Modal";
import styles from "./inventory-sell.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import useWindowDimensions from "@/helpers/window-dimensions";
import Image from "next/image";
import { useEffect, useState } from "react";
import Scrollbars from "react-custom-scrollbars";
import { mdScreenWidth, xsScreenWidth } from "@/helpers/global-constants";
import ItemDND from "../inventory/item-dnd";
import {
	selectItemsToSell,
	toggleItemToSell,
} from "@/state-mgmt/vendor/vendorSlice";
import DCXButton from "../dcx-button/dcx-button";
import PriorityHighIcon from "@mui/icons-material/CheckBoxSharp";
import {
	resetSellItemStatus,
	selectInventory,
	selectSellItemStatus,
	sellItemToBlacksmith,
} from "@/state-mgmt/hero/heroSlice";
import { Status } from "@/state-mgmt/app/appTypes";
import { ItemSlotTypeDto } from "@dcx/dcx-backend";

interface Props {}

const InventorySell: React.FC<Props> = (props: Props) => {
	const dispatch = useAppDispatch();
	const inventory = useAppSelector(selectInventory);
	const itemsToSell = useAppSelector(selectItemsToSell);
	const sellItemStatus = useAppSelector(selectSellItemStatus);
	const { width, height } = useWindowDimensions();

	const selectedItemIds = itemsToSell.items.map((i) => i.id);

	useEffect(() => {
		if (sellItemStatus.status === Status.Failed) {
			dispatch(toggleItemToSell({ showConfirmation: false }));
			dispatch(resetSellItemStatus());
		}
	}, [sellItemStatus]);

	const handleConfirmClick = () => {
		if (itemsToSell.items.length === 10) {
			dispatch(sellItemToBlacksmith(selectedItemIds));
			dispatch(
				toggleItemToSell({ unselectedAll: true, showConfirmation: false })
			);
		} else {
			dispatch(toggleItemToSell({ showConfirmation: false }));
		}
	};

	const handleConfirmClose = () => {
		dispatch(toggleItemToSell({ showConfirmation: false }));
	};

	const renderInventoryItems = () => {
		const items: JSX.Element[] = [];
		for (let i = 0; i < 60; i++) {
			let inventoryItem;
			let detailsLeft = false;
			let isNftItem = false;
			let detailsLeftMargin;
			if (typeof inventory[i] !== "undefined") {
				inventoryItem = inventory[i];
				if (
					inventoryItem.slot === ItemSlotTypeDto.Shard ||
					inventoryItem.slot === ItemSlotTypeDto.UnidentifiedSkill
				) {
					isNftItem = true;
				}
				// Determine if details should show on left or right of item and above or below
				if (width > mdScreenWidth) {
					const itemPosition = i % 10;
					if (itemPosition === 7 || itemPosition === 8 || itemPosition === 9) {
						detailsLeft = true;
					}
				} else if (width > xsScreenWidth) {
					const itemPosition = i % 6;
					if (itemPosition === 3 || itemPosition === 4 || itemPosition === 5) {
						detailsLeft = true;
					}
				} else {
					const itemPosition = i % 3;
					if (itemPosition === 0) {
						detailsLeftMargin = 0;
					}
					if (itemPosition === 1) {
						detailsLeftMargin = -70;
					}
					if (itemPosition === 2) {
						detailsLeftMargin = -140;
					}
				}
			}
			items.push(
				<Grid className={styles.sellDidContainer}>
					<ItemDND
						item={inventoryItem}
						itemIndex={i}
						isSellableItem={true}
						isNftItem={isNftItem}
						detailsLeft={detailsLeft}
						detailsMarginLeft={detailsLeftMargin}
						disableDND={true}
						key={i}
					/>
					{inventoryItem && selectedItemIds.includes(inventoryItem.id) && (
						<PriorityHighIcon className={styles.sellSelectedIcon} />
					)}
				</Grid>
			);
		}
		return items;
	};

	return (
		<Grid container className={styles.main}>
			<Grid container className={styles.container}>
				{inventory.length > 0 ? (
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
							{renderInventoryItems()}
						</Grid>
					</Scrollbars>
				) : (
					<Grid container className={styles.noItemsMessageContainer}>
						<Typography component="span" className={styles.noItemsMessage}>
							{`YOU'VE GOT NOTHING TO OFFER!`}
						</Typography>
					</Grid>
				)}
			</Grid>
			<Modal
				open={itemsToSell.showConfirmation}
				onClose={handleConfirmClose}
				className={styles.modalMain}
			>
				<Grid container className={styles.confirmModalContainer}>
					<Grid item>
						<Image
							src="/img/unity-assets/shared/tooltip_bg.png"
							height={125}
							width={262.5}
							quality={100}
						/>
					</Grid>
					<Grid container className={styles.confirmContainer}>
						<Typography component="span" className={styles.headerText}>
							{itemsToSell.items.length != 10 ? (
								<>
									SELECTED{" "}
									{
										<span
											className={styles.itemName}
										>{`${itemsToSell.items.length} of 10 items`}</span>
									}{" "}
									REQUIRED TO SELL FOR A QUEST
								</>
							) : (
								<>
									SELL{" "}
									{
										<span
											className={styles.itemName}
										>{`${itemsToSell.items.length} item(s)`}</span>
									}{" "}
									FOR{" "}
									{
										<span className={styles.goldText}>
											{/* {itemToSell.item?.sellPrice} */}
										</span>
									}{" "}
									A QUEST?
								</>
							)}
						</Typography>
					</Grid>
					<Grid container className={styles.confirmButton}>
						<DCXButton
							title={itemsToSell.items.length == 10 ? "CONFIRM" : "CHOOSE MORE"}
							height={32}
							width={120}
							color="blue"
							onClick={() => handleConfirmClick()}
						/>
					</Grid>
				</Grid>
			</Modal>
		</Grid>
	);
};

export default InventorySell;
