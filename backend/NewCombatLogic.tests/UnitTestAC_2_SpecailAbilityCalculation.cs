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
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.NewCombatLogic.tests.ContextSeed;

namespace DragonsCrossing.NewCombatLogic.tests
{
    public class UnitTestAC_2_SpecailAbilityCalculation
    {
        private readonly Mock<ISeasonsDbService> _dbServiceMock;
        private readonly Mock<IBlockchainService> _blockchainServiceMock;
        private readonly Mock<ILogger<CombatEngine>> _loggerMock;
        private readonly Mock<DbGameState> _combat;
        private readonly CombatEngine _gameEngine;

        public UnitTestAC_2_SpecailAbilityCalculation()
        {
            var _dbServiceMock = new Mock<ISeasonsDbService>();
            var _blockchainServiceMock = new Mock<IBlockchainService>();
            var _loggerMock = new Mock<ILogger<CombatEngine>>();
            //_gameEngine = new CombatEngine(_dbServiceMock.Object, _blockchainServiceMock.Object, _loggerMock.Object);
           // _gameEngine.GameState = new DbGameState();
        }

        [Fact]
        public void CalculateMonsterSpecialAbility_AffectMonster_ChanceToDodge_by30()
        {
            #region Combat initialization

            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();
            _gameEngine.Combat.Monster.WhichRoundMonsterUsedSpecialAbility = 1;
            _gameEngine.Combat.CurrentRound = 2;
            _gameEngine.Combat.Monster.SpecialAbility.Affects = new StatusEffectDto[] 
            { 
                new StatusEffectDto()
                {
                    AffectType = CombatantType.Monster,
                    StatName = EffectedbySpecialAbilityStat.Q4_ChanceToDodge,
                    AffectAmount = 20,
                    Duration = 2 
                },

                new StatusEffectDto()
                {
                    AffectType = CombatantType.Hero,
                    StatName = EffectedbySpecialAbilityStat.Q4_ChanceToDodge,
                    AffectAmount = 15,
                    Duration = 3
                },

                new StatusEffectDto()
                {
                    AffectType = CombatantType.Monster,
                    StatName = EffectedbySpecialAbilityStat.Q3_ChanceToHit,
                    AffectAmount = 40,
                    Duration = 4
                }
            };

            #endregion

            #region Params setup

            CombatantType affectType = CombatantType.Monster;
            EffectedbySpecialAbilityStat property = EffectedbySpecialAbilityStat.Q4_ChanceToDodge;
            int orginalChanceToDodge = 10;

            #endregion

            int result = _gameEngine.CalculateSpecialAbilityIfApplicable(affectType, property, orginalChanceToDodge);

            Assert.Equal(30, result);
        }

        [Fact]
        public void CalculateMonsterSpecialAbility_AffectHero_BonusDamage_WithMultipleStatusAffects_by40()
        {
            #region Combat initialization

            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();
            _gameEngine.Combat.Monster.WhichRoundMonsterUsedSpecialAbility = 1;
            _gameEngine.Combat.CurrentRound = 2;
            _gameEngine.Combat.Monster.SpecialAbility.Affects = new StatusEffectDto[]
            {
                new StatusEffectDto()
                {
                    AffectType = CombatantType.Monster,
                    StatName = EffectedbySpecialAbilityStat.Q4_ChanceToDodge,
                    AffectAmount = 20,
                    Duration = 2
                },

                new StatusEffectDto()
                {
                    AffectType = CombatantType.Hero,
                    StatName = EffectedbySpecialAbilityStat.Q8_BonusDamage,
                    AffectAmount = 8,
                    Duration = 3
                },

                new StatusEffectDto()
                {
                    AffectType = CombatantType.Hero,
                    StatName = EffectedbySpecialAbilityStat.Q8_BonusDamage,
                    AffectAmount = 7,
                    Duration = 3
                },

                new StatusEffectDto()
                {
                    AffectType = CombatantType.Monster,
                    StatName = EffectedbySpecialAbilityStat.Q3_ChanceToHit,
                    AffectAmount = 40,
                    Duration = 4
                }
            };

            #endregion

            #region Params setup

            CombatantType affectType = CombatantType.Hero;
            EffectedbySpecialAbilityStat property = EffectedbySpecialAbilityStat.Q8_BonusDamage;
            int orginalBonusDamage = 25;

            #endregion

            int result = _gameEngine.CalculateSpecialAbilityIfApplicable(affectType, property, orginalBonusDamage);

            Assert.Equal(40, result);
        }

        [Fact]
        public void CalculateMonsterSpecialAbility_AffectMonster_ChanceToDodge_NoActiveStatusAffect_by2()
        {
            #region Combat initialization

            _gameEngine.Combat.Monster = MonsterContextSeed.GenerateMonsterDto();
            _gameEngine.Combat.Monster.WhichRoundMonsterUsedSpecialAbility = 1;
            _gameEngine.Combat.CurrentRound = 5;
            _gameEngine.Combat.Monster.SpecialAbility.Affects = new StatusEffectDto[]
            {
                new StatusEffectDto()
                {
                    AffectType = CombatantType.Monster,
                    StatName = EffectedbySpecialAbilityStat.Q5_Parry,
                    AffectAmount = 20,
                    Duration = 2
                },

                new StatusEffectDto()
                {
                    AffectType = CombatantType.Hero,
                    StatName = EffectedbySpecialAbilityStat.Q4_ChanceToDodge,
                    AffectAmount = 15,
                    Duration = 3
                },

                new StatusEffectDto()
                {
                    AffectType = CombatantType.Monster,
                    StatName = EffectedbySpecialAbilityStat.Q3_ChanceToHit,
                    AffectAmount = 40,
                    Duration = 2
                }
            };

            #endregion

            #region Params setup

            CombatantType affectType = CombatantType.Monster;
            EffectedbySpecialAbilityStat property = EffectedbySpecialAbilityStat.Q5_Parry;
            int orginalChanceToDodge = 2;

            #endregion

            int result = _gameEngine.CalculateSpecialAbilityIfApplicable(affectType, property, orginalChanceToDodge);

            Assert.Equal(2, result);
        }


    }
}
