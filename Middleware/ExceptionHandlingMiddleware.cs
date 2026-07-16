using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoutineFlow.Common.Exceptions;

namespace RoutineFlow.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IProblemDetailsService problemDetailsService)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (statusCode, title) = ex switch
            {
                NotFoundException => (HttpStatusCode.NotFound, "Not Found"),
                ConflictException => (HttpStatusCode.Conflict, "Conflict"),
                UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized"),
                ArgumentException => (HttpStatusCode.BadRequest, "Bad Request"),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(ex, "Unhandled exception");
            }

            context.Response.StatusCode = (int)statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = statusCode == HttpStatusCode.InternalServerError ? null : ex.Message,
                Type = $"https://tools.ietf.org/html/rfc9110#section-{StatusCodeSection(statusCode)}",
                Instance = context.Request.Path
            };

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = problemDetails
            });
        }
    }

    private static string StatusCodeSection(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest => "15.5.1",
        HttpStatusCode.Unauthorized => "15.5.2",
        HttpStatusCode.NotFound => "15.5.5",
        HttpStatusCode.Conflict => "15.5.10",
        _ => "15.6.1"
    };
}
