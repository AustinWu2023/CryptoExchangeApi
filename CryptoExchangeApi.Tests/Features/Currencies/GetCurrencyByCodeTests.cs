using CryptoExchangeApi.Features.Currencies.Detail;
using FastEndpoints.Testing;
using Shouldly;

namespace CryptoExchangeApi.Tests.Features.Currencies {
    public class GetCurrencyByCodeTests : TestBase {

        private readonly Endpoint _endpoint;

        public GetCurrencyByCodeTests() {
            _endpoint = new Endpoint { DbContext = _dbContext };
        }

        [Fact, Priority(3)]
        public async Task GetCurrencyByCoode_ShouldReturnData() {
            // Arrange          
            var request = new Request {
                CurrencyCode = _dbContext.CryptoCurrency.First().CurrencyCode,
            };

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);
            var rsp = _endpoint.Response;

            // Assert
            rsp.ShouldNotBeNull();
            rsp.Data.ShouldNotBeNull();
            rsp.Data.Count.ShouldBeGreaterThan(0);
            _dbContext.CryptoCurrency.ShouldContain(x => x.CurrencyCode == request.CurrencyCode);
        }

        [Fact, Priority(4)]
        public async Task GetCurrencyByCode_ShouldReturnNoData() {
            // Arrange          
            var request = new Request {
                CurrencyCode = "ZZZ",
            };

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);
            var rsp = _endpoint.Response;

            // Assert
            rsp.ShouldNotBeNull();
            if (rsp.Data is List<DetailDto> data) {
                data.Count.ShouldBe(0);
            }            
        }

    }
}
