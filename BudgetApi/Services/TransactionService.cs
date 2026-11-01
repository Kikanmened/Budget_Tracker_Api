using System.Collections.Concurrent;
using BudgetApi.Dtos.Transactions;
using BudgetApi.Models;

namespace BudgetApi.Services;

public sealed class TransactionService : ITransactionService
{
    private readonly ConcurrentDictionary<Guid, Transaction> _transactions = new();

    public Task<IReadOnlyList<TransactionResponseDto>> GetAllAsync()
    {
        IReadOnlyList<TransactionResponseDto> transactions = _transactions.Values
            .OrderByDescending(transaction => transaction.Timestamp)
            .Select(ToResponseDto)
            .ToList();

        return Task.FromResult(transactions);
    }

    public Task<IReadOnlyList<TransactionResponseDto>> GetInRangeAsync(
        DateOnly start,
        DateOnly end)
    {
        IReadOnlyList<TransactionResponseDto> transactions = _transactions.Values
            .Where(transaction =>
            {
                var transactionDate = DateOnly.FromDateTime(
                    transaction.Timestamp.DateTime);

                return transactionDate >= start &&
                       transactionDate <= end;
            })
            .OrderByDescending(transaction => transaction.Timestamp)
            .Select(ToResponseDto)
            .ToList();

        return Task.FromResult(transactions);
    }

    public Task<TransactionResponseDto?> GetByIdAsync(Guid id)
    {
        TransactionResponseDto? transaction =
            _transactions.TryGetValue(id, out var storedTransaction)
                ? ToResponseDto(storedTransaction)
                : null;

        return Task.FromResult(transaction);
    }

    public Task<TransactionResponseDto> CreateAsync(
        CreateTransactionDto dto)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Timestamp = dto.Timestamp ?? DateTimeOffset.Now,
            Type = dto.Type!.Value,
            Description = dto.Description.Trim(),
            Amount = dto.Amount
        };

        _transactions[transaction.Id] = transaction;

        return Task.FromResult(ToResponseDto(transaction));
    }

    public Task<TransactionResponseDto?> UpdateAsync(
        Guid id,
        UpdateTransactionDto dto)
    {
        while (_transactions.TryGetValue(id, out var existingTransaction))
        {
            var updatedTransaction = new Transaction
            {
                Id = id,
                Timestamp = dto.Timestamp!.Value,
                Type = dto.Type!.Value,
                Description = dto.Description.Trim(),
                Amount = dto.Amount
            };

            if (_transactions.TryUpdate(
                id,
                updatedTransaction,
                existingTransaction))
            {
                return Task.FromResult<TransactionResponseDto?>(
                    ToResponseDto(updatedTransaction));
            }
        }

        return Task.FromResult<TransactionResponseDto?>(null);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var deleted = _transactions.TryRemove(id, out _);

        return Task.FromResult(deleted);
    }

    private static TransactionResponseDto ToResponseDto(
        Transaction transaction)
    {
        return new TransactionResponseDto(
            transaction.Id,
            transaction.Timestamp,
            transaction.Type,
            transaction.Description,
            transaction.Amount);
    }
}