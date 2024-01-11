using DragonsCrossing.Core.Contracts.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.IntegrationTests
{
    [TestClass]
    public class CombatsServiceTests : ServiceTestsBase<ICombatsService>
    {
        //[TestMethod]
        //public async Task StartCombatTest()
        //{
        //    var tileId = 1;
        //    var heroId = 16720;
        //    var combat = await coreService.StartCombat(heroId, tileId);

        //    Assert.IsNotNull(combat);
        //}

        //[TestMethod]
        //[TestCategory("Nightly")]
        //public async Task AttackTest()
        //{
        //    var heroId = 16720;
        //    var combat = await coreService.Attack(heroId);
        //    Assert.IsNotNull(combat);
        //}
    }
}
