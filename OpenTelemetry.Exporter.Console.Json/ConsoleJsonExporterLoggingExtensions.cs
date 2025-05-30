using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace OpenTelemetry.Exporter.Console.Json;

/// <summary>
/// Helper class to simplify registering of the Console JSON exporter.
/// </summary>
public static class ConsoleJsonExporterLoggingExtensions
{
    /// <summary>
    /// Adds Json Console exporter with OpenTelemetryLoggerOptions.
    /// </summary>
    /// <param name="loggerOptions"><see cref="OpenTelemetryLoggerOptions"/> options to use.</param>
    /// <returns>The instance of <see cref="OpenTelemetryLoggerOptions"/> to chain the calls.</returns>
    // TODO: [Obsolete("Call LoggerProviderBuilder.AddJsonConsoleExporter instead this method will be removed in a future version.")]
    public static OpenTelemetryLoggerOptions AddJsonConsoleExporter(this OpenTelemetryLoggerOptions loggerOptions)
        => AddJsonConsoleExporter(loggerOptions, configure: null);

    /// <summary>
    /// Adds Json Console exporter with OpenTelemetryLoggerOptions.
    /// </summary>
    /// <param name="loggerOptions"><see cref="OpenTelemetryLoggerOptions"/> options to use.</param>
    /// <param name="configure">Optional callback action for configuring <see cref="ConsoleExporterOptions"/>.</param>
    /// <returns>The instance of <see cref="OpenTelemetryLoggerOptions"/> to chain the calls.</returns>
    // TODO: [Obsolete("Call LoggerProviderBuilder.AddJsonConsoleExporter instead this method will be removed in a future version.")]
    public static OpenTelemetryLoggerOptions AddJsonConsoleExporter(this OpenTelemetryLoggerOptions loggerOptions, Action<ConsoleExporterOptions>? configure)
    {
        ArgumentNullException.ThrowIfNull(loggerOptions);
        var options = new ConsoleExporterOptions();
        configure?.Invoke(options);
        return loggerOptions.AddProcessor(new SimpleLogRecordExportProcessor(new ConsoleJsonLogRecordExporter(options)));
    }

    /// <summary>
    /// Adds Json Console exporter with LoggerProviderBuilder.
    /// </summary>
    /// <param name="loggerProviderBuilder"><see cref="LoggerProviderBuilder"/>.</param>
    /// <returns>The supplied instance of <see cref="LoggerProviderBuilder"/> to chain the calls.</returns>
    public static LoggerProviderBuilder AddJsonConsoleExporter(
        this LoggerProviderBuilder loggerProviderBuilder)
        => AddJsonConsoleExporter(loggerProviderBuilder, name: null, configure: null);

    /// <summary>
    /// Adds Json Console exporter with LoggerProviderBuilder.
    /// </summary>
    /// <param name="loggerProviderBuilder"><see cref="LoggerProviderBuilder"/>.</param>
    /// <param name="configure">Callback action for configuring <see cref="ConsoleExporterOptions"/>.</param>
    /// <returns>The supplied instance of <see cref="LoggerProviderBuilder"/> to chain the calls.</returns>
    public static LoggerProviderBuilder AddJsonConsoleExporter(
        this LoggerProviderBuilder loggerProviderBuilder,
        Action<ConsoleExporterOptions> configure)
        => AddJsonConsoleExporter(loggerProviderBuilder, name: null, configure);

    /// <summary>
    /// Adds Json Console exporter with LoggerProviderBuilder.
    /// </summary>
    /// <param name="loggerProviderBuilder"><see cref="LoggerProviderBuilder"/>.</param>
    /// <param name="name">Optional name which is used when retrieving options.</param>
    /// <param name="configure">Optional callback action for configuring <see cref="ConsoleExporterOptions"/>.</param>
    /// <returns>The supplied instance of <see cref="LoggerProviderBuilder"/> to chain the calls.</returns>
    public static LoggerProviderBuilder AddJsonConsoleExporter(
        this LoggerProviderBuilder loggerProviderBuilder,
        string? name,
        Action<ConsoleExporterOptions>? configure)
    {
        name ??= Options.DefaultName;

        if (configure != null)
        {
            loggerProviderBuilder.ConfigureServices(services => services.Configure(name, configure));
        }

        return loggerProviderBuilder.AddProcessor(sp =>
        {
            var options = sp.GetRequiredService<IOptionsMonitor<ConsoleExporterOptions>>().Get(name);

            return new SimpleLogRecordExportProcessor(new ConsoleJsonLogRecordExporter(options));
        });
    }
}