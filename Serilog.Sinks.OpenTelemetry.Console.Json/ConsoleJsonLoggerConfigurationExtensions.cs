using OpenTelemetry.Exporter;
using Serilog.Configuration;
using Serilog.Events;

namespace Serilog.Sinks.OpenTelemetry.Console.Json;

/// <summary>
/// Extensions for Serilog configuration.
/// </summary>
public static class ConsoleJsonLoggerConfigurationExtensions
{
    /// <summary>
    /// Writes log events to the console as Open Telemetry log records in JSON format.
    /// </summary>
    /// <param name="configuration">The logger sink configuration</param>
    /// <param name="targets">Targets to write JSON data to</param>
    /// <param name="restrictedToMinimumLevel">The minimum level for events passed through the sink</param>
    /// <param name="formatProvider">Format provider</param>
    /// <returns>The <see cref="LoggerConfiguration" /> so that further objects can be changed</returns>
    public static LoggerConfiguration OpenTelemetryConsoleJson(this LoggerSinkConfiguration configuration,
        ConsoleExporterOutputTargets targets = ConsoleExporterOutputTargets.Console,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        IFormatProvider? formatProvider = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        return configuration.Sink(new OpenTelemetryConsoleJsonSink(targets, formatProvider), restrictedToMinimumLevel);
    }
}