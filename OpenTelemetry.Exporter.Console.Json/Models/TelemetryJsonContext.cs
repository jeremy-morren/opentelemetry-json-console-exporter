using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenTelemetry.Exporter.Console.Json.Models;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UseStringEnumConverter = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
    Converters = [typeof(ObjectJsonConverter)])]
[JsonSerializable(typeof(Telemetry))]
internal partial class TelemetryJsonContext : JsonSerializerContext
{
    /// <summary>
    /// Serializes a <see cref="Telemetry"/> object to JSON
    /// </summary>
    /// <param name="value"></param>
    /// <param name="json"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static bool TrySerializeTelemetry(Telemetry value,
        [MaybeNullWhen(false)] out string json,
        [MaybeNullWhen(true)] out string error)
    {
        try
        {
            json = JsonSerializer.Serialize(value, Default.Telemetry);
            error = null;
            return true;
        }
        catch (Exception e)
        {
            error = $"{e.GetType()}: {e.Message}";
            json = null;
            return false;
        }
    }
}