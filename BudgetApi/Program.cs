using System.Text.Json.Serialization;
using BudgetApi.Endpoints;
using BudgetApi.Services;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/", () => "Hello World!");
app.MapTransactionEndpoints();

app.Run();
