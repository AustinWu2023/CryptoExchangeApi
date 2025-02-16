using CryptoExchangeApi.Features.Currencies.Create;
using FastEndpoints.Testing;
using Shouldly;

namespace CryptoExchangeApi.Tests.Features.Currencies {
    public class CreateCurrencyTests : TestBase {
        
        private readonly Endpoint _endpoint;

        public CreateCurrencyTests() {

            _endpoint = new Endpoint { DbContext = _dbContext };
        }

        [Fact, Priority(1)]
        public async Task CreateCurrency_ShouldReturnSuccess() {
            // Arrange 
            var request = new Request {
                CurrencyCode = "JPY",
                CurrencyName = "JapaneseYen",
                ExchangeRate = 110.5m
            };
                       
            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);
            var rsp = _endpoint.Response;

            // Assert
            rsp.ShouldNotBeNull();
            rsp.Data.ShouldNotBeNull();
            rsp.Data.CurrencyCode.ShouldBe("JPY");
            rsp.Data.CurrencyName.ShouldBe("JapaneseYen");
            rsp.Data.ExchangeRate.ShouldBe(110.5m);
            _dbContext.CryptoCurrency.ShouldContain(x => x.CurrencyCode == "JPY");
        }
    }
}
