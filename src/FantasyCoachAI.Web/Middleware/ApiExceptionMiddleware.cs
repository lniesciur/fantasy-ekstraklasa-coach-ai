using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using FantasyCoachAI.Domain.Exceptions;

namespace FantasyCoachAI.Web.Middleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the API request.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            message = "An error occurred while processing your request.",
            details = (string?)null
        };

        switch (exception)
        {
            case ArgumentException argEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new { message = argEx.Message, details = (string?)null };
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = new { message = "Unauthorized access.", details = (string?)null };
                break;
            case NotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new { message = "Resource not found.", details = (string?)null };
                break;
            case InvalidOperationException opEx:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = new { message = opEx.Message, details = (string?)null };
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new { message = "Internal server error", details = exception.Message };
                break;
        }

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}
