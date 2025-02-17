using Serilog;

public class LoggingHttpHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 外部 API Request Body
        var requestContent = request.Content != null ? await request.Content.ReadAsStringAsync() : string.Empty;
        Log.Information("Outgoing API Request: {Method} {Url} | Body: {Body}", 
            request.Method, request.RequestUri, 
            string.IsNullOrWhiteSpace(requestContent) ? "[EMPTY BODY]" : requestContent);
        
        var response = await base.SendAsync(request, cancellationToken);

        // 外部 API Response Body
        var responseContent = response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty;
        Log.Information("Incoming API Response: {StatusCode} | Body: {Body}", 
            response.StatusCode,
            string.IsNullOrWhiteSpace(requestContent) ? "[EMPTY BODY]" : requestContent);

        return response;
    }
}
