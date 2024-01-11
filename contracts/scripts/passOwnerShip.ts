import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** Send ownership over to somewhere else
 * 
 * TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/passOwnerShip.ts --network NETWORK 
 */

const NEW_OWNER = "0x0F11C394F17a4a93b6ec63C07afd51e32CC20848";

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

  const {DCXHeroContract,DCXItemContract, tokenomicsContract} = stats[hardhatArguments.network];

  if(!DCXHeroContract){
    throw new Error("DCXHeroContract is null");
  }
  if(!DCXItemContract){
    throw new Error("DCXItemContract is null");
  }
  if(!tokenomicsContract){
    throw new Error("tokenomicsContract is null");
  }

  
  

  {
    console.log(`Updating Contract with the tokenomicsContract - ${tokenomicsContract}`);

    const factory = await ethers.getContractFactory("Tokenomics");
    const contract = await factory.attach(tokenomicsContract);

    const currentOwner = await contract.owner();

    console.log(`currentOwner = ${currentOwner}`);

    if(currentOwner.toLowerCase() != NEW_OWNER.toLowerCase()){
      console.log(`updating owner to ${NEW_OWNER}` );
      const tx = await contract.transferOwnership(NEW_OWNER);
      console.log(`tx  = ${tx.hash}`);
      await tx.wait();
      console.log('tx completed');
    }else{
      console.log(`Owner is already ${NEW_OWNER}`);
    }
  }
  

  {
    console.log(`Updating Contract with the DCXHeroContract - ${DCXHeroContract}`);

    const factory = await ethers.getContractFactory("DCXHero");
    const contract = await factory.attach(DCXHeroContract);

    const currentOwner = await contract.owner();

    console.log(`currentOwner = ${currentOwner}`);

    if(currentOwner.toLowerCase() != NEW_OWNER.toLowerCase()){
      console.log(`updating owner to ${NEW_OWNER}` );
      const tx = await contract.transferOwnership(NEW_OWNER);
      console.log(`tx  = ${tx.hash}`);
      await tx.wait();
      console.log('tx completed');
    }else{
      console.log(`Owner is already ${NEW_OWNER}`);
    }
  }
  
  {
    console.log(`Updating Contract with the DCXItemContract - ${DCXItemContract}`);

    const factory = await ethers.getContractFactory("DCXItem");
    const contract = await factory.attach(DCXItemContract);

    const currentOwner = await contract.owner();

    console.log(`currentOwner = ${currentOwner}`);

    if(currentOwner.toLowerCase() != NEW_OWNER.toLowerCase()){
      console.log(`updating owner to ${NEW_OWNER}` );
      const tx = await contract.transferOwnership(NEW_OWNER);
      console.log(`tx  = ${tx.hash}`);
      await tx.wait();
      console.log('tx completed');
    }else{
      console.log(`Owner is already ${NEW_OWNER}`);
    }
  }
  
  
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
