using FastEndpoints;

namespace CryptoExchangeApi.Features.Coindesk {
    internal sealed class Request {

    }

    internal sealed class Validator : Validator<Request> {
        public Validator() {

        }
    }

    internal sealed class Response {
        public List<CurrencyInfo> Data { get; set; }
    }

    public class CurrencyInfo {
        public string Symbol { get; set; } = string.Empty!;
        public string Name { get; set; } = string.Empty!;
        public decimal PriceUsd { get; set; }
        public string UpdatedAt { get; set; }
    }
}
