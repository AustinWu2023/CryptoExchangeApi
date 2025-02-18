using FastEndpoints;
using FluentValidation;

namespace CryptoExchangeApi.Features.Currencies.Update {
    internal sealed class Request {
        public int Id { get; set; }
        public string CurrencyCode { get; set; } = string.Empty!;
        public string CurrencyName { get; set; } = string.Empty!;
        public decimal ExchangeRate { get; set; } = 0;
    }

    internal sealed class Validator : Validator<Request> {
        public Validator() {
            RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required.")
            .GreaterThan(0).WithMessage("Id must be a positive integer greater than 0.");
            RuleFor(x => x.CurrencyCode)
                .NotEmpty().WithMessage("CurrencyCode is required.")
                .Matches("^[A-Z0-9]{1,8}$")
                .WithMessage("Code must contain 1 to 8 uppercase letters (A-Z) or digits (0-9).");
            RuleFor(x => x.CurrencyName)
                .NotEmpty().WithMessage("CurrencyName is required.")
                .MaximumLength(50).WithMessage("CurrencyName must not exceed 50 characters.");
            RuleFor(x => x.ExchangeRate)
            .NotNull().WithMessage("ExchangeRate is required.")
            .GreaterThanOrEqualTo(0).WithMessage("ExchangeRate must be greater than 0.");
        }
    }

    internal sealed class Response {        
    }
}
