import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/deployDCXHero.ts --network goerli 
 */
async function main() {
  console.log("starting to deploy");

  const [deployer] = await ethers.getSigners();
  console.log("Deploying Contract with the account:", deployer.address);
  console.log(
    `Account Balance: ${ethers.utils.formatEther(
      await deployer.getBalance()
    )} [eth]`
  );

  const factory = await ethers.getContractFactory("DCXHero");
  const asset = await factory.deploy("Dragon's Crossing Hero NFTs","DC_HERO","https://game.dragonscrossing.com/api/metadata/hero/");

  await asset.deployed();

  console.log("asset deployed to:", asset.address);

  
  if(hardhatArguments.network){
    const stats = getDeployedStats();
    stats[hardhatArguments.network] = {
      ...stats[hardhatArguments.network],
      DCXHeroContract: asset.address,
    };
    writeStatusFile(stats);
  }else{
    console.log("empty network no stat file")
  }

  
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
