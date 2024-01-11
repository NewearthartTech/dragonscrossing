import { utils } from "ethers";
import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats } from "./utils";


/** TO mint 1000 DCX tokens
 *  export DEPLOYER_PRIVATE_KEY=
 *  export HERO_ADDRESS=
 *  export FOR_ADDRESS=
 *  export HERO_TOKEN_ID=
 *  npx hardhat run scripts/transferHero.ts --network avax_fuji 
 */
async function main() {

const beneficiary = "0x379bA20b339849bBfafA822b2c5714b616842Aca";
const heroTokenId = 7;

const [deployer] = await ethers.getSigners();
  console.log("minting with account:", deployer.address);
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
    throw new Error("hero contract not set");
   }

  console.log(`starting to transfer hero, contract ${DCXHeroContract} for wallet  ${beneficiary}, tokenId ${heroTokenId}`);

  
  const factory = await ethers.getContractFactory("DCXHero");
  const asset = factory.attach(DCXHeroContract)

  console.log("asset attached to:", asset.address);

  let tx = await asset.updateAuthorizer(deployer.getAddress());

  console.log(`updated auth with tx ${(await tx).hash}`);

  var authis = await asset.authorizer();

  console.log(`authis is ${authis}`);

  throw new Error('escalpe');

  //const tx = asset.transferFrom(deployer.address, beneficiary,heroTokenId);
  //console.log(`transferred with tx ${(await tx).hash}`);

}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
