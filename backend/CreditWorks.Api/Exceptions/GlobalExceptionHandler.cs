using CreditWorks.Application.Exceptions;
using CreditWorks.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CreditWorks.Api.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            DomainValidationException =>
                StatusCodes.Status400BadRequest,

            ResourceNotFoundException =>
                StatusCodes.Status404NotFound,

            _ => StatusCodes.Status500InternalServerError
        };

        _logger.Log(
            statusCode == 500 ? LogLevel.Error : LogLevel.Warning,
            exception,
            "Request failed with status {StatusCode}",
            statusCode);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode switch
            {
                400 => "Validation error",
                404 => "Resource not found",
                _ => "Internal server error"
            },
            Detail = statusCode == 500
                ? "An unexpected error occurred."
                : exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            cancellationToken);

        return true;
    }
}