using System.Diagnostics;
using System.Text.Json;

namespace OpenTelemetry.Exporter.Console.Json;

/// <summary>
/// This is another Console exporter for OpenTelemetry.
/// The difference from regular console exporter is that it generates JSON for easy parsing.
/// Mainly used for debugging. And not suggested in production.
/// </summary>
public class ConsoleJsonActivityExporter : ConsoleExporter<Activity>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleJsonActivityExporter"/> class.
    /// </summary>
    /// <param name="options"></param>
    public ConsoleJsonActivityExporter(ConsoleExporterOptions options) : base(options)
    {
    }

    /// <inheritdoc />
    public override ExportResult Export(in Batch<Activity> batch)
    {
        foreach (var activity in batch)
        {
            var output = new Telemetry(activity);
            var json = JsonSerializer.Serialize(output, TelemetryJsonContext.Default.Telemetry);
            WriteLine($"{Constants.Prefix}{json}");
        }

        return ExportResult.Success;
    }
}