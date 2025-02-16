using CryptoExchangeApi.Models;
using CryptoExchangeApi.Share;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CryptoExchangeApi.Features.Currencies.Detail {
    internal sealed class Endpoint : Endpoint<Request, Response> {

        public required AppDbContext DbContext { get; set; }

        public override void Configure() {
            Get("/Currencies/{CurrencyCode}");
            Summary(s => {
                s.Summary = "Get Currencies By CurrencyCode";
                s.ExampleRequest = new Request {
                    CurrencyCode = "TTT",
                };
            });
        }

        public override async Task HandleAsync(Request r, CancellationToken c) {

            try {
                var result = await DbContext.CryptoCurrency
                .Where(w => w.CurrencyCode == r.CurrencyCode)
                .OrderByDescending(obd => obd.UpdatedAtUTC)
                .Select(s => new DetailDto() {
                    Id = s.Id,
                    CurrencyCode = s.CurrencyCode,
                    CurrencyName = s.CurrencyName,
                    ExchangeRate = s.ExchangeRate,
                    UpdatedAtUTC = DateTimeHelper.ConvertUtcToLocalTimeStr(s.UpdatedAtUTC),
                }).ToListAsync(c);

                if (!result.Any()) {
                    Log.Warning($"Currency not found. CurrencyCode : {r.CurrencyCode}");
                }

                Response = new Response { Data = result };
                //await SendAsync(new Response() { Data = result });
            } catch (Exception ex) {
                Log.Error($"Get Currencies By CurrencyCode failed. Message : {ex.Message}");

                Response = new Response { Data = null };
                //await SendAsync(new Response(), 500);
            }

        }
    }
}