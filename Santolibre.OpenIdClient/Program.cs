using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using WinRT;

namespace Santolibre.OpenIdClient
{
    public static class Program
    {
        [DllImport("Microsoft.ui.xaml.dll")]
        private static extern void XamlCheckProcessRequirements();

        [GeneratedCode("Microsoft.UI.Xaml.Markup.Compiler", " 1.0.0.0")]
        [STAThread]
        static async Task Main(string[] args)
        {
            XamlCheckProcessRequirements();

            var appInstance = AppInstance.FindOrRegisterForKey("F526CB3D-F968-4AB0-9D46-B297C4947BD5");
            if (appInstance.IsCurrent)
            {
                appInstance.Activated += OnActivated;

                ComWrappersSupport.InitializeComWrappers();
                Application.Start((p) =>
                {
                    var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    new App();
                });
            }
            else
            {
                await appInstance.RedirectActivationToAsync(AppInstance.GetCurrent().GetActivatedEventArgs());
            }
        }

        private static void OnActivated(object sender, AppActivationArguments args)
        {
            if (args.Kind == ExtendedActivationKind.Protocol)
            {
                if (args.Data is IProtocolActivatedEventArgs protoArgs)
                {
                    if (App.Current is App app)
                    {
                        app.LaunchFromProtocol(protoArgs.Uri);
                    }
                }
            }
        }
    }
}
