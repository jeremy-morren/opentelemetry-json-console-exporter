using System.Text.Json.Serialization;

namespace OpenTelemetry.Exporter.Console.Json;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UseStringEnumConverter = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals)]
[JsonSerializable(typeof(Telemetry))]
internal partial class TelemetryJsonContext : JsonSerializerContext;
