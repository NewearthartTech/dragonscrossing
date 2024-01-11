using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Services;
using DragonsCrossing.Core;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DragonsCrossing.NewCombatLogic.tests.ContextSeed.MonsterContextSeed;
using static DragonsCrossing.Core.Helper.DataHelper;

namespace DragonsCrossing.NewCombatLogic.tests
{
    public class UnitTestAC_Minus_2_LevelScalingToMonsterStats
    {
        private readonly Mock<ISeasonsDbService> _dbServiceMock;
        private readonly Mock<IBlockchainService> _blockchainServiceMock;
        private readonly Mock<ILogger<CombatEngine>> _loggerMock;

        private readonly CombatEngine _gameEngine;

        public UnitTestAC_Minus_2_LevelScalingToMonsterStats()
        {
            var _dbServiceMock = new Mock<ISeasonsDbService>();
            var _blockchainServiceMock = new Mock<IBlockchainService>();
            var _loggerMock = new Mock<ILogger<CombatEngine>>();
            //_gameEngine = new CombatEngine(_dbServiceMock.Object, _blockchainServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void TestGenerateLevelIncreasePercentages()
        {
            int levelDiff = 2;
            int result = _gameEngine.GenerateLevelIncreasePercentages(levelDiff);
            Assert.Equal(0, result);

            levelDiff = 3;
            result = _gameEngine.GenerateLevelIncreasePercentages(levelDiff);
            Assert.Equal(7, result);

            // level diff 4 => 7 + 15 = 22
            levelDiff = 4;
            result = _gameEngine.GenerateLevelIncreasePercentages(levelDiff);
            Assert.Equal(22, result);

            // level diff 10 => 7 + 15 + 15 = 37
            levelDiff = 5;
            result = _gameEngine.GenerateLevelIncreasePercentages(levelDiff);
            Assert.Equal(37, result);

            // level diff 10 => 7 + (10-3)*15 = 112
            levelDiff = 10;
            result = _gameEngine.GenerateLevelIncreasePercentages(levelDiff);
            Assert.Equal(112, result);
        }

        /// <summary>
        /// For this test, level diff is 2 so all stats maintains the same
        /// </summary>
        [Fact]
        public void TestApplyLevelScaling_Level_Diff_2()
        {
            // Level diff is 2, levelIncreasePercentage is 0
            int levelIncreasePercentage = 0;
            var monster = GenerateMonsterDto();

            int originalMaxHitPoints = monster.MaxHitPoints;
            int originalChanceToHit = monster.ChanceToHit;
            int originalChanceToDodge = monster.ChanceToDodge;
            int originalHitPoints = monster.HitPoints;

            var effectedProperties = typeof(MonsterDto).GetProperties()
            .Where(x => x.GetCustomAttributes(typeof(EffectedByLevelDifferenceAttribute), true).Any());

            _gameEngine.ApplyLevelScalingToMonsterStats(effectedProperties, levelIncreasePercentage, monster);

            // Things affected
            Assert.Equal(originalMaxHitPoints, monster.MaxHitPoints);
            Assert.Equal(originalChanceToHit, monster.ChanceToHit);

            // Things not affected
            Assert.Equal(originalChanceToDodge, monster.ChanceToDodge);
            Assert.Equal(originalHitPoints, monster.HitPoints);
        }

        [Fact]
        public void TestApplyLevelScaling_Level_Diff_3()
        {
            // Level diff is 3, levelIncreasePercentage is 7
            int levelIncreasePercentage = 7;
            var monster = GenerateMonsterDto();

            int originalDifficultyToHit = monster.DifficultyToHit;
            int originalPower = monster.Power;
            int originalCritChance = monster.CritChance;
            int originalParryChance = monster.ParryChance;

            var effectedProperties = typeof(MonsterDto).GetProperties()
            .Where(x => x.GetCustomAttributes(typeof(EffectedByLevelDifferenceAttribute), true).Any());

            _gameEngine.ApplyLevelScalingToMonsterStats(effectedProperties, levelIncreasePercentage, monster);

            // Things affected
            Assert.Equal(CalculateAndRound(originalDifficultyToHit, 1.0 + levelIncreasePercentage / 100.0), monster.DifficultyToHit);
            Assert.Equal(CalculateAndRound(originalPower, 1.0 + levelIncreasePercentage / 100.0), monster.Power);

            // Things not affected
            Assert.Equal(originalCritChance, monster.CritChance);
            Assert.Equal(originalParryChance, monster.ParryChance);
        }

        [Fact]
        public void TestApplyLevelScaling_Level_Diff_4()
        {
            // Level diff is 4, levelIncreasePercentage is 22
            int levelIncreasePercentage = 22;
            var monster = GenerateMonsterDto();

            int originalMaxHitPoints = monster.MaxHitPoints;
            int originalChanceToHit = monster.ChanceToHit;
            int originalChanceToDodge = monster.ChanceToDodge;
            int originalHitPoints = monster.HitPoints;

            var effectedProperties = typeof(MonsterDto).GetProperties()
            .Where(x => x.GetCustomAttributes(typeof(EffectedByLevelDifferenceAttribute), true).Any());

            _gameEngine.ApplyLevelScalingToMonsterStats(effectedProperties, levelIncreasePercentage, monster);

            // Things affected
            Assert.Equal(CalculateAndRound(originalMaxHitPoints, 1.0 + levelIncreasePercentage / 100.0), monster.MaxHitPoints);
            Assert.Equal(CalculateAndRound(originalChanceToHit, 1.0 + levelIncreasePercentage / 100.0), monster.ChanceToHit);

            // Things not affected
            Assert.Equal(originalChanceToDodge, monster.ChanceToDodge);
            Assert.Equal(originalHitPoints, monster.HitPoints);
        }

        [Fact]
        public void TestEffectedByLevelDifferenceCount_6()
        {
            
            var effectedProperties = typeof(MonsterDto).GetProperties()
            .Where(x => x.GetCustomAttributes(typeof(EffectedByLevelDifferenceAttribute), true).Any());

            // There are 6 properties with the EffectedByLevelDifference attribute tag
            Assert.Equal(6, effectedProperties.Count());
        }
    }
}
