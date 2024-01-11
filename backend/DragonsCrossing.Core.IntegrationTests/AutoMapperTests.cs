using AutoMapper;
using DragonsCrossing.Core.Mappings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.IntegrationTests
{
    [TestClass]
    public class AutoMapperTests
    {
        [TestMethod]
        public void ConfigTest()
        {
//            var config = AutoMapperConfiguration.Configure();

//config.AssertConfigurationIsValid();
           
        }

        [TestMethod]
        public void AllProfilesTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ZoneProfile>();
                cfg.AddProfile<HeroProfile>();
                cfg.AddProfile<MonsterProfile>();
                cfg.AddProfile<CombatProfile>();
            });
            config.AssertConfigurationIsValid();            
        }        
    }
}
