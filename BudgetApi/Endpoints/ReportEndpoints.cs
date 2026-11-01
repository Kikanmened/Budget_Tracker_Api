using BudgetApi.Dtos.Reports;
using BudgetApi.Models;
using BudgetApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BudgetApi.Endpoints;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports")
            .WithTags("Reports");

        group.MapGet("/summary", GetSummaryAsync)
            .WithName("GetSummaryReport")
            .WithSummary("Get a summary report for a date range")
            .ProducesValidationProblem();

        return app;
    }

    private static async Task<
        Results<Ok<SummaryReportResponseDto>, ValidationProblem>>
        GetSummaryAsync(
            DateOnly start,
            DateOnly end,
            TransactionType? type,
            IReportService service)
    {
        if (start > end)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["end"] =
                    [
                        "End date must be on or after start date."
                    ]
                });
        }

        var report = await service.GetSummaryAsync(
            start,
            end,
            type);

        return TypedResults.Ok(report);
    }
}