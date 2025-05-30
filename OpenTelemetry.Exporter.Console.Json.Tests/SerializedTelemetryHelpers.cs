using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;

namespace OpenTelemetry.Exporter.Console.Json.Tests;

internal static class SerializedTelemetryHelpers
{
    public static SerializedTelemetry Deserialize(string json)
    {
        var serialized = JsonSerializer.Deserialize<SerializedTelemetry>(json, DeserializeJsonOptions);
        serialized.Should().NotBeNull();
        return serialized;
    }

    // ReSharper disable once StaticMemberInGenericType
    private static readonly JsonSerializerOptions DeserializeJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
}