using DragonsCrossing.Core.Services;
using DragonsCrossing.Core;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using Nethereum.Contracts.Standards.ERC20.TokenList;
using System.Reflection;
using Xunit.Sdk;
using Microsoft.VisualBasic.FileIO;
using SearchOption = System.IO.SearchOption;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.NewCombatLogic.tests.ContextSeed;

namespace DragonsCrossing.NewCombatLogic.tests
{
    public class UnitTestQ5_CalculateChanceToParry
    {
        private readonly Mock<ISeasonsDbService> _dbServiceMock;
        private readonly Mock<IBlockchainService> _blockchainServiceMock;
        private readonly Mock<ILogger<CombatEngine>> _loggerMock;

        private readonly CombatEngine _gameEngine;

        public UnitTestQ5_CalculateChanceToParry()
        {
            var _dbServiceMock = new Mock<ISeasonsDbService>();
            var _blockchainServiceMock = new Mock<IBlockchainService>();
            var _loggerMock = new Mock<ILogger<CombatEngine>>();
            //_gameEngine = new CombatEngine(_dbServiceMock.Object, _blockchainServiceMock.Object, _loggerMock.Object);
            //_gameEngine.GameState = new DbGameState();

            #region Combat initialization

            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();
            _gameEngine.Combat.Monster.WhichRoundMonsterUsedSpecialAbility = 1;
            _gameEngine.Combat.CurrentRound = 1;
            _gameEngine.Combat.Monster.SpecialAbility.Affects = new StatusEffectDto[]{};

            #endregion
        }

        [Fact]
        public void HerosTurnWithNoSpecialAbility_MonstersOriginalChanceToParry_10()
        {
            #region Data Setup

            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.GameState.Hero.level = 17;
            _gameEngine.Combat.isHerosTurn = true;
            _gameEngine.Combat.Monster.ParryChance = 10;

            #endregion

            int chanceToDodge = _gameEngine.CalculateChanceToParry();
            

            Assert.Equal(10, chanceToDodge);
        }

        [Fact]
        public void MonstersTurnWithNoSpecialAbilityAndNoEquippedItems_HerosWisdom17_85()
        {
            #region Data Setup

            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.GameState.Hero.wisdom = 17;
            _gameEngine.Combat.isHerosTurn = false;
            _gameEngine.Combat.Monster.ParryChance = 10;
            #endregion

            int chanceToDodge = _gameEngine.CalculateChanceToParry();


            Assert.Equal(85, chanceToDodge);
        }

        [Fact]
        public void MonstersTurnWithNoSpecialAbility_EquippedOneItemParryRate20_HerosWisdom17_105()
        {
            #region Data Setup

            _gameEngine.GameState.Hero = HeroContextSeed.HeroSeedData;
            _gameEngine.GameState.Hero.wisdom = 17;
            _gameEngine.Combat.isHerosTurn = false;
            _gameEngine.Combat.Monster.ParryChance = 10;
            //_gameEngine._GameState.Hero.EquippedItems = ItemContextSeed.ItemsSeedData;

            #endregion

            int chanceToDodge = _gameEngine.CalculateChanceToParry();


            Assert.Equal(105, chanceToDodge);
        }
    }
}
