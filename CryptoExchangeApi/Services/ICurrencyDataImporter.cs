namespace CryptoExchangeApi.Services {
    public interface ICurrencyDataImporter {
        Dictionary<string, string> GetCurrencyNameDictionary();
    }
}
