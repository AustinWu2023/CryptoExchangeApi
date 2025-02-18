using CryptoExchangeApi.Features.Coindesk.GetApiData;
using CryptoExchangeApi.Models;
using CryptoExchangeApi.Services;
using FastEndpoints.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using RichardSzalay.MockHttp;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CryptoExchangeApi.Tests.Features.Coindesk {
    public class GetApiTests : TestBase {

        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<ICurrencyDataImporter> _currencyDataImporterMock;
        private readonly Mock<ICurrencyDatabaseService> _currencyDatabaseServiceMock;        
        private readonly HttpClient _coinDeskClient;
        private readonly HttpClient _coinCapClient;

        public GetApiTests() {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _currencyDataImporterMock = new Mock<ICurrencyDataImporter>();
            _currencyDatabaseServiceMock = new Mock<ICurrencyDatabaseService>();

            var coinDeskHandler = new MockHttpMessageHandler();
            coinDeskHandler.When("https://api.coindesk.com/*")
                           .Respond("application/json", "{\"data\": [{\"symbol\": \"BTC\", \"name\": \"Bitcoin\", \"priceUsd\": \"50000.00\"}], \"timestamp\": 1739810783644}");

            _coinDeskClient = new HttpClient(coinDeskHandler) { BaseAddress = new Uri("https://api.coindesk.com/") };

            var coinCapHandler = new MockHttpMessageHandler();
            coinCapHandler.When("https://api.coincap.io/*")
                          .Respond("application/json", "{\"data\": [{\"symbol\": \"ETH\", \"name\": \"Ethereum\", \"priceUsd\": \"4000.00\"}], \"timestamp\": 1739810783644}");

            _coinCapClient = new HttpClient(coinCapHandler) { BaseAddress = new Uri("https://api.coincap.io/") };


            _httpClientFactoryMock.Setup(f => f.CreateClient("CoinDeskApiClient")).Returns(_coinDeskClient);
            _httpClientFactoryMock.Setup(f => f.CreateClient("CoinCapApiClient")).Returns(_coinCapClient);

        }

        [Fact, Priority(1)]
        public async Task HandleAsync_ShouldReturnDataFromCoinDeskApi() {
            // Arrange
            var endpoint = new Endpoint(_dbContext, _httpClientFactoryMock.Object, _currencyDataImporterMock.Object, _currencyDatabaseServiceMock.Object);

            // Act
            await endpoint.HandleAsync(CancellationToken.None);
            var rsp = endpoint.Response;

            // Assert            
            rsp.Data.ShouldNotBeNull();
        }

        [Fact, Priority(2)]
        public async Task HandleAsync_ShouldReturn500_WhenBothApisFail() {
            // Arrange
            var failingHandler = new MockHttpMessageHandler();
            failingHandler.When("https://api.coindesk.com/*")
                          .Respond(HttpStatusCode.InternalServerError);
            failingHandler.When("https://api.coincap.io/*")
                          .Respond(HttpStatusCode.InternalServerError);

            var failingClient = new HttpClient(failingHandler) { BaseAddress = new Uri("https://api.fake.com/") };

            _httpClientFactoryMock.Setup(f => f.CreateClient("CoinDeskApiClient")).Returns(failingClient);
            _httpClientFactoryMock.Setup(f => f.CreateClient("CoinCapApiClient")).Returns(failingClient);

            var endpoint = new Endpoint(_dbContext, _httpClientFactoryMock.Object, _currencyDataImporterMock.Object, _currencyDatabaseServiceMock.Object);

            // Act
            await endpoint.HandleAsync(CancellationToken.None);
            var rsp = endpoint.Response;

            // Assert
            //Assert.Null(endpoint.Response.Data); 
            rsp.Data.ShouldBeNull();
        }
    }
}
