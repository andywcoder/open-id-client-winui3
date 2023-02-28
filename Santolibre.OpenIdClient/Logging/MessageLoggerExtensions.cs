using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Santolibre.OpenIdClient.Logging
{
    public static class MessageLoggerExtensions
    {
        public static ILoggingBuilder AddMessageLogger(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, MessageLoggerProvider>());

            return builder;
        }
    }
}
