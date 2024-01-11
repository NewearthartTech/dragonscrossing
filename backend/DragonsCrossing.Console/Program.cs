// See https://aka.ms/new-console-template for more information

using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

IServiceCollection services = new ServiceCollection();
IConfigurationBuilder builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true);
var configuration = builder.Build();
services.AddInfrastructure(configuration);
services.AddApplicationCore();

var heroesService = services.BuildServiceProvider().GetService<IHeroesService>();

//var heroes = await heroesService.GetHeroes();
//HeroDto newHero = null;
//for (int i = 0; i < 4050; i++)
//{
//    newHero = await heroesService.CreateHero(0);
//    Console.WriteLine(newHero.Name);
//}


Console.WriteLine("Finished!");
Console.ReadKey();
//Console.WriteLine(JsonSerializer.Serialize(newHero));

// dependency injection
//builder.Services.AddInfrastructure(builder.Configuration);
//builder.Services.AddApplicationCore();

