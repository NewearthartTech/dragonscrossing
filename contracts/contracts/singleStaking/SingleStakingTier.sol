// SPDX-License-Identifier: UNLICENSED
// DCX Tokenomics Contract. It mints heros for money and ditribute funds
pragma solidity ^0.8.9;

import "hardhat/console.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "../DCXToken.sol";
import "@openzeppelin/contracts/utils/math/SafeMath.sol";

contract SingleStakingTier is Ownable {
    using SafeMath for uint256;
    
    uint256 depositFeesPercentage;
    address public dcxTokenAddress;
    uint public poolClosedDate;

    constructor(uint256 _depositFeesPercentage, address _dcxTokenAddress, uint _poolClosedDate){
        depositFeesPercentage = _depositFeesPercentage;
        dcxTokenAddress = _dcxTokenAddress;
        poolClosedDate = _poolClosedDate;
        epocEmits[0].open = true;
    }

    uint256[] withdrawlFeesPercentages;

    function addwithdrawlFees(uint16 epoch, uint256 percentage) external onlyOwner{
        require(0 == withdrawlFeesPercentages[epoch], "withdrawl fees is already set");
        withdrawlFeesPercentages[epoch] = percentage;
    }

    struct EpocStake {
        uint256 staked;
        bool isWithdrawnLocked;
        bool isWithdrawnUnLocked;
    }

    mapping(address=>mapping(uint16 => EpocStake))  stakedAmounts;
    uint256 public totalStaked;

    function stake(uint16 epoch, uint256 dcxAmount, uint256 fees) external{
        //block.number
        require(poolClosedDate <= block.timestamp,"pool closed for deposit");
        require(epocEmits[epoch].open,"epoc not open to stake");
        require(fees>= dcxAmount.mul(depositFeesPercentage).div(100),"fees not correct");
        DCXToken(dcxTokenAddress).transferFrom(msg.sender, address(this),(dcxAmount.add(fees)));

        totalStaked.add(dcxAmount);

        stakedAmounts[msg.sender][epoch].staked.add(dcxAmount);
    }


    struct EpocEmit{
        uint256 totalStaked;
        uint256 unlocked;
        uint256 locked;

        //is it open for staking
        bool open;
    }

    EpocEmit[] epocEmits;

    /**
        called by SingleStaking emitter
    */
    function receiveEmitsLocked(uint16 epoch, uint256 amount) external{
        require(0==epocEmits[epoch].locked, "epoch already emitted");

        DCXToken(dcxTokenAddress).transferFrom(msg.sender, address(this),amount);

        epocEmits[epoch].locked = amount;

    }

    /**
        called by SingleStaking emitter
    */
    function receiveEmitsUnlocked(uint16 epoch, uint256 amount) external{
        require(0==epocEmits[epoch].unlocked, "epoch already emitted");

        DCXToken(dcxTokenAddress).transferFrom(msg.sender, address(this),amount);

        epocEmits[epoch].open = false;
        epocEmits[epoch+1].open = true;
        epocEmits[epoch].unlocked = amount;
        epocEmits[epoch].totalStaked = totalStaked;
    }


    function cumulativeStake(uint16 epoch, address forAddres) public view returns (uint256){

        uint256 cumulativeAmout =0;
        for(uint i=0;i<=epoch;i++){
            cumulativeAmout.add(stakedAmounts[forAddres][epoch].staked);
        } 

        return cumulativeAmout;
    }

    function withDrawStake(uint16 epoch, uint256 amount) external{
        require(epocEmits[epoch].open,"epoc not open to stake");
        uint256 cumulativeAmout =cumulativeStake(epoch, msg.sender);
        require(cumulativeAmout>= amount,"amount should be less then total Staked");

        stakedAmounts[msg.sender][epoch].staked.sub(amount);

        uint256 fees = amount.mul(withdrawlFeesPercentages[epoch]).div(100);

        DCXToken(dcxTokenAddress).transfer(msg.sender,amount.sub(fees));

    }


    function withdrawEmittionUnlocked(uint16 epoch) external{
        require(0 != epocEmits[epoch].unlocked,"epoc not yet emitted");
        require(!stakedAmounts[msg.sender][epoch].isWithdrawnUnLocked,"already withdrawn");

        uint256 cumulativeAmout =cumulativeStake(epoch, msg.sender);

        uint256 emittion = epocEmits[epoch].unlocked.mul(cumulativeAmout).div(epocEmits[epoch].totalStaked);

        stakedAmounts[msg.sender][epoch].isWithdrawnUnLocked = true;

        DCXToken(dcxTokenAddress).transfer(msg.sender,emittion);

    }

    function withdrawEmittionLocked(uint16 epoch) external{
        require(0 != epocEmits[epoch].locked,"epoc not yet emitted");
        require(!stakedAmounts[msg.sender][epoch].isWithdrawnLocked,"already withdrawn");

        uint256 cumulativeAmout =cumulativeStake(epoch, msg.sender);

        uint256 emittion = epocEmits[epoch].locked.mul(cumulativeAmout).div(epocEmits[epoch].totalStaked);

        stakedAmounts[msg.sender][epoch].isWithdrawnLocked = true;

        DCXToken(dcxTokenAddress).transfer(msg.sender,emittion);

    }

}