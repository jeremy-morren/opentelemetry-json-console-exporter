using OpenTelemetry.Metrics;

namespace OpenTelemetry.Exporter.Console.Json.Framework;

/// <summary>
/// Helpers for enumerating various types that do not implement <see cref="IEnumerable{T}"/>.
/// </summary>
internal static class EnumerationHelpers
{
    public static IEnumerable<KeyValuePair<string, object?>> ToEnumerable(this ReadOnlyTagCollection collection)
    {
        foreach (var pair in collection)
            yield return pair;
    }
}