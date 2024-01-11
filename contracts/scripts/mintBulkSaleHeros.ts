import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats } from "./utils";
import * as fs from "fs";
import * as path from "path";

/** This is used to check stuff
 *
 * TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/mintBulkSaleHeros.ts --network NETWORK
 */

const SALE_DATA_FILE = "sale_data_1";
const maxRecords =70;

async function main() {
  console.log("starting to mint");

  if (!hardhatArguments.network) {
    throw new Error("network is required");
  }

  const sellFile = `/Users/dee/dragonCross/NftSmartContracts/data/${SALE_DATA_FILE}.json`;
  const bulkFileName = `/Users/dee/dragonCross/NftSmartContracts/data/${hardhatArguments.network}_SOLD_${SALE_DATA_FILE}.json`;

  console.log("sellFile: ", sellFile);
  console.log("bulkFileName: ", bulkFileName);

  const [deployer] = await ethers.getSigners();
  console.log("Running with account:", deployer.address);
  console.log(
    `Account Balance: ${ethers.utils.formatEther(
      await deployer.getBalance()
    )} [eth]`
  );

  if (!fs.existsSync(bulkFileName)) {
    console.log(`creating output file ${bulkFileName}`);
    fs.copyFileSync(sellFile, bulkFileName);
  }

  const statStr = fs.readFileSync(bulkFileName, "utf-8");
  const bulkSales = JSON.parse(statStr) as BulkSale[];

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

  const tokenomicsContract = (
    await ethers.getContractFactory("Tokenomics")
  ).attach(tokenomicsContractaddress);

  const dcxHero = (await ethers.getContractFactory("DCXHero")).attach(
    DCXHeroContract!
  );

  for (let i = 0; i < bulkSales.length && i < maxRecords; i++) {
    const bukRecord = bulkSales[i];

    if (!bukRecord.minted) bukRecord.minted = [];

    console.log(
      `processing record ${i}, qty:${bukRecord.qty}, currentMinted = ${bukRecord.minted.length}`
    );

    for (let j = bukRecord.minted.length; j < bukRecord.qty; j++) {
      const strOrderhash = `bulk-3-${i}-{j}`;
      console.log(`running order hash ${strOrderhash}`);

      const orderHash = ethers.utils.arrayify(
        ethers.utils.keccak256(ethers.utils.toUtf8Bytes(strOrderhash))
      );

      const boost = bukRecord.boosted
        ? {
            c: bukRecord.class,
            g: bukRecord.gender,
          }
        : {
            c: "",
            g: "",
          };

      const randomHeroProps = await ethers.utils.arrayify(
        ethers.utils.toUtf8Bytes(JSON.stringify(boost))
      );

      const costInBuyToken = 0;
      const quantity = 1;

      const abiCoder = new ethers.utils.AbiCoder();
      const packed = abiCoder.encode(
        ["bytes32", "uint256", "address", "uint256", "bytes", "address"],
        [
          orderHash,
          quantity,
          dcxTokenContract,
          costInBuyToken,
          randomHeroProps,
          deployer.address,
        ]
      );

      const signature = await deployer.signMessage(
        ethers.utils.arrayify(ethers.utils.keccak256(packed))
      );

      const tx = await tokenomicsContract.mintHero(
        orderHash,
        quantity,
        dcxTokenContract,
        costInBuyToken,
        randomHeroProps,
        signature
      );

      console.log(`minting with tx: ${tx.hash} `);

      await tx.wait();

      const [heroIds] = await tokenomicsContract.getMintedHeroByHash(orderHash);
      const mintedIds = heroIds.map((id) => id.toNumber());
      const [mintedId] = mintedIds;
      console.log(`completed minted ${mintedId}`);
      bukRecord.minted.push({
        heroId: mintedId,
        txHash: tx.hash,
      });

      writeSale();
    }

    for (let j = 0; j < bukRecord.minted.length; j++) {
      const minted = bukRecord.minted[j];

      if (!minted.transferred) {
        const tx = dcxHero.transferFrom(
          deployer.address,
          bukRecord.send_to_address,
          minted.heroId
        );
        console.log(`transferred ${minted.heroId} with tx ${(await tx).hash}`);

        minted.transferred = true;
        writeSale();
      }

      if (!minted.locked) {
        const expirationDate = new Date();
        expirationDate.setDate(expirationDate.getDate() + (bukRecord.lock_period_months*31));

        let signExpirationDate = new Date();
        signExpirationDate.setSeconds(signExpirationDate.getSeconds() + 15);
        let sol_SignExpiration = Math.floor(
          signExpirationDate.getTime() / 1000
        );

        const sol_lockExpirationDate = Math.floor(
          expirationDate.getTime() / 1000
        );

        //const sol_lockExpirationDate = 0;

        const abiCoder = new ethers.utils.AbiCoder();

        const packed =
          0 == sol_lockExpirationDate
            ? abiCoder.encode(
                ["uint", "uint"],
                [minted.heroId, sol_SignExpiration]
              )
            : abiCoder.encode(
                ["uint", "uint", "uint"],
                [minted.heroId, sol_SignExpiration, sol_lockExpirationDate]
              );

        const encoded = ethers.utils.keccak256(packed);
        //console.log(`encoded is ${encoded}`);

        const signatureToLock = await deployer.signMessage(
          ethers.utils.arrayify(encoded)
        );

        const tx = dcxHero.updateLocks(
          minted.heroId,
          sol_SignExpiration,
          sol_lockExpirationDate,
          signatureToLock
        );

        console.log(`locked ${minted.heroId} with tx ${(await tx).hash}`);

        minted.locked = true;
        writeSale();


      }
    }

    console.log(`record ${i} done`);
  }

  console.log(`all done total record ${bulkSales.length}`);
}

type BulkSale = {
  minted: {
    locked?: boolean;
    transferred?: boolean;
    heroId: number;
    txHash: string;
  }[];

  user: string;
  boosted: string;
  gender: string;
  class: string;
  qty: number;
  lock_period_months: number;
  send_to_address: string;
};

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
