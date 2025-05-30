using OpenTelemetry.Exporter;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry.Console.Json.Models;
using Constants = OpenTelemetry.Exporter.Console.Json.Constants;

namespace Serilog.Sinks.OpenTelemetry.Console.Json;

/// <summary>
/// A sink that writes log events to the console as open telemetry JSON objects.
/// Mainly used for debugging. And not suggested in production.
/// </summary>
public class OpenTelemetryConsoleJsonSink : ILogEventSink
{
    private readonly ConsoleExporterOutputTargets _targets;
    private readonly IFormatProvider? _formatProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenTelemetryConsoleJsonSink"/> class.
    /// </summary>
    /// <param name="targets">Targets to write output to</param>
    /// <param name="formatProvider">Format provider</param>
    public OpenTelemetryConsoleJsonSink(ConsoleExporterOutputTargets targets, IFormatProvider? formatProvider = null)
    {
        _targets = targets;
        _formatProvider = formatProvider;
    }

    /// <inheritdoc/>
    public void Emit(LogEvent logEvent)
    {
        try
        {
            var telemetry = new Telemetry(logEvent, _formatProvider);
            var json = System.Text.Json.JsonSerializer.Serialize(telemetry, Models.TelemetryJsonContext.Default.Telemetry);
            WriteLine($"{Constants.Prefix}{json}");
        }
        catch (Exception e)
        {
            SelfLog.WriteLine($"Exception while emitting event: {e}");
        }
    }

    private void WriteLine(string message)
    {
        if (_targets.HasFlag(ConsoleExporterOutputTargets.Console))
        {
            System.Console.WriteLine(message);
        }

        if (_targets.HasFlag(ConsoleExporterOutputTargets.Debug))
        {
            System.Diagnostics.Trace.WriteLine(message);
        }
    }
}