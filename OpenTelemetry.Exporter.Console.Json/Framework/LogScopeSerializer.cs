using OpenTelemetry.Exporter.Console.Json.Models;
using OpenTelemetry.Logs;

namespace OpenTelemetry.Exporter.Console.Json.Framework;

internal static class LogScopeSerializer
{
    public static List<LogRecordScopeInfo> SerializeScope(LogRecord record)
    {
        var scopes = new List<LogRecordScopeInfo>();
        record.ForEachScope(ProcessScope, scopes);
        return scopes;
    }

    private static void ProcessScope(LogRecordScope scope, List<LogRecordScopeInfo> scopes)
    {
        var values = EnumerateScope(scope).CreateDictionary();
        var formatted = values.FormatValues().ToDictionary();
        scopes.Add(new LogRecordScopeInfo()
        {
            Values = values,
            Formatted = formatted
        });
    }

    private static IEnumerable<KeyValuePair<string, object?>> EnumerateScope(LogRecordScope scope)
    {
        foreach (var p in scope)
            yield return p;
    }
}