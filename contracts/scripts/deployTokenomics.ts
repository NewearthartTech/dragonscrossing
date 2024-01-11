import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/deployTokenomics.ts --network goerli 
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

  if(!hardhatArguments.network){
    throw new Error("network is required")    ;
  }

  const stats = getDeployedStats();

  const {dcxTokenContract,DCXHeroContract,DCXItemContract} = stats[hardhatArguments.network];

  if(!dcxTokenContract){
    throw new Error("dcxTokenContract is null");
  }
  if(!DCXHeroContract){
    throw new Error("DCXHeroContract is null");
  }
  if(!DCXItemContract){
    throw new Error("DCXItemContract is null");
  }

  console.log(`Deploying Contract with the dcxTokenContract - ${dcxTokenContract}`);
  console.log(`Deploying Contract with the DCXHeroContract - ${DCXHeroContract}`);
  console.log(`Deploying Contract with the DCXItemContract - ${DCXItemContract}`);

  

  const factory = await ethers.getContractFactory("Tokenomics");
  const asset = await factory.deploy(dcxTokenContract,DCXHeroContract,DCXItemContract);

  await asset.deployed();

  console.log("asset deployed to:", asset.address);

  if(hardhatArguments.network){
    
    stats[hardhatArguments.network] = {
      ...stats[hardhatArguments.network],
      tokenomicsContract: asset.address,
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
