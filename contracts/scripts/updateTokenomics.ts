import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** This is used to set the authroizer of tokenomic and to update minter for Hero's and Items.
 * USED when we update the tokenomic contract
 * TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/updateTokenomics.ts --network goerli 
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

  console.log(`Updating Contract with the tokenomicsContract - ${tokenomicsContract}`);
  console.log(`Updating Contract with the DCXHeroContract - ${DCXHeroContract}`);
  console.log(`Updating Contract with the DCXItemContract - ${DCXItemContract}`);


  const factory = await ethers.getContractFactory("Tokenomics");
  const tokenomics = await factory.attach(tokenomicsContract);

  const currentAuthorizer = await tokenomics.authorizer();

  if(currentAuthorizer.toLowerCase() != deployer.address.toLowerCase()){
    console.log(`updating authorizer to ${deployer.address}` );
    const tx = await tokenomics.updateAuthorizer(deployer.address);
    console.log(`updated tokenomics authorizer to be ${ deployer.address} with tx ${tx.hash}`);
    await tx.wait();
    console.log('tx completed');
  }else{
    console.log(`Authorizer is already ${deployer.address}`);
  }

  {
    const heroContract =  await (await ethers.getContractFactory("DCXHero")).attach(DCXHeroContract);
    const heroMinter = await heroContract.minterAccount();
    if(heroMinter.toLowerCase() != tokenomics.address.toLowerCase()){
      console.log(`updating hero minter to  Tokenomics : ${tokenomics.address}` );
      const tx = await heroContract.updateMinter(tokenomics.address);
      console.log(`with tx ${tx.hash}`);
      await tx.wait();
      console.log('tx completed');
    }else{
      console.log(`heroMinter is already ${tokenomics.address}`);
    }
  }
  
  const itemContract =  await (await ethers.getContractFactory("DCXItem")).attach(DCXItemContract);
  const minter = await itemContract.minterAccount();
  if(minter.toLowerCase() != tokenomics.address.toLowerCase()){
    console.log(`updating item minter to Tokenomics : ${tokenomics.address}` );
    const tx = await itemContract.updateMinter(tokenomics.address);
    console.log(`with tx ${tx.hash}`);
    await tx.wait();
    console.log('tx completed');
  }else{
    console.log(`item minter is already ${tokenomics.address}`);
  }
  
  
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
