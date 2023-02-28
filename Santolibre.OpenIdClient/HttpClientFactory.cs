using IdentityModel.OidcClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Santolibre.OpenIdClient
{
    public class HttpClientFactory
    {
        private readonly IConfiguration _configuration;
        private readonly OicdClientFactory _oicdClientFactory;
        private readonly LocalSettings _localSettings;
        private readonly ILoggerProvider _loggerProvider;
        private readonly ILogger<HttpClientFactory> _logger;

        public HttpClientFactory(
            IConfiguration configuration,
            OicdClientFactory oicdClientFactory,
            LocalSettings localSettings,
            ILoggerProvider loggerProvider,
            ILogger<HttpClientFactory> logger)
        {
            _configuration = configuration;
            _oicdClientFactory = oicdClientFactory;
            _localSettings = localSettings;
            _loggerProvider = loggerProvider;
            _logger = logger;
        }

        public HttpClient Create()
        {
            if (_localSettings.IdentityInformation == null)
            {
                throw new Exception("No identity information found, please sign in");
            }

            var refreshTokenDelegatingHandler = new RefreshTokenDelegatingHandler(
                _oicdClientFactory.Create(),
                _localSettings.IdentityInformation.AccessToken,
                _localSettings.IdentityInformation.RefreshToken,
                HttpHelper.CreateHttpLoggingHandler(_loggerProvider.CreateLogger(typeof(HttpClient).FullName)));

            refreshTokenDelegatingHandler.TokenRefreshed += (s, args) =>
            {
                _logger.LogDebug("Refresh access and refresh tokens");

                _localSettings.IdentityInformation.AccessToken = args.AccessToken;
                _localSettings.IdentityInformation.RefreshToken = args.RefreshToken;
                _localSettings.IdentityInformation.IdentityToken = args.IdentityToken;
                _localSettings.IdentityInformation.AccessTokenExpiration = DateTime.UtcNow + new TimeSpan(0, 0, args.ExpiresIn);

                _localSettings.IdentityInformation = _localSettings.IdentityInformation;
                _localSettings.Save();

                _logger.LogDebug("Identity information");
                _logger.LogDebug($"Access token: {_localSettings.IdentityInformation?.AccessToken}");
                _logger.LogDebug($"Refresh token: {_localSettings.IdentityInformation?.RefreshToken}");
                _logger.LogDebug($"Identity token: {_localSettings.IdentityInformation?.IdentityToken}");
                _logger.LogDebug($"Access token expiration: {_localSettings.IdentityInformation?.AccessTokenExpiration}");
            };

            var httpClient = new HttpClient(refreshTokenDelegatingHandler)
            {
                BaseAddress = new Uri(_configuration["Values:ApiServerBaseUrl"])
            };

            return httpClient;
        }
    }
}
