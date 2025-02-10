using System.Diagnostics;
using System.Globalization;

namespace OpenTelemetry.Exporter.Console.Json;

internal static class SerializationHelpers
{
    public static string? Serialize(this ActivitySpanId id) => id != default ? id.ToHexString() : null;

    public static string? Serialize(this ActivityTraceId id) => id != default ? id.ToHexString() : null;

    /// <summary>
    /// Returns a culture-independent string representation of the given <paramref name="exception"/> object,
    /// appropriate for diagnostics tracing.
    /// </summary>
    /// <param name="exception">Exception to convert to string.</param>
    /// <returns>Exception as string with no culture.</returns>
    public static string ToInvariantString(this Exception exception)
    {
        var originalUICulture = Thread.CurrentThread.CurrentUICulture;

        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            return exception.ToString();
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
        }
    }

    #region Tag Writer

    #endregion

    #region Dictionary

    public static Dictionary<string, string?> ToDictionary(this IEnumerable<KeyValuePair<string, string?>> source)
    {
        var count = new Dictionary<string, int>();
        var result = new Dictionary<string, string?>();
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

    public static Dictionary<string, string?> ToDictionary(this IEnumerable<KeyValuePair<string, object?>> source) =>
        ToDictionary(source.Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value?.ToString())));

    #endregion
}