using OpenTelemetry.Exporter.Console.Json.Framework;
using OpenTelemetry.Metrics;

namespace OpenTelemetry.Exporter.Console.Json.Models;

internal readonly struct MetricInfo
{
    private readonly Metric _metric;

    public MetricInfo(Metric metric)
    {
        _metric = metric;
    }

    public MetricType MetricType => _metric.MetricType;

    public AggregationTemporality Temporality => _metric.Temporality;

    public string Name => _metric.Name;

    public string Description => _metric.Description;

    public string Unit => _metric.Unit;

    public string MeterName => _metric.MeterName;

    public string MeterVersion => _metric.MeterVersion;

    public Dictionary<string, object?>? MeterTags => _metric.MeterTags?.CreateDictionary();
    public Dictionary<string, string?>? FormattedMeterTags => MeterTags?.FormatValues();

    public IEnumerable<MetricPointInfo> Points => EnumeratePoints(_metric.GetMetricPoints(), _metric.MetricType);

    private static IEnumerable<MetricPointInfo> EnumeratePoints(MetricPointsAccessor points, MetricType type)
    {
        foreach (var p in points)
            yield return new MetricPointInfo(p, type);
    }
}

internal readonly struct MetricPointInfo
{
    private readonly MetricPoint _metricPoint;
    private readonly MetricType _type;

    public MetricPointInfo(MetricPoint metricPoint, MetricType type)
    {
        _metricPoint = metricPoint;
        _type = type;
    }

    public DateTimeOffset StartTime => _metricPoint.StartTime;

    public DateTimeOffset EndTime => _metricPoint.EndTime;

    public long? LongSum => _type is MetricType.LongSum or MetricType.LongSumNonMonotonic ? _metricPoint.GetSumLong() : null;

    public double? DoubleSum => _type is MetricType.DoubleSum or MetricType.DoubleSumNonMonotonic ? _metricPoint.GetSumDouble() : null;

    public long? LongGauge => _type == MetricType.LongGauge ? _metricPoint.GetGaugeLastValueLong() : null;

    public double? DoubleGauge => _type == MetricType.DoubleGauge ? _metricPoint.GetGaugeLastValueDouble() : null;

    public long? HistogramCount => _type is MetricType.Histogram or MetricType.ExponentialHistogram
        ? _metricPoint.GetHistogramCount()
        : null;
    public double? HistogramSum => _type is MetricType.Histogram or MetricType.ExponentialHistogram
        ? _metricPoint.GetHistogramSum()
        : null;

    public IEnumerable<HistogramBucket>? HistogramBuckets =>
        _type is MetricType.Histogram or MetricType.ExponentialHistogram
            ? _metricPoint.GetHistogramBuckets().ToEnumerable()
            : null;

    public Dictionary<string, object?> Tags => _metricPoint.Tags.ToEnumerable().CreateDictionary();

    public Dictionary<string, string?> FormattedTags => Tags.FormatValues();
}