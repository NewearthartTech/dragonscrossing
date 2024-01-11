// SPDX-License-Identifier: UNLICENSED
// The DCX Utility tokens
pragma solidity ^0.8.9;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "hardhat/console.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

contract DCXToken is ERC20, Ownable {
    constructor(
        string memory name,
        string memory symbol
    )  ERC20(name, symbol) {
    }

    /**
    * @dev The Owner can keep creating more supply. 
        In prod we will burn the owner or send it to Gnosis Safe or something
    */
    function mint(address forAccount, uint256 amount) onlyOwner public {
        _mint(forAccount, amount);
    } 
}

