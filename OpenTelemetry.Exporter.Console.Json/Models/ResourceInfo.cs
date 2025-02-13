using OpenTelemetry.Exporter.Console.Json.Framework;
using OpenTelemetry.Resources;

namespace OpenTelemetry.Exporter.Console.Json.Models;

internal readonly record struct ResourceInfo
{
    private readonly Resource _resource;

    public ResourceInfo(Resource resource)
    {
        _resource = resource;
    }

    public Dictionary<string, object> Attributes => _resource.Attributes.CreateDictionary();
    public Dictionary<string, string?> FormattedAttributes => Attributes.FormatValues();
}