import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/deployDCXToken.ts --network goerli 
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

  const factory = await ethers.getContractFactory("DCXToken");
  const asset = await factory.deploy("EricS PooCoin","Eric_PN");

  await asset.deployed();

  console.log("asset deployed to:", asset.address);

  if(hardhatArguments.network){
    const stats = getDeployedStats();
    stats[hardhatArguments.network] = {
      ...stats[hardhatArguments.network],
      dcxTokenContract: asset.address,
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
