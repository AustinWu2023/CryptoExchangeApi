using FastEndpoints;

namespace CryptoExchangeApi.Features.Currencies.All {
    internal sealed class Request {

    }

    internal sealed class Validator : Validator<Request> {
        public Validator() {

        }
    }

    internal sealed class Response {
        public List<CurrencyDto> Data { get; set; }
    }

    internal class CurrencyDto {
        public int Id { get; set; }
        public string CurrencyCode { get; set; } = string.Empty!;
        public string CurrencyName { get; set; } = string.Empty!;
        public decimal ExchangeRate { get; set; }
        public string UpdatedAtUTC { get; set; } = string.Empty!;
    }
}
