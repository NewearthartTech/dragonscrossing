using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace DragonsCrossing.Api;

public static class UserIdExtensions
{
    public static string? GetUserId(this ControllerBase controller)
    {
        return GetUserId(controller.HttpContext);
    }

    public static int GetHeroId(this ControllerBase controller, bool throwIfNull = true)
    {
        return GetHeroId(controller.HttpContext, throwIfNull);
    }

    public static int GetSeasonId(this ControllerBase controller, bool throwIfNull = true)
    {
        return GetSeasonId(controller.HttpContext, throwIfNull);
    }


    public static string? GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        if (null == httpContextAccessor.HttpContext)
            throw new Exception("HttpContext is null");

        return GetUserId(httpContextAccessor.HttpContext);
    }

    public static int GetHeroId(this IHttpContextAccessor httpContextAccessor, bool throwIfNull = true)
    {
        if (null == httpContextAccessor.HttpContext)
            throw new Exception("HttpContext is null");

        return GetHeroId(httpContextAccessor.HttpContext, throwIfNull);
    }

    public static string? GetUserId(this HttpContext httpContext)
    {
        return GetUserId(httpContext?.User);
    }

    public static int GetHeroId(this HttpContext httpContext, bool throwIfNull = true)
    {
        return GetHeroId(httpContext?.User, throwIfNull);
    }

    public static int GetSeasonId(this HttpContext httpContext, bool throwIfNull = true)
    {
        return GetSeasonId(httpContext?.User, throwIfNull);
    }


    public static int GetSeasonId(this ClaimsPrincipal? user, bool throwIfNull = true)
    {
        if (!(user?.Identity?.IsAuthenticated ?? false))
        {
            if(throwIfNull)
                throw new Exception("Not Autneticated");

            return 0;
        }


        /* We idenify NOT by name
        if (!string.IsNullOrWhiteSpace(user?.Identity?.Name))
            return user.Identity.Name;
        */

        var selectedSeasonIdStr = user.FindFirst(Controllers.AuthController.ClaimSelectedSeasonId)?.Value;

        if (string.IsNullOrWhiteSpace(selectedSeasonIdStr))
        {
            if (throwIfNull)
                throw new Exception("SeasonId claim not found");

            return 0;
        }

        
        if (int.TryParse(selectedSeasonIdStr, out var seasonId))
        {
            return seasonId;
        }
        else
        {
            throw new Exception($"season claim {selectedSeasonIdStr} is not a number");
        }

    }

    public static int GetHeroId(this ClaimsPrincipal? user, bool throwIfNull = true)
    {
        if (!(user?.Identity?.IsAuthenticated ?? false))
        {
            if (throwIfNull)
                throw new Exception("hero claim not found");

            return 0;
        }
        

        /* We idenify NOT by name
        if (!string.IsNullOrWhiteSpace(user?.Identity?.Name))
            return user.Identity.Name;
        */

        var heroIdStr = user.FindFirst(Controllers.AuthController.ClaimSelectedHeroId)?.Value;

        if (string.IsNullOrWhiteSpace(heroIdStr))
        {
            if (throwIfNull)
                throw new Exception("hero claim not found");

            return 0;
        }

        int heroId;
        if(int.TryParse(heroIdStr,out heroId))
        {
            return heroId;
        }
        else
        {
            throw new Exception($"hero claim {heroIdStr} is not a number");
        }

    }

    public static string? GetUserId(this ClaimsPrincipal? user)
    {
        if (!(user?.Identity?.IsAuthenticated ?? false))
            return null;

        /* We idenify NOT by name
        if (!string.IsNullOrWhiteSpace(user?.Identity?.Name))
            return user.Identity.Name;
        */

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return null;


        return userId;


        //return user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }
}
