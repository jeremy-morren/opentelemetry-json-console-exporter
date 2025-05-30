using OpenTelemetry.Exporter.Console.Json.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace OpenTelemetry.Exporter.Console.Json;

/// <inheritdoc />
public class ConsoleJsonLogRecordExporter : ConsoleJsonExporter<LogRecord>
{
#if NET9_0_OR_GREATER
    private readonly Lock _syncObject = new();
#else
    private readonly object _syncObject = new();
#endif
    private bool _disposed;
    private string? _disposedStackTrace;
    private bool _isDisposeMessageSent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleJsonLogRecordExporter"/> class.
    /// </summary>
    /// <param name="options"></param>
    public ConsoleJsonLogRecordExporter(ConsoleExporterOptions options)
        : base(options)
    {
    }

    internal override bool ShouldExport(LogRecord value) => true;

    internal override Telemetry CreateTelemetry(LogRecord value, Resource resource) => new(value, resource);

    /// <inheritdoc />
    public override ExportResult Export(in Batch<LogRecord> batch)
    {
        if (!_disposed)
            return base.Export(in batch);

        lock (_syncObject)
        {
            if (_isDisposeMessageSent)
                return ExportResult.Failure;

            _isDisposeMessageSent = true;
        }

        WriteLine("The console exporter is still being invoked after it has been disposed. This could be due to the application's incorrect lifecycle management of the LoggerFactory/OpenTelemetry .NET SDK.");
        WriteLine(Environment.StackTrace);
        WriteLine(Environment.NewLine + "Dispose was called on the following stack trace:");
        WriteLine(_disposedStackTrace!);

        return ExportResult.Failure;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
            _disposedStackTrace = Environment.StackTrace;
        }

        base.Dispose(disposing);
    }
}