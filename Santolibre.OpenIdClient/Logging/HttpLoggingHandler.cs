using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Santolibre.OpenIdClient.Logging
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        private readonly Func<string, IEnumerable<string>, IEnumerable<string>> _redactRequestHeaders;

        private readonly ILogger _logger;

        public HttpLoggingHandler(Func<string, IEnumerable<string>, IEnumerable<string>> redactRequestHeaders, HttpMessageHandler innerHandler, ILogger logger)
            : base(innerHandler)
        {
            _redactRequestHeaders = redactRequestHeaders;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await LogRequest(request);
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            await LogResponse(request, response);
            return response;
        }

        private async Task LogRequest(HttpRequestMessage request)
        {
            string requestHeaders = JsonSerializer.Serialize(request.Headers.ToDictionary((x) => x.Key, (x) => string.Join(",", _redactRequestHeaders(x.Key, x.Value))));
            string requestBodyHeaders = JsonSerializer.Serialize(request.Content?.Headers?.ToDictionary((x) => x.Key, (x) => string.Join(",", x.Value))) ?? "None";
            string text = request.Content == null ? "None" : await request.Content.ReadAsStringAsync();
            string text2 = text;
            _logger.LogTrace("HTTP request {requestMethod} {requestUrl}, Headers={requestHeaders}, BodyHeaders={requestBodyHeaders}, Body={requestBody}", request.Method, request.RequestUri, requestHeaders, requestBodyHeaders, text2);
        }

        private async Task LogResponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            string responseHeaders = JsonSerializer.Serialize(response.Headers.ToDictionary((x) => x.Key, (x) => string.Join(",", x.Value)));
            string responseBodyHeaders = JsonSerializer.Serialize(response.Content?.Headers?.ToDictionary((x) => x.Key, (x) => string.Join(",", x.Value))) ?? "None";
            string text = response.Content == null ? "None" : await response.Content.ReadAsStringAsync();
            string text2 = text;
            _logger.LogTrace("HTTP response {requestMethod} {requestUrl}, Status={responseStatusCode}, Headers={responseHeaders}, BodyHeaders={responseBodyHeaders}, Body={responseBody}", request.Method, request.RequestUri, response.StatusCode, responseHeaders, responseBodyHeaders, text2);
        }
    }
}
