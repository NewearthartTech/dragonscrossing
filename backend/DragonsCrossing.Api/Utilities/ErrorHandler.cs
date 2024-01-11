using System;
using System.Net;
using DragonsCrossing.Core.Common;
using Newtonsoft.Json;

namespace DragonsCrossing.Api.Utilities;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ILogger<ErrorHandlerMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (ExceptionWithCode error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)error.Status;
            var message = error.Message;
            var errorId = Guid.NewGuid();
            logger.LogError($"errorId: {errorId} - " + error.ToString());
            var result = JsonConvert.SerializeObject(new { errorId, message });

            await response.WriteAsync(result);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Internal error";

            var errorId = Guid.NewGuid();
            logger.LogError($"errorId: {errorId} - " + error.ToString());
            var result = JsonConvert.SerializeObject(new { errorId, message });

            await response.WriteAsync(result);
        }
    }
}
