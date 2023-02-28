using Microsoft.Extensions.Logging;

namespace Santolibre.OpenIdClient.Logging
{
    public class LogMessageEventArgs
    {
        public LogLevel LogLevel { get; }
        public string Message { get; }

        public LogMessageEventArgs(LogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
        }
    }
}
