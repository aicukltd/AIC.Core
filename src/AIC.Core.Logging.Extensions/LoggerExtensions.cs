namespace AIC.Core.Logging.Extensions;

using AIC.Core.Logging.Implementations;
using Microsoft.Extensions.Logging;

/// <summary>
///     Extensions for <see cref="ILogger" />.
/// </summary>
public static class LoggerExtensions
{
    public static void Log(this ILogger logger, LogSeverity severity, string message)
    {
        switch (severity)
        {
            case LogSeverity.Fatal:
                logger.LogCritical(message);
                break;
            case LogSeverity.Error:
                logger.LogError(message);
                break;
            case LogSeverity.Warn:
                logger.LogWarning(message);
                break;
            case LogSeverity.Info:
                logger.LogInformation(message);
                break;
            case LogSeverity.Debug:
                logger.LogDebug(message);
                break;
            default:
                throw new Exception("Unknown LogSeverity value: " + severity);
        }
    }

    public static void Log(this ILogger logger, LogSeverity severity, string message, Exception exception)
    {
        switch (severity)
        {
            case LogSeverity.Fatal:
                logger.LogCritical(message, exception);
                break;
            case LogSeverity.Error:
                logger.LogError(message, exception);
                break;
            case LogSeverity.Warn:
                logger.LogWarning(message, exception);
                break;
            case LogSeverity.Info:
                logger.LogInformation(message, exception);
                break;
            case LogSeverity.Debug:
                logger.LogDebug(message, exception);
                break;
            default:
                throw new Exception("Unknown LogSeverity value: " + severity);
        }
    }

    public static void Log(this ILogger logger, LogSeverity severity, Func<string> messageFactory)
    {
        switch (severity)
        {
            case LogSeverity.Fatal:
                logger.LogCritical(messageFactory.Invoke());
                break;
            case LogSeverity.Error:
                logger.LogError(messageFactory.Invoke());
                break;
            case LogSeverity.Warn:
                logger.LogWarning(messageFactory.Invoke());
                break;
            case LogSeverity.Info:
                logger.LogInformation(messageFactory.Invoke());
                break;
            case LogSeverity.Debug:
                logger.LogDebug(messageFactory.Invoke());
                break;
            default:
                throw new Exception("Unknown LogSeverity value: " + severity);
        }
    }
}