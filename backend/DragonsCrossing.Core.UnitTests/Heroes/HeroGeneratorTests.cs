using DragonsCrossing.Core.Heroes;
using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DragonsCrossing.Core.UnitTests
{
    [TestClass]
    public class HeroGeneratorTests
    {
        [TestMethod]
        public void CreateHeroTest()
        {
            var heroTemplate = new HeroTemplate()
            {
                StartingStatPointsEachStat = new RangeInt(4, 10),
                TotalSkillPoints = 20,
                TotalDailyQuests = 20,
                ImageBaseUrl = "/img/api/heroes/",
                MaxHitPoints = 20,
            };
            var heroNames = new List<HeroName>()
            {
                new HeroName("Test Skywalker"),
                new HeroName("Test Goku"),
                new HeroName("Test Thor"),
                new HeroName("Test Vageta")
            };
            var heroLevel = new HeroLevel(1);
            var heroGenerator = new HeroGenerator(heroTemplate, heroNames);
            var newHero = heroGenerator.CreateHero(0, heroLevel);
            Assert.IsNotNull(newHero);
            Assert.AreEqual(heroLevel, newHero.Level);
            Assert.IsTrue(heroNames.Contains(newHero.HeroName));
            Assert.IsTrue(newHero.CombatStats.MaxAgility > 0);
            Assert.IsTrue(newHero.CombatStats.MaxStrength > 0);
            Assert.IsTrue(newHero.CombatStats.MaxWisdom > 0);
            Assert.IsTrue(newHero.CombatStats.HitPoints > 0);
        }
    }
}