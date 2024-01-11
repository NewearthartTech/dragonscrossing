import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** This is used to withdraw funds from tokenomics
 * 
 * TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/withdrawFromTokenomics.ts --network NETWORK 
 */

//const amountToLeave = "0.01";
const amountToLeave = "0";

async function main() {
  console.log("starting to deploy");

  const [deployer] = await ethers.getSigners();
  console.log("Running with account:", deployer.address);
  console.log(
    `Account Balance: ${ethers.utils.formatEther(
      await deployer.getBalance()
    )} [eth]`
  );

  if(!hardhatArguments.network){
    throw new Error("network is required")    ;
  }

  const stats = getDeployedStats();

  const {tokenomicsContract, dcxTokenContract } = stats[hardhatArguments.network];

  if(!dcxTokenContract){
    throw new Error("dcxTokenContract is null");
  }
  
  if(!tokenomicsContract){
    throw new Error("tokenomicsContract is null");
  }

  console.log(`tokenomicsContract - ${tokenomicsContract}`);
  console.log(`dcxTokenContract - ${dcxTokenContract}`);

  const tokenomics =  (await ethers.getContractFactory("Tokenomics")).attach(tokenomicsContract);
  const dcx =  (await ethers.getContractFactory("DCXToken")).attach(dcxTokenContract);

  const currentMintAddress = await tokenomics.mintWalletAddress();
  const mintBalance = ethers.utils.formatEther(
    await tokenomics.mintBalance()
  );
  const currentSeasonalAddress = await tokenomics.seasonalWalletAddress();
  const seasonBalance = ethers.utils.formatEther(
    await tokenomics.seasonalBalance()
  );
  const currentOtherAddress = await tokenomics.otherFeesWalletAddress();

  console.log(`currentMintAddress - ${currentMintAddress}, balance : ${mintBalance}`);
  console.log(`currentSeasonalAddress - ${currentSeasonalAddress}, balance : ${seasonBalance}`);
  console.log(`currentOtherAddress - ${currentOtherAddress}`);

  console.log(
    `tokenomics Balance: ${ethers.utils.formatEther(
      await dcx.balanceOf(tokenomicsContract)
    )} [eth]`
  );

  
  
  const tx = await tokenomics.withdrawFunds(ethers.utils.parseEther(amountToLeave));
  console.log(` tx ${tx.hash}`);
  await tx.wait();
  console.log('tx completed');

  console.log(
    `currentMintAddress Balance: ${ethers.utils.formatEther(
      await dcx.balanceOf(currentMintAddress)
    )} [eth]`
  );

  console.log(
    `currentSeasonalAddress Balance: ${ethers.utils.formatEther(
      await dcx.balanceOf(currentSeasonalAddress)
    )} [eth]`
  );

  console.log(
    `currentOtherAddress Balance: ${ethers.utils.formatEther(
      await dcx.balanceOf(currentOtherAddress)
    )} [eth]`
  );

  console.log(
    `tokenomics Balance: ${ethers.utils.formatEther(
      await dcx.balanceOf(tokenomicsContract)
    )} [eth]`
  );
  
  
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
