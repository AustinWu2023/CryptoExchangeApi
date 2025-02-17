using CryptoExchangeApi.Models;
using CryptoExchangeApi.Services;
using CryptoExchangeApi.Share;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

namespace CryptoExchangeApi.Features.Coindesk {
    internal sealed class Endpoint : EndpointWithoutRequest<Response> {

        public required AppDbContext DbContext { get; set; }

        private readonly HttpClient _coinDeskClient;
        private readonly HttpClient _coinCapClient;
        private readonly ICurrencyDataImporter _currencyDataImporter;
        private readonly ICurrencyDatabaseService _currencyDatabaseService;


        public Endpoint(IHttpClientFactory httpClientFactory, ICurrencyDataImporter currencyDataImporter, ICurrencyDatabaseService currencyDatabaseService) {
            _coinDeskClient = httpClientFactory.CreateClient("CoinDeskApiClient");
            _coinCapClient = httpClientFactory.CreateClient("CoinCapApiClient");
            _currencyDataImporter = currencyDataImporter;
            _currencyDatabaseService = currencyDatabaseService;
        }

        public override void Configure() {
            Get("/coindesk/");
            Summary(s =>            {
                s.Summary = "Get Bitcoin exchange rates from CoinDesk API or CoinCap API";                
            });
        }

        public override async Task HandleAsync(CancellationToken c) {
            string responseContent = "";

            try {
                responseContent = await _coinDeskClient.GetStringAsync("", c);
                Log.Information("Fetched data from CoinDesk API");       
                
            } catch (Exception ex) {
                Log.Warning("Failed to fetch CoinDesk API: {Message}", ex.Message);
                try {
                    responseContent = await _coinCapClient.GetStringAsync("", c);
                    Log.Information("Fetched data from CoinCap API as fallback");
                } catch (Exception fallbackEx) {
                    Log.Error("Both CoinDesk and CoinCap APIs failed: {Message}", fallbackEx.Message);
                    await SendErrorsAsync(500);
                    return;
                }
            }
            
            // 處理json data
            var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("data", out var data) || !root.TryGetProperty("timestamp", out var timestampElement)) {
                Log.Error("Invalid response format from CoinCap API");
                await SendErrorsAsync(500);
                return;
            }

            // 處理時間
            var timestamp = timestampElement.GetInt64();
            var updatedAt = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;

            var currencyNameDict = _currencyDataImporter.GetCurrencyNameDictionary();
            var currencies = data.EnumerateArray()
            .Select(coin => new CurrencyInfo {
                Symbol = coin.GetProperty("symbol").GetString()!, // 幣別
                Name = currencyNameDict.TryGetValue(coin.GetProperty("symbol").GetString()!, out var translatedName)
                       ? translatedName
                       : coin.GetProperty("name").GetString()!, // 如果找得到翻譯，就用翻譯的名稱，沒有就用原始資料的名稱
                PriceUsd = decimal.Parse(coin.GetProperty("priceUsd").GetString()!), //匯率
                UpdatedAt = DateTimeHelper.ConvertUtcToLocalTimeStr(updatedAt) // 更新時間
            })
            .OrderBy(x => x.Symbol);

            // 用時間檢查資料庫有沒有資料            
            if (await DbContext.CryptoCurrency.FirstOrDefaultAsync(f => f.UpdatedAtUtc == updatedAt) is null) {
                bool isSuccess = await _currencyDatabaseService.ImportCurrenciesAsync(currencies);
                if (!isSuccess) {
                    Log.Error("Database import failed.");
                }
            }

            await SendAsync(new Response() { Data = currencies.ToList() });
        }

    }
}