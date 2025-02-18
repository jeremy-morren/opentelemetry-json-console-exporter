using OpenTelemetry.Exporter.Console.Json.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace OpenTelemetry.Exporter.Console.Json;

/// <inheritdoc/>
public class ConsoleJsonMetricExporter : ConsoleJsonExporter<Metric>
{
    private readonly ConsoleJsonMetricExporterOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleJsonMetricExporter"/> class.
    /// </summary>
    public ConsoleJsonMetricExporter(ConsoleJsonMetricExporterOptions options) : base(options)
    {
        _options = options;
    }

    internal override bool ShouldExport(Metric value) => _options.Filter?.Invoke(value) ?? true;

    internal override Telemetry CreateTelemetry(Metric value, Resource resource) => new(value, resource);
}