import { HardhatUserConfig } from "hardhat/config";
import "@nomicfoundation/hardhat-toolbox";
import "hardhat-gas-reporter"
import dotenv from "dotenv";
dotenv.config();

const config: HardhatUserConfig = {
  solidity: "0.8.9",
  typechain:{
    outDir: 'export-npm/src'
  },
  gasReporter: {
    enabled: true,
    currency: 'ETH',
    gasPrice: 21
  }
};

if(process.env.DEPLOYER_PRIVATE_KEY){
  config.networks={
    /* our testnet */
    goerli: {
      url: `https://eth-goerli.g.alchemy.com/v2/${process.env.ALCHEMY_API_KEY}`,
      accounts: [process.env.DEPLOYER_PRIVATE_KEY],
    },
    
    avax_fuji: {
      url: `https://rpc.ankr.com/avalanche_fuji`,
      accounts: [process.env.DEPLOYER_PRIVATE_KEY],
    },

    polygon_mumbai_dee: {
      url: `https://polygon-mumbai.g.alchemy.com/v2/${process.env.ALCHEMY_API_KEY}`,
      accounts: [process.env.DEPLOYER_PRIVATE_KEY],
    },

    dfk_testnet_dee: {
      url: `https://subnets.avax.network/defi-kingdoms/dfk-chain-testnet/rpc`,
      accounts: [process.env.DEPLOYER_PRIVATE_KEY],
    },

    dfk_mainnet_dee: {
      url: `https://subnets.avax.network/defi-kingdoms/dfk-chain/rpc`,
      accounts: [process.env.DEPLOYER_PRIVATE_KEY],
    },

    polygon_mumbai_staging: {
      url: `https://polygon-mumbai.g.alchemy.com/v2/${process.env.ALCHEMY_API_KEY}`,
      accounts: [process.env.DEPLOYER_PRIVATE_KEY],
    },

    rinkeby: {
      url: `https://eth-rinkeby.alchemyapi.io/v2/${process.env.ALCHEMY_API_KEY}`,
      accounts: [process.env.DEPLOYER_PRIVATE_KEY],
    },

    /* the contracts used to test out metadata from local dev machine */
    rinkebyDev: {
      url: `https://eth-rinkeby.alchemyapi.io/v2/${process.env.ALCHEMY_API_KEY}`,
      accounts: [process.env.DEPLOYER_PRIVATE_KEY],
    },

    arb: {
      url: `https://arb-mainnet.g.alchemy.com/v2/${process.env.ALCHEMY_API_KEY}`,
      accounts: [process.env.DEPLOYER_PRIVATE_KEY],
    },

    arb_main: {
      url: `https://arb-mainnet.g.alchemy.com/v2/${process.env.ALCHEMY_API_KEY}`,
      accounts: [process.env.DEPLOYER_PRIVATE_KEY],
    }
  };
}

export default config;
