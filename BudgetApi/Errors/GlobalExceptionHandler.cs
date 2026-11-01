using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace BudgetApi.Errors;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception is BadHttpRequestException badRequest
            ? badRequest.StatusCode
            : StatusCodes.Status500InternalServerError;

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred.");
        }

        httpContext.Response.StatusCode = statusCode;

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = ReasonPhrases.GetReasonPhrase(statusCode),
                    Detail = exception is BadHttpRequestException
                        ? exception.Message
                        : "An unexpected error occurred."
                }
            });
    }
}