import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, getBulkMints, writBulkMints } from "./utils";
import * as fs from "fs";
/** This is used to check stuff
 *
 * TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/burnTheRest.ts --network NETWORK
 */

const deadAddress = "0x000000000000000000000000000000000000dEaD";


async function main() {
  //const quantityToMint = 1109;
  const quantityToMint = 100;
  const boost = {
    c: "",
    g: "",
    //g:"Female"
  };

  console.log("starting to mint");

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
    txHash:string;
    burnt?: boolean;
    
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

  const dcxHero = (await ethers.getContractFactory("DCXHero")).attach(
    DCXHeroContract!
  );


  console.log(`tokenomicsContract - ${tokenomicsContractaddress}`);
  console.log(`DCXHeroContract - ${DCXHeroContract}`);

  const tokenomicsContract = (
    await ethers.getContractFactory("Tokenomics")
  ).attach(tokenomicsContractaddress);

  for (let i = 0; i < quantityToMint; i++) {
    if (i >= bulkSales.length) {
      const strOrderhash = `toBurn-1-${i}`;

      console.log(`running order hash ${strOrderhash}   - (${i})`);

      const orderHash = ethers.utils.arrayify(
        ethers.utils.keccak256(ethers.utils.toUtf8Bytes(strOrderhash))
      );

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
      bulkSales.push({
        minted: mintedId,
        txHash: tx.hash,
      });

      writeSale();
    }else{
      console.log(`hero ${i} already minted`);
    }

    if(bulkSales[i].burnt){
      console.log(`hero ${i} already burnt`);
    }else{
      const tx = dcxHero.transferFrom(
        deployer.address,
        deadAddress,
        bulkSales[i].minted
      );
      console.log(`transferred ${bulkSales[i].minted} with tx ${(await tx).hash}`);

      bulkSales[i].burnt = true;
      writeSale();
    }
  }

  console.log("all done");
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
