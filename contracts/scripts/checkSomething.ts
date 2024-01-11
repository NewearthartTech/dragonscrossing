import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** This is used to check stuff
 * 
 * TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/checkSomething.ts --network NETWORK 
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

  const {tokenomicsContract, DCXItemContract, DCXHeroContract } = stats[hardhatArguments.network];

  if(!DCXItemContract){
    throw new Error("DCXItemContract is null");
  }

  if(!DCXHeroContract){
    throw new Error("DCXHeroContract is null");
  }
  
  if(!tokenomicsContract){
    throw new Error("tokenomicsContract is null");
  }

  console.log(`DCXHeroContract - ${DCXHeroContract}`);
  console.log(`tokenomicsContract - ${tokenomicsContract}`);
  console.log(`DCXItemContract - ${DCXItemContract}`);


  const dcxHero =  (await ethers.getContractFactory("DCXHero")).attach(DCXHeroContract);

  const maxMInts = await dcxHero.maxCurrentMints();
  console.log(
    `maxMInts :${maxMInts}`
  );

  /*
  const items =  (await ethers.getContractFactory("DCXItem")).attach(DCXItemContract);
  const baseURI = await items.tokenURI(31)

  console.log(
    `baseURI :${baseURI}`
  );
*/
  
  
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
