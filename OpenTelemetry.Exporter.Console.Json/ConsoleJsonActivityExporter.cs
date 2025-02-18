using System.Diagnostics;
using OpenTelemetry.Exporter.Console.Json.Models;
using OpenTelemetry.Resources;

namespace OpenTelemetry.Exporter.Console.Json;

/// <inheritdoc />
public class ConsoleJsonActivityExporter : ConsoleJsonExporter<Activity>
{
    private readonly ConsoleJsonActivityExporterOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleJsonActivityExporter"/> class.
    /// </summary>
    public ConsoleJsonActivityExporter(ConsoleJsonActivityExporterOptions options) : base(options)
    {
        _options = options;
    }

    internal override bool ShouldExport(Activity value) => _options.Filter?.Invoke(value) ?? true;

    internal override Telemetry CreateTelemetry(Activity value, Resource resource) => new(value, resource);
}