//using DragonsCrossing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace DragonsCrossing.Core.IntegrationTests
{
    public class ServiceTestsBase<T> where T : class
    {
        protected T coreService;
        public ServiceTestsBase()
        {
            IServiceCollection services = new ServiceCollection();
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            var configuration = builder.Build();
            //services.AddInfrastructure(configuration);
            services.AddApplicationCore();

            coreService = services.BuildServiceProvider().GetService<T>();
        }
    }
}