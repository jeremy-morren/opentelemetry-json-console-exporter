using System.Diagnostics;
using FluentAssertions;
using Serilog;
using Serilog.Context;
namespace OpenTelemetry.Exporter.Console.Json.Tests;

public class LogEventSinkTests
{
    [Fact]
    public void EmitLogEvent()
    {
        var sink = new TestLogEventSink();

        var logConf = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Sink(sink);

        const int count = 10;

        using (var logger = logConf.CreateLogger())
        {
            var log = logger.ForContext<LogEventSinkTests>();
            for (var i = 0; i < count; i++)
            {
                using var _ = BeginScope(i);
                log.Information("Hello, {Name}!. {Index} Collection: {@Collection}", "World", i, new[] { i, i * 2 });
                log.Warning("Goodbye, {Name}!. Object: {@Object}, Formatted: {Value:0.000}!. Collection: {@Collection}",
                    "World", new { Index = i }, 42.42, new List<string>());
            }
        }

        var telemetries = sink.Collect().ToList();
        telemetries.Should().HaveCount(count * 2);

        Assert.All(telemetries, (telemetry, i) =>
        {
            telemetry.Log.Should().NotBeNull();
            telemetry.Log.CategoryName.Should().Be(typeof(LogEventSinkTests).FullName);
            telemetry.Log.FormattedMessage.Should().Contain("World!");

            telemetry.Log.Attributes.Should()
                .ContainKey("Index").And
                .ContainKey("Enum").And
                .ContainKey("Collection").And
                .ContainKey("Object");
        });
    }

    private static IDisposable BeginScope(int i)
    {
        var disposable = new CompositeDisposable();
        foreach (var (key, value) in TelemetryExporterTests.GetTags(i))
            disposable.Add(LogContext.PushProperty(key, value));
        return disposable;
    }

    private sealed class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables = [];

        public void Add(IDisposable disposable) => _disposables.Add(disposable);

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}