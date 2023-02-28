using Microsoft.UI.Dispatching;

namespace Santolibre.OpenIdClient
{
    public class Dispatcher
    {
        public DispatcherQueue DispatcherQueue { get; set; }

        public Dispatcher()
        {
            DispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }
    }
}
