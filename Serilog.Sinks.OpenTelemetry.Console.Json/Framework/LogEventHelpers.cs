using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Serilog.Sinks.OpenTelemetry.Console.Json.Framework;

internal static class LogEventHelpers
{
    public static string? GetSourceContext(this LogEvent log) =>
        log.Properties.TryGetValue("SourceContext", out var property) && property is ScalarValue { Value: string value}
            ? value
            : null;

    public static EventId? GetEventId(this LogEvent log)
    {
        if (!log.Properties.TryGetValue("EventId", out var property) || property is not StructureValue s) return null;
        if (!s.TryGetScalarValue<int>(nameof(EventId.Id), out var id)) return null;
        return s.TryGetScalarValue<string>(nameof(EventId.Name), out var name) 
            ? new EventId(id, name) 
            : new EventId(id);
    }

    private static bool TryGetScalarValue<T>(this StructureValue structure, string key, out T value)
    {
        foreach (var property in structure.Properties)
        {
            if (property.Name != key || property.Value is not ScalarValue { Value: T v })
                continue;

            value = v;
            return true;
        }

        value = default!;
        return false;
    }

    public static Dictionary<string, object?> GetAttributeObjects(this LogEvent log)
    {
        return log.Properties.ToDictionary(k => k.Key, v => Serialize(v.Value));

        static object? Serialize(LogEventPropertyValue value)
        {
            return value switch
            {
                ScalarValue v => v.Value,
                StructureValue s => s.Properties.ToDictionary(p => p.Name, p => Serialize(p.Value)),
                SequenceValue s => s.Elements.Select(Serialize).ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }
    }


    public static LogLevel ToLogLevel(this LogEventLevel level) => level switch
    {
        LogEventLevel.Verbose => LogLevel.Trace,
        LogEventLevel.Debug => LogLevel.Debug,
        LogEventLevel.Information => LogLevel.Information,
        LogEventLevel.Warning => LogLevel.Warning,
        LogEventLevel.Error => LogLevel.Error,
        LogEventLevel.Fatal => LogLevel.Critical,
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
    };
}