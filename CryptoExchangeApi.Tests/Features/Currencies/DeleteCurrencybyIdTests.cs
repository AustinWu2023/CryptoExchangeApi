using CryptoExchangeApi.Features.Currencies.Delete;
using FastEndpoints.Testing;
using Shouldly;

namespace CryptoExchangeApi.Tests.Features.Currencies {
    public class DeleteCurrencybyIdTests : TestBase{

        private readonly Endpoint _endpoint;
        public DeleteCurrencybyIdTests() {
            _endpoint = new Endpoint { DbContext = _dbContext };
        }

        [Fact, Priority(5)]
        public async Task DeleteCurrencybyIdTests_ShouldReturnSuccess() {
            // Arrange          
            var request = new Request {
                Id = _dbContext.CryptoCurrency.First().Id,
            };

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);
            var rsp = _endpoint.Response;

            // Assert
            rsp.ShouldNotBeNull();
            var deletedCurrency = await _dbContext.CryptoCurrency.FindAsync(request.Id);
            deletedCurrency.ShouldBeNull();
        }

        [Fact, Priority(6)]
        public async Task DeleteCurrencybyIdTests_ShouldReturnFail() {
            // Arrange          
            var request = new Request {
                Id = 1000,
            };

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);
            var rsp = _endpoint.Response;

            // Assert
            rsp.ShouldNotBeNull();
            var deletedCurrency = await _dbContext.CryptoCurrency.FindAsync(request.Id);
            deletedCurrency.ShouldBeNull();
        }
    }
}
