using Serilog.Events;

namespace Serilog.Sinks.OpenTelemetry.Console.Json.Models;

/// <summary>
/// Object exported to console as JSON
/// </summary>
internal class Telemetry
{
    public Telemetry(LogEvent log, IFormatProvider? formatProvider)
    {
        Log = new LogRecordInfo(log, formatProvider);
    }

    public LogRecordInfo? Log { get; set; }
}

