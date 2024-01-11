// SPDX-License-Identifier: UNLICENSED
// The DCX Hero NFT contract
pragma solidity ^0.8.9;

import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Burnable.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "hardhat/console.sol";

contract DCXItemDFK is ERC721Burnable, Ownable {

    //We keep this as a state, so that we can change the base URI in case we need to
    // move our server domains. Also no harm in it being public
    string public baseTokensUri;

    //The address that has permissions to Mint, It will normally be our Tokenomics contract
    address public minterAccount;

    //The largest tokenId minted so far
    uint256 public lastTokenId;

    constructor(
        string memory name,
        string memory symbol,
        string memory baseUri
    )
        ERC721(name, symbol)
    {
        baseTokensUri = baseUri;
        lastTokenId=0;
        minterAccount = msg.sender;
    }

    /**
     * @dev Base URI for computing {tokenURI}. If set, the resulting URI for each
     * token will be the concatenation of the `baseURI` and the `tokenId`. Empty
     * by default, can be overridden in child contracts.
     */
    function _baseURI() internal view override returns (string memory) {
        return baseTokensUri;
    }

        /**
     * @dev Used by the tokenomics smart contract to Mint Heros
     */
    function mint(address forAccount, uint256 tokenId) public payable {
        require(msg.sender == minterAccount, "not minter");
        if(tokenId > lastTokenId)
            lastTokenId = tokenId;
        _mint(forAccount, tokenId);
    }

    /**
     * @dev The Owner can update the Minter, We keep these keys seperate to reduce attack surface
     */
    function updateMinter(address _account) public onlyOwner {
        console.log("minter updated", _account);
        minterAccount = _account;
    }

    function burnByOwner(uint256 tokenId) public onlyOwner {
        _burn(tokenId);
    }


}