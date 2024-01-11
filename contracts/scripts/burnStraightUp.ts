import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, writeStatusFile } from "./utils";
import * as fs from "fs";

/** This is used to check stuff
 *
 * TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/burnStraightUp.ts --network NETWORK
 */
const deadAddress = "0x000000000000000000000000000000000000dEaD";

async function main() {
  console.log("starting to burn");
  const quantityToMint = 1104;
  //const quantityToMint = 250;

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
  const bulkFileName = `/Users/dee/dragonCross/NftSmartContracts/data/${hardhatArguments.network}_BURN}.json`;
  console.log("bulkFileName: ", bulkFileName);

  const statStr = fs.existsSync(bulkFileName)
    ? fs.readFileSync(bulkFileName, "utf-8")
    : "[]";
  const bulkSales = JSON.parse(statStr) as {
    minted: number;
    txHash: string;
    burnt?: boolean;
    orderNumber?: number;
  }[];

  function writeSale() {
    const strData = JSON.stringify(bulkSales, null, "\t");

    fs.writeFileSync(bulkFileName, strData);

    console.log(`stat file ${bulkFileName} updated`);
  }

  const stats = getDeployedStats();

  const {
    tokenomicsContract: tokenomicsContractaddress,
    DCXHeroContract,
    dcxTokenContract,
  } = stats[hardhatArguments.network];

  if (!DCXHeroContract) {
    throw new Error("DCXHeroContract is null");
  }

  if (!tokenomicsContractaddress) {
    throw new Error("tokenomicsContract is null");
  }

  if (!dcxTokenContract) {
    throw new Error("dcxTokenContract is null");
  }

  console.log(`tokenomicsContract - ${tokenomicsContractaddress}`);
  console.log(`DCXHeroContract - ${DCXHeroContract}`);

  const dcxHero = (await ethers.getContractFactory("DCXHero")).attach(
    DCXHeroContract
  );

  /*
  // will need to move this to deployer
  let tx = await dcxHero.updateMinter(deployer.address);
  await tx.wait();
  */

  let loopWait = 0;

  for (let mainLoop=0;mainLoop<10000;mainLoop++) {
    console.log(`running main loop ${mainLoop}`)
    try {
      const totalBurnt = (await dcxHero.balanceOf(deadAddress)).toNumber();
      console.log(`totalBurnt = ${totalBurnt}`);

      let waitAfter1 = 0;

      for (let i = totalBurnt + 1; i <= quantityToMint; i++) {
        if (++waitAfter1 > 4) {
          console.log("waiting a bit");
          await new Promise((r) => setTimeout(r, 1000));
          waitAfter1 = 0;
        }

        console.log(`order submitting ${i} `);

        const tx1 = await dcxHero.mint(deadAddress);
        console.log(`order submitted ${i} - ${tx1.hash}`);

        await tx1.wait();
        console.log(`burnt ${i} - ${tx1.hash}`);

        bulkSales.push({
          minted: 0,
          txHash: tx1.hash,
          orderNumber: i,
          burnt: true,
        });

        writeSale();
        loopWait=0;
      }

      console.log(`completed ${quantityToMint}`);

      const tx2 = await dcxHero.updateMinter(tokenomicsContractaddress);
      await tx2.wait();

      console.log(`minter set back to tokenomics ${tokenomicsContractaddress}`);

      return;
    } catch (e) {
      console.log("failed main loop", e);
    }

    const towWait = 2 * ++loopWait;
    console.log(`main loop waiting ${towWait} second`)
    await new Promise((r) => setTimeout(r, 1000 * towWait));
  }
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
