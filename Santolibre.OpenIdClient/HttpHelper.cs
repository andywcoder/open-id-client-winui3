using Microsoft.Extensions.Logging;
using Santolibre.OpenIdClient.Logging;
using System.Net.Http;

namespace Santolibre.OpenIdClient
{
    public static class HttpHelper
    {
        public static HttpLoggingHandler CreateHttpLoggingHandler(ILogger logger)
        {
            return new HttpLoggingHandler((name, value) =>
            {
                return value;
            },
            new HttpClientHandler(),
            logger);
        }
    }
}
