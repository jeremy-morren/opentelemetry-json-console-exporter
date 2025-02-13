using OpenTelemetry.Exporter.Console.Json.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace OpenTelemetry.Exporter.Console.Json;

/// <inheritdoc/>
public class ConsoleJsonMetricExporter : ConsoleJsonExporter<Metric>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleJsonMetricExporter"/> class.
    /// </summary>
    /// <param name="options"></param>
    public ConsoleJsonMetricExporter(ConsoleExporterOptions options) : base(options)
    {
    }

    internal override Telemetry CreateTelemetry(Metric value, Resource resource) => new(value, resource);
}