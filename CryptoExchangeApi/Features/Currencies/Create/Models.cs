using FastEndpoints;
using FluentValidation;

namespace CryptoExchangeApi.Features.Currencies.Create {
    internal sealed class Request {
        public string CurrencyCode { get; set; } = string.Empty!;
        public string CurrencyName { get; set; } = string.Empty!;
        public decimal ExchangeRate { get; set; } = 0;
    }


    internal sealed class Validator : Validator<Request> {
        public Validator() {
            RuleFor(x => x.CurrencyCode).Matches("^[A-Z]{3}$").WithMessage("Code must be exactly 3 uppercase letters (A-Z)");
            RuleFor(x => x.CurrencyName).NotEmpty().WithMessage("CurrencyName must have at least one character.")
                .MaximumLength(50).WithMessage("CurrencyName must not exceed 50 characters.");
        }
    }

    internal sealed class Response {
        public CreateDto Data { get; set; } = new CreateDto();
    }

    internal class CreateDto {
        public int Id { get; set; }
        public string CurrencyCode { get; set; } = string.Empty!;
        public string CurrencyName { get; set; } = string.Empty!;
        public decimal ExchangeRate { get; set; } = 0;
        public string UpdatedAtUTC { get; set; } = string.Empty!;
    }

}
