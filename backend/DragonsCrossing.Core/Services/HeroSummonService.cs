using System;
using System.ComponentModel.DataAnnotations;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;

namespace DragonsCrossing.Core.Services;

public class HeroSummonService
{
    readonly ILogger _logger;
    readonly ISeasonsDbService _db;
    readonly HeroSummonConfig _config;
    readonly IBlockchainService _blockChain;
    readonly IHeroesService _heroService;

    public HeroSummonService(ISeasonsDbService db,
        IConfiguration config,
        IBlockchainService blockChain,
        IHeroesService heroService,
        ILogger<HeroSummonService> logger)
    {
        _logger = logger;
        _db = db;
        _blockChain = blockChain;
        _heroService = heroService;

        _config = config.GetSection("heroSummon").Get<HeroSummonConfig>() ?? new HeroSummonConfig();
    }


    
}

public class HeroSummonConfig
{
    public decimal minFeesInDsx { get; set; } = 2.25m;
    
}

public class HeroMintBoosts
{
    public Range dcxSpendRange { get; set; } = new Range();

}


