import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** This is used to check stuff
 * 
 * TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/mintReservedHeros.ts --network NETWORK 
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

  const {tokenomicsContract:tokenomicsContractaddress, DCXHeroContract, dcxTokenContract } = stats[hardhatArguments.network];

  if(!DCXHeroContract){
    throw new Error("DCXHeroContract is null");
  }
  
  if(!tokenomicsContractaddress){
    throw new Error("tokenomicsContract is null");
  }

  if(!dcxTokenContract){
    throw new Error("dcxTokenContract is null");
  }

  console.log(`tokenomicsContract - ${tokenomicsContractaddress}`);
  console.log(`DCXHeroContract - ${DCXHeroContract}`);
  const dcxHero =  (await ethers.getContractFactory("DCXHero")).attach(DCXHeroContract);

  // will need to move this to deployer
  //let tx = await dcxHero.updateMinter(deployer.address);
  //let tx = await dcxHero.updateMinter(tokenomicsContractaddress);

  //for(let i=0; i<3; i++){
    try{
      let tx = await dcxHero.mint("0x8AC25f6F64d781D322d9B5390621b017384C79da");
      tx.wait();
      //console.log(`minted ${i} - ${tx.hash}`);  
      await new Promise(r=>setTimeout(r,500));
  
    }catch(ex){
      console.log('got error');
      await new Promise(r=>setTimeout(r,5000));
    //  i--;
    }
  //}
  
  console.log("completed");
  
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
