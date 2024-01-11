import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** This is used to check stuff
 * 
 * TO DEPLOY
 * export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/mintReport.ts --network NETWORK 
 */


async function main() {
  console.log("starting to run");

  
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
  const tokenomics =  (await ethers.getContractFactory("Tokenomics")).attach(tokenomicsContractaddress);

  const totalSupply = (await dcxHero.totalSupply()).toNumber();
  console.log(`total Supply = totalSupply`);

  for(let tokenId=301; tokenId<= totalSupply; tokenId++){
    const [,mintPropsStr] = await tokenomics.getMintedHeroById(tokenId);

    //const mint1 = ethers.utils.toUtf8String (mintPropsStr);

    //console.log(`mint1 = ${mint1}`);

    //const mintProps:{c:string, g:string} = JSON.parse(ethers.utils.toUtf8String (mint1));
  
    //const boostedMint = !!(mintProps.c || mintProps.g);

    const currentOwner = await dcxHero.ownerOf(tokenId);
  
    //console.log(`${tokenId},${currentOwner},${boostedMint}`);
    console.log(`${tokenId},${currentOwner}`);
    
  }

  
  console.log(`completed`);
  
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
