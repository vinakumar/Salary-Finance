using System.Net;
using System.Text.Json;
using FullStack.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Middleware;

/// <summary>
/// Global exception handling middleware that maps domain exceptions to RFC 7807 ProblemDetails responses.
/// </summary>
public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        var (statusCode, problemDetails) = exception switch
        {
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = notFound.Message,
                    Status = (int)HttpStatusCode.NotFound,
                    Instance = context.Request.Path
                }),

            ConflictException conflict => (
                HttpStatusCode.Conflict,
                new ProblemDetails
                {
                    Title = "Conflict",
                    Detail = conflict.Message,
                    Status = (int)HttpStatusCode.Conflict,
                    Instance = context.Request.Path
                }),

            ValidationException validation => (
                HttpStatusCode.UnprocessableEntity,
                CreateValidationProblemDetails(validation, context.Request.Path)),

            UnauthorizedAccessException unauthorized => (
                HttpStatusCode.Unauthorized,
                new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = unauthorized.Message,
                    Status = (int)HttpStatusCode.Unauthorized,
                    Instance = context.Request.Path
                }),

            _ => (
                HttpStatusCode.InternalServerError,
                new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred.",
                    Status = (int)HttpStatusCode.InternalServerError,
                    Instance = context.Request.Path
                })
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", traceId);
        }
        else
        {
            logger.LogWarning("Handled domain exception: {ExceptionType}. TraceId: {TraceId}",
                exception.GetType().Name, traceId);
        }

        problemDetails.Extensions["traceId"] = traceId;

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }

    private static ProblemDetails CreateValidationProblemDetails(ValidationException ex, string instance)
    {
        var problemDetails = new ProblemDetails
        {
            Title = "Validation Error",
            Detail = ex.Message,
            Status = (int)HttpStatusCode.UnprocessableEntity,
            Instance = instance
        };

        problemDetails.Extensions["errors"] = ex.Errors;

        return problemDetails;
    }
}
