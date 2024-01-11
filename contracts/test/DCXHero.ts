import { expect } from "chai";
import { ethers } from "hardhat";

//If these imports are showing failure, please compile
// I Like my test being type safe
import { DCXHero } from "../export-npm/src/contracts/DCXHero";

describe("DCXHero", function () {
  async function deployDCXHero() {
    const factory = await ethers.getContractFactory("DCXHero");

    const contract = await factory.deploy(0,"tHero","tHero","http://test.com");
    await contract.deployed();

    return { contract };
  }

  describe("Normal path", function () {
    let dcxHero: DCXHero;
    const tokenId = 1;

    it("Should deploy", async () => {
      const deployed = await deployDCXHero();
      dcxHero = deployed.contract;
      expect(dcxHero.address).to.be.not.null;
    });

    it("can mint", async function () {
      const [, minter, backend, fan, hacker] = await ethers.getSigners();

      await dcxHero.updateMinter(minter.address);

      await dcxHero.connect(minter).mint(fan.address);

      
    });

    

    it("can be locked", async function () {
      const [, minter, backend, fan, hacker] = await ethers.getSigners();

      const adArra = await ethers.getSigners();
      for(let i=0; i<5;i++){
        console.log(`address ${i} = ${adArra[i].address}`);
      }


      await dcxHero.updateAuthorizer(backend.address);

      expect(await dcxHero.authorizer(), "has correct authorizer").to.be.eq(
        backend.address
      );

      await expect(
        dcxHero.connect(hacker).updateAuthorizer(backend.address),
        "hacker cannot update authorizer"
      ).to.be.revertedWith("Ownable: caller is not the owner");

      const expirationDate = new Date();
      expirationDate.setDate(expirationDate.getDate() + 7);

      console.log(`lock expiration set for ${expirationDate}`);

      const signLock = async (
        expirationToSign: number,
        solSignExpiration: number
      ) => {
        const abiCoder = new ethers.utils.AbiCoder();

        const packed =
          0 == expirationToSign
            ? abiCoder.encode(["uint", "uint"], [tokenId, solSignExpiration])
            : abiCoder.encode(
                ["uint", "uint", "uint"],
                [tokenId, solSignExpiration, sol_lockExpirationDate]
              );

        console.log(`packed1 is ${packed}`);

        const encoded = ethers.utils.keccak256(packed);
        console.log(`encoded is ${encoded}`);

        return {
          signature: await backend.signMessage(ethers.utils.arrayify(encoded)),
          encoded,
        };
      };

      const sol_lockExpirationDate = Math.floor(
        expirationDate.getTime() / 1000
      );

      expect(
        await dcxHero.currentLock(tokenId),
        "token is currently unlocked"
      ).to.be.eq(0);

      let signExpirationDate = new Date();
      signExpirationDate.setSeconds(signExpirationDate.getSeconds() + 15);
      let sol_SignExpiration = Math.floor(signExpirationDate.getTime() / 1000);

      const { signature: signatureToLock, encoded } = await signLock(
        sol_lockExpirationDate,
        sol_SignExpiration
      );
      console.log(`signature is ${signatureToLock}`);

      await dcxHero
        .connect(fan)
        .updateLocks(
          tokenId,
          sol_SignExpiration,
          sol_lockExpirationDate,
          signatureToLock
        );
  

      expect(
        await dcxHero.currentLock(tokenId),
        "token is currently locked"
      ).to.be.eq(sol_lockExpirationDate);

      console.log("token is locked");

      const signatureHacker = await hacker.signMessage(
        ethers.utils.arrayify(encoded)
      );

      signExpirationDate = new Date();
      signExpirationDate.setSeconds(signExpirationDate.getSeconds() + 15);
      sol_SignExpiration = Math.floor(signExpirationDate.getTime() / 1000);

      await await expect(
        dcxHero
          .connect(fan)
          .updateLocks(tokenId, sol_SignExpiration, 0, signatureHacker),
        "hacker is trying to unlock"
      ).to.be.revertedWith("not authorized");

      expect(await dcxHero.ownerOf(tokenId), 
      "making sure fan now owns the NFT").to.be.eq(
        fan.address
      );

      expect(await dcxHero.ownerOf(tokenId),
         "fan  owns the NFT before trying to sell locked token").to.be.eq(
        fan.address
      );

      
      await expect(
        dcxHero.connect(fan).transferFrom(fan.address, hacker.address, tokenId),
        "cannot transfer while locked"
      ).to.be.revertedWith("token is locked");
      

      expect(await dcxHero.ownerOf(tokenId),
         "fan  owns the NFT after trying to sell locked token").to.be.eq(
        fan.address
      );


      signExpirationDate = new Date();
      signExpirationDate.setSeconds(signExpirationDate.getSeconds() + 15);
      sol_SignExpiration = Math.floor(signExpirationDate.getTime() / 1000);
      const { signature: signatureToUnLock } = await signLock(
        0,
        sol_SignExpiration
      );

      expect(await dcxHero.ownerOf(tokenId), 
        "fan  owns the NFT before updating locks").to.be.eq(
        fan.address
      );

      await dcxHero
        .connect(fan)
        .updateLocks(tokenId, sol_SignExpiration, 0, signatureToUnLock);

        expect(await dcxHero.ownerOf(tokenId), "fan still owns the NFT").to.be.eq(
          fan.address
        );
    
      await dcxHero
        .connect(fan)
        .transferFrom(fan.address, hacker.address, tokenId);
      console.log("token is unlocked");

      expect(
        await dcxHero.ownerOf(tokenId),
        "fan now transfer the NFT"
      ).to.be.eq(hacker.address);
    });
  });
});
