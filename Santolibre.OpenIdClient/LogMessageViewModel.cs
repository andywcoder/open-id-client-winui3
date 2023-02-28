using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Santolibre.OpenIdClient
{
    public class LogMessageViewModel : ObservableObject
    {
        public LogLevel LogLevel { get; }
        public string Message { get; }

        public LogMessageViewModel(LogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
        }
    }
}
