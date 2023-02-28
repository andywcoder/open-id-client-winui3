using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Santolibre.OpenIdClient.Logging
{
    [ProviderAlias("MessageLogger")]
    public sealed class MessageLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, MessageLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);
        private readonly MessageLoggerSink _messageLoggerSink;

        public MessageLoggerProvider(MessageLoggerSink messageLoggerSink)
        {
            _messageLoggerSink = messageLoggerSink;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new MessageLogger(_messageLoggerSink));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
