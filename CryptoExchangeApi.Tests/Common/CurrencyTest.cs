using FastEndpoints.Testing;

namespace CryptoExchangeApi.Tests.Common {
    public class CurrencyTest : StateFixture {
        public DummyDataDto DummyData { get; set; } = new DummyDataDto();

    }

    public class DummyDataDto {
        public int Id { get; set; }
        public string CurrencyCode { get; set; } = string.Empty!;
        public string CurrencyName { get; set; } = string.Empty!;
        public decimal ExchangeRate { get; set; }
    }
}
