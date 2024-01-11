import { utils } from "ethers";
import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";

/** TO mint 1000 DCX tokens
 *  export DEPLOYER_PRIVATE_KEY=
 *  npx hardhat run scripts/mintDCXToken.ts --network avax_fuji 
 */
async function main() {
  console.log("starting to mint");

  if(!hardhatArguments.network){
    throw new Error("please call with --network")
  }

  const [deployer] = await ethers.getSigners();
  console.log("minting with account:", deployer.address);
  console.log(
    `Account Balance: ${ethers.utils.formatEther(
      await deployer.getBalance()
    )} [eth]`
  );

  const dcxTokenContractAddress = getDeployedStats()[hardhatArguments.network]?.dcxTokenContract;
  if(!dcxTokenContractAddress){
    throw new Error("dcxTokenContract deployment address not set");
  }


  const factory = await ethers.getContractFactory("DCXToken");
  const asset = factory.attach(dcxTokenContractAddress)

  console.log("asset attached to:", dcxTokenContractAddress);

  const tx = asset.mint(deployer.address,utils.parseEther("5000"));
  console.log(`minted with tx ${(await tx).hash}`);

}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
