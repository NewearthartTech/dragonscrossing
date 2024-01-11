using System;
using Serilog.Core;
using Serilog.Events;

namespace DragonsCrossing.Api.Utilities;

public class HeroIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HeroIdEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return;
        }

        var heroId = httpContext.GetHeroId(false);

        if(0 != heroId)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("heroId", heroId));
        }

    }
}
