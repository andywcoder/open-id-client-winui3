using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Santolibre.OpenIdClient
{
    public class OicdClientFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerProvider _loggerProvider;

        public OicdClientFactory(
            IConfiguration configuration,
            ILoggerProvider loggerProvider)
        {
            _configuration = configuration;
            _loggerProvider = loggerProvider;
        }

        public IdentityModel.OidcClient.OidcClient Create()
        {
            var options = new IdentityModel.OidcClient.OidcClientOptions
            {
                Authority = _configuration["Values:Authority"],
                ClientId = _configuration["Values:ClientId"],
                Scope = _configuration["Values:Scope"],
                RedirectUri = _configuration["Values:RedirectUri"],
                PostLogoutRedirectUri = _configuration["Values:PostLogoutRedirectUri"],
                Browser = new SystemBrowser()
            };

            options.RefreshTokenInnerHttpHandler = HttpHelper.CreateHttpLoggingHandler(_loggerProvider.CreateLogger(typeof(IdentityModel.OidcClient.OidcClient).FullName));

            options.LoggerFactory.AddProvider(_loggerProvider);

            var oidcClient = new IdentityModel.OidcClient.OidcClient(options);
            return oidcClient;
        }
    }
}
