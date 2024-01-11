import * as fs from "fs";
import * as path from "path";

const deployedStatsFile = "./deployedContracts.json";
const deployedTSFile = "./deployedContracts.ts";
const deployedCSFile = "./deployedContracts.cs";

export type DeployedStats = {[network:string]:{
    distributeContract?: string;   
    entryPassContract?: string; 
    DCXHeroContract?: string;
    DCXItemContract?: string;
    DCXItemDFKContract?: string;
    dcxTokenContract?: string;
    vaultContract?: string;
    tokenomicsContract?:string;

    mintWalletAddress?: string;
    seasonalWalletAddress?: string;
    otherFeesWalletAddress?: string;

}};

export function getDeployedStats() {
    if (!fs.existsSync(deployedStatsFile)) {
      return {} as DeployedStats;
    }
  
    const statStr = fs.readFileSync(deployedStatsFile, "utf-8");
    return JSON.parse(statStr) as DeployedStats;
}

export function writeStatusFile(stat: DeployedStats) {
    const justDir = path.dirname(deployedStatsFile);

    if (!fs.existsSync(justDir)) {
      console.log(`creating folder ${justDir}`);
      fs.mkdirSync(justDir, { recursive: true });
    }

    const strData = JSON.stringify(stat, null, "\t");
  
    fs.writeFileSync(deployedStatsFile, strData);
    fs.writeFileSync(deployedTSFile, `export const  deployedContracts = ${strData}`);
    fs.writeFileSync(deployedCSFile, `namespace dcx_Contracts; public static class DeployedContracts{public static string contractsStr { get; } = @"${strData.replace(/\"/g,'""')}";}`);

    console.log(`stat file ${deployedStatsFile} updated`)
}

export type BulkMints ={[heroId: string]:{
  beneficiary:string;
  lock_period_months:number;
  locked?:boolean;
  transferred?:boolean;
  gender:string;
  class:string;
}};

export function getBulkMints(fileName:string) {
  if (!fs.existsSync(fileName)) {
    return {} as BulkMints;
  }

  const statStr = fs.readFileSync(fileName, "utf-8");
  return JSON.parse(statStr) as BulkMints;
}

export function writBulkMints(fileName:string, stat: BulkMints) {
  const justDir = path.dirname(fileName);

  if (!fs.existsSync(justDir)) {
    console.log(`creating folder ${justDir}`);
    fs.mkdirSync(justDir, { recursive: true });
  }

  const strData = JSON.stringify(stat, null, "\t");

  fs.writeFileSync(fileName, strData);
  
  console.log(`stat file ${fileName} updated`)
}