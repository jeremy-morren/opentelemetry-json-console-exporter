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
    public Dictionary<string, JsonNode?>? Resource { get; init; }
}

internal record SerializedActivity
{
    public required Dictionary<string, JsonNode?> Tags { get; init; }
}

internal record SerializedMetric
{
    public required string Name { get; init; }
    public required Dictionary<string, JsonNode?> MeterTags { get; init; }

    public required SerializedMetricPoint[] Points { get; init; }
}

internal record SerializedMetricPoint
{
    public required Dictionary<string, JsonNode?> Tags { get; init; }
}

internal record SerializedLogRecord
{
    public required string Body { get; init; }
    public required string FormattedMessage { get; init; }

    public required LogLevel LogLevel { get; init; }

    public required string CategoryName { get; init; }

    public required Dictionary<string, JsonNode?> Attributes { get; init; }

    public IReadOnlyList<Dictionary<string, JsonNode?>>? Scope { get; init; }
}