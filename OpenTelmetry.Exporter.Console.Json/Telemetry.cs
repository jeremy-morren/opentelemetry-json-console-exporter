using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace OpenTelemetry.Exporter.Console.Json;

/// <summary>
/// Object exported to console as JSON
/// </summary>
internal class Telemetry
{
    public Telemetry(Activity activity)
    {
        Activity = new ActivityInfo(activity);
    }

    public Telemetry(Metric metric)
    {
        Metric = new MetricInfo(metric);
    }

    public Telemetry(LogRecord log, Resource? resource)
    {
        Log = new LogRecordInfo(log, resource);
    }

    public ActivityInfo? Activity { get; }

    public MetricInfo? Metric { get; }

    public LogRecordInfo? Log { get; set; }
}

#region Activity

internal readonly struct ActivityInfo
{
    private readonly Activity _activity;

    public ActivityInfo(Activity activity)
    {
        _activity = activity;
    }

    public TimeSpan Duration => _activity.Duration;

    public string? Id => _activity.Id;

    public bool HasRemoteParent => _activity.HasRemoteParent;

    public ActivityKind Kind => _activity.Kind;

    public string OperationName => _activity.OperationName;

    public string DisplayName => _activity.DisplayName;

    public ActivitySource Source => _activity.Source;

    public string? ParentId => _activity.ParentId;

    public string? ParentSpanId => _activity.ParentSpanId.Serialize();

    public string? RootId => _activity.RootId;

    public string? SpanId => _activity.SpanId.Serialize();

    public DateTime StartTime => _activity.StartTimeUtc;

    public ActivityStatusCode Status => _activity.Status;

    public string? StatusDescription => _activity.StatusDescription;

    public Dictionary<string, string?> Tags => _activity.Tags.ToDictionary();

    public IEnumerable<ActivityEventInfo> Events => _activity.Events.Select(e => new ActivityEventInfo(e));

    public IEnumerable<ActivityLinkInfo> Links => _activity.Links.Select(l => new ActivityLinkInfo(l));

    public string TraceId => _activity.TraceId.ToString();

    public string? TraceStateString => _activity.TraceStateString;
}

internal readonly struct ActivityEventInfo
{
    private readonly ActivityEvent _activityEvent;

    public ActivityEventInfo(ActivityEvent activityEvent)
    {
        _activityEvent = activityEvent;
    }

    public string Name => _activityEvent.Name;

    public DateTimeOffset Timestamp => _activityEvent.Timestamp;

    public Dictionary<string, string?> Tags => _activityEvent.Tags.ToDictionary();
}

internal readonly struct ActivityLinkInfo
{
    private readonly ActivityLink _activityLink;

    public ActivityLinkInfo(ActivityLink activityLink)
    {
        _activityLink = activityLink;
    }

    public string? Context => _activityLink.Context.ToString();

    public Dictionary<string, string?>? Tags => _activityLink.Tags?.ToDictionary();
}

#endregion

#region Metric

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

    public Dictionary<string, string?>? MeterTags => _metric.MeterTags?.ToDictionary();

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
            ? EnumerateBuckets(_metricPoint.GetHistogramBuckets())
            : null;

    public Dictionary<string, string?> Tags => EnumerateTags(_metricPoint.Tags).ToDictionary();

    private static IEnumerable<HistogramBucket> EnumerateBuckets(HistogramBuckets buckets)
    {
        foreach (var b in buckets)
            yield return b;
    }

    private static IEnumerable<KeyValuePair<string, object?>> EnumerateTags(ReadOnlyTagCollection collection)
    {
        foreach (var pair in collection)
            yield return pair;
    }
}

#endregion

#region Log Record

internal readonly record struct LogRecordInfo
{
    private readonly LogRecord _log;
    private readonly Resource? _resource;

    public LogRecordInfo(LogRecord log, Resource? resource)
    {
        _log = log;
        _resource = resource;
    }

    public DateTime Timestamp => _log.Timestamp;

    public string? TraceId => _log.TraceId.Serialize();

    public string? SpanId => _log.SpanId.Serialize();

    public ActivityTraceFlags? TraceFlags => _log.TraceFlags;

    public string? TraceState => _log.TraceState;

    public string? CategoryName => _log.CategoryName;

    public LogLevel? LogLevel => _log.LogLevel;

    public EventId? EventId => _log.EventId;

    public string? FormattedMessage => _log.FormattedMessage;

    public string? Body => _log.Body;

    public Dictionary<string, string?>? Attributes => _log.Attributes?.ToDictionary();

    public ExceptionInfo? Exception => _log.Exception != null ? new ExceptionInfo(_log.Exception) : null;

    public JsonElement Scope => LogScopeHelpers.SerializeScope(_log);

    public ResourceInfo? Resource => _resource != null && _resource != Resources.Resource.Empty ? new ResourceInfo(_resource) : null;
}

internal readonly record struct ExceptionInfo
{
    private readonly Exception _exception;

    public ExceptionInfo(Exception exception)
    {
        _exception = exception;
    }

    public string? Type => _exception.GetType().FullName;

    public string Message => _exception.Message;

    public string? StackTrace => _exception.StackTrace;

    public string? Source => _exception.Source;

    public int HResult => _exception.HResult;

    public ExceptionInfo? InnerException => _exception.InnerException != null
        ? new ExceptionInfo(_exception.InnerException)
        : null;

    public string Display => _exception.ToInvariantString();
}

internal readonly record struct ResourceInfo
{
    private readonly Resource _resource;

    public ResourceInfo(Resource resource)
    {
        _resource = resource;
    }

    public Dictionary<string, string?> Attributes => _resource.Attributes!.ToDictionary();
}

#endregion
