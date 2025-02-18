using CryptoExchangeApi.Models;
using CryptoExchangeApi.Services;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using Serilog;
using System.Text.Json;

var bld = WebApplication.CreateBuilder(args);

// Setting use Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(bld.Configuration)
    .CreateLogger();

bld.Host.UseSerilog();

// Setting DbContext
if (bld.Environment.IsEnvironment("Testing")) {
    bld.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase(Guid.NewGuid().ToString()));  // 測試時使用 InMemory
} else {
    bld.Services.AddDbContext<AppDbContext>(options =>
     options.UseSqlServer(bld.Configuration.GetConnectionString("DefaultConnection")));
}


// Add services to the container.
bld.Services.AddFastEndpoints(opt => {
    opt.IncludeAbstractValidators = true;
}).SwaggerDocument(opt => {
    opt.DocumentSettings = s => {
        s.SchemaSettings.SchemaType = SchemaType.OpenApi3;
        s.SchemaSettings.GenerateEnumMappingDescription = true;
        s.SchemaSettings.GenerateKnownTypes = true;
    };

}); //define a swagger document


var coinDeskApiUrl = bld.Configuration.GetValue<string>("CoinDeskApi:BaseUrl");
var coinCapApiUrl = bld.Configuration.GetValue<string>("CoinCapApi:BaseUrl");

// CoinDesk API HttpClient
bld.Services.AddHttpClient("CoinDeskApiClient", client => {
    client.BaseAddress = new Uri(coinDeskApiUrl!);
}).AddHttpMessageHandler<LoggingHttpHandler>();

//CoinCap API HttpClient
bld.Services.AddHttpClient("CoinCapApiClient", client => {
    client.BaseAddress = new Uri(coinCapApiUrl!);
}).AddHttpMessageHandler<LoggingHttpHandler>();

bld.Services.AddTransient<LoggingHttpHandler>();

// Importer
bld.Services.AddSingleton<ICurrencyDataImporter, CurrencyDataImporter>();
bld.Services.AddScoped<ICurrencyDatabaseService, CurrencyDatabaseService>();

var app = bld.Build();

// Internal API
app.UseMiddleware<RequestResponseLoggingMiddleware>(); 

app.UseDefaultExceptionHandler() //攔截所有未處理的錯誤
   .UseFastEndpoints(cfg => {
    cfg.Endpoints.RoutePrefix = "api"; // Set API route prefix (optional)
    cfg.Serializer.Options.PropertyNameCaseInsensitive = true;
    cfg.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    // Global configuration for all endpoints
    cfg.Endpoints.Configurator = ep => {
        ep.AllowAnonymous(); // Allow all requests to be anonymous for development purposes
    };

}).UseSwaggerGen(); // Enable Swagger Files

app.Run();

