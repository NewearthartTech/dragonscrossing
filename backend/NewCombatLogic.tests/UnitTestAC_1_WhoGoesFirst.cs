using DragonsCrossing.Core.Services;
using DragonsCrossing.Core;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using DragonsCrossing.NewCombatLogic.tests.ContextSeed;

namespace DragonsCrossing.NewCombatLogic.tests
{
    public class UnitTestAC_1_WhoGoesFirst
    {
        private readonly Mock<ISeasonsDbService> _dbServiceMock;
        private readonly Mock<IBlockchainService> _blockchainServiceMock;
        private readonly Mock<ILogger<CombatEngine>> _loggerMock;
        private readonly Mock<DbGameState> _combat;
        private readonly CombatEngine _gameEngine;

        public UnitTestAC_1_WhoGoesFirst()
        {
            var _dbServiceMock = new Mock<ISeasonsDbService>();
            var _blockchainServiceMock = new Mock<IBlockchainService>();
            var _loggerMock = new Mock<ILogger<CombatEngine>>();
            //_gameEngine = new CombatEngine(_dbServiceMock.Object, _blockchainServiceMock.Object, _loggerMock.Object);
            //_gameEngine.GameState = new DbGameState();
        }

        [Fact]
        public void SameStat_SameRoll_False()
        {
            int playerRoll = 10;
            int monsterRoll = 10;

            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();

            _gameEngine.GameState.Hero.quickness = 3;
            _gameEngine.Combat.Monster.Quickness = 3;

            Assert.False(_gameEngine.IsHerosTurn(playerRoll, monsterRoll));
        }

        [Fact]
        public void HeroHigherStat_SameRoll_True()
        {
            int playerRoll = 10;
            int monsterRoll = 10;

            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();

            _gameEngine.GameState.Hero.quickness = 5;
            _gameEngine.Combat.Monster.Quickness = 3;

            Assert.True(_gameEngine.IsHerosTurn(playerRoll, monsterRoll));
        }

        [Fact]
        public void MonsterHigherStat_SameRoll_False()
        {
            int playerRoll = 10;
            int monsterRoll = 10;

            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();

            _gameEngine.GameState.Hero.quickness = 3;
            _gameEngine.Combat.Monster.Quickness = 4;

            Assert.False(_gameEngine.IsHerosTurn(playerRoll, monsterRoll));
        }

        [Fact]
        public void SameStat_HeroHigherRoll_True()
        {
            int playerRoll = 20;
            int monsterRoll = 10;

            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();

            _gameEngine.GameState.Hero.quickness = 3;
            _gameEngine.Combat.Monster.Quickness = 3;

            Assert.True(_gameEngine.IsHerosTurn(playerRoll, monsterRoll));
        }

        [Fact]
        public void SameStatMonsterHigherRoll_False()
        {
            int playerRoll = 10;
            int monsterRoll = 15;

            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();

            _gameEngine.GameState.Hero.quickness = 3;
            _gameEngine.Combat.Monster.Quickness = 3;

            Assert.False(_gameEngine.IsHerosTurn(playerRoll, monsterRoll));
        }
    }
}
