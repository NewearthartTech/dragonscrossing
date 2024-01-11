import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** This is used to check stuff
 * 
 * TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/updateMaxMints.ts --network NETWORK 
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

  const { DCXHeroContract } = stats[hardhatArguments.network];

  
  if(!DCXHeroContract){
    throw new Error("DCXHeroContract is null");
  }
  
  
  console.log(`DCXHeroContract - ${DCXHeroContract}`);
  

  const dcxHero =  (await ethers.getContractFactory("DCXHero")).attach(DCXHeroContract);

  const tx = await dcxHero.updateMaxCurrentMints(100000000000000);
  
  await tx.wait();

  console.log(`updated max mints with tx ${tx.hash}`);
  
  
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
