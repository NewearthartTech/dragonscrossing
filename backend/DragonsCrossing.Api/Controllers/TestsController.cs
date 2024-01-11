using DragonsCrossing.Api.Utilities;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Core.Sagas;
using DragonsCrossing.Core.Services;
using DragonsCrossing.NewCombatLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Nethereum.ABI;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers;

/// <summary>
/// This is a child of heroes and shouldn't have a root url.
/// </summary>

[Route("api/[controller]")]
[ApiController]
public class TestsController : ControllerBase
{
    readonly ISeasonsDbService _seasonDb;
    readonly Web3Config _web3Config;
    readonly IGameStateService _gameStateService;
    readonly IDataHelperService _dataHelperService;
    readonly ILogger<TestsController> _logger;

    public TestsController(
        ISeasonsDbService db,
        IConfiguration config,
        IGameStateService gameStateService,
        IDataHelperService dataHelperService,
        ILogger<TestsController> logger
        )
    {
        _seasonDb = db;
        _web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
        _gameStateService = gameStateService;
        _dataHelperService = dataHelperService;
        _logger = logger;

        if (!_web3Config.isTestEndPointAvailable)
        {
            throw new Exception("web3Config.isTestEndPointAvailable is false");
        }
    }

    #region resetStuff

    [HttpGet("fixDice/{reason}/{value}")]
    public string[] FixDie(string reason, int value)
    {
        try
        {
            var done = DiceService.AddFixedResult(reason, value);

            return done.Select(d => $"dice = {d.Key.ToString()}, fixedTo = {d.Value} " ).ToArray();
        }
        catch (Exception ex)
        {
            throw new ExceptionWithCode($"failed to fix dice {ex}");
        }
        
    }

    [HttpGet("resetQuestAndSkill/{heroId}")]
    [Authorize(Policy = "SeasonJoined")]
    public async Task<ContentResult> resetQuestAndSkill(int heroId)
    {
        //this triggers the reset
        await _gameStateService.ResetQuestsIfNeeded(heroId, forceItNow:true);

        var gameStateCollection = _seasonDb.getCollection<DbGameState>();
        var existingGameState = await gameStateCollection.Find(c => c.HeroId == heroId).SingleAsync();

        return Content(JsonConvert.SerializeObject(new
        {
            existingGameState.Hero.lastDailyResetAt
        }), "application/json");

    }

    [HttpGet("learn/{skillSlug}")]
    [Authorize(Policy = "SeasonJoined")]
    public async Task<ContentResult> learnSkill(string skillSlug)
    {
        var heroId = this.GetHeroId();

        using (var session = await _seasonDb.StartTransaction())
        {
            var gameState =  _seasonDb.getCollection<DbGameState>();

            var unLearnedSkill = DataHelper.CreateTypefromJsonTemplate($"templates.skills.{skillSlug}.json", new UnlearnedHeroSkill());

            var learnedSkill = unLearnedSkill.CreateSkillFromUnlearned();

            learnedSkill.levelRequirement = 1;

            var done = await gameState.UpdateOneAsync(g => g.HeroId == heroId,
            Builders<DbGameState>.Update.Push(g => g.Hero.skills, learnedSkill));

            if (done.ModifiedCount != 1)
            {
                throw new Exception("failed to update hero skills");
            }

            await session.CommitTransactionAsync();
        }

        return Content(JsonConvert.SerializeObject(await _seasonDb.getCollection<DbGameState>()
                .Find(c => c.HeroId == heroId).SingleAsync()), "application/json");
    }

    [HttpGet("fixMonster/{slug}")]
    public void fixMonster(string slug)
    {
        throw new NotImplementedException();
    }
    #endregion


    #region gamestate
    [HttpGet("export/{heroId}")]
    [Authorize(Policy = "SeasonJoined")]
    public async Task<ContentResult> ExportGameState(int heroId)
    {
        var gameStateCollection = _seasonDb.getCollection<DbGameState>();
        var existingGameState = await gameStateCollection.Find(c => c.HeroId == heroId).SingleAsync();
        HttpContext.Response.Headers.Add($"Content-Disposition", $"attachment;filename=hero_{heroId}.json");

        return Content(JsonConvert.SerializeObject(existingGameState), "application/json");

    }

    [HttpGet("import/{heroId}/{url}")]
    [Authorize(Policy = "SeasonJoined")]
    public async Task<ContentResult> ImportGameState(int heroId,string url)
    {

        var gameStateCollection = _seasonDb.getCollection<DbGameState>();
        if (null == gameStateCollection)
            throw new InvalidDataException();

        var existingGameState = await gameStateCollection.Find(c => c.HeroId == heroId).SingleAsync();

        var gameState = await ExtentionMethods.LoadObjectFromURL<DbGameState>(
            "https://raw.githubusercontent.com/deeNewearth/dcxgamestates/main/" + url
            );

        _ = await gameStateCollection.ReplaceOneAsync(c => c.HeroId == heroId, gameState);

        var orderCollection = _seasonDb.getCollection<DcxOrder>();
        await orderCollection.DeleteManyAsync(t => true);

        HttpContext.Response.Headers.Add($"Content-Disposition", $"attachment;filename=hero_{heroId}.json");

        return Content(JsonConvert.SerializeObject(existingGameState), "application/json");

    }
    #endregion


    #region helper methods
    #endregion


    #region blockchain tests
    [HttpGet("testSign/{sender}")]
    public string SignMessage(string sender,
        [FromQuery] string? signType,
        [FromQuery] int tokenId =0,
        [FromQuery] decimal priceInDcx = 0,
        [FromQuery] string orderId = ""
        )
    {
        var account = new Account(_web3Config.serverPrivateKey, chainId: _web3Config.defaultChainId);
        var web3 = new Web3(account, _web3Config.chainRpc[_web3Config.defaultChainId.ToString()].rpcUrl);

        //var balance = await web3.Eth.GetBalance.SendRequestAsync(account.Address);

        //Console.WriteLine($"deploying with account {account.Address}, balance = {Web3.Convert.FromWei(balance)} ");
        Console.WriteLine($"using server with account {account.Address}");


        if (string.IsNullOrWhiteSpace(signType))
        {
            signType = "mintItem";
        }

        switch (signType)
        {
            case "mintItem":
                return Web3Utils.generateSignature(_web3Config,
                new ABIValue("uint256", tokenId),
                new ABIValue("address", sender)
                );
            case "claimDcx":
                var amountIWei = Web3.Convert.ToWei(priceInDcx);
                return Web3Utils.generateSignature(_web3Config,
                new ABIValue("uint256", amountIWei),
                new ABIValue("string", orderId),
                new ABIValue("address", sender)
                );
        }


        throw new ExceptionWithCode("no sign type");

    }
    #endregion

    [HttpGet("checkAllLoot")]
    public void CheckAllLoot()
    {
        var MonsterTemplates = TileDto.loadMonsterTemplates();

        var allLootFileName = MonsterTemplates.Select(t => t.LootItemsTemplates).Select(t => t.Keys).Select(k => k.Select(k => k));

        foreach (var monsterLoot in allLootFileName)
        {
            foreach (var fileName in monsterLoot)
            {
                try
                {
                    _dataHelperService.CreateItemFromTemplate(fileName, DcxZones.darkTower);
                }
                catch
                {
                    _logger.LogError($"Filename {fileName} cannot be found.");
                }
            }
        }
    }
}

