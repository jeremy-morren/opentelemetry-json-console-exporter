using System.Collections;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using OpenTelemetry.Logs;

namespace OpenTelemetry.Exporter.Console.Json;

internal static class LogScopeHelpers
{
    public static JsonElement SerializeScope(LogRecord record)
    {
        var scopes = new List<Dictionary<string, object?>>();
        record.ForEachScope(ProcessScope, scopes);
        //NB: We don't use json context to allow for arbitrary log scope values
        //Therefore logging does not support AOT serialization
        return JsonSerializer.SerializeToElement(scopes);
    }

    private static void ProcessScope(LogRecordScope scope, List<Dictionary<string, object?>> scopes)
    {
        var dictionary = new Dictionary<string, object?>();
        var count = new Dictionary<string, int>();
        foreach (var (key,value) in scope)
        {
            if (count.TryGetValue(key, out var c))
            {
                count[key] = c + 1;
                dictionary.Add($"{key}_{c}", CreateTagValue(value));
            }
            else
            {
                count[key] = 2;
                dictionary.Add(key, CreateTagValue(value));
            }
        }
        scopes.Add(dictionary);
    }
    /// <summary>
    /// Creates tag value. Either string or JSON-serializable object.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static object? CreateTagValue(object? value)
    {
        try
        {
            return value switch
            {
                null => null,
                string s => s,
                IEnumerable<byte> bytes => Convert.ToBase64String(bytes.ToArray()),
                IEnumerable<char> chars => new string(chars.ToArray()),

                IEnumerable e => e.Cast<object?>().Select(CreateTagValue).ToArray(),

                _ => value
            };
        }
        catch (Exception e)
        {
            // An error occurred while converting the value to a string.
            // Nothing else we can do here, really.
            return e.ToInvariantString();
        }
    }

    private static KeyValuePair<string, string> CreateTag(string key, long value) => new(key, value.ToString());

    private static KeyValuePair<string, string> CreateTag(string key, double value) => new(key, value.ToString(CultureInfo.InvariantCulture));

    private static KeyValuePair<string, string> CreateTag(string key, bool value) => new(key, value ? "true" : "false");

    private static KeyValuePair<string, string> CreateTag(string key, string value) => new(key, value);
}