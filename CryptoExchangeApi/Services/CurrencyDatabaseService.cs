using CryptoExchangeApi.Models;
using Serilog;

using Microsoft.EntityFrameworkCore;
using CryptoExchangeApi.Features.Coindesk.GetApiData;

namespace CryptoExchangeApi.Services {
    public class CurrencyDatabaseService : ICurrencyDatabaseService {

        private readonly AppDbContext _dbContext;
        private readonly ILogger<CurrencyDatabaseService> _logger;

        public CurrencyDatabaseService(AppDbContext dbContext, ILogger<CurrencyDatabaseService> logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> ImportCurrenciesAsync(IEnumerable<CurrencyInfo> currencies) {
            _logger.LogInformation("Starting to import {Count} currencies into database.", currencies.Count());
            try {
                var currencyData = currencies.Select(s => new CryptoCurrency() {
                    CurrencyCode = s.Symbol,
                    CurrencyName = s.Name,
                    ExchangeRate = s.PriceUsd
                }).ToList();

                _logger.LogInformation("Prepared {Count} currency records for insertion.", currencyData.Count);

                await _dbContext.CryptoCurrency.AddRangeAsync(currencyData);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Successfully imported {Count} currencies into database.", currencyData.Count);
                return true;
            } catch (Exception ex) {
                _logger.LogError(ex, "Failed to import currencies into database.");
                return false;
            }
        }
    }
}
