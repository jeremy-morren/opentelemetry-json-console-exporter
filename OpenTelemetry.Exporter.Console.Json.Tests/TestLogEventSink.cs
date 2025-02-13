using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry.Console.Json.Models;
using TelemetryJsonContext = Serilog.Sinks.OpenTelemetry.Console.Json.Models.TelemetryJsonContext;

namespace OpenTelemetry.Exporter.Console.Json.Tests;

internal class TestLogEventSink : ILogEventSink
{
    private readonly List<string> _telemetries = [];
    private readonly List<Exception> _exceptions = [];

    public void Emit(LogEvent logEvent)
    {
        try
        {
            var telemetry = new Telemetry(logEvent, null);
            var json = System.Text.Json.JsonSerializer.Serialize(telemetry, TelemetryJsonContext.Default.Telemetry);
            _telemetries.Add(json);
        }
        catch (Exception e)
        {
            _exceptions.Add(e);
        }
    }

    public IEnumerable<SerializedTelemetry> Collect() =>
        _exceptions.Count switch
        {
            0 => _telemetries.Select(SerializedTelemetryHelpers.Deserialize),
            1 => throw _exceptions[0],
            _ => throw new AggregateException(_exceptions)
        };
}