using CryptoExchangeApi.Features.Currencies.Detail;
using CryptoExchangeApi.Models;
using CryptoExchangeApi.Share;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CryptoExchangeApi.Features.Currencies.All {
        
    internal sealed class Endpoint : EndpointWithoutRequest<Response> {
        public required AppDbContext DbContext { get; set; }

        public override void Configure() {
            Get("/Currencies");
            Summary(s => {
                s.Summary = "Get Currencies";
            });
        }

        public override async Task HandleAsync(CancellationToken c) {

            try {
                var result = await DbContext.CryptoCurrency
                .OrderBy(ob => ob.CurrencyCode)
                .ThenByDescending(obd => obd.UpdatedAtUtc)
                .Select(s => new CurrencyDto() {
                    Id = s.Id,
                    CurrencyCode = s.CurrencyCode,
                    CurrencyName = s.CurrencyName,
                    ExchangeRate = s.ExchangeRate,
                    UpdatedAtUTC = DateTimeHelper.ConvertUtcToLocalTimeStr(s.UpdatedAtUtc),
                }).ToListAsync(c);

                if (!result.Any()) {
                    Log.Warning($"Currencies not found.");
                }                
                await SendAsync(new Response() { Data = result });
                //Response = new Response { Data = result };
            } catch (Exception ex) {
                Log.Error($"Get Currencies failed. Message : {ex.Message}");
                await SendAsync(new Response(), 500);
                //Response = new Response { Data = null};
            }

        }
    }
}