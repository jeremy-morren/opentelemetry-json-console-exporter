using System.Diagnostics;
using OpenTelemetry.Exporter.Console.Json.Models;
using OpenTelemetry.Resources;

namespace OpenTelemetry.Exporter.Console.Json;

/// <inheritdoc />
public class ConsoleJsonActivityExporter : ConsoleJsonExporter<Activity>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleJsonActivityExporter"/> class.
    /// </summary>
    /// <param name="options"></param>
    public ConsoleJsonActivityExporter(ConsoleExporterOptions options) : base(options)
    {
    }

    internal override Telemetry CreateTelemetry(Activity value, Resource resource) => new(value, resource);
}