using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serilog;

public class RequestResponseLoggingMiddleware {
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context) {
        // 記錄內部 Request Body
        context.Request.EnableBuffering();
        var requestBody = await new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, leaveOpen: true).ReadToEndAsync();
        // Body Position 要重設，這樣後面才有辦法繼續讀取
        context.Request.Body.Position = 0; 

        Log.Information("Incoming Request: {Method} {Path} {Query} | Body: {Body}",
            context.Request.Method, context.Request.Path, context.Request.QueryString,
            string.IsNullOrWhiteSpace(requestBody) ? "[EMPTY BODY]" : requestBody);

        // 記錄內部 Response Body
        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        await _next(context); // FastEndpoints 處理

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
        responseBodyStream.Seek(0, SeekOrigin.Begin);
        await responseBodyStream.CopyToAsync(originalBodyStream);

        Log.Information("Outgoing Response: {StatusCode} | Body: {Body}",
            context.Response.StatusCode, 
            string.IsNullOrWhiteSpace(responseBody) ? "[EMPTY BODY]" : responseBody);
    }
}
