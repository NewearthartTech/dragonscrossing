import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** This is used to update the wallets for Tokenomics distribution
 * 
 * TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/updateTokenomicsWallets.ts --network NETWORK 
 */
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

  const {tokenomicsContract, mintWalletAddress, seasonalWalletAddress, otherFeesWalletAddress} = stats[hardhatArguments.network];

  if(!mintWalletAddress){
    throw new Error("mintWalletAddress is null");
  }
  if(!seasonalWalletAddress){
    throw new Error("seasonalWalletAddress is null");
  }
  if(!otherFeesWalletAddress){
    throw new Error("otherFeesWalletAddress is null");
  }
  if(!tokenomicsContract){
    throw new Error("tokenomicsContract is null");
  }

  console.log(`tokenomicsContract - ${tokenomicsContract}`);
  console.log(`mintWalletAddress - ${mintWalletAddress}`);
  console.log(`seasonalWalletAddress - ${seasonalWalletAddress}`);
  console.log(`otherFeesWalletAddress - ${otherFeesWalletAddress}`);


  const factory = await ethers.getContractFactory("Tokenomics");
  const tokenomics = await factory.attach(tokenomicsContract);

  const currentMintAddress = await tokenomics.mintWalletAddress();
  const currentSeasonalAddress = await tokenomics.seasonalWalletAddress();
  const currentOtherAddress = await tokenomics.otherFeesWalletAddress();

  console.log(`currentMintAddress - ${currentMintAddress}`);
  console.log(`currentSeasonalAddress - ${currentSeasonalAddress}`);
  console.log(`currentOtherAddress - ${currentOtherAddress}`);


  if(
    currentMintAddress.toLowerCase() != mintWalletAddress.toLowerCase() ||
    currentSeasonalAddress.toLowerCase() != seasonalWalletAddress.toLowerCase() ||
    currentOtherAddress.toLowerCase() != otherFeesWalletAddress.toLowerCase()
  
  ){
    console.log(`Updating addresses ` );
    const tx = await tokenomics.updateWallets(mintWalletAddress.toLowerCase(),seasonalWalletAddress.toLowerCase(),otherFeesWalletAddress.toLowerCase());
    console.log(` tx ${tx.hash}`);
    await tx.wait();
    console.log('tx completed');
  }else{
    console.log(`All wallet addresses are same`);
  }
  
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
