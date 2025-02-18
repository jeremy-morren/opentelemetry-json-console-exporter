using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenTelemetry.Exporter.Console.Json.Models;

/// <summary>
/// Json converter for object values. Avoids polymorphic serialization errors.
/// </summary>
internal class ObjectJsonConverter : JsonConverter<object?>
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions _)
    {
        if (value is Type type)
            value = type.FullName ?? type.Name;
        JsonSerializer.Serialize(writer, value, Options);
    }

    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions _)
    {
        throw new NotImplementedException();
    }
}