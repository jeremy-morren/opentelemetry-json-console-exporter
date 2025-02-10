using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace OpenTelemetry.Exporter.Console.Json;

/// <summary>
/// This is another Console exporter for OpenTelemetry.
/// The difference from regular console exporter is that it generates JSON for easy parsing.
/// Mainly used for debugging. And not suggested in production.
/// </summary>
[RequiresDynamicCode(Constants.DynamicCodeMessage)]
[RequiresUnreferencedCode(Constants.DynamicCodeMessage)]
public class ConsoleJsonLogRecordExporter : ConsoleExporter<LogRecord>
{
    private const int RightPaddingLength = 35;
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

    /// <inheritdoc />
    public override ExportResult Export(in Batch<LogRecord> batch)
    {
        if (_disposed)
        {
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

        foreach (var logRecord in batch)
        {
            var output = new Telemetry(logRecord, ParentProvider?.GetResource());
            var json = JsonSerializer.Serialize(output, TelemetryJsonContext.Default.Telemetry);
            WriteLine($"{Constants.Prefix}{json}");
        }

        return ExportResult.Success;
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