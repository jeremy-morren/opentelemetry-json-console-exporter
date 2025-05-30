namespace OpenTelemetry.Exporter.Console.Json;

/// <summary>
/// Helper class to add OpenTelemetry Console Json exporter to the builder.
/// </summary>
public static class OpenTelemetryBuilderExtensions
{
    /// <summary>
    /// Adds OpenTelemetry Console Json exporter to the builder, configuring it with the provided options.
    /// </summary>
    public static IOpenTelemetryBuilder AddJsonConsoleExporter(this IOpenTelemetryBuilder builder,
        Action<ConsoleExporterOptions>? configureOptions = null,
        Action<ConsoleJsonActivityExporterOptions>? configureActivityOptions = null,
        Action<ConsoleJsonMetricExporterOptions>? configureMetricOptions = null)
    {
        configureOptions ??= _ => { };
        builder.WithTracing(b => b.AddJsonConsoleExporter(o =>
        {
            configureOptions(o);
            configureActivityOptions?.Invoke(o);
        }));
        builder.WithMetrics(b => b.AddJsonConsoleExporter(o =>
        {
            configureOptions(o);
            configureMetricOptions?.Invoke(o);
        }));
        builder.WithLogging(b => b.AddJsonConsoleExporter(o => configureOptions(o)));

        return builder;
    }
}