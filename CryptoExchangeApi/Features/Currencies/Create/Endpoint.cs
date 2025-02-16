using CryptoExchangeApi.Models;
using CryptoExchangeApi.Share;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using Serilog;

namespace CryptoExchangeApi.Features.Currencies.Create {
    internal sealed class Endpoint : Endpoint<Request, Response> {

        public required AppDbContext DbContext { get; set; }

        public override void Configure() {
            Post("/Currencies/");
            Summary(s => {
                s.Summary = "Create new Currency";
                s.ExampleRequest = new Request {
                    CurrencyCode = "TTT",
                    CurrencyName = "測試",
                    ExchangeRate = 0
                };
            });
        }

        public override async Task HandleAsync(Request r, CancellationToken c) {            

            var newCurrency = new CryptoCurrency() {
                CurrencyCode = r.CurrencyCode,
                CurrencyName = r.CurrencyName,
                ExchangeRate = r.ExchangeRate
            };

            try {
                DbContext.CryptoCurrency.Add(newCurrency);
                await DbContext.SaveChangesAsync(c);

                var createResult = new CreateDto() {
                    Id = newCurrency.Id,
                    CurrencyCode = newCurrency.CurrencyCode,
                    CurrencyName = newCurrency.CurrencyName,
                    ExchangeRate = newCurrency.ExchangeRate,
                    UpdatedAtUTC = DateTimeHelper.ConvertUtcToLocalTimeStr(newCurrency.UpdatedAtUTC)
                };

                Response = new Response { Data = createResult };
                
                //await SendAsync(new Response() { Data = createResult });

            } catch (Exception ex) {

                Log.Error($"Create new currency failed. Message : {ex.Message}");
                Response = new Response { Data = null };
                //await SendAsync(new Response() { Data = new() }, 500);
            }
        }
    }
}