using FastEndpoints;
using FluentValidation;

namespace CryptoExchangeApi.Features.Currencies.Detail {
    internal sealed class Request {
        public string CurrencyCode { get; set; } = string.Empty;
    }

    internal sealed class Validator : Validator<Request> {
        public Validator() {           
            RuleFor(x => x.CurrencyCode).Matches("^[A-Z0-9]{1,8}$")
                .WithMessage("Code must contain 1 to 8 uppercase letters (A-Z) or digits (0-9).");
        }
    }

    internal sealed class Response {
        public List<DetailDto> Data { get; set; }
    }

    internal class DetailDto {
        public int Id { get; set; }
        public string CurrencyCode { get; set; } = string.Empty!;
        public string CurrencyName { get; set; } = string.Empty!;
        public decimal ExchangeRate { get; set; }
        public string UpdatedAtUTC { get; set; } = string.Empty!;
    }
}
