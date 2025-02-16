using CryptoExchangeApi.Models;
using FastEndpoints;
using Serilog;

namespace CryptoExchangeApi.Features.Currencies.Delete {
    internal sealed class Endpoint : Endpoint<Request, Response> {

        public required AppDbContext DbContext { get; set; }
        public override void Configure() {
            Delete("/Currencies/{Id}");
            Summary(s => {
                s.Summary = "Delete Currency By Id";
                s.ExampleRequest = new Request {
                    Id = 5,
                };
            });
        }

        public override async Task HandleAsync(Request r, CancellationToken c) {

            var result = await DbContext.CryptoCurrency.FindAsync(r.Id);

            if (result == null) {
                Log.Warning($"Delete Currency By Id, currency not found. Id: {r.Id}");
                await SendNotFoundAsync();
                //Response = new Response();
            }

            try {
                DbContext.CryptoCurrency.Remove(result);
                await DbContext.SaveChangesAsync(c);                
                await SendOkAsync();
                //Response = new Response();
            } catch (Exception ex) {
                Log.Error($"Delete Currency by Id failed. Message : {ex.Message}");
                await SendAsync(new Response(), 500);
                //Response = new Response();                
            }
        }
    }
}