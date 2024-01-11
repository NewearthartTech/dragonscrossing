// SPDX-License-Identifier: UNLICENSED
// DCX Tokenomics Contract. It mints heros for money and ditribute funds
pragma solidity ^0.8.9;

import "hardhat/console.sol";
import "@openzeppelin/contracts/utils/cryptography/SignatureChecker.sol";
import "@openzeppelin/contracts/utils/math/SafeMath.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/finance/PaymentSplitter.sol";
import "./DCXHero.sol";
import "./DCXItem.sol";
import "./DCXToken.sol";


contract Tokenomics is Ownable {
    using SignatureChecker for address;
    using SafeMath for uint256;

    //the address allowed to authorize the mint.. our backend
    address public authorizer; 

    address public dcxTokenAddress;
    address public dcxHeroAddress;
    address public dcxItemAddress;

    address public mintWalletAddress;
    address public seasonalWalletAddress;
    address public otherFeesWalletAddress;


    /**
     * @dev Creates an instance of `PaymentSplitter` where each account in `payees` is assigned the number of shares at
     * the matching position in the `shares` array.
     *
     * All addresses in `payees` must be non-zero. Both arrays must have the same non-zero length, and there must be no
     * duplicates in `payees`.
     */
    constructor(
        address _dcxTokenAddress,
        address _dcxHeroAddress,
        address _dcxItemAddress
    ){
        dcxTokenAddress = _dcxTokenAddress;
        dcxHeroAddress = _dcxHeroAddress;
        dcxItemAddress = _dcxItemAddress;

        mintWalletAddress = msg.sender;
        seasonalWalletAddress = msg.sender;
        otherFeesWalletAddress = msg.sender;
    }

    function updateWallets(address _mintWalletAddress, address _seasonalWalletAddress, address _otherFeesWalletAddress) external onlyOwner {
        console.log("updateWallets called");

        mintWalletAddress= _mintWalletAddress;
        seasonalWalletAddress= _seasonalWalletAddress;
        otherFeesWalletAddress= _otherFeesWalletAddress;
    }


    uint256 public mintBalance;
    uint256 public seasonalBalance;

    function withdrawFunds(uint256 quantityToLeave) external {
        console.log("withDrawToken", quantityToLeave);

        uint256 balance = DCXToken(dcxTokenAddress).balanceOf(address(this));
        require(balance>quantityToLeave, "not enough balance");

        balance = balance - quantityToLeave;

        console.log("balance: ", balance);
        console.log("mintBalance: ", mintBalance);
        require(balance >= mintBalance,"balance needs to be more then or equal mintBalance");
        if(mintBalance > 0 ){
            balance = balance - mintBalance;
            console.log("to mintWalletAddress", mintBalance);
            DCXToken(dcxTokenAddress).transfer( mintWalletAddress,mintBalance);
            mintBalance = 0;
        }else{
            console.log("mintBalance is greater then balance or 0");
        }


        require(balance >= seasonalBalance,"balance needs to be more then or equal seasonalBalance");
        if(seasonalBalance > 0 ){
            balance = balance - seasonalBalance;
            console.log("to seasonalWalletAddress", seasonalBalance);
            DCXToken(dcxTokenAddress).transfer( seasonalWalletAddress,seasonalBalance);
            seasonalBalance = 0;
        }else{
            console.log("seasonalWalletAddress is greated then balance or 0");
        }

        if(balance > 0 ){
            console.log("to otherFeesWalletAddress", balance);
            DCXToken(dcxTokenAddress).transfer( otherFeesWalletAddress,balance);
        }else{
            console.log("otherFeesWalletAddress is 0");
        }

    }



    function updateAuthorizer(address account) external onlyOwner {
        console.log("authorizer updated", account);
        authorizer= account;
    }

    
    mapping(string => bool) dcxClaimOrdersfulfiled;

    function claimDCX(
        uint256 amount, // total dcx to give in gwi
        string memory orderId, //to prevent replay, we only give DCX once per order

        bytes memory _signature //server signature
    )  external{

        require(dcxClaimOrdersfulfiled[orderId] == false,"orderId is already fulfilled");
        dcxClaimOrdersfulfiled[orderId] = true;


        bytes32 _hash = keccak256(abi.encode(amount, orderId, msg.sender));

        bytes32 signedHash = keccak256(
            abi.encodePacked("\x19Ethereum Signed Message:\n32", _hash)
        );

        //verify that the contractAddress, tokenId, amount matches the message signed by our backend
        require(authorizer.isValidSignatureNow(signedHash, _signature), "not authorized");


        DCXToken(dcxTokenAddress).transfer( msg.sender,amount);
    }
    
    function exchangeItem(
        uint256 newTokenId,  // the token new Id to get
        uint256 oldTokenId, // the tokenId to Burn

        uint256 priceInDcx,

        bytes memory _signature //server signature
    )  external{

        bytes32 _hash = keccak256(abi.encode(newTokenId,oldTokenId,priceInDcx,msg.sender));

        bytes32 signedHash = keccak256(
            abi.encodePacked("\x19Ethereum Signed Message:\n32", _hash)
        );

        //verify that the contractAddress, tokenId, amount matches the message signed by our backend
        require(authorizer.isValidSignatureNow(signedHash, _signature), "not authorized");

        DCXToken(dcxTokenAddress).transferFrom(msg.sender, address(this),priceInDcx);
        DCXItem(dcxItemAddress).burn(oldTokenId);

        if(0 != newTokenId){
            DCXItem(dcxItemAddress).mint(msg.sender, newTokenId);
        }
        

        emit itemExchanged(newTokenId,oldTokenId);
    }

    event itemExchanged(uint256 newTokenId, uint256 oldTokenId);

    function mintItem(
        uint256 tokenId,  // the token Id

        bytes memory _signature //server signature
    )  external{

        bytes32 _hash = keccak256(abi.encode(tokenId, msg.sender));

        bytes32 signedHash = keccak256(
            abi.encodePacked("\x19Ethereum Signed Message:\n32", _hash)
        );

        //verify that the contractAddress, tokenId, amount matches the message signed by our backend
        require(authorizer.isValidSignatureNow(signedHash, _signature), "not authorized");

        DCXItem(dcxItemAddress).mint(msg.sender, tokenId);

        emit itemMinted(tokenId);
    }

    event itemMinted(uint256 tokenId);

    //used to keep track of what we used to mint a Hero With
    struct HeroMintProps{
        uint256 quantity;
        uint256[] ids;
        bytes heroProps;
    }

    //map of orderHash => HeroMinted
    mapping(bytes32 => HeroMintProps) mintedHerosMap;

    //map of HeroIds to orderhash
    mapping( uint256 => bytes32) mintedHeroIdMap;

    function getMintedHeroByHash(bytes32 orderHash) public view returns(uint256[] memory,bytes memory){
        return (mintedHerosMap[orderHash].ids,mintedHerosMap[orderHash].heroProps);
    }

    function getMintedHeroById(uint256 id) public view returns(uint256[] memory,bytes memory){
        return getMintedHeroByHash(mintedHeroIdMap[id]);
    }


    function summonHero(
        bytes32 orderHash,  //keeps track of which order minted what Hero
        uint256 runeStoneId, //the token Id of the RuneStone
        uint256 priceInDcx,

        bytes memory heroProps, //How we want this hero minted

        uint256 heroIdToTransfer, //if non 0 transfer this hero instead of minting

        bytes memory _signature //server signature
    ) public{

        require(mintedHerosMap[orderHash].quantity == 0,"duplicate orderHash");

        mintedHerosMap[orderHash].quantity = 1;
        mintedHerosMap[orderHash].heroProps = heroProps;

        bytes32 _hash = keccak256(abi.encode(orderHash, runeStoneId, priceInDcx, heroProps,heroIdToTransfer,msg.sender));

        bytes32 signedHash = keccak256(
            abi.encodePacked("\x19Ethereum Signed Message:\n32", _hash)
        );

        //verify that the contractAddress, tokenId, amount matches the message signed by our backend
        require(authorizer.isValidSignatureNow(signedHash, _signature), "not authorized");

        DCXToken(dcxTokenAddress).transferFrom(msg.sender, address(this),priceInDcx);

        if(runeStoneId > 0){
            DCXItem(dcxItemAddress).burn(runeStoneId);
        }
        
        if(heroIdToTransfer>0){
            DCXHero(dcxHeroAddress).safeTransferFrom(address(this), msg.sender, heroIdToTransfer);
        }else{

            uint256 mintedId = DCXHero(dcxHeroAddress).mint(msg.sender);
            mintedHerosMap[orderHash].ids = [mintedId];
            mintedHeroIdMap[mintedId] = orderHash;
            emit heroSummoned(orderHash,mintedId);
        }



    }

    event heroSummoned(bytes32 orderHash,uint256 mintedId);

    //used to mint 1st get heros
    function mintHero(
        bytes32 orderHash,  //keeps track of which order minted what Hero
        uint256 quantity,

        address buyTokenAddress, //The ERC20 we are using to mint The hero
        uint256 costInBuyToken, //how may buy tokens we need for this

        bytes memory heroProps, //How we want this hero minted

        bytes memory _signature //server signature
    ) public{

        require( quantity <= 5, "max 5");

        console.log("Minting Hero order ");
        console.logBytes32(orderHash);

        require(mintedHerosMap[orderHash].quantity == 0,"duplicate orderHash");

        mintedHerosMap[orderHash].quantity = quantity;
        mintedHerosMap[orderHash].heroProps = heroProps;

        bytes32 _hash = keccak256(abi.encode(orderHash, quantity, buyTokenAddress, costInBuyToken, heroProps, msg.sender));

        bytes32 signedHash = keccak256(
            abi.encodePacked("\x19Ethereum Signed Message:\n32", _hash)
        );

        //verify that the contractAddress, tokenId, amount matches the message signed by our backend
        require(authorizer.isValidSignatureNow(signedHash, _signature), "not authorized");

        //yeah it's NOT DCXToken but it is ERC20
        DCXToken(buyTokenAddress).transferFrom(msg.sender, address(this),costInBuyToken);

        mintBalance += costInBuyToken;

        for(uint256 i=0;i<quantity;i++){
            uint256 mintedId = DCXHero(dcxHeroAddress).mint(msg.sender);
            mintedHerosMap[orderHash].ids.push(mintedId);
            mintedHeroIdMap[mintedId] = orderHash;

        }

        emit heroMinted(orderHash);

    }

    event heroMinted(bytes32 orderHash);

    
    mapping(string => bool) registeredOrders;
    /**
    * @dev register DCX tokens for game play
    */
    function registerForSeason(uint256 amount, string memory orderId) public{

        require(registeredOrders[orderId] == false,"orderId is already registered");

        seasonalBalance += amount; 

        if(amount > 0){
            DCXToken(dcxTokenAddress).transferFrom(msg.sender, address(this),amount);
        }
        

        emit registered(orderId, amount);
    }

    event registered(string indexed orderId, uint256 amount);

    function mintLoanerHero(
        bytes32 orderHash,  //keeps track of whihc order minted what Hero
        uint256 priceInDcx,
        bytes memory _signature //server signature
    ) public{

        require(mintedHerosMap[orderHash].quantity == 0,"duplicate orderHash");

        mintedHerosMap[orderHash].quantity = 1;

        bytes32 _hash = keccak256(abi.encode(orderHash, priceInDcx ,msg.sender));

        bytes32 signedHash = keccak256(
            abi.encodePacked("\x19Ethereum Signed Message:\n32", _hash)
        );

        //verify that the contractAddress, tokenId, amount matches the message signed by our backend
        require(authorizer.isValidSignatureNow(signedHash, _signature), "not authorized");

        if(priceInDcx>0){
            DCXToken(dcxTokenAddress).transferFrom(msg.sender, address(this),priceInDcx);
        }

        uint256 mintedId = DCXHero(dcxHeroAddress).mint(address(this));
        mintedHerosMap[orderHash].ids = [mintedId];
        mintedHeroIdMap[mintedId] = orderHash;

        emit heroSummoned(orderHash,mintedId);

    }
    

    
}
