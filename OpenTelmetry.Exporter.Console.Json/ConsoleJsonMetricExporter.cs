using System.Text.Json;
using OpenTelemetry.Metrics;

namespace OpenTelemetry.Exporter.Console.Json;

/// <summary>
/// This is another Console exporter for OpenTelemetry.
/// The difference from regular console exporter is that it generates JSON for easy parsing.
/// Mainly used for debugging. And not suggested in production.
/// </summary>
public class ConsoleJsonMetricExporter : ConsoleExporter<Metric>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleJsonMetricExporter"/> class.
    /// </summary>
    /// <param name="options"></param>
    public ConsoleJsonMetricExporter(ConsoleExporterOptions options) : base(options)
    {
    }

    /// <inheritdoc />
    public override ExportResult Export(in Batch<Metric> batch)
    {
        foreach (var metric in batch)
        {
            var output = new Telemetry(metric);
            var json = JsonSerializer.Serialize(output, TelemetryJsonContext.Default.Telemetry);
            WriteLine($"{Constants.Prefix}{json}");
        }

        return ExportResult.Success;
    }

}