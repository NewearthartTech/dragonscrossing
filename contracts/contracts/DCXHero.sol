// SPDX-License-Identifier: UNLICENSED
// The DCX Hero NFT contract
pragma solidity ^0.8.9;

import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Enumerable.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/cryptography/SignatureChecker.sol";
import "@openzeppelin/contracts/utils/Counters.sol";
import "hardhat/console.sol";

contract DCXHero is ERC721Enumerable, Ownable {
    using SignatureChecker for address;
    using Counters for Counters.Counter;
    Counters.Counter private _tokenIds;

    uint256 public initialId;

    //We keep this as a state, so that we can change the base URI in case we need to
    // move our server domains. Also no harm in it being public
    string public baseTokensUri;

    //The address that has permissions to Mint, It will normally be our Tokenomics contract
    address public minterAccount;

    //the address allowed to authorize locks and unlocks.. our backend
    address public authorizer;

    // Mapping from token ID to locked in QUEST timelock
    mapping(uint256 => uint256) private _tokenLocks;

    constructor(
        uint256 _initialId,
        string memory name,
        string memory symbol,
        string memory baseUri)
        ERC721(name, symbol)
    {
        baseTokensUri = baseUri;
        initialId = _initialId;

        //we want the HeroIds to be 1 based
        _tokenIds.increment();
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
    function mint(address forAccount) public returns (uint256){
        require(msg.sender == minterAccount , "not minter");
        uint256 newItemId = initialId+_tokenIds.current();

        require(newItemId <= maxCurrentMints, "current batch already minted");

        _mint(forAccount, newItemId);

        console.log("minted hero with tokenId", newItemId);
        
        _tokenIds.increment();
        return newItemId;
    }


    //we know how many are genesis
    uint256 public maxGenesisHeros = 2500;

    function updateMaxGenesisHeros(uint256 _value) public onlyOwner {
        console.log("maxGenesisHeros updated", _value);
        maxGenesisHeros = _value;
    }

    //We limit the total mints to this number
    //used to limit number of Heros that can be minted
    uint256 public maxCurrentMints = 300;

    function updateMaxCurrentMints(uint256 _value) public onlyOwner {
        console.log("maxCurrentMints updated", _value);
        maxCurrentMints = _value;
    }

    function isGenesisHero(uint256 id) public view returns(bool){
        return id <= maxGenesisHeros;
    }

    /**
     * @dev The Owner can update the Minter, We keep these keys seperate to reduce attack surface
     */
    function updateMinter(address _account) public onlyOwner {
        console.log("minter updated", _account);
        minterAccount = _account;
    }

    /**
     * @dev Throws if the sender is not the owner.
     */
    function _checkIfNotLocked(uint256 tokenId) internal view {
        require(
            0 == _tokenLocks[tokenId] || (_tokenLocks[tokenId] < block.timestamp),
            "token is locked"
        );
    }

    /**
     * @dev Block transfers for locked tokens
     */
    function _beforeTokenTransfer(
        address from,
        address to,
        uint256 tokenId
    ) internal override {
        console.log("calling our _beforeTokenTransfer");
        _checkIfNotLocked(tokenId);
        super._beforeTokenTransfer(from, to, tokenId);
        
    }

    /**
     * @dev Block approvals for locked tokens
     */
    function _approve(address to, uint256 tokenId) internal override {
        _checkIfNotLocked(tokenId);
        super._approve(to, tokenId);
    }

    /**
     * @dev The Owner can update the Authorizer, We keep these keys seperate to reduce attack surface
     */
    function updateAuthorizer(address account) public onlyOwner {
        console.log("authorizer updated", account);
        authorizer = account;
    }

    /**
     * @dev Updates a locked hero. It needs the transaction to be signed by our backend
     */
    function updateLocks(
        uint256 tokenId, // the token Id
        uint256 signatureExipry, //when the signature expires, used to prevent replay attacks
        uint256 lockExpirationTimestamp, //when the lock expires. 0 for unlocking the token
        bytes memory _signature //server signature
    ) external {
        //console.log("signatureExipry ", signatureExipry);
        //console.log("block.timestamp ", block.timestamp);
        require(signatureExipry > block.timestamp,"signature has expired");

        bytes32 _hash = 0 == lockExpirationTimestamp
            ? keccak256(abi.encode(tokenId, signatureExipry))
            : keccak256(
                abi.encode(tokenId, signatureExipry, lockExpirationTimestamp)
            );

        bytes32 signedHash = keccak256(
            abi.encodePacked("\x19Ethereum Signed Message:\n32", _hash)
        );

        //verify that the contractAddress, tokenId, amount matches the message signed by our backend
        require(
            authorizer.isValidSignatureNow(signedHash, _signature),
            "not authorized"
        );

        _tokenLocks[tokenId] = lockExpirationTimestamp;

        emit lockUpdated(tokenId, lockExpirationTimestamp);
    }

    function currentLock(uint256 tokenId) external view returns (uint256) {
        return _tokenLocks[tokenId];
    }

    event lockUpdated(uint256 indexed tokenId, uint256 lockExpirationTimestamp);
}
