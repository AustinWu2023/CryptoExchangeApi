using CryptoExchangeApi.Features.Coindesk;

namespace CryptoExchangeApi.Services {
    public interface ICurrencyDatabaseService {
        Task<bool> ImportCurrenciesAsync(IEnumerable<CurrencyInfo> currencie);
    }
}
