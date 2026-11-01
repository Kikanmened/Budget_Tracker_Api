using System.Text.Json.Serialization;
using BudgetApi.Endpoints;
using BudgetApi.Services;
using BudgetApi.Errors;

var builder = WebApplication.CreateBuilder(args);

// VALIDATION AND PROBLEM DETAILS
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// STRING <=> ENUM CONVERTER
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(allowIntegerValues: false));
});

// TransactionService holds the in-memory data, so it must be a singleton.
builder.Services.AddSingleton<ITransactionService, TransactionService>();

// ReportService is stateless and depends on the singleton transaction service.
builder.Services.AddSingleton<IReportService, ReportService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapGet("/", () => "Hello World!");
app.MapTransactionEndpoints();
app.MapReportEndpoints();

app.Run();
