using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Items;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.IntegrationTests
{
    [TestClass]
    public class HeroesServiceTests : ServiceTestsBase<IHeroesService>
    {
        //[TestMethod]
        //[Ignore]
        //public async Task CreateHeroTest()
        //{
        //    // see https://stackoverflow.com/questions/56384128/how-to-inject-dbcontext-into-repository-constructor-in-asp-net-core
        //    // see https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-6.0

        //    // act
        //    var hero = await coreService.CreateHero(0);

        //    // assert
        //    Assert.IsNotNull(hero);
        //}

        //[TestMethod]
        //public async Task EquipWeaponTest()
        //{
        //    // create and equip the default weapon
        //    //var hero = new Hero()
        //    //var itemGenerator = new ItemGenerator(hero);
        //    //var weaponTemplate = await weaponsRepository.GetWeaponTemplate(newHero.HeroClass, true);
        //    //var newWeapon = itemGenerator.CreateNewWeapon(weaponTemplate);
        //    //await weaponsRepository.CreateWeapon(newWeapon);
        //    //itemGenerator.EquipWeapon(newWeapon, weaponTemplate.WeaponSlotType);
        //}
    }
}