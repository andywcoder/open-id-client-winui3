using Microsoft.Extensions.Logging;
using System;

namespace Santolibre.OpenIdClient.Logging
{
    public class MessageLoggerSink
    {
        public event EventHandler<LogMessageEventArgs> MessageLogged;

        public void LogMessage(LogLevel logLevel, string message)
        {
            MessageLogged?.Invoke(this, new LogMessageEventArgs(logLevel, message));
        }
    }
}
