using DragonsCrossing.Core.Services;
using DragonsCrossing.Core;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.NewCombatLogic.tests
{
    public class UnitTestQ1_IsHerosTurn
    {
        private readonly Mock<ISeasonsDbService> _dbServiceMock;
        private readonly Mock<IBlockchainService> _blockchainServiceMock;
        private readonly Mock<ILogger<CombatEngine>> _loggerMock;

        private readonly CombatEngine _gameEngine;

        public UnitTestQ1_IsHerosTurn()
        {
            var _dbServiceMock = new Mock<ISeasonsDbService>();
            var _blockchainServiceMock = new Mock<IBlockchainService>();
            var _loggerMock = new Mock<ILogger<CombatEngine>>();
            //_gameEngine = new CombatEngine(_dbServiceMock.Object, _blockchainServiceMock.Object, _loggerMock.Object);
            //_gameEngine.GameState = new DbGameState();
        }

        [Fact]
        public void Is2ndTurn_IsNotHeroTurn_True()
        {
            _gameEngine.Combat.is2ndTurn = true;
            _gameEngine.Combat.isHerosTurn = false;

            _gameEngine.IsHerosTurn();

            Assert.True(_gameEngine.Combat.isHerosTurn);
            Assert.True(_gameEngine.Combat.is2ndTurn);
        }

        [Fact]
        public void Is2ndTurn_IsHeroTurn_False()
        {
            _gameEngine.Combat.is2ndTurn = true;
            _gameEngine.Combat.isHerosTurn = true;

            _gameEngine.IsHerosTurn();

            Assert.False(_gameEngine.Combat.isHerosTurn);
            Assert.True(_gameEngine.Combat.is2ndTurn);
        }

        [Fact]
        public void IsNot2ndTurn_IsNotHeroTurn_False()
        {
            _gameEngine.Combat.is2ndTurn = false;
            _gameEngine.Combat.isHerosTurn = false;

            _gameEngine.IsHerosTurn();

            Assert.False(_gameEngine.Combat.isHerosTurn);
            Assert.True(_gameEngine.Combat.is2ndTurn);
        }

        [Fact]
        public void IsNot2ndTurn_IsHeroTurn_true()
        {
            _gameEngine.Combat.is2ndTurn = false;
            _gameEngine.Combat.isHerosTurn = true;

            _gameEngine.IsHerosTurn();

            Assert.True(_gameEngine.Combat.isHerosTurn);
            Assert.True(_gameEngine.Combat.is2ndTurn);
        }
    }
}
