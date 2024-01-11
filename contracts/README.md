# Sample Hardhat Project

This project demonstrates a basic Hardhat use case. It comes with a sample contract, a test for that contract, and a script that deploys that contract.

Try running some of the following tasks:

```shell
npx hardhat help
npx hardhat typechain
npx hardhat test
GAS_REPORT=true npx hardhat test
npx hardhat node
npx hardhat run scripts/deploy.ts
```

# deploying prod contracts
export DEPLOYER_PRIVATE_KEY= 
export ALCHEMY_API_KEY=
npx hardhat run scripts/deployDCXHero.ts --network arb 
npx hardhat run scripts/deployDCXItem.ts --network arb 


` we are just updating the tokenomics now
npx hardhat run scripts/deployTokenomics.ts --network arb 
npx hardhat run scripts/updateTokenomics.ts --network arb 
npx hardhat run scripts/updateTokenomicsWallets.ts --network arb 

https://game.dragonscrossing.com/api/mints/loadwhitelist/prod_season_load_final.json

# forcing a build again 
