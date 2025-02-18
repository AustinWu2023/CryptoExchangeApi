using CryptoExchangeApi.Models;
using CryptoExchangeApi.Services;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using Serilog;
using System.Diagnostics;
using System.Text.Json;

var bld = WebApplication.CreateBuilder(args);

bld.WebHost.UseUrls("http://0.0.0.0:8080");

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

//初始化DB
Log.Information("DB init start !");
using (var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    var connectionString = bld.Configuration.GetConnectionString("DefaultConnection");
    var sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "SqlScripts", "init.sql");
    Log.Information($"connectionString : {connectionString},  sqlFilePath : {sqlFilePath}.");
    if (File.Exists(sqlFilePath)) {
        try {
            using (var connection = new SqlConnection(connectionString)) {
                connection.Open();
                var command = new SqlCommand(File.ReadAllText(sqlFilePath), connection);
                command.ExecuteNonQuery();
                Log.Information("SQL init end");
            }
        } catch (Exception ex) {
            Log.Error("DB init Failed!!" + ex.Message);
        }       
    }
}

Log.Information("DB init end !");

//var providerName = "Microsoft.EntityFrameworkCore.SqlServer";
//var outputDir = "Models";

//try {
//    Log.Information("Start EF Core Scaffold-DbContext...");
//    var process = new Process {
//        StartInfo = new ProcessStartInfo {
//            FileName = "dotnet",
//            Arguments = $"ef dbcontext scaffold \"{bld.Configuration.GetConnectionString("DefaultConnection")}\" {providerName} -o {outputDir} --force",
//            RedirectStandardOutput = true,
//            RedirectStandardError = true,
//            UseShellExecute = false,
//            CreateNoWindow = true
//        }
//    };

//    process.Start();
//    string result = process.StandardOutput.ReadToEnd();
//    string error = process.StandardError.ReadToEnd();
//    process.WaitForExit();

//    if (process.ExitCode == 0) {
//        Log.Information("EF Core Scaffold-DbContext end!");
//        Log.Information(result);
//    } else {
//        Log.Error("EF Core Scaffold-DbContext Error: " + error);
//    }
//} catch (Exception ex) {
//    Log.Error("EF Core Scaffold-DbContext Failed", ex);
//}





// Internal API
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseDefaultExceptionHandler()
   .UseFastEndpoints(cfg => {
       cfg.Endpoints.RoutePrefix = "api"; // Set API route prefix (optional)
       cfg.Serializer.Options.PropertyNameCaseInsensitive = true;
       cfg.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

       // Global configuration for all endpoints
       cfg.Endpoints.Configurator = ep => {
           ep.AllowAnonymous(); // Allow all requests to be anonymous for development purposes
       };

   });

app.UseSwaggerGen();





app.Run();

