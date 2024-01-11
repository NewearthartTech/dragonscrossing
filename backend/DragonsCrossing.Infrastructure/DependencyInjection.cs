using DragonsCrossing.Core.Contracts.Infrastructure;
using DragonsCrossing.Infrastructure.Persistence;
using DragonsCrossing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // TODO: Change this to use app secrets
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DragonsCrossingDatabase"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)), ServiceLifetime.Scoped);

            //services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            services.AddScoped<IHeroesRepository, HeroesRepository>();
            services.AddScoped<ICombatsRepository, CombatsRepository>();
            services.AddScoped<IGameStatesRepository, GameStatesRepository>();
            services.AddScoped<IMonstersRepository, MonstersRepository>();
            services.AddScoped<ITransactionsRepository, TransactionsRepository>();
            services.AddScoped<IZonesRepository, ZonesRepository>();
            services.AddScoped<IPlayersRepository, PlayersRepository>();
            services.AddScoped<IBlockchainRepository, BlockchainRepository>();
            services.AddScoped<IHeroInventoriesRepository, HeroInventoriesRepository>();

            return services;
        }
    }
}
