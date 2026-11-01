using BudgetApi.Dtos.Transactions;
using BudgetApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BudgetApi.Endpoints;

public static class TransactionEndpoints
{
    public static IEndpointRouteBuilder MapTransactionEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions")
            .WithTags("Transactions");

        group.MapGet("/", GetAllAsync)
            .WithName("GetTransactions")
            .WithSummary("Get all transactions, newest first");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetTransactionById")
            .WithSummary("Get a transaction by ID");

        group.MapPost("/", CreateAsync)
            .WithName("CreateTransaction")
            .WithSummary("Create a transaction")
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateAsync)
            .WithName("UpdateTransaction")
            .WithSummary("Replace a transaction")
            .ProducesValidationProblem();

        group.MapDelete("/{id:guid}", DeleteAsync)
            .WithName("DeleteTransaction")
            .WithSummary("Delete a transaction");

        return app;
    }

    private static async Task<
        Ok<IReadOnlyList<TransactionResponseDto>>>
        GetAllAsync(ITransactionService service)
    {
        var transactions = await service.GetAllAsync();

        return TypedResults.Ok(transactions);
    }

    private static async Task<
        Results<Ok<TransactionResponseDto>, NotFound>>
        GetByIdAsync(
            Guid id,
            ITransactionService service)
    {
        var transaction = await service.GetByIdAsync(id);

        if (transaction is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(transaction);
    }

    private static async Task<
        Results<Created<TransactionResponseDto>, ValidationProblem>>
        CreateAsync(
            CreateTransactionDto dto,
            ITransactionService service)
    {
        var transaction = await service.CreateAsync(dto);

        return TypedResults.Created(
            $"/api/transactions/{transaction.Id}",
            transaction);
    }

    private static async Task<
        Results<Ok<TransactionResponseDto>, ValidationProblem, NotFound>>
        UpdateAsync(
            Guid id,
            UpdateTransactionDto dto,
            ITransactionService service)
    {
        var transaction = await service.UpdateAsync(id, dto);

        if (transaction is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(transaction);
    }

    private static async Task<Results<NoContent, NotFound>>
        DeleteAsync(
            Guid id,
            ITransactionService service)
    {
        var deleted = await service.DeleteAsync(id);

        if (!deleted)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }
}
