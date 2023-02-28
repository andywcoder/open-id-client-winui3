using Microsoft.Extensions.Logging;
using System;

namespace Santolibre.OpenIdClient.Logging
{
    public sealed class MessageLogger : ILogger
    {
        private readonly MessageLoggerSink _messageLoggerSink;

        public MessageLogger(MessageLoggerSink messageLoggerSink)
        {
            _messageLoggerSink = messageLoggerSink;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);

            _messageLoggerSink.LogMessage(logLevel, message);
        }
    }
}
