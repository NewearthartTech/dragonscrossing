using System;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Common;
using DragonsCrossing.NewCombatLogic;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;

namespace DragonsCrossing.Api.Utilities;

public class ActionCheckerMiddleware
{
    private readonly RequestDelegate _next;
    public ActionCheckerMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    readonly static string[] _AllowedInCombat = new[]
    {
        "/api/combats",
        "/api/noncombatencounter",
        "/api/season/openseasons",
        "/api/web3/gameconfig",
        "/api/heroes/mine",
        "/api/web3/ownedheros",
        "/api/players/me",
        "/api/heroes/selectedhero",
        "/api/gamestates",
        "/api/web3/owneditems",
        "/api/items/fromnfts",
        "/api/gamestates/updatelocation",
        "/api/items/pickuploot",
        "/api/season/leaderboard",
        "/api/auth",
    };


    public async Task Invoke(HttpContext context,
        ILogger<ActionCheckerMiddleware> _logger,
        IHttpContextAccessor _httpContextAccessor,
        ISeasonsDbService _seasonsDb,
        IDistributedCache _cache
        )
    {
        var lockAcquired = false;
        var heroLock = String.Empty;
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var heroId = httpContext?.GetHeroId(false) ?? 0;

            if (0 == heroId)
            {
                _logger.LogTrace("Called without HeroId");
            }
            else
            {

                if (_seasonsDb.isAvailable)
                {
                    //"/api/Combats/StartRound"
                    var thePath = context.Request.Path.ToString().ToLower();

                    var inAllowedInCombat = _AllowedInCombat.Where(a => thePath.StartsWith(a)).FirstOrDefault();

                    if (null == inAllowedInCombat)
                    {
                        var CurrentEncounters = await _seasonsDb.getCollection<DbGameState>()
                                            .Find(c => c.HeroId == heroId)
                                            .Project(c=>c.CurrentEncounters)
                                            .SingleOrDefaultAsync();

                        if(null != CurrentEncounters && CurrentEncounters.Length > 0)
                        {
                            throw new ExceptionWithCode($"Not allowed during encounter {thePath}");
                        }
                    }

                }


                heroLock = $"heroLock_{heroId}";

#if DEBUG
                if (_resetLockONStartup)
                {
                    await _cache.RemoveAsync(heroLock);
                    _resetLockONStartup = false;
                }
#endif

                for (var i = 0;i<=100;i++)
                {
                    var gotLock = await _cache.GetStringAsync(heroLock);
                    if (string.IsNullOrWhiteSpace(gotLock))
                    {
                        break;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));

                    if(i >= 99)
                    {
                        throw new Exception("Failed to acquire HeroLock");
                    }
                }

                lockAcquired = true;
                await _cache.SetStringAsync(heroLock, DateTime.Now.ToString());

            }

            await _next(context);

        }
        finally{

            if (lockAcquired)
            {
                if (string.IsNullOrWhiteSpace(heroLock))
                {
                    _logger.LogCritical("found empty herLock String");
                }

                await _cache.RemoveAsync(heroLock);
            }

        }

        
    }

#if DEBUG
    static bool _resetLockONStartup = true;
#endif

}

