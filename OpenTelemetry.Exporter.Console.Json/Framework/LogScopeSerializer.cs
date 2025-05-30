using OpenTelemetry.Exporter.Console.Json.Models;
using OpenTelemetry.Logs;

namespace OpenTelemetry.Exporter.Console.Json.Framework;

internal static class LogScopeSerializer
{
    public static List<Dictionary<string, object?>> SerializeScope(LogRecord record)
    {
        var scopes = new List<Dictionary<string, object?>>();
        record.ForEachScope(ProcessScope, scopes);
        return scopes;
    }

    private static void ProcessScope(LogRecordScope scope, List<Dictionary<string, object?>> scopes)
    {
        var values = EnumerateScope(scope).CreateDictionary();
        scopes.Add(values);
    }

    private static IEnumerable<KeyValuePair<string, object?>> EnumerateScope(LogRecordScope scope)
    {
        foreach (var p in scope)
            yield return p;
    }
}