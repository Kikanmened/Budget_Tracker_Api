using BudgetApi.Dtos.Reports;
using BudgetApi.Models;

namespace BudgetApi.Services;

public sealed class ReportService : IReportService
{
    private readonly ITransactionService _transactionService;

    public ReportService(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    public async Task<SummaryReportResponseDto> GetSummaryAsync(
        DateOnly start,
        DateOnly end,
        TransactionType? type)
    {
        var transactions = await _transactionService.GetInRangeAsync(
            start,
            end);

        var filteredTransactions = transactions
            .Where(transaction =>
                !type.HasValue || transaction.Type == type.Value)
            .ToList();

        var totalIncome = filteredTransactions
            .Where(transaction =>
                transaction.Type == TransactionType.Income)
            .Sum(transaction => transaction.Amount);

        var totalExpense = filteredTransactions
            .Where(transaction =>
                transaction.Type == TransactionType.Expense)
            .Sum(transaction => transaction.Amount);

        return new SummaryReportResponseDto(
            start,
            end,
            type,
            totalIncome,
            totalExpense,
            totalIncome - totalExpense,
            filteredTransactions);
    }
}