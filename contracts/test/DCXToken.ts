import { expect } from "chai";
import { ethers } from "hardhat";

import { DCXToken } from "../export-npm/src/contracts/DCXToken";

describe("DCXToken", function () {
  async function deployToken() {
    const dcxFactory = await ethers.getContractFactory("DCXToken");
    const dcxContract = await dcxFactory.deploy("test", "test");
    await dcxContract.deployed();

    return { dcxContract };
  }

  describe("Normal path", function () {
    let dcxToken: DCXToken;

    it("Should deploy", async () => {

      const deployed = await deployToken();

      dcxToken = deployed.dcxContract;
      expect(dcxToken.address).to.be.not.null;
    });


    it("Can mint", async () => {
        const [, holder1] = await ethers.getSigners();
  
        const toMintAmount = 10.66;

        const inWei = ethers.utils.parseEther(toMintAmount.toString());
        const tx = await dcxToken.mint(holder1.address,inWei);

        await tx.wait();

        expect(await dcxToken.balanceOf(holder1.address)).to.be.eq(inWei);

      });


  });
});
