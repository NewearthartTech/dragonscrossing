using DragonsCrossing.Api.Utilities;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Armors;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Core.Sagas;
using DragonsCrossing.Domain.GameStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using static DragonsCrossing.Api.Utilities.ExtentionMethods;
using static DragonsCrossing.Core.Helper.DataHelper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "SeasonJoined")]
public class SkillsController : ControllerBase
{
    readonly ISeasonsDbService _seasonDb;
    

    public SkillsController(ISeasonsDbService db)
    {
        _seasonDb = db;
    }

    [HttpGet("forget/{skillId}")]
    public async Task<HeroDto> ForgetSkill(string skillId)
    {
        var heroId = this.GetHeroId();

        using (var session = await _seasonDb.StartTransaction())
        {
            var gameState = await _seasonDb.getCollection<DbGameState>().Find(session, c => c.HeroId == heroId).SingleAsync();

            var skill = gameState.Hero.skills.Single(s => s.id == skillId);

            var usedInstances = skill.skillUseInstance.Where(s => s.alreadyUsed)
                .FirstOrDefault();
            
            if (null != usedInstances)
            {
                throw new ExceptionWithCode("Skill has been used");
            }

            await deAllocateUseInternal(heroId, skillId, true, session);

            await _seasonDb.getCollection<DbGameState>().UpdateOneAsync(session,
                c => c.HeroId == heroId,
                Builders<DbGameState>.Update.PullFilter(c => c.Hero.skills,
                Builders<LearnedHeroSkill>.Filter.Eq(s => s.id, skillId)
                ));


            await session.CommitTransactionAsync();
        }

        return await _seasonDb.getCollection<DbGameState>().Find(c => c.HeroId == heroId)
            .Project(c => c.Hero)
            .SingleAsync();

    }

    void ensureAdventuringGuild(DbGameState gameState)
    {
        var strTitle = gameState.CurrentTile.ToString();

        if (!strTitle.Contains("adventuringGuild"))
        {
            throw new Exception($"Can only be called from adventuringGuild, current is {strTitle}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="heroId"></param>
    /// <param name="skillId"></param>
    /// <param name="removeAll"></param>
    /// <param name="session"></param>
    /// <returns>number of instances removed</returns>
    /// <exception cref="Exception"></exception>
    async Task<int> deAllocateUseInternal(int heroId, string skillId, bool removeAll, IClientSessionHandle session)
    {
        var gameState = await _seasonDb.getCollection<DbGameState>().Find(session, c => c.HeroId == heroId).SingleAsync();

        ensureAdventuringGuild(gameState);

        var skill = gameState.Hero.skills.Single(s => s.id == skillId);

        var toRemoveInstanceIds = skill.skillUseInstance.Where(s => !s.alreadyUsed)
            .Select(s=>s.id)
            .ToArray();

        if (0 == toRemoveInstanceIds.Length)
        {
            return 0;
        }

        if (!removeAll)
        {
            toRemoveInstanceIds = new[] { toRemoveInstanceIds.First() };
        }


        //var toRemoveInstance = skill.skillUseInstance.Where(s => !s.alreadyUsed).FirstOrDefault();


        skill.skillUseInstance = skill.skillUseInstance.Where(s => !toRemoveInstanceIds.Contains(s.id) ).ToArray();

        var maxPointsGained = skill.requiredSkillPoints * toRemoveInstanceIds.Length;

        var pointsGained = gameState.Hero.usedUpSkillPoints >= maxPointsGained ? (maxPointsGained * -1) : 0;

        await _seasonDb.getCollection<DbGameState>().UpdateOneAsync(session,
            c => c.HeroId == heroId && c.Hero.usedUpSkillPoints == gameState.Hero.usedUpSkillPoints,
            Builders<DbGameState>.Update
            .Inc(c => c.Hero.usedUpSkillPoints, pointsGained)
            .Set(c => c.Hero.skills, gameState.Hero.skills)
            );

        return toRemoveInstanceIds.Length;

    }



    [HttpGet("deAllocateUse/{skillId}")]
    public async Task<HeroDto> deAllocateUse(string skillId)
    {
        var heroId = this.GetHeroId();

        using (var session = await _seasonDb.StartTransaction())
        {
            var removed = await deAllocateUseInternal(heroId, skillId, false, session);
            if(0 == removed)
            {
                throw new ExceptionWithCode("No unused skill found");
            }

            await session.CommitTransactionAsync();
        }

            

        return await _seasonDb.getCollection<DbGameState>().Find(c => c.HeroId == heroId)
            .Project(c => c.Hero)
            .SingleAsync();
    }


    [HttpGet("allocateUse/{skillId}")]
    public async Task<HeroDto> AllocatePoints(string skillId)
    {
        var heroId = this.GetHeroId();
        
        var gameState = await _seasonDb.getCollection<DbGameState>().Find(c => c.HeroId == heroId).SingleAsync();


        ensureAdventuringGuild(gameState);


        var skill = gameState.Hero.skills.Single(s => s.id == skillId);

        var remainingSkillPoints = gameState.Hero.baseSkillPoints - gameState.Hero.usedUpSkillPoints;

        if (skill.requiredSkillPoints > remainingSkillPoints)
            throw new ExceptionWithCode("not enough remaining points");

        skill.skillUseInstance = skill.skillUseInstance.Concat(new[] { new SkillUseInstance() }).ToArray();


        await _seasonDb.getCollection<DbGameState>().UpdateOneAsync(
            c => c.HeroId == heroId && c.Hero.usedUpSkillPoints == gameState.Hero.usedUpSkillPoints
            && c.Hero.baseSkillPoints == gameState.Hero.baseSkillPoints,
            Builders<DbGameState>.Update
            .Inc(c=>c.Hero.usedUpSkillPoints, skill.requiredSkillPoints)
            .Set(c=>c.Hero.skills, gameState.Hero.skills)
            );

        return await _seasonDb.getCollection<DbGameState>().Find(c => c.HeroId == heroId)
            .Project(c=>c.Hero)
            .SingleAsync();
    }

}
