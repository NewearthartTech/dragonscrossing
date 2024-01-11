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
    public class UnitTestQ2_MonsterRollForSpecialAbility
    {
        private readonly Mock<ISeasonsDbService> _dbServiceMock;
        private readonly Mock<IBlockchainService> _blockchainServiceMock;
        private readonly Mock<ILogger<CombatEngine>> _loggerMock;

        private readonly CombatEngine _gameEngine;

        public UnitTestQ2_MonsterRollForSpecialAbility()
        {
            var _dbServiceMock = new Mock<ISeasonsDbService>();
            var _blockchainServiceMock = new Mock<IBlockchainService>();
            var _loggerMock = new Mock<ILogger<CombatEngine>>();
            //_gameEngine = new CombatEngine(_dbServiceMock.Object, _blockchainServiceMock.Object, _loggerMock.Object);
            //_gameEngine.GameState = new DbGameState();
        }

        [Fact]
        public void MonsterAlreadyUsedSpeciality_CannotUseMoreThanOnce_True()
        {
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();
            _gameEngine.Combat.Monster.WhichRoundMonsterUsedSpecialAbility = 2;
            _gameEngine.Combat.Monster.SpecialAbility.CanUseSpecialAbilityMoreThanOnce = false;

            Assert.True(_gameEngine.MonsterHasUsedSpecialAblilty());
        }

        [Fact]
        public void MonsterNotYetUsedSpeciality_CannotUseMoreThanOnce_False()
        {
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();
            _gameEngine.Combat.Monster.WhichRoundMonsterUsedSpecialAbility = null;
            _gameEngine.Combat.Monster.SpecialAbility.CanUseSpecialAbilityMoreThanOnce = false;

            Assert.False(_gameEngine.MonsterHasUsedSpecialAblilty());
        }

        [Fact]
        public void MonsterAlreadyUsedSpeciality_CanUseMoreThanOnce_False()
        {
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();
            _gameEngine.Combat.Monster.WhichRoundMonsterUsedSpecialAbility = 2;
            _gameEngine.Combat.Monster.SpecialAbility.CanUseSpecialAbilityMoreThanOnce = true;

            Assert.False(_gameEngine.MonsterHasUsedSpecialAblilty());
        }

        [Fact]
        public void MonsterNotYetUsedSpeciality_CanUseMoreThanOnce_False()
        {
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();
            _gameEngine.Combat.Monster.WhichRoundMonsterUsedSpecialAbility = null;
            _gameEngine.Combat.Monster.SpecialAbility.CanUseSpecialAbilityMoreThanOnce = true;

            Assert.False(_gameEngine.MonsterHasUsedSpecialAblilty());
        }

        [Fact]
        public void CanMonsterUserSpecailAbility_HigherRoll_False()
        {
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();
            _gameEngine.Combat.Monster.ChanceOfUsingSpecialAbility = 20;

            int diceRoll = 40;

            Assert.False(_gameEngine.CanMonsterUseSpecailAbility(diceRoll));
        }

        [Fact]
        public void CanMonsterUserSpecailAbility_LowerRoll_True()
        {
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();
            _gameEngine.Combat.Monster.ChanceOfUsingSpecialAbility = 20;

            int diceRoll = 3;

            Assert.True(_gameEngine.CanMonsterUseSpecailAbility(diceRoll));
        }

        [Fact]
        public void CanMonsterUserSpecailAbility_SameResult_True()
        {
            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();
            _gameEngine.Combat.Monster.ChanceOfUsingSpecialAbility = 20;

            int diceRoll = 20;

            Assert.True(_gameEngine.CanMonsterUseSpecailAbility(diceRoll));
        }
    }
}
