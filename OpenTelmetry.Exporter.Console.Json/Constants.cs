namespace OpenTelemetry.Exporter.Console.Json;

internal static class Constants
{
    /// <summary>
    /// Prefix used in the output to allow parsing
    /// </summary>
    public const string Prefix = "OpenTelemetry ";

    public const string DynamicCodeMessage =
        "JSON serialization of arbitrary log scope values might require types that cannot be statically analyzed";
}