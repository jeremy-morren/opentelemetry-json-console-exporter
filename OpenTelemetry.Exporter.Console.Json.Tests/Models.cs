using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter.Console.Json.Models;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace OpenTelemetry.Exporter.Console.Json.Tests;

internal record SerializedTelemetry
{
    public SerializedActivity? Activity { get; init; }
    public SerializedMetric? Metric { get; init; }
    public SerializedLogRecord? Log { get; init; }
    public SerializedResource? Resource { get; init; }
}

internal record SerializedResource
{
    public required Dictionary<string, JsonNode?> Attributes { get; init; }
    public required Dictionary<string, string?> FormattedAttributes { get; init; }
}

internal record SerializedActivity
{
    public required Dictionary<string, JsonNode?> Tags { get; init; }
    public required Dictionary<string, string?> FormattedTags { get; init; }
}

internal record SerializedMetric
{
    public required string Name { get; init; }
    public required Dictionary<string, JsonNode?> MeterTags { get; init; }
    public required Dictionary<string, string?> FormattedMeterTags { get; init; }

    public required SerializedMetricPoint[] Points { get; init; }
}

internal record SerializedMetricPoint
{
    public required Dictionary<string, JsonNode?> Tags { get; init; }
    public required Dictionary<string, string?> FormattedTags { get; init; }
}

internal record SerializedLogRecord
{
    public required string Body { get; init; }
    public required string FormattedMessage { get; init; }

    public required LogLevel LogLevel { get; init; }

    public required string CategoryName { get; init; }

    public required Dictionary<string, JsonNode?> Attributes { get; init; }
    public required Dictionary<string, string?> FormattedAttributes { get; init; }

    public IReadOnlyList<LogRecordScopeInfo>? Scope { get; init; }
}