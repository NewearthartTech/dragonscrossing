using DragonsCrossing.Core.Services;
using DragonsCrossing.Core;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonsCrossing.NewCombatLogic.tests.ContextSeed;

namespace DragonsCrossing.NewCombatLogic.tests
{
    public class UnitTestQ3_RollToSeeIfAttactIsHit
    {
        private readonly Mock<ISeasonsDbService> _dbServiceMock;
        private readonly Mock<IBlockchainService> _blockchainServiceMock;
        private readonly Mock<ILogger<CombatEngine>> _loggerMock;

        private readonly CombatEngine _gameEngine;

        public UnitTestQ3_RollToSeeIfAttactIsHit()
        {
            var _dbServiceMock = new Mock<ISeasonsDbService>();
            var _blockchainServiceMock = new Mock<IBlockchainService>();
            var _loggerMock = new Mock<ILogger<CombatEngine>>();
            //_gameEngine = new CombatEngine(_dbServiceMock.Object, _blockchainServiceMock.Object, _loggerMock.Object);
            //_gameEngine.GameState = new DbGameState();
        }

        [Fact]
        public void HerosTurn_HigherCombinedStat_Ture()
        {
            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();

            _gameEngine.GameState.Hero.baseChanceToHit = 30;
            _gameEngine.Combat.Monster.DifficultyToHit = 40;

            int diceRoll = 30;

            Assert.True(_gameEngine.DidHeroHit(diceRoll));
        }

        [Fact]
        public void HerosTurn_LowerCombinedStat_False()
        {
            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();

            _gameEngine.GameState.Hero.baseChanceToHit = 30;
            _gameEngine.Combat.Monster.DifficultyToHit = 40;

            int diceRoll = 5;

            Assert.False(_gameEngine.DidHeroHit(diceRoll));
        }

        [Fact]
        public void MonstersTurn_HigherCombinedStat_Ture()
        {
            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();

            _gameEngine.Combat.Monster.ChanceToHit = 30;
            _gameEngine.GameState.Hero.difficultyToHit = 40;

            int diceRoll = 30;

            Assert.True(_gameEngine.DidMonsterHit(diceRoll));
        }

        [Fact]
        public void MonstersTurn_LowerCombinedStat_False()
        {
            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();

            _gameEngine.Combat.Monster.ChanceToHit = 30;
            _gameEngine.GameState.Hero.difficultyToHit = 40;

            int diceRoll = 5;

            Assert.False(_gameEngine.DidMonsterHit(diceRoll));
        }
    }
}
