using CryptoExchangeApi.Models;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
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

// External API
bld.Services.AddHttpClient("ExternalApiClient")
    .AddHttpMessageHandler<LoggingHttpHandler>();

bld.Services.AddTransient<LoggingHttpHandler>();


var app = bld.Build();


app.UseDefaultExceptionHandler();
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

}).UseSwaggerGen(); // Enable Swagger Files


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.Run();

