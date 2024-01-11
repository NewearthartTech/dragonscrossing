using System;
using DragonsCrossing.Core.Helper;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DragonsCrossing.Core.Sagas;

public class DailyResetConsumer : IConsumer<ResetHeroIfNeededMessage>
{
    readonly ILogger _logger;
    readonly IGameStateService _gameStateService;

    public DailyResetConsumer(IGameStateService gameStateService,
        ILogger<DailyResetConsumer> logger)
    {
        _gameStateService = gameStateService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ResetHeroIfNeededMessage> context)
    {
        if(null != context.Message.runAfter && context.Message.runAfter > DateTime.Now)
        {
            var deferBy = context.Message.runAfter.Value - DateTime.Now;

            _logger.LogDebug($"defer  DailyResetConsumer for hero {context.Message.heroId} by {deferBy}");

            await context.Defer(deferBy);
            return;
        }


        _logger.LogDebug($"DailyResetConsumer for hero {context.Message.heroId}");

        await _gameStateService.ResetQuestsIfNeeded(context.Message.heroId);
    }
}

/// <summary>
/// this message triggers when we should check for owner updates
/// </summary>
public class ResetHeroIfNeededMessage
{
    /// <summary>
    /// The hero that might need reset
    /// </summary>
    public int heroId { get; set; }

    /// <summary>
    /// if set we consume this after the DateTime
    /// </summary>
    public DateTime? runAfter { get; set; }

}

