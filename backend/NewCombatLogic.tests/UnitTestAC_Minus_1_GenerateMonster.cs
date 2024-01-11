using AutoMapper;
using Castle.Core.Logging;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection.Emit;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;
using static DragonsCrossing.NewCombatLogic.tests.ContextSeed.MonsterContextSeed;

namespace DragonsCrossing.NewCombatLogic.tests;

public class UnitTestAC_Minus_1_GenerateMonster
{
    private readonly Mock<ISeasonsDbService> _dbServiceMock;
    private readonly Mock<IBlockchainService> _blockchainServiceMock;
    private readonly Mock<ILogger<CombatEngine>> _loggerMock;

    private readonly CombatEngine _gameEngine;

    public UnitTestAC_Minus_1_GenerateMonster()
    {
        var _dbServiceMock = new Mock<ISeasonsDbService>();
        var _blockchainServiceMock = new Mock<IBlockchainService>();
        var _loggerMock = new Mock<ILogger<CombatEngine>>();
        //_gameEngine = new CombatEngine(_dbServiceMock.Object, _blockchainServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void TestGenerateMonster()
    {
        //wen-todo:// Create dummy MonsterTemplate that kinda reflect real world values
        //work with Eric. 
        //Call generateMonster Method
        //Make sure the generatedMonster is correct
        //you will be instnaciating the GameEngine class from NewCombatLogic/Engine/GameEngine.cs
        //Please use your fav mock implemenetation to pass in Interfaces in the constructor

        var monsterTemplate = MonsterTemplateSeed;

        //var monsterDto = DragonsCrossing.Core.Contracts.Api.Dto.Zones.TileDto.GenerateMonster(monsterTemplate);

        /*
        Assert.NotNull(monsterDto);
        Assert.Equal(null, monsterDto.WhichRoundMonsterUsedSpecialAbility);
        Assert.Equal(monsterTemplate.MonsterSlug, monsterDto.MonsterSlug);
        Assert.Equal(monsterTemplate.Name, monsterDto.Name);
        Assert.Equal(monsterTemplate.Description, monsterDto.Description);
        Assert.Equal(monsterTemplate.MonsterItems, monsterDto.MonsterItems);
        Assert.Equal(monsterTemplate.AppearChance, monsterDto.AppearChance);
        Assert.Equal(monsterTemplate.DieDamage, monsterDto.DieDamage);
        Assert.Equal(monsterTemplate.LocationTile, monsterDto.LocationTile);
        Assert.Equal(monsterTemplate.SpecialAbility, monsterDto.SpecialAbility);
        Assert.Equal(monsterDto.MaxHitPoints, monsterDto.HitPoints);
        Assert.InRange(monsterDto.MaxHitPoints, 3, 10);
        Assert.InRange(monsterDto.ChanceToHit, 4, 11);
        Assert.InRange(monsterDto.ChanceToDodge, 5, 12);
        Assert.InRange(monsterDto.DifficultyToHit, 6, 13);
        Assert.InRange(monsterDto.CritChance, 7, 14);
        Assert.InRange(monsterDto.ParryChance, 8, 15);
        Assert.InRange(monsterDto.Charisma, 9, 16);
        Assert.InRange(monsterDto.Quickness, 10, 17);
        Assert.InRange(monsterDto.Level, 11, 18);
        Assert.InRange(monsterDto.Power, 12, 19);
        Assert.InRange(monsterDto.ArmorMitigation, 13, 20);
        Assert.InRange(monsterDto.BonusDamage, 14, 21);
        Assert.InRange(monsterDto.ChanceOfUsingSpecialAbility, 15, 22);
        */
    }

    [Fact]
    public void TestApplyCombatOpportunityStat()
    {
        var monsterTemplate = MonsterTemplateSeed;

        //var monsterDto = _gameEngine.GenerateMonster(monsterTemplate);
        //var oridinalCharisma = monsterDto.Charisma;

        //_gameEngine.ApplyCombatOpportunityStat(monsterDto);

        // The template monster's personality is Inspired so charisma will become 130%
        //var expectedCharisma = (int)Math.Round(oridinalCharisma * 1.3);

        //Assert.Equal(expectedCharisma, monsterDto.Charisma);
    }
}
