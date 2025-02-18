using OpenTelemetry.Metrics;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace OpenTelemetry.Exporter.Console.Json;

/// <summary>
/// Options to configure the <see cref="ConsoleJsonMetricExporter"/>
/// </summary>
public class ConsoleJsonMetricExporterOptions : ConsoleExporterOptions
{
    /// <summary>
    /// Callback to determine which metrics should be written
    /// </summary>
    public Func<Metric, bool>? Filter { get; set; }
}