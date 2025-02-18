using System.Diagnostics;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace OpenTelemetry.Exporter.Console.Json;

/// <summary>
/// Options to configure the <see cref="ConsoleJsonActivityExporter"/>
/// </summary>
public class ConsoleJsonActivityExporterOptions : ConsoleExporterOptions
{
    /// <summary>
    /// Delegate to determine which activities should be written
    /// </summary>
    public Func<Activity, bool>? Filter { get; set; }
}