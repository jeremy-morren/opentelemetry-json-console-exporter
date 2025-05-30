using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter.Console.Json.Framework;
using OpenTelemetry.Logs;

namespace OpenTelemetry.Exporter.Console.Json.Models;

internal readonly record struct LogRecordInfo
{
    private readonly LogRecord _log;

    public LogRecordInfo(LogRecord log)
    {
        _log = log;
    }

    public DateTime Timestamp => _log.Timestamp;

    public string? TraceId => _log.TraceId.Serialize();

    public string? SpanId => _log.SpanId.Serialize();

    public ActivityTraceFlags? TraceFlags => _log.TraceFlags;

    public string? TraceState => _log.TraceState;

    public string? CategoryName => _log.CategoryName;

    public LogLevel? LogLevel => _log.LogLevel;

    public EventId? EventId => _log.EventId;

    public string? FormattedMessage =>
        _log.FormattedMessage ?? _log.Attributes?.ToString(); //Attributes contains a LogMessageFormatter that is used in ToString

    public string? Body => _log.Body;

    public Dictionary<string, object?>? Attributes => _log.Attributes?.CreateDictionary();

    public ExceptionInfo? Exception => _log.Exception != null ? new ExceptionInfo(_log.Exception) : null;

    public List<Dictionary<string, object?>> Scope => LogScopeSerializer.SerializeScope(_log);
}