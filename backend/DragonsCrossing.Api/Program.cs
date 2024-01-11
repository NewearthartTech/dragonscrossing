using DragonsCrossing.Core;
using DragonsCrossing.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DragonsCrossing.Api.Utilities;
using DragonsCrossing.Core.Contracts.Api;
using System.Reflection;
using DragonsCrossing.NewCombatLogic;
using DragonsCrossing.Core.Services;
using Newtonsoft.Json;
using DragonsCrossing.Core.Contracts.Api.Dto.GameStates;
using DragonsCrossing.Api.Controllers;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Helper;
using Serilog.Sinks.Elasticsearch;
using Serilog;
using DragonsCrossing.Api;
using DragonsCrossing.Core.Sagas;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(ContractDeployer.ContractsSettingFile, true, false);

var appConfig = builder.Configuration.GetSection("app").Get<AppConfig>() ?? new AppConfig();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


var redisConfig = builder.Configuration.GetSection("redis").Get<RedisConfig>() ?? new RedisConfig();

Log.Logger = new LoggerConfiguration()
        .CreateLogger();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<HeroIdEnricher>();


builder.Services.AddControllers().AddJsonOptions(opts =>
{
    // convert enum values from int to string
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    //opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.DocumentFilter<DragonsCrossing.Api.CustomDocFilter>();
    c.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v2",
        Title = "cb-server",
    });
});

if (appConfig?.AllowCors??false)
{
    builder.Services.AddCors();
}



// dependency injection
builder.Services
    .AddTransient<IDiceService, DiceService>()
    .AddTransient<ICombatZoneService, CombatZoneService>()
    .AddTransient<IPerpetualDbService, PerpetualDbService>()
    .AddTransient<IDataHelperService, DataHelperService>()
    .AddTransient<ICombatEngine, CombatEngine>()
    .AddTransient<IHeroesService, HeroesService>()
    .AddTransient<IBlockchainService, BlockchainService>()
    .AddTransient<IGameStateService, GameStateService>()
    .AddTransient<NonCombatHelpers>()
    .AddTransient<IUpdateNFTOwnerService, UpdateNFTOwnerService>()

    .AddScoped<ISeasonsDbService>(sp =>
    {
        var httpAccessor = sp.GetRequiredService<IHttpContextAccessor>();
        var seasonId = httpAccessor?.HttpContext?.User.GetSeasonId(throwIfNull:false);

        if(null == seasonId ||  0 == seasonId)
        {
            return ActivatorUtilities.CreateInstance<NoSeasonsAvailable>(sp);
        }

        return ActivatorUtilities.CreateInstance<SeasonsDbService>(sp, seasonId!);
    })

    .ConfigureDcxSagas(builder.Configuration);

//builder.Services.AddInfrastructure(builder.Configuration);
//builder.Services.AddApplicationCore();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var loginConfig = DragonsCrossing.Api.Controllers.AuthConfig.CreateFromConfig(builder.Configuration);

    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = loginConfig.JwtIssuer,
        ValidAudience = loginConfig.JwtIssuer,
        ClockSkew = TimeSpan.FromSeconds(5),


        //https://www.allkeysgenerator.com/Random/Security-Encryption-Key-Generator.aspx
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(loginConfig.PrivateKey)),

    };

    //presence.NameUserIdProvider.ParseJWTinQueryString(options, "/messaging");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HeroSelected", policy => policy.RequireClaim(AuthController.ClaimSelectedHeroId));
    options.AddPolicy("SeasonJoined", policy => policy.RequireClaim(AuthController.ClaimSelectedSeasonId));
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    //options.InstanceName = "dcx-cache";
    options.Configuration = redisConfig.connectionString;
});

builder.Host.UseSerilog((_, sp,cng) =>
{
    var enricher = sp.GetRequiredService<HeroIdEnricher>();

    cng
        .Enrich.With(enricher)
        .Enrich.FromLogContext()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticConfiguration:Uri"]))
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM-dd}"
        })
        .ReadFrom.Configuration(builder.Configuration)
        ;

});


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "dcx-server v3");
});

//if (appConfig?.AllowCors ?? false)
{
    app.UseCors(c =>
    {
        c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
}

//Dee The middleware order is important don't switch them around
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ActionCheckerMiddleware>();



app.MapControllers();



/* We are using docker container to run our app. It's good to keep things containerized, with not much external dependencies.
 * It's better for apps to migrate there own db, instead of expecting an external utility to keep code and db in sync)
 
if (appConfig?.MigrateDb ?? false)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<DragonsCrossing.Infrastructure.Persistence.ApplicationDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }

}
*/

#region AppCommands

if (!string.IsNullOrWhiteSpace(appConfig?.Command))
{
    switch (appConfig.Command.ToLower())
    {
        case "showinfo":
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            var blockchainService = builder.Services.BuildServiceProvider().GetService<IBlockchainService>();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

            var web3Info = blockchainService!.Web3Info();


            Console.WriteLine($"web3 info: \n {JsonConvert.SerializeObject(web3Info, Formatting.Indented)}");


            break;
        case "deploycontracts":
            builder.EnsureContractsAreDeployed(appConfig.Command);
            break;

        case "resetHero":
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            var gameStateService = builder.Services.BuildServiceProvider().GetService<IGameStateService>();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

            if (0 == appConfig.HeroId)
                throw new Exception("/app:HeroId is required");

            gameStateService!.ResetQuestsIfNeeded(appConfig.HeroId).Wait();

            Console.WriteLine("reset Quest done");

            break;

    }

}
else
{
    builder.EnsureContractsAreDeployed();
    app.Run();
}

public class RedisConfig
{
    public string connectionString { get; set; } = "localhost:6379";
}


/// <summary>
/// config options for the whole app
/// </summary>
public class AppConfig
{
    public bool AllowCors { get; set; }
    public bool MigrateDb { get; set; }

    public string? Command { get; set; }

    /// <summary>
    /// some command need an Account
    /// </summary>
    public string? Account { get; set; }

    /// <summary>
    /// Some commands need text file
    /// </summary>
    public string? file { get; set; }

    /// <summary>
    /// Used for maintenance operations on Hero
    /// </summary>
    public int HeroId { get; set; }
}

#endregion


