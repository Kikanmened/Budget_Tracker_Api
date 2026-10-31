using System.ComponentModel.DataAnnotations;
using BudgetApi.Models;

namespace BudgetApi.Dtos.Transactions;

public record CreateTransactionDto(
    [property: Required, EnumDataType(typeof(TransactionType))]
    TransactionType? Type,

    [property: Required, StringLength(200, MinimumLength = 1)]
    string Description,

    [property: Range(
        typeof(decimal),
        "0.01",
        "1000000",
        ParseLimitsInInvariantCulture = true)]
    decimal Amount,

    DateTimeOffset? Timestamp = null
);