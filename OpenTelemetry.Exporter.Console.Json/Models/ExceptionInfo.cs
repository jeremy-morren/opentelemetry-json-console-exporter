using OpenTelemetry.Exporter.Console.Json.Framework;

namespace OpenTelemetry.Exporter.Console.Json.Models;

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