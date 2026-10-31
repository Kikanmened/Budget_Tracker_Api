using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// STRING <=> ENUM CONVERTER
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(allowIntegerValues: false));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
