using CryptoExchangeApi.Features.Coindesk.GetApiData;

namespace CryptoExchangeApi.Services {
    public interface ICurrencyDatabaseService {
        Task<bool> ImportCurrenciesAsync(IEnumerable<CurrencyInfo> currencie);
    }
}
