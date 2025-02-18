using System.Diagnostics;
using System.Diagnostics.Metrics;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter.Console.Json.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace OpenTelemetry.Exporter.Console.Json.Tests;

public class TelemetryExporterTests
{
    private static readonly ActivitySource ActivitySource = new(nameof(TelemetryExporterTests));
    private static readonly Meter Meter = new (nameof(TelemetryExporterTests), null, GetTags(-1));

    [Fact]
    public void SerializeActivity()
    {
        var exporter = new TestExporter<Activity>((a,r) => new Telemetry(a, r));
        var builder = Sdk.CreateTracerProviderBuilder()
            .AddSource(ActivitySource.Name)
            .AddProcessor(new SimpleActivityExportProcessor(exporter));

        const int count = 10;

        using (var _ = builder.Build())
        {
            for (var i = 0; i < count; i++)
            {
                using var activity = ActivitySource.StartActivity();
                activity.Should().NotBeNull();
                activity.IsAllDataRequested.Should().BeTrue();

                foreach (var (key, value) in GetTags(i))
                    activity.SetTag(key, value);
            }
        }

        var telemetries = exporter.Collect().ToList();
        telemetries.Should().HaveCount(count);
        telemetries.Should().AllSatisfy(telemetry =>
        {
            telemetry.Resource.Should().NotBeNull();

            telemetry.Activity.Should().NotBeNull();
            telemetry.Activity.Tags.Should()
                .ContainKey("Id").And
                .ContainKey("Type").And
                .ContainKey("Index").And
                .ContainKey("Enum");
        });
    }

    [Fact]
    public void SerializeMetric()
    {
        var exporter = new TestExporter<Metric>((m, r) => new Telemetry(m, r));
        var reader = new BaseExportingMetricReader(exporter);
        var builder = Sdk.CreateMeterProviderBuilder()
            .AddMeter(Meter.Name)
            .AddReader(reader);

        const int count = 10;

        using (var _ = builder.Build())
        {
            var counter1 = Meter.CreateCounter<int>("counter1");
            Meter.CreateObservableGauge("counter2", () => 0);

            for (var i = 0; i < count; i++)
            {
                counter1.Add(i, GetTags(i));

                reader.Collect();
            }
        }

        var telemetries = exporter.Collect().ToList();

        telemetries.Should().HaveCount(count * 2 + 2);
        telemetries.Where(m => m.Metric is { Name: "counter1" }).Should().HaveCount(count + 1);
        telemetries.Where(m => m.Metric is { Name: "counter2" }).Should().HaveCount(count + 1);
        telemetries.Should().AllSatisfy(telemetry =>
        {
            telemetry.Resource.Should().NotBeNull();

            telemetry.Metric.Should().NotBeNull();
            telemetry.Metric.MeterTags.Should()
                .ContainKey("Id").And
                .ContainKey("Type").And
                .ContainKey("Index").And
                .ContainKey("Enum");

            telemetry.Metric.Points.Should().NotBeEmpty();

            if (telemetry.Metric.Name == "counter1")
                telemetry.Metric.Points.Should().AllSatisfy(p =>
                    {
                        p.Tags.Should().ContainKey("Index");
                    });
        });
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(true, false)]
    public void SerializeLogRecord(bool formatMessage, bool includeScopes)
    {
        var exporter = new TestExporter<LogRecord>((r, t) => new Telemetry(r, t));

        var services = new ServiceCollection()
            .AddLogging(lb => lb.AddOpenTelemetry(o =>
            {
                o.IncludeScopes = includeScopes;
                o.IncludeFormattedMessage = formatMessage;
                o.AddProcessor(new SimpleLogRecordExportProcessor(exporter));
            }));

        const int count = 10;
        using (var sp = services.BuildServiceProvider())
        {
            var logger = sp.GetRequiredService<ILogger<TelemetryExporterTests>>();

            for (var i = 0; i < count; i++)
            {
                using var x1 = logger.BeginScope(GetTags(i));
                using var x2 = logger.BeginScope(GetTags(i));
                logger.LogInformation("Test {IntProp}. Formatted: {FormattedProp:0.0}", i, (double)5);
                logger.LogWarning("No Props");
            }
        }

        var telemetries = exporter.Collect().ToList();
        telemetries.Should().HaveCount(count * 2);
        Assert.All(telemetries, (telemetry,i) =>
        {
            telemetry.Resource.Should().NotBeNull();
            telemetry.Log.Should().NotBeNull();

            telemetry.Log.CategoryName.Should().Be(typeof(TelemetryExporterTests).FullName);

            telemetry.Log.Attributes.Should().ContainKey("{OriginalFormat}");
            
            if (includeScopes)
            {
                telemetry.Log.Scope.Should().HaveCount(2);
                telemetry.Log.Scope.Should().AllSatisfy(scope => 
                    scope.Should()
                        .ContainKey("Id").And
                        .ContainKey("Type").And
                        .ContainKey("Index").And
                        .ContainKey("Enum"));
            }
            else
            {
                telemetry.Log.Scope.Should().BeEmpty();
            }

            if (i % 2 == 0)
            {
                telemetry.Log.LogLevel.Should().Be(LogLevel.Information);
                telemetry.Log.Attributes.Should()
                    .ContainKey("IntProp")
                    .And.ContainKey("FormattedProp");

                telemetry.Log.FormattedMessage.Should().MatchRegex(@"Test \d+\. Formatted: 5\.0");
            }
            else
            {
                telemetry.Log.LogLevel.Should().Be(LogLevel.Warning);
                telemetry.Log.FormattedMessage.Should().Be("No Props");
            }
        });
    }

    public static KeyValuePair<string, object?>[] GetTags(int i) =>
    [
        new("String", Guid.NewGuid().ToString()),
        new("Index", i),
        new("Double", (double)i),
        new("Id", Guid.NewGuid()),
        new("Enum", ActivityKind.Client),
        new("Type", typeof(TelemetryExporterTests)),
        new("Object", new { Name = "Test" })
    ];
}