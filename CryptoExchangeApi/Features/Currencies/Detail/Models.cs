using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CryptoExchangeApi.Features.Currencies.Detail {
    internal sealed class Request {
        public string CurrencyCode { get; set; } = string.Empty;
    }

    internal sealed class Validator : Validator<Request> {
        public Validator() {
            RuleFor(x => x.CurrencyCode)
            .Matches("^[A-Z]{3}$") // 驗證 Code 必須是 3 個大寫字母
            .WithMessage("Code must be exactly 3 uppercase letters (A-Z)");
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
