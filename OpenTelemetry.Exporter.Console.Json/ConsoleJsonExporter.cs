using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using OpenTelemetry.Exporter.Console.Json.Models;
using OpenTelemetry.Resources;

// ReSharper disable MemberCanBeProtected.Global

namespace OpenTelemetry.Exporter.Console.Json;

/// <summary>
/// This is another Console exporter for OpenTelemetry.
/// The difference from regular console exporter is that it generates JSON for easy parsing.
/// Mainly used for debugging. And not suggested in production.
/// </summary>
[RequiresDynamicCode(Constants.DynamicCodeMessage)]
[RequiresUnreferencedCode(Constants.DynamicCodeMessage)]
public abstract class ConsoleJsonExporter<T> : ConsoleExporter<T> where T : class
{
    /// <summary>
    /// Creates a new instance of <see cref="ConsoleJsonExporter{T}"/>.
    /// </summary>
    /// <param name="options"></param>
    protected ConsoleJsonExporter(ConsoleExporterOptions options) : base(options)
    {
    }

    internal abstract bool ShouldExport(T value);

    internal abstract Telemetry CreateTelemetry(T value, Resource resource);

    /// <inheritdoc />
    public override ExportResult Export(in Batch<T> batch)
    {
        var success = true;
        foreach (var value in batch)
        {
            try
            {
                if (!ShouldExport(value)) continue;
                var resource = ParentProvider.GetResource();
                var telemetry = CreateTelemetry(value, resource);
                var json = JsonSerializer.Serialize(telemetry, TelemetryJsonContext.Default.Telemetry);
                WriteLine($"{Constants.Prefix}{json}");
            }
            catch (Exception e)
            {
                success = false;
                var error = $"{e.GetType()}: {e.Message}";
                WriteLine($"{Constants.ErrorPrefix}{error}");
            }
        }
        return success ? ExportResult.Success : ExportResult.Failure;
    }
}