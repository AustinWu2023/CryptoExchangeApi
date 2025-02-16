using CryptoExchangeApi.Models;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CryptoExchangeApi.Features.Currencies.Update {
    internal sealed class Endpoint : Endpoint<Request, Response> {
        public required AppDbContext DbContext { get; set; }
        public override void Configure() {
            Put("/Currencies/");
            Summary(s => {
                s.Summary = "Update Currency";
                s.ExampleRequest = new Request {
                    Id = 1,
                    CurrencyCode = "TTT",
                    CurrencyName = "測試",
                    ExchangeRate = 0
                };
            });
        }

        public override async Task HandleAsync(Request r, CancellationToken c) {

            var result = await DbContext.CryptoCurrency.FindAsync(r.Id);

            if (result == null) {
                Log.Warning($"Currency not found. Id : {r.Id}");
                await SendNotFoundAsync();
                //Response = new Response();
                //return;
            }

            try {
                result.CurrencyCode = r.CurrencyCode;
                result.CurrencyName = r.CurrencyName;
                result.ExchangeRate = r.ExchangeRate;

                DbContext.Entry(result).State = EntityState.Modified;
                await DbContext.SaveChangesAsync(c);
                await SendNoContentAsync();
                //Response = new Response();
            } catch (Exception ex) {
                Log.Error($"Update Currency failed. Message : {ex.Message}");
                await SendAsync(new Response(), 500);
                //Response = new Response();
            }

        }
    }
}