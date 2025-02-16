using CryptoExchangeApi.Features.Currencies.All;
using FastEndpoints.Testing;
using Shouldly;

namespace CryptoExchangeApi.Tests.Features.Currencies {
    public class GetCurrenciesTests : TestBase {
        
        private readonly Endpoint _endpoint;

        public GetCurrenciesTests() {

            _endpoint = new Endpoint { DbContext = _dbContext };
        }

        [Fact, Priority(2)]
        public async Task GetCurrencies_ShouldReturnData() {
            // Act
            await _endpoint.HandleAsync(CancellationToken.None);
            var rsp = _endpoint.Response;

            // Assert
            rsp.ShouldNotBeNull(); // 回應本身非Null
            rsp.Data.ShouldNotBeNull(); // 要有資料回傳出來
            rsp.Data.Count.ShouldBeGreaterThan(0); // 最少要有一筆資料
            // 檢查資料的內容
            if (rsp.Data.Any()) {
                rsp.Data.First().CurrencyCode.ShouldNotBeNullOrEmpty(); // 確保 CurrencyCode 存在
                rsp.Data.First().CurrencyName.ShouldNotBeNullOrEmpty(); // 確保 CurrencyName 存在
                rsp.Data.First().ExchangeRate.ShouldBeGreaterThan(0); // 確保 ExchangeRate 是有效數值

            }                   
        }
    }
}
