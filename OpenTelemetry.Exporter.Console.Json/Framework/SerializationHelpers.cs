using System.Diagnostics;
using System.Globalization;

namespace OpenTelemetry.Exporter.Console.Json.Framework;

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
        var originalUiCulture = Thread.CurrentThread.CurrentUICulture;

        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            return exception.ToString();
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = originalUiCulture;
        }
    }
}