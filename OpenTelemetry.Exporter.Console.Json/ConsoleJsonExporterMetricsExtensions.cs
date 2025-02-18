using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace OpenTelemetry.Exporter.Console.Json;

/// <summary>
/// Extension methods to simplify registering the Console JSON exporter.
/// </summary>
[SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
public static class ConsoleJsonExporterMetricsExtensions
{
    private const int DefaultExportIntervalMilliseconds = 10000;
    private const int DefaultExportTimeoutMilliseconds = Timeout.Infinite;

    /// <summary>
    /// Adds <see cref="ConsoleMetricExporter"/> to the <see cref="MeterProviderBuilder"/> using default options.
    /// </summary>
    /// <param name="builder"><see cref="MeterProviderBuilder"/> builder to use.</param>
    /// <returns>The instance of <see cref="MeterProviderBuilder"/> to chain the calls.</returns>
    public static MeterProviderBuilder AddJsonConsoleExporter(this MeterProviderBuilder builder)
        => AddJsonConsoleExporter(builder, name: null, configureExporter: null);

    /// <summary>
    /// Adds <see cref="ConsoleMetricExporter"/> to the <see cref="MeterProviderBuilder"/>.
    /// </summary>
    /// <param name="builder"><see cref="MeterProviderBuilder"/> builder to use.</param>
    /// <param name="configureExporter">Callback action for configuring <see cref="ConsoleJsonMetricExporterOptions"/>.</param>
    /// <returns>The instance of <see cref="MeterProviderBuilder"/> to chain the calls.</returns>
    public static MeterProviderBuilder AddJsonConsoleExporter(this MeterProviderBuilder builder, Action<ConsoleJsonMetricExporterOptions> configureExporter)
        => AddJsonConsoleExporter(builder, name: null, configureExporter);

    /// <summary>
    /// Adds <see cref="ConsoleMetricExporter"/> to the <see cref="MeterProviderBuilder"/>.
    /// </summary>
    /// <param name="builder"><see cref="MeterProviderBuilder"/> builder to use.</param>
    /// <param name="name">Optional name which is used when retrieving options.</param>
    /// <param name="configureExporter">Optional callback action for configuring <see cref="ConsoleJsonMetricExporterOptions"/>.</param>
    /// <returns>The instance of <see cref="MeterProviderBuilder"/> to chain the calls.</returns>
    public static MeterProviderBuilder AddJsonConsoleExporter(
        this MeterProviderBuilder builder,
        string? name,
        Action<ConsoleJsonMetricExporterOptions>? configureExporter)
    {
        ArgumentNullException.ThrowIfNull(builder);

        name ??= Options.DefaultName;

        if (configureExporter != null)
        {
            builder.ConfigureServices(services => services.Configure(name, configureExporter));
        }

        return builder.AddReader(sp => BuildConsoleJsonExporterMetricReader(
            sp.GetRequiredService<IOptionsMonitor<ConsoleJsonMetricExporterOptions>>().Get(name),
            sp.GetRequiredService<IOptionsMonitor<MetricReaderOptions>>().Get(name)));
    }

    /// <summary>
    /// Adds <see cref="ConsoleMetricExporter"/> to the <see cref="MeterProviderBuilder"/>.
    /// </summary>
    /// <param name="builder"><see cref="MeterProviderBuilder"/> builder to use.</param>
    /// <param name="configureExporterAndMetricReader">Callback action for
    /// configuring <see cref="ConsoleJsonMetricExporterOptions"/> and <see
    /// cref="MetricReaderOptions"/>.</param>
    /// <returns>The instance of <see cref="MeterProviderBuilder"/> to chain the calls.</returns>
    public static MeterProviderBuilder AddJsonConsoleExporter(
        this MeterProviderBuilder builder,
        Action<ConsoleJsonMetricExporterOptions, MetricReaderOptions>? configureExporterAndMetricReader)
        => AddJsonConsoleExporter(builder, name: null, configureExporterAndMetricReader);

    /// <summary>
    /// Adds <see cref="ConsoleMetricExporter"/> to the <see cref="MeterProviderBuilder"/>.
    /// </summary>
    /// <param name="builder"><see cref="MeterProviderBuilder"/> builder to use.</param>
    /// <param name="name">Name which is used when retrieving options.</param>
    /// <param name="configureExporterAndMetricReader">Callback action for
    /// configuring <see cref="ConsoleJsonMetricExporterOptions"/> and <see
    /// cref="MetricReaderOptions"/>.</param>
    /// <returns>The instance of <see cref="MeterProviderBuilder"/> to chain the calls.</returns>
    public static MeterProviderBuilder AddJsonConsoleExporter(
        this MeterProviderBuilder builder,
        string? name,
        Action<ConsoleJsonMetricExporterOptions, MetricReaderOptions>? configureExporterAndMetricReader)
    {
        ArgumentNullException.ThrowIfNull(builder);

        name ??= Options.DefaultName;

        return builder.AddReader(sp =>
        {
            var exporterOptions = sp.GetRequiredService<IOptionsMonitor<ConsoleJsonMetricExporterOptions>>().Get(name);
            var metricReaderOptions = sp.GetRequiredService<IOptionsMonitor<MetricReaderOptions>>().Get(name);

            configureExporterAndMetricReader?.Invoke(exporterOptions, metricReaderOptions);

            return BuildConsoleJsonExporterMetricReader(exporterOptions, metricReaderOptions);
        });
    }

    private static MetricReader BuildConsoleJsonExporterMetricReader(
        ConsoleJsonMetricExporterOptions exporterOptions,
        MetricReaderOptions options)
    {
        var exporter = new ConsoleJsonMetricExporter(exporterOptions);

        var exportInterval =
            options.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds ?? DefaultExportIntervalMilliseconds;

        var exportTimeout =
            options.PeriodicExportingMetricReaderOptions.ExportTimeoutMilliseconds ?? DefaultExportTimeoutMilliseconds;

        var metricReader = new PeriodicExportingMetricReader(exporter, exportInterval, exportTimeout)
        {
            TemporalityPreference = options.TemporalityPreference,
        };

        return metricReader;
    }
}