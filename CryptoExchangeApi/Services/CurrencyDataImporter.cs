using System.Text.Json;

namespace CryptoExchangeApi.Services {
    public class CurrencyDataImporter : ICurrencyDataImporter {

        private readonly Dictionary<string, string> _currencyNameDictionary;
        private static readonly string TranslatedJsonPath = Path.Combine(AppContext.BaseDirectory, "ImportFile", "translated_currencies.json");

        public CurrencyDataImporter() {
            _currencyNameDictionary = LoadCurrencyData();
        }

        private Dictionary<string, string> LoadCurrencyData() {
            if (!File.Exists(TranslatedJsonPath)) {
                throw new FileNotFoundException($"Currency JSON file not found: {TranslatedJsonPath}");
            }

            var jsonContent = File.ReadAllText(TranslatedJsonPath);
            var translatedData = JsonSerializer.Deserialize<List<TranslatedCurrencyDto>>(jsonContent);

            return translatedData?.ToDictionary(t => t.Symbol, t => t.Name) ?? new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetCurrencyNameDictionary() {
            return _currencyNameDictionary;
        }

        public class TranslatedCurrencyDto {
            public string Symbol { get; set; } = default!;
            public string Name { get; set; } = default!;
        }
    }
}
