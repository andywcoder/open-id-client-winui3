using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Santolibre.OpenIdClient.Logging;
using System;

namespace Santolibre.OpenIdClient
{
    public partial class App : Application
    {
        private static Window _window = null!;

        public IServiceProvider Services { get; }
        public new static App Current { get { return (App)Application.Current; } }
        public static Window Window { get { return _window; } }

        public App()
        {
            Services = ConfigureServices();
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }

        internal void LaunchFromProtocol(Uri protocol)
        {
            switch (protocol.Host)
            {
                case "signin-oidc":
                case "signout-oidc":
                    SystemBrowser.ProcessResponse(protocol);
                    break;
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Trace);
                config.ClearProviders();
                config.AddMessageLogger();
            });

            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build());

            services.AddSingleton<MessageLoggerSink>();
            services.AddSingleton(LocalSettings.Load());
            services.AddSingleton<Dispatcher>();
            services.AddSingleton<HttpClientFactory>();
            services.AddSingleton<OicdClientFactory>();

            services.AddTransient<MainPageViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
