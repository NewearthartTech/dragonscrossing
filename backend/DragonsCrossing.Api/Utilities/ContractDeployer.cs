using System;
using System.Dynamic;
using DragonsCrossing.Core.Services;

using Newtonsoft.Json;

namespace DragonsCrossing.Api.Utilities
{
    /// <summary>
    /// Used to enusre Contracts are deployed
    /// </summary>
    public static class ContractDeployer
    {
        public static readonly string ContractsSettingFile = @"deployedContracts.json";

        public static void EnsureContractsAreDeployed(this WebApplicationBuilder builder, string? appCommand = null)
        {
            var web3Config = builder.Configuration.GetSection("web3").Get<Web3Config>() ?? new Web3Config();

            var contractsAreDeployed = null != web3Config.deployedContracts(web3Config.defaultChainId);

            if ("deploycontracts" == appCommand?.ToLower())
            {
                if (contractsAreDeployed)
                {
                    Console.WriteLine("Contracts are already deployed");
                    return;
                }
            }
            else
            {
                if (!contractsAreDeployed)
                {
                    for(var i= 0; i < 100; i++)
                    {
                        Console.Error.WriteLine("Contratcs addresses not configured. Please run will /app:Command deployContracts");
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }

                    throw new Exception("Contratcs addresses not configured. Please run will /app:Command deployContracts");
                }

                return;
            }

            var settingFile = Path.Combine(builder.Environment.ContentRootPath, ContractsSettingFile);
            if (!File.Exists(settingFile))
            {
                throw new Exception($"setting file {settingFile} does not exist");
            }

            var settingsStr = File.ReadAllText(settingFile);

            dynamic settingsJson = JsonConvert.DeserializeObject<ExpandoObject>(settingsStr)??new object();

            web3Config = Web3Utils.deployContracts(web3Config, web3Config.defaultChainId).Result;

            settingsJson.web3 = web3Config;

            settingsStr = JsonConvert.SerializeObject(settingsJson, Formatting.Indented);

            File.WriteAllText(settingFile, settingsStr);

        }
    }
}

