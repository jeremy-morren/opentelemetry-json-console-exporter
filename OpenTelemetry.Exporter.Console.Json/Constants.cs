namespace OpenTelemetry.Exporter.Console.Json;

internal static class Constants
{
    /// <summary>
    /// Prefix used in the output to allow parsing
    /// </summary>
    public const string Prefix = "OpenTelemetry ";

    /// <summary>
    /// Prefix used when there is an error in serialization
    /// </summary>
    public const string ErrorPrefix = "OpenTelemetrySerializationError ";

    public const string DynamicCodeMessage =
        "JSON serialization of arbitrary tag values might require types that cannot be statically analyzed";
}