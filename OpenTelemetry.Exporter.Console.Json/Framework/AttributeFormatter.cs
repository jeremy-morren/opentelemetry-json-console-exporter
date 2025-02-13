using System.Globalization;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace OpenTelemetry.Exporter.Console.Json.Framework;

internal static class AttributeFormatter
{
    public static Dictionary<string, string?> FormatValues<T>(this Dictionary<string, T> values)
    {
        return values.ToDictionary(p => p.Key, p => CreateString(p.Value));
    }

    /// <summary>
    /// Converts the given object to a string representation
    /// </summary>
    private static string? CreateString(object? o) =>
        o switch
        {
            null => null,
            string s => s,
            DateTime dt => dt.ToString("O", CultureInfo.InvariantCulture),
            DateTimeOffset dto => dto.ToString("O", CultureInfo.InvariantCulture),
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            Type t => t.FullName ?? t.Name,
            _ => o.ToString()
        };
}