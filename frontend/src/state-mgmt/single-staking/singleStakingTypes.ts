export enum PoolType {
  FLEX = "Flex",
  TIER_ONE = "Tier 1",
  TIER_TWO = "Tier 2",
}

export interface StakingPool {
  poolType: PoolType;
  apr: number;
  expirationDate: number;
  totalDcxStaked: number;
  totalWalletDcxStaked: number;
  emissionsAllocation: number;
  claimableUnlockedDcx: number;
  claimableLockedDcx: number;
  depositFee: number;
  withdrawalFee: number;
}
