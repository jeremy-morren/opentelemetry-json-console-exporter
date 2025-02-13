using System.Diagnostics.Contracts;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace OpenTelemetry.Exporter.Console.Json.Framework;

internal static class DictionarySerializer
{
    /// <summary>
    /// Creates a dictionary from the given source (de-duping keys as necessary).
    /// </summary>
    public static Dictionary<string, T> CreateDictionary<T>(this IEnumerable<KeyValuePair<string, T>> source)
    {
        var count = new Dictionary<string, int>();
        var result = new Dictionary<string, T>();
        foreach (var (key, value) in source)
        {
            if (count.TryGetValue(key, out var c))
            {
                count[key] = c + 1;
                result.Add($"{key}_{c}", value);
            }
            else
            {
                count.Add(key, 2);
                result.Add(key, value);
            }
        }
        return result;
    }

    [Pure]
    public static IEnumerable<KeyValuePair<string, object?>> Concat(
        this IEnumerable<KeyValuePair<string, object?>> first,
        IEnumerable<KeyValuePair<string, string?>> second)
    {
        return first.Concat(second.Select(p => new KeyValuePair<string, object?>(p.Key, p.Value)));
    }
}