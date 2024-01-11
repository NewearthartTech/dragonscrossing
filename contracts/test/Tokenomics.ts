import { expect } from "chai";
import { ethers } from "hardhat";

//If these imports are showing failure, please compile
// I Like my test being type safe
import { Tokenomics } from "../export-npm/src/contracts/Tokenomics";
import { DCXHero } from "../export-npm/src/contracts/DCXHero";
import { DCXItem } from "../export-npm/src/contracts/DCXItem";
import { DCXToken } from "../export-npm/src/contracts/DCXToken";

describe("Tokenomics", function () {
  async function deployTokenomics(payees: string[], shares: number[]) {
    const dcxHeroFactory = await ethers.getContractFactory("DCXHero");
    const dcxHeroContract = await dcxHeroFactory.deploy(0,"tHero","tHero","http://test.com");
    await dcxHeroContract.deployed();

    const dcxItemFactory = await ethers.getContractFactory("DCXItem");
    const dcxItem = await dcxItemFactory.deploy("tItem","tItem","http://test.com");
    await dcxItem.deployed();

    const dcxTokenFactory = await ethers.getContractFactory("DCXToken");
    const dcxToken = await dcxTokenFactory.deploy("token", "y1");
    await dcxToken.deployed();

    const tokenomicsFactory = await ethers.getContractFactory("Tokenomics");
    const tokenomicsContract = await tokenomicsFactory.deploy(
      dcxToken.address,
      dcxHeroContract.address,
      dcxItem.address,
      //payees,
      //shares
    );
    await tokenomicsContract.deployed();

    return { dcxHeroContract, tokenomicsContract, dcxItem, dcxToken };
  }

  describe("Normal path", function () {
    let dcxHero: DCXHero;
    let dcxItem: DCXItem;
    let dcxToken: DCXToken;
    let tokenomics: Tokenomics;
    const tokenId = 1;
    const shares = [12, 6, 56];
    const tokenPrice = 2.0;
    ("in eth");

    it("Should deploy", async () => {
      const [, backend, holder1, holder2, holder3] = await ethers.getSigners();

      const deployed = await deployTokenomics(
        [holder1.address, holder2.address, holder3.address],
        shares
      );

      dcxHero = deployed.dcxHeroContract;
      expect(dcxHero.address).to.be.not.null;

      dcxItem = deployed.dcxItem;
      expect(dcxItem.address).to.be.not.null;

      dcxToken = deployed.dcxToken;
      expect(dcxToken.address).to.be.not.null;

      tokenomics = deployed.tokenomicsContract;
      expect(tokenomics.address).to.be.not.null;
    });

    it("can mint Item", async function () {
      const [, backend, holder1, holder2, holder3, fan] =
        await ethers.getSigners();

        

      await dcxItem.updateMinter(tokenomics.address);
      await tokenomics.updateAuthorizer(backend.address);

      expect(await tokenomics.authorizer(), "Authorizer is updated").to.be.eq(
        backend.address
      );

      const abiCoder = new ethers.utils.AbiCoder();
      const packed = abiCoder.encode(
        ["uint", "address"],
        [tokenId, fan.address]
      );

      const signature = await backend.signMessage(
        ethers.utils.arrayify(ethers.utils.keccak256(packed))
      );

      const tx = await tokenomics.connect(fan).mintItem(tokenId, signature);

      await tx.wait();

      expect(await dcxItem.ownerOf(tokenId), "fan now owns the token").to.be.eq(
        fan.address
      );
    });

    it("can mint Item with server auth", async function () {
      const [, , holder1, holder2, holder3, fan] =
        await ethers.getSigners();

        const newTokenId = 2;

        console.log(`minting token ${newTokenId} for fan ${fan.address}`);

        const backendAddress = "0x0F11C394F17a4a93b6ec63C07afd51e32CC20848";

      await dcxItem.updateMinter(tokenomics.address);
      await tokenomics.updateAuthorizer(backendAddress);

      //http://localhost:5264/api/tests/testSign/2/0x9965507D1a55bcC2695C58ba16FB37d819B0A4dc
      const signature = "0xc29755e5db45a2ee4987e14c0e766a4c512258fb3cd7d99c645ae0ba822057b209488075066e26aa6af3c2ebf4b32fb7231a86adf73186168dbe65dda48ba3751c";


      const tx = await tokenomics.connect(fan).mintItem(newTokenId, signature);

      await tx.wait();

      expect(await dcxItem.ownerOf(tokenId), "fan now owns the token").to.be.eq(
        fan.address
      );
    });

    it("can claim dcx with server auth", async function () {
      const [, , holder1, holder2, holder3, fan] =
        await ethers.getSigners();

        console.log(`claiming dcx for fan ${fan.address}`);

        const backendAddress = "0x0F11C394F17a4a93b6ec63C07afd51e32CC20848";

        await tokenomics.updateAuthorizer(backendAddress);

        const dcxAmount = 8;
        const priceInWei = ethers.utils.parseEther(dcxAmount.toString());

        const orderId = "abc12345";

        //make sure there's enough dcx to give
        let tx  = await dcxToken.mint(tokenomics.address,priceInWei);
        await tx.wait();


        //http://localhost:5264/api/tests/testSign/0x9965507D1a55bcC2695C58ba16FB37d819B0A4dc?signType=claimDcx&priceInDcx=8&orderId=abc12345
        const signature = "0xcb3e2e11f1b13b68f93060bd35a86de9e53222a8b4d4ff1dce145312583ef7744fd52920df8ca444222eb453130e69b310af7422028aeef71ee243dd11b9cc001b";


       tx = await tokenomics.connect(fan).claimDCX(priceInWei,orderId, signature);

      await tx.wait();

     
    });

    it("can claim DCX", async function () {
      const [, backend, holder1, holder2, holder3, fan] =
        await ethers.getSigners();

      await dcxHero.updateMinter(tokenomics.address);

      await tokenomics.updateAuthorizer(backend.address);

      expect(await tokenomics.authorizer(), "Authorizer is updated").to.be.eq(
        backend.address
      );

      const priceInWei = ethers.utils.parseEther(tokenPrice.toString());

      const orderId = "abc";

      const abiCoder = new ethers.utils.AbiCoder();
      const packed = abiCoder.encode(
        ["uint", "string", "address"],
        [priceInWei, orderId, fan.address]
      );

      const signature = await backend.signMessage(
        ethers.utils.arrayify(ethers.utils.keccak256(packed))
      );

      let tx  = await dcxToken.mint(tokenomics.address,priceInWei);
      await tx.wait();

      const currentFanBalance  = await dcxToken.balanceOf(fan.address);

      tx = await tokenomics
        .connect(fan)
        .claimDCX(priceInWei, orderId, signature);

      await tx.wait();

      expect(await dcxToken.balanceOf(fan.address), "fan now owns the dcx").to.be.eq(
        priceInWei.add(currentFanBalance)
      );

      await expect(
        tokenomics.connect(fan)
        .claimDCX(priceInWei, orderId, signature),
        "cannot resubmit order"
      ).to.be.revertedWith("orderId is already fulfilled");

    });

    it("can mint Hero", async function () {
      const [deployer, backend, mintWallet, seasonWallet, otherWallet, fan] =
        await ethers.getSigners();

      await dcxHero.updateMinter(tokenomics.address);

      let tx = await tokenomics.updateAuthorizer(backend.address);
      await tx.wait();

      expect(await tokenomics.authorizer(), "Authorizer is updated").to.be.eq(
        backend.address
      );


      tx = await tokenomics.updateWallets(mintWallet.address,seasonWallet.address,otherWallet.address);
      await tx.wait();

      expect(await tokenomics.authorizer(), "Authorizer is updated").to.be.eq(
        backend.address
      );


      const orderHash = await ethers.utils.arrayify(ethers.utils.keccak256("0x1111"));
      const randomHeroProps = await ethers.utils.arrayify("0x115511");
      const costInBuyToken =5;
      const quantity =1;

      const abiCoder = new ethers.utils.AbiCoder();
      const packed = abiCoder.encode(
        ["bytes32", "uint256", "address","uint256", "bytes", "address"],
        [orderHash, quantity, dcxToken.address,costInBuyToken, randomHeroProps, fan.address]
      );

      const signature = await backend.signMessage(
        ethers.utils.arrayify(ethers.utils.keccak256(packed))
      );

      tx  = await dcxToken.mint(fan.address,costInBuyToken);
      await tx.wait();

      tx  = await dcxToken.connect(fan).approve(tokenomics.address,costInBuyToken);
      await tx.wait();

      tx = await tokenomics
        .connect(fan)
        .mintHero(orderHash, quantity, dcxToken.address,costInBuyToken, randomHeroProps, signature);

      await tx.wait();

      tx  = await dcxToken.mint(deployer.address, 10);
      await tx.wait();

      tx = await dcxToken.approve(tokenomics.address, 10);
      await tx.wait();

      tx = await tokenomics.registerForSeason(2,"test1");
      await tx.wait();
      tx = await tokenomics.registerForSeason(0,"test2");
      await tx.wait();

      tx = await tokenomics.withdrawFunds(0);
      await tx.wait();

      expect(await dcxToken.balanceOf(mintWallet.address)).to.be.eq("5");
      expect(await dcxToken.balanceOf(seasonWallet.address)).to.be.eq("2");

      const [heroIds,retrievedHeroProps] = await tokenomics.getMintedHeroByHash(orderHash);

      expect(heroIds.length, "we got a hero").to.be.eq(
        1
      );

      expect(heroIds[0], "heros are 1 based").to.be.eq(
        1
      );

      expect(await dcxHero.ownerOf(heroIds[0]), "fan now owns the token").to.be.eq(
        fan.address
      );

      expect(retrievedHeroProps, "we get props back").to.be.eq(
        "0x115511"
      );

      /*
      const holdercurrentBalance = Number.parseFloat(
        ethers.utils.formatEther(await holder1.getBalance())
      );
      await tokenomics["release(address)"](holder1.address);
      const holderNewBalance = Number.parseFloat(
        ethers.utils.formatEther(await holder1.getBalance())
      );

      console.log(
        `holdercurrentBalance = ${holdercurrentBalance}, holderNewBalance = ${holderNewBalance}`
      );

      expect(holderNewBalance).to.be.eq(
        holdercurrentBalance +
          (tokenPrice * shares[0]) / (shares[0] + shares[1] + shares[2])
      );
        */

    });
  });
});
