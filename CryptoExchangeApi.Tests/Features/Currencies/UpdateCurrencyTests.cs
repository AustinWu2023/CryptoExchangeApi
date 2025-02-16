using CryptoExchangeApi.Features.Currencies.Update;
using FastEndpoints.Testing;
using Shouldly;

namespace CryptoExchangeApi.Tests.Features.Currencies {
    public class UpdateCurrencyTests : TestBase{

        private readonly Endpoint _endpoint;
        public UpdateCurrencyTests() {
            _endpoint = new Endpoint { DbContext = _dbContext };
        }

        [Fact, Priority(7)]
        public async Task UpdateCurrency_ShouldReturnSuccess() {

            // Arrange
            var updateCurrency = _dbContext.CryptoCurrency.First();
            var request = new Request {
                Id = updateCurrency.Id,
                CurrencyCode = "UDT",
                CurrencyName = "UpdateTest",
                ExchangeRate = updateCurrency.ExchangeRate,
            };

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);
            var rsp = _endpoint.Response;

            // Assert
            rsp.ShouldNotBeNull();
            var updatedCurrency = await _dbContext.CryptoCurrency.FindAsync(request.Id);
            updatedCurrency.ShouldNotBeNull();
            updatedCurrency.CurrencyCode.ShouldBe(request.CurrencyCode);
            updatedCurrency.CurrencyName.ShouldBe(request.CurrencyName);

        }

        [Fact, Priority(8)]
        public async Task UpdateCurrency_ShouldReturnFail() {

            // Arrange
            
            var request = new Request {
                Id = 1234,
                CurrencyCode = "UDT",
                CurrencyName = "UpdateTest",
                ExchangeRate = 0,
            };

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);
            var rsp = _endpoint.Response;

            // Assert
            rsp.ShouldNotBeNull();
            var updatedCurrency = await _dbContext.CryptoCurrency.FindAsync(request.Id);
            updatedCurrency.ShouldBeNull();;

        }

    }
}
