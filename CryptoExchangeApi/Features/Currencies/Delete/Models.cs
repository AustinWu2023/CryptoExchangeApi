using FastEndpoints;
using FluentValidation;

namespace CryptoExchangeApi.Features.Currencies.Delete {
    internal sealed class Request {
        public int Id { get; set; }
    }

    internal sealed class Validator : Validator<Request> {
        public Validator() {
            RuleFor(x => x.Id)
            .NotNull().WithMessage("Id is required.")
            .GreaterThan(0).WithMessage("Id must be a positive integer greater than 0.");

        }
    }

    internal sealed class Response {
    }
}
