import Grid from "@mui/material/Grid";
import Modal from "@mui/material/Modal";
import styles from "./staking.module.scss";
import { useAppDispatch, useAppSelector } from "@/state-mgmt/store/hooks";
import { selectDisplayStaking, setDisplayStaking } from "@/state-mgmt/app/appSlice";
import Image from "next/image";
import useWindowDimensions from "../../helpers/window-dimensions";
import { inputTheme, mdScreenWidth, xlScreenWidth } from "@/helpers/global-constants";
import { useEffect, useState } from "react";
import {
  getSingleStakingMock,
  selectGetStakingPoolsStatus,
  selectStakingPools,
} from "@/state-mgmt/single-staking/singleStakingSlice";
import { Status } from "@/state-mgmt/app/appTypes";
import { PoolType, StakingPool } from "@/state-mgmt/single-staking/singleStakingTypes";
import DcxLogo from "../dcx-logo/dcx-logo";
import DCXButton from "../dcx-button/dcx-button";
import Typography from "@mui/material/Typography";
import { ThemeProvider } from "@mui/material/styles";
import TextField from "@mui/material/TextField";

interface Props {}

const Staking: React.FC<Props> = (props: Props) => {
  const dispatch = useAppDispatch();
  const displayStaking = useAppSelector(selectDisplayStaking);
  const stakingPools = useAppSelector(selectStakingPools);
  const getStakingPoolsStatus = useAppSelector(selectGetStakingPoolsStatus);
  const { width, height } = useWindowDimensions();

  const [flex, setFlex] = useState<StakingPool>();
  const [tierOne, setTierOne] = useState<StakingPool>();
  const [tierTwo, setTierTwo] = useState<StakingPool>();
  const [showDepositModal, setShowDepositModal] = useState(false);
  const [showWithdrawModal, setShowWithdrawModal] = useState(false);
  const [selectedPool, setSelectedPool] = useState<StakingPool>();

  useEffect(() => {
    dispatch(getSingleStakingMock());
  }, []);

  useEffect(() => {
    if (getStakingPoolsStatus.status === Status.Loaded) {
      const flexPool = stakingPools.find((p) => p.poolType === PoolType.FLEX);
      if (flexPool) {
        setFlex(flexPool);
      }
      const tierOnePool = stakingPools.find((p) => p.poolType === PoolType.TIER_ONE);
      if (tierOnePool) {
        setTierOne(tierOnePool);
      }
      const tierTwoPool = stakingPools.find((p) => p.poolType === PoolType.TIER_TWO);
      if (tierTwoPool) {
        setTierTwo(tierTwoPool);
      }
    }
  }, [getStakingPoolsStatus]);

  const convertEpochToDate = (epoch: number) => {
    const date = new Date(0);
    date.setUTCSeconds(epoch);
    const year = date.getFullYear();
    let month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : "0" + month;
    let day = date.getDate().toString();
    day = day.length > 1 ? day : "0" + day;

    return month + "/" + day + "/" + year;
  };

  const handleDepositClick = (pool: StakingPool) => {
    setSelectedPool(pool);
    setShowDepositModal(true);
  };

  const handleWithdrawClick = (pool: StakingPool) => {
    setSelectedPool(pool);
    setShowWithdrawModal(true);
  };

  const handleDepositConfirm = () => {
    console.log("Deposit Confirmed");
  };

  const handleWithdrawConfirm = () => {
    console.log("Withdraw Confirmed");
  };

  const handleClose = () => dispatch(setDisplayStaking(false));

  const handleTransactionModalClose = () => {
    setShowDepositModal(false);
    setShowWithdrawModal(false);
  };

  return (
    <Modal open={displayStaking} onClose={handleClose} className={styles.modalMain}>
      <Grid container direction="row" className={styles.container}>
        <Grid container className={styles.stakingContainer}>
          <Image
            src="/img/unity-assets/shared/action_bg_vertical_small.png"
            height={width <= mdScreenWidth ? 500 : 550}
            width={350}
            quality={100}
          />
          <Grid container className={styles.topLeftContainer}>
            <Image src="/img/unity-assets/shared/window_top_left.png" height={40} width={56} quality={100} />
          </Grid>
          <Grid container className={styles.topRightContainer}>
            <Image src="/img/unity-assets/shared/window_top_right.png" height={40} width={60} quality={100} />
          </Grid>
          <Grid container className={styles.stakingHeaderContainer}>
            <Image src="/img/unity-assets/shared/header_red.png" height={45} width={205} quality={100} />
            <Typography component="span" className={styles.stakingHeaderText}>
              FLEX
            </Typography>
          </Grid>
          {flex ? (
            <Grid container className={styles.poolDetailsMain}>
              <Grid container className={styles.poolDetailsContainer}>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailTextApr}>
                    {`APR: ${flex.apr}%`}
                  </Typography>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`TIME UNTIL POOL EXPIRATION: ${convertEpochToDate(flex.expirationDate)}`}
                  </Typography>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`TOTAL STAKED: ${flex.totalDcxStaked}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`PERSONAL STAKED: ${flex.totalWalletDcxStaked}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`EMISSIONS ALLOCATION: ${flex.emissionsAllocation}%`}
                  </Typography>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`CLAIMABLE UNLOCKED: ${flex.claimableUnlockedDcx}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`CLAIMABLE LOCKED: ${flex.claimableLockedDcx}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
              </Grid>
              <Grid container direction="column" className={styles.buttonContainer}>
                <DCXButton
                  title="DEPOSIT"
                  height={45}
                  width={160}
                  color="blue"
                  disabledLayerHeightAdjustment={11}
                  disabledLayerWidthAdjustment={20}
                  arrowTopAdjustment={16}
                  onClick={() => handleDepositClick(flex)}
                />
                <Grid container className={styles.divider} />
                <DCXButton
                  title="WITHDRAW"
                  height={45}
                  width={160}
                  color="red"
                  disabledLayerHeightAdjustment={11}
                  disabledLayerWidthAdjustment={20}
                  arrowTopAdjustment={16}
                  onClick={() => handleWithdrawClick(flex)}
                />
              </Grid>
            </Grid>
          ) : (
            <Grid container className={styles.errorMessageContainer}>
              <Typography
                component="span"
                className={styles.errorMessage}
              >{`FAILED TO RETRIEVE POOL DETAILS`}</Typography>
            </Grid>
          )}
        </Grid>
        <Grid container className={styles.stakingContainer}>
          <Image
            src="/img/unity-assets/shared/action_bg_vertical_small.png"
            height={width <= mdScreenWidth ? 500 : 550}
            width={350}
            quality={100}
          />
          <Grid container className={styles.topLeftContainer}>
            <Image src="/img/unity-assets/shared/window_top_left.png" height={40} width={56} quality={100} />
          </Grid>
          <Grid container className={styles.topRightContainer}>
            <Image src="/img/unity-assets/shared/window_top_right.png" height={40} width={60} quality={100} />
          </Grid>
          <Grid container className={styles.stakingHeaderContainer}>
            <Image src="/img/unity-assets/shared/header_red.png" height={45} width={205} quality={100} />
            <Typography component="span" className={styles.stakingHeaderText}>
              TIER 1
            </Typography>
          </Grid>
          {tierOne ? (
            <Grid container className={styles.poolDetailsMain}>
              <Grid container className={styles.poolDetailsContainer}>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailTextApr}>
                    {`APR: ${tierOne.apr}%`}
                  </Typography>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`TIME UNTIL POOL EXPIRATION: ${convertEpochToDate(tierOne.expirationDate)}`}
                  </Typography>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`TOTAL STAKED: ${tierOne.totalDcxStaked}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`PERSONAL STAKED: ${tierOne.totalWalletDcxStaked}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`EMISSIONS ALLOCATION: ${tierOne.emissionsAllocation}%`}
                  </Typography>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`CLAIMABLE UNLOCKED: ${tierOne.claimableUnlockedDcx}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`CLAIMABLE LOCKED: ${tierOne.claimableLockedDcx}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
              </Grid>
              <Grid container direction="column" className={styles.buttonContainer}>
                <DCXButton
                  title="DEPOSIT"
                  height={45}
                  width={160}
                  color="blue"
                  disabledLayerHeightAdjustment={11}
                  disabledLayerWidthAdjustment={20}
                  arrowTopAdjustment={16}
                  onClick={() => handleDepositClick(tierOne)}
                />
                <Grid container className={styles.divider} />
                <DCXButton
                  title="WITHDRAW"
                  height={45}
                  width={160}
                  color="red"
                  disabledLayerHeightAdjustment={11}
                  disabledLayerWidthAdjustment={20}
                  arrowTopAdjustment={16}
                  onClick={() => handleWithdrawClick(tierOne)}
                />
              </Grid>
            </Grid>
          ) : (
            <Grid container className={styles.errorMessageContainer}>
              <Typography
                component="span"
                className={styles.errorMessage}
              >{`FAILED TO RETRIEVE POOL DETAILS`}</Typography>
            </Grid>
          )}
        </Grid>
        <Grid container className={styles.stakingContainer}>
          <Image
            src="/img/unity-assets/shared/action_bg_vertical_small.png"
            height={width <= mdScreenWidth ? 500 : 550}
            width={350}
            quality={100}
          />
          <Grid container className={styles.topLeftContainer}>
            <Image src="/img/unity-assets/shared/window_top_left.png" height={40} width={56} quality={100} />
          </Grid>
          <Grid container className={styles.topRightContainer}>
            <Image src="/img/unity-assets/shared/window_top_right.png" height={40} width={60} quality={100} />
          </Grid>
          <Grid container className={styles.stakingHeaderContainer}>
            <Image src="/img/unity-assets/shared/header_red.png" height={45} width={205} quality={100} />
            <Typography component="span" className={styles.stakingHeaderText}>
              TIER 2
            </Typography>
          </Grid>
          {tierTwo ? (
            <Grid container className={styles.poolDetailsMain}>
              <Grid container className={styles.poolDetailsContainer}>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailTextApr}>
                    {`APR: ${tierTwo.apr}%`}
                  </Typography>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`TIME UNTIL POOL EXPIRATION: ${convertEpochToDate(tierTwo.expirationDate)}`}
                  </Typography>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`TOTAL STAKED: ${tierTwo.totalDcxStaked}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`PERSONAL STAKED: ${tierTwo.totalWalletDcxStaked}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`EMISSIONS ALLOCATION: ${tierTwo.emissionsAllocation}%`}
                  </Typography>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`CLAIMABLE UNLOCKED: ${tierTwo.claimableUnlockedDcx}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
                <Grid container className={styles.row}>
                  <Typography component="span" className={styles.detailText}>
                    {`CLAIMABLE LOCKED: ${tierTwo.claimableLockedDcx}`}
                  </Typography>
                  <Grid container className={styles.dcxLogoContainer}>
                    <DcxLogo />
                  </Grid>
                </Grid>
              </Grid>
              <Grid container direction="column" className={styles.buttonContainer}>
                <DCXButton
                  title="DEPOSIT"
                  height={45}
                  width={160}
                  color="blue"
                  disabledLayerHeightAdjustment={11}
                  disabledLayerWidthAdjustment={20}
                  arrowTopAdjustment={16}
                  onClick={() => handleDepositClick(tierTwo)}
                />
                <Grid container className={styles.divider} />
                <DCXButton
                  title="WITHDRAW"
                  height={45}
                  width={160}
                  color="red"
                  disabledLayerHeightAdjustment={11}
                  disabledLayerWidthAdjustment={20}
                  arrowTopAdjustment={16}
                  onClick={() => handleWithdrawClick(tierTwo)}
                />
              </Grid>
            </Grid>
          ) : (
            <Grid container className={styles.errorMessageContainer}>
              <Typography
                component="span"
                className={styles.errorMessage}
              >{`FAILED TO RETRIEVE POOL DETAILS`}</Typography>
            </Grid>
          )}
        </Grid>
        <Modal
          open={showDepositModal || showWithdrawModal}
          onClose={handleTransactionModalClose}
          className={styles.modalMain}
        >
          <Grid container direction="row" className={styles.transactionContainer}>
            <Image src="/img/unity-assets/shared/rectangle_vertical_bg.png" height={400} width={380} quality={100} />
            <Grid container className={styles.topLeftContainer}>
              <Image src="/img/unity-assets/shared/window_top_left.png" height={40} width={56} quality={100} />
            </Grid>
            <Grid container className={styles.topRightContainer}>
              <Image src="/img/unity-assets/shared/window_top_right.png" height={40} width={60} quality={100} />
            </Grid>
            <Grid container className={styles.stakingHeaderContainer}>
              <Image src="/img/unity-assets/shared/header_red.png" height={45} width={205} quality={100} />
              <Typography component="span" className={styles.stakingHeaderText}>
                {`${selectedPool?.poolType}`}
              </Typography>
            </Grid>
            <Grid container className={styles.transactionPoolDetailsMain}>
              <Grid container className={styles.transactionPoolDetailsContainer}>
                <Grid container className={styles.transactionRow}>
                  <Typography component="span" className={styles.detailText}>
                    {`DCX IN WALLET:`}
                  </Typography>
                  <Grid container className={styles.amountContainer}>
                    <Typography component="span" className={styles.detailText}>
                      {`1000`}
                    </Typography>
                    <Grid container className={styles.dcxLogoContainer}>
                      <DcxLogo />
                    </Grid>
                  </Grid>
                </Grid>
                <Grid container className={styles.transactionRow}>
                  <Typography component="span" className={styles.detailText}>
                    {`CURRENTLY STAKED:`}
                  </Typography>
                  <Grid container className={styles.amountContainer}>
                    <Typography component="span" className={styles.detailText}>
                      {`400`}
                    </Typography>
                    <Grid container className={styles.dcxLogoContainer}>
                      <DcxLogo />
                    </Grid>
                  </Grid>
                </Grid>
                <Grid container className={styles.transactionRow}>
                  <Typography component="span" className={styles.detailText}>
                    {showDepositModal ? `DCX TO DEPOSIT:` : showWithdrawModal ? `DCX TO WITHDRAW:` : ``}
                  </Typography>
                  <Grid container className={styles.amountContainer}>
                    <Grid container className={styles.inputFieldContainer}>
                      <ThemeProvider theme={inputTheme}>
                        <TextField variant="outlined" />
                      </ThemeProvider>
                    </Grid>
                    <Grid container className={styles.dcxLogoContainer}>
                      <DcxLogo />
                    </Grid>
                  </Grid>
                </Grid>
                <Grid container className={styles.transactionRow}>
                  <Typography component="span" className={styles.detailText}>
                    {showDepositModal ? `DEPOSIT FEE %:` : showWithdrawModal ? `WITHDRAWAL FEE %:` : ``}
                  </Typography>
                  <Grid container className={styles.amountContainer}>
                    <Typography component="span" className={styles.detailText}>
                      {`1%`}
                    </Typography>
                  </Grid>
                </Grid>
                <Grid container className={styles.transactionRow}>
                  <Typography component="span" className={styles.detailText}>
                    {showDepositModal ? `DEPOSIT FEE:` : showWithdrawModal ? `WITHDRAWAL FEE:` : ``}
                  </Typography>
                  <Grid container className={styles.amountContainer}>
                    <Typography component="span" className={styles.detailText}>
                      {`2`}
                    </Typography>
                    <Grid container className={styles.dcxLogoContainer}>
                      <DcxLogo />
                    </Grid>
                  </Grid>
                </Grid>
              </Grid>
              <Grid container direction="column" className={styles.transactionButtonContainer}>
                <DCXButton
                  title={showDepositModal ? "CONFIRM DEPOSIT" : showWithdrawModal ? "CONFIRM WITHDRAW" : ""}
                  height={45}
                  width={160}
                  color={showDepositModal ? "blue" : "red"}
                  disabledLayerHeightAdjustment={11}
                  disabledLayerWidthAdjustment={20}
                  arrowTopAdjustment={16}
                  onClick={() =>
                    showDepositModal ? handleDepositConfirm() : showWithdrawModal ? handleWithdrawConfirm() : undefined
                  }
                />
              </Grid>
            </Grid>
          </Grid>
        </Modal>
      </Grid>
    </Modal>
  );
};

export default Staking;
