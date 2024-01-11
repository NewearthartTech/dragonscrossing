using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Armors;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.GameStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using static DragonsCrossing.Api.Utilities.ExtentionMethods;
using static DragonsCrossing.Core.Helper.DataHelper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "SeasonJoined")]
    public class HerbalistController : ControllerBase
    {
        private ISeasonsDbService _db;

        public HerbalistController(ISeasonsDbService db)
        {
            _db = db;
        }

        [HttpGet("herbalistOptions")]
        public async Task<HerbalistDto[]> HerbalistOptions()
        {
            return CreateTypefromJsonTemplate<HerbalistDto[]>($"templates.herbalist.herbalist.json"); ;
        }

        [HttpPut("purchase/{option}")]
        public async Task<HeroDto> PurchaseFromHerbalist(HerbalistOption option)
        {
            var heroId = this.GetHeroId();
            var collection = _db.getCollection<NewCombatLogic.DbGameState>();
            var gameState = await _db.getCollection<NewCombatLogic.DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();

            var herbalistOptions = CreateTypefromJsonTemplate<HerbalistDto[]>($"templates.herbalist.herbalist.json");

            var herbalist = herbalistOptions.Where(o => o.option == option).Single();

            var healthRecoverPercentage = herbalist.percentage;
            var questToConsume = herbalist.quests;
            var maxDailyQuestNeeded = (gameState.Hero.maxDailyQuests + gameState.Hero.extraDailyQuestGiven) - questToConsume;

            if (gameState.Hero.dailyQuestsUsed > maxDailyQuestNeeded)
            {
                throw new ExceptionWithCode("Purchase failed. Hero doesn't have enough quests.");
            }

            gameState.Hero.remainingHitPointsPercentage = gameState.Hero.remainingHitPointsPercentage + healthRecoverPercentage >= 100.0
                ? 100.0
                : gameState.Hero.remainingHitPointsPercentage + healthRecoverPercentage;




            // update quest, and health of a hero
            await collection
                .UpdateOneAsync(h => h.HeroId == heroId &&
                h.Hero.dailyQuestsUsed <= maxDailyQuestNeeded,
                Builders<NewCombatLogic.DbGameState>.Update
                .Inc(h => h.Hero.dailyQuestsUsed, questToConsume)
                .Set(h => h.Hero.remainingHitPointsPercentage, gameState.Hero.remainingHitPointsPercentage));

            return await collection.Find(h => h.HeroId == heroId)
                    .Project(g => g.Hero)
                    .SingleOrDefaultAsync();
        }
    }
}
