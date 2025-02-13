using System.Text.Json.Serialization;
using OpenTelemetry.Exporter.Console.Json.Models;

namespace Serilog.Sinks.OpenTelemetry.Console.Json.Models;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UseStringEnumConverter = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
    Converters = [typeof(ObjectJsonConverter)] )]

[JsonSerializable(typeof(Telemetry))]
internal partial class TelemetryJsonContext : JsonSerializerContext;
