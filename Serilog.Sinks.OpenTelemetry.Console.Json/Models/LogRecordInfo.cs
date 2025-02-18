using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter.Console.Json.Framework;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry.Console.Json.Formatting;
using Serilog.Sinks.OpenTelemetry.Console.Json.Framework;

namespace Serilog.Sinks.OpenTelemetry.Console.Json.Models;

internal readonly record struct LogRecordInfo
{
    private readonly LogEvent _log;

    public LogRecordInfo(LogEvent log, IFormatProvider? formatProvider)
    {
        _log = log;
        FormattedMessage = CleanMessageTemplateFormatter.Format(log.MessageTemplate, log.Properties, formatProvider);
    }

    public DateTime Timestamp => _log.Timestamp.UtcDateTime;

    public string? TraceId => _log.TraceId?.Serialize();

    public string? SpanId => _log.SpanId?.Serialize();

    public string? CategoryName => _log.GetSourceContext();

    public LogLevel? LogLevel => _log.Level.ToLogLevel();

    public EventId? EventId => _log.GetEventId();

    public string? FormattedMessage { get; }

    public string Body => _log.MessageTemplate.Text;

    public Dictionary<string, object?> Attributes => _log.GetAttributeObjects();

    public ExceptionInfo? Exception => _log.Exception != null ? new ExceptionInfo(_log.Exception) : null;
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