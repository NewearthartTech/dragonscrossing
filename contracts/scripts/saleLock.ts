import { utils } from "ethers";
import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, getBulkMints, writBulkMints } from "./utils";

/** TO mint 1000 DCX tokens
 *  export DEPLOYER_PRIVATE_KEY=
 *  export HERO_ADDRESS=
 *  export FOR_ADDRESS=
 *  export HERO_TOKEN_ID=
 *  npx hardhat run scripts/saleLock.ts --network NETWORK
 */
async function main() {
  const sellTo = "0x7400d5a0D4a691Be85FA89504BF1Ff9160DBe67b";
  const bulkFileName = "/Users/dee/dragonCross/bulkMin-100.json";

  const [deployer] = await ethers.getSigners();
  console.log("Running with account:", deployer.address);
  console.log(
    `Account Balance: ${ethers.utils.formatEther(
      await deployer.getBalance()
    )} [eth]`
  );

  if (!hardhatArguments.network) {
    throw new Error("network is required");
  }

  const stats = getDeployedStats();

  const { DCXHeroContract } = stats[hardhatArguments.network];

  console.log(`starting to transfer hero, contract ${DCXHeroContract} `);

  const factory = await ethers.getContractFactory("DCXHero");
  const asset = factory.attach(DCXHeroContract!);

  console.log("asset attached to:", asset.address);

  const bulkMints = getBulkMints(bulkFileName);

  const allKeys = Object.keys(bulkMints);

  for (let i = 0; i < allKeys.length; i++) {
    const heroTokenId = allKeys[i];

    console.log(`processing ${heroTokenId} - (${i+1})`);

    const { beneficiary, locked } = bulkMints[heroTokenId];

    if (locked) {
      console.log(`key ${heroTokenId} is already locked`);
      continue;
    }

    if (!beneficiary) {
      const tx = asset.transferFrom(deployer.address, sellTo, heroTokenId);
      console.log(`transferred with tx ${(await tx).hash}`);


      bulkMints[heroTokenId].beneficiary = sellTo;
      writBulkMints(bulkFileName, bulkMints);

    }

    const expirationDate = new Date();
    expirationDate.setDate(expirationDate.getDate() + 60);

    let signExpirationDate = new Date();
    signExpirationDate.setSeconds(signExpirationDate.getSeconds() + 15);
    let sol_SignExpiration = Math.floor(signExpirationDate.getTime() / 1000);

    const sol_lockExpirationDate = Math.floor(expirationDate.getTime() / 1000);

    //const sol_lockExpirationDate = 0;

    const abiCoder = new ethers.utils.AbiCoder();

    const packed =
      0 == sol_lockExpirationDate
        ? abiCoder.encode(["uint", "uint"], [heroTokenId, sol_SignExpiration])
        : abiCoder.encode(
            ["uint", "uint", "uint"],
            [heroTokenId, sol_SignExpiration, sol_lockExpirationDate]
          );

    //console.log(`packed1 is ${packed}`);

    const encoded = ethers.utils.keccak256(packed);
    //console.log(`encoded is ${encoded}`);

    const signatureToLock = await deployer.signMessage(
      ethers.utils.arrayify(encoded)
    );

    const tx = asset.updateLocks(
      heroTokenId,
      sol_SignExpiration,
      sol_lockExpirationDate,
      signatureToLock
    );

    console.log(`locked with tx ${(await tx).hash}`);

    bulkMints[heroTokenId].locked = true;
    writBulkMints(bulkFileName, bulkMints);

  }
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
