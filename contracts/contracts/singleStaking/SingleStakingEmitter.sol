// SPDX-License-Identifier: UNLICENSED
// DCX Tokenomics Contract. It mints heros for money and ditribute funds
pragma solidity ^0.8.9;

import "hardhat/console.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "../DCXToken.sol";

contract SingleStakingEmitter is Ownable {

    struct Emittion {
        uint256 locked;
        uint256 unLocked;
    }

    struct PoolEmmittions{
        uint  emmitDate;
        mapping (address => Emittion) poolEmittions;
    }

    PoolEmmittions[] allEmisstions;
    address public dcxTokenAddress;

    /**
     * Emissions start ocuring from this date
    **/ 
    uint public deployDate;

    constructor(address _dcxTokenAddress){
        dcxTokenAddress = _dcxTokenAddress;
    }

    function addEpocEmittion(uint16 epoch, uint emitDate) external onlyOwner{
        require(0 == allEmisstions[epoch].emmitDate, "emit date is already set");

        allEmisstions[epoch].emmitDate = emitDate;
    }

    /**
     * @dev Once all emissions are pre set we will burn the owner, so that no further changes can be done
    */
    function addEpocEmittion(uint16 epoch, address poolAddress, uint256 locked, uint256 unLocked ) external onlyOwner {
        require(0 != allEmisstions[epoch].emmitDate, "emit date is not set");

        require(0 == allEmisstions[epoch].poolEmittions[poolAddress].locked, "locked balance is not 0");
        require(0 == allEmisstions[epoch].poolEmittions[poolAddress].unLocked, "locked balance is not 0");

        DCXToken(dcxTokenAddress).transferFrom(msg.sender, address(this),(locked+unLocked));

        allEmisstions[epoch].poolEmittions[poolAddress].locked = locked;
        allEmisstions[epoch].poolEmittions[poolAddress].unLocked = unLocked;
    }

    /** 
     * @dev This can be called by anyone
    
    function emitEpoc(uint16 epoch, uint totalPools) public{
        require(allEmisstions[epoch].emmitDate < block.timestamp, "not emit date yet");
        
        for(uint i=0;i<totalPools;i++){
            SingleStakingTier(allEmisstions[epoch].pool).receiveEmitsLocked(allEmisstions[epoch].poolEmittions[poolAddress].locked);
            //DCXToken(dcxTokenAddress).transfer
        }
        
    }
    */

}