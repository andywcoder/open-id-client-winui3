using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IdentityModel.OidcClient;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Santolibre.OpenIdClient.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Santolibre.OpenIdClient
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly OicdClientFactory _oicdClientFactory;
        private readonly HttpClientFactory _httpClientFactory;
        private readonly Dispatcher _dispatcher;
        private readonly LocalSettings _localSettings;
        private readonly MessageLoggerSink _messageLoggerSink;
        private readonly ILogger<MainPageViewModel> _logger;

        private List<LogMessageViewModel> _logMessages = new List<LogMessageViewModel>();
        private bool _isSignedIn;
        private string _selectedLogLevel;

        public ObservableCollection<LogMessageViewModel> LogMessages { get; set; } = new ObservableCollection<LogMessageViewModel>();

        public bool IsSignedIn
        {
            get { return _isSignedIn; }
            set { SetProperty(ref _isSignedIn, value); }
        }

        public string SelectedLogLevel
        {
            get { return _selectedLogLevel; }
            set
            {
                SetProperty(ref _selectedLogLevel, value);
                RenderLogMessages();
            }
        }

        public MainPageViewModel(
            OicdClientFactory oicdClientFactory,
            HttpClientFactory httpClientFactory,
            Dispatcher dispatcher,
            LocalSettings localSettings,
            MessageLoggerSink messageLoggerSink,
            ILogger<MainPageViewModel> logger)
        {
            _oicdClientFactory = oicdClientFactory;
            _httpClientFactory = httpClientFactory;
            _dispatcher = dispatcher;
            _localSettings = localSettings;
            _messageLoggerSink = messageLoggerSink;
            _logger = logger;

            _messageLoggerSink.MessageLogged += (sender, args) =>
            {
                _dispatcher.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, () =>
                {
                    _logMessages.Insert(0, new LogMessageViewModel(args.LogLevel, args.Message));
                    RenderLogMessages();
                });
            };

            IsSignedIn = _localSettings.IdentityInformation != null;
            SelectedLogLevel = LogLevel.Trace.ToString();
        }

        private void RenderLogMessages()
        {
            LogMessages.Clear();

            foreach (var logMessage in _logMessages.Where(x => x.LogLevel >= Enum.Parse<LogLevel>(SelectedLogLevel)))
            {
                LogMessages.Add(logMessage);
            }
        }

        [RelayCommand]
        private async Task SignIn(object param)
        {
            _logger.LogInformation("Signing in");

            var oidcClient = _oicdClientFactory.Create();
            var loginResult = await oidcClient.LoginAsync(new LoginRequest());

            if (!string.IsNullOrEmpty(loginResult.Error))
            {
                _logger.LogError(loginResult.Error);
                return;
            }

            _logger.LogDebug($"Claims: {string.Join(", ", loginResult.User.Claims.Select(x => $"{x.Type}: {x.Value}"))}");
            _logger.LogDebug($"Access token: {loginResult.AccessToken}");
            _logger.LogDebug($"Access token expiration: {loginResult.AccessTokenExpiration}");
            _logger.LogDebug($"Refresh token: {loginResult.RefreshToken}");
            _logger.LogDebug($"Identity token: {loginResult.IdentityToken}");

            _logger.LogInformation("Storing identity information");

            _localSettings.IdentityInformation = new IdentityInformation()
            {
                UserId = loginResult.User.Claims.Single(x => x.Type == "sub").Value,
                AccessToken = loginResult.AccessToken,
                RefreshToken = loginResult.RefreshToken,
                IdentityToken = loginResult.IdentityToken,
                AccessTokenExpiration = loginResult.AccessTokenExpiration.DateTime
            };
            _localSettings.Save();

            IsSignedIn = true;
        }

        [RelayCommand]
        private async Task SignOut(object param)
        {
            _logger.LogInformation("Signing out");

            var oidcClient = _oicdClientFactory.Create();

            int timeout = 5000;
            try
            {
                var logoutTask = oidcClient.LogoutAsync(new LogoutRequest() { IdTokenHint = _localSettings.IdentityInformation.IdentityToken });
                if (await Task.WhenAny(logoutTask, Task.Delay(timeout)) == logoutTask)
                {
                    var logoutResult = logoutTask.Result;

                    if (!string.IsNullOrEmpty(logoutResult.Error))
                    {
                        _logger.LogError(logoutResult.Error);
                        return;
                    }
                }
                else
                {
                    _logger.LogWarning("Didn't get browser redirection, forcing sign out");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while signing out ({e.Message})");
            }

            _logger.LogInformation("Deleting stored identity information");

            _localSettings.IdentityInformation = null;
            _localSettings.Save();

            IsSignedIn = false;
        }

        [RelayCommand]
        private async Task LoadUserInfo(object param)
        {
            try
            {
                _logger.LogInformation("Loading user info");
                var httpClient = _httpClientFactory.Create();
                var userInfo = await httpClient.GetStringAsync("/userinfo");
                _logger.LogInformation($"User info: {userInfo}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while loading user info ({e.Message})");
            }
        }

        [RelayCommand]
        private void ClearLogMessages(object param)
        {
            _logMessages.Clear();
            RenderLogMessages();
        }
    }
}
