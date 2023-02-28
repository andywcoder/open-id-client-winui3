using IdentityModel.OidcClient.Browser;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;

namespace Santolibre.OpenIdClient
{
    public class SystemBrowser : IBrowser
    {
        static TaskCompletionSource<BrowserResult> inFlightRequest;

        public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            inFlightRequest?.TrySetCanceled();
            inFlightRequest = new TaskCompletionSource<BrowserResult>();

            var res = Launcher.LaunchUriAsync(new Uri(options.StartUrl));

            return inFlightRequest.Task;
        }

        public static void ProcessResponse(Uri responseData)
        {
            var result = new BrowserResult
            {
                Response = responseData.OriginalString,
                ResultType = BrowserResultType.Success
            };

            inFlightRequest.SetResult(result);
            inFlightRequest = null;
        }
    }
}
