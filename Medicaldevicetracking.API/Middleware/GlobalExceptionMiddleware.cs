using System.Net;
using System.Text.Json;
using MedicalDeviceTracking.Application.DTOs;

namespace MedicalDeviceTracking.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Medical Device Tracking: An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ArgumentNullException => ApiResponse.ErrorResponse("Medical Device Tracking: Bad request", (int)HttpStatusCode.BadRequest),
            ArgumentException => ApiResponse.ErrorResponse("Medical Device Tracking: Bad request", (int)HttpStatusCode.BadRequest),
            UnauthorizedAccessException => ApiResponse.ErrorResponse("Medical Device Tracking: Unauthorized", (int)HttpStatusCode.Unauthorized),
            KeyNotFoundException => ApiResponse.ErrorResponse("Medical Device Tracking: Not found", (int)HttpStatusCode.NotFound),
            _ => ApiResponse.ErrorResponse("Medical Device Tracking: Internal server error", (int)HttpStatusCode.InternalServerError)
        };

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
