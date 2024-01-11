import { ethers, hardhatArguments } from "hardhat";
import { getDeployedStats, getBulkMints, writBulkMints } from "./utils";

/** This is used to check stuff
 *
 * TO DEPLOY
 *  export DEPLOYER_PRIVATE_KEY=
 *  export ALCHEMY_API_KEY=
 *  npx hardhat run scripts/mintReservedHeros.ts --network NETWORK
 */

async function main() {
  const bulkFileName = "/Users/dee/dragonCross/bulkMin-100.json";
  const quantityToMint = 1;
  const boost = {
    c: "Ranger",
    g: "Male",
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


  const bulkMints = getBulkMints(bulkFileName);

  for (let i = 0; i < quantityToMint; i++) {
    const currentMintCount = Object.keys(bulkMints).length;
    const strOrderhash = `bulk-2-${currentMintCount}`;

    console.log(`running order hash ${strOrderhash}   - (${i})`);

    const orderHash = ethers.utils.arrayify(
      ethers.utils.keccak256(ethers.utils.toUtf8Bytes(strOrderhash))
    );
    //const orderHash = await ethers.utils.toUtf8Bytes("bulk-1-1");

    //const randomHeroProps =
    //  ethers.utils.toUtf8Bytes('{\"c\":\"\",\"g\":\"\"}');

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

    bulkMints[mintedId] = {
      gender: boost.g,
      class: boost.c,
    };

    writBulkMints(bulkFileName, bulkMints);
  }
}

// We recommend this pattern to be able to use async/await everywhere
// and properly handle errors.
main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
