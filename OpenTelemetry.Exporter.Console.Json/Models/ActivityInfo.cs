using System.Diagnostics;
using OpenTelemetry.Exporter.Console.Json.Framework;

namespace OpenTelemetry.Exporter.Console.Json.Models;

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

    public Dictionary<string, object?> Tags => _activity.TagObjects.CreateDictionary();

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

    public Dictionary<string, object?> Tags => _activityEvent.Tags.CreateDictionary();
}

internal readonly struct ActivityLinkInfo
{
    private readonly ActivityLink _activityLink;

    public ActivityLinkInfo(ActivityLink activityLink)
    {
        _activityLink = activityLink;
    }

    public string? Context => _activityLink.Context.ToString();

    public Dictionary<string, object?>? Tags => _activityLink.Tags?.CreateDictionary();
}