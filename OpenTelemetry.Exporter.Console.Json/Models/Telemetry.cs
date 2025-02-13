using System.Diagnostics;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace OpenTelemetry.Exporter.Console.Json.Models;

/// <summary>
/// Object exported to console as JSON
/// </summary>
internal class Telemetry
{
    private readonly Resource? _resource;

    private Telemetry(Resource? resource)
    {
        if (resource != Resources.Resource.Empty)
            _resource = resource;
    }

    public Telemetry(Activity activity, Resource? resource)
        : this(resource)
    {
        Activity = new ActivityInfo(activity);
    }

    public Telemetry(Metric metric, Resource? resource)
        : this(resource)
    {
        Metric = new MetricInfo(metric);
    }

    public Telemetry(LogRecord log, Resource? resource)
        : this(resource)
    {
        Log = new LogRecordInfo(log);
    }

    public ActivityInfo? Activity { get; }

    public MetricInfo? Metric { get; }

    public LogRecordInfo? Log { get; }

    public ResourceInfo? Resource => _resource != null ? new ResourceInfo(_resource) : null;
}