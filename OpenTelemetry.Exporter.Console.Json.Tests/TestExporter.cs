using System.Text.Json;
using OpenTelemetry.Exporter.Console.Json.Models;
using OpenTelemetry.Resources;

namespace OpenTelemetry.Exporter.Console.Json.Tests;

internal class TestExporter<T> : BaseExporter<T> where T : class
{
    private readonly Func<T, Resource?, Telemetry> _factory;

    private readonly List<string> _telemetries = [];
    private readonly List<Exception> _exceptions = [];

    public TestExporter(Func<T, Resource?, Telemetry> factory)
    {
        _factory = factory;
    }

    public override ExportResult Export(in Batch<T> batch)
    {
        var resource = ParentProvider.GetResource();
        foreach (var item in batch)
        {
            try
            {
                var telemetry = _factory(item, resource);
                var json = JsonSerializer.Serialize(telemetry, TelemetryJsonContext.Default.Telemetry);
                _telemetries.Add(json);
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        }
        return ExportResult.Success;
    }

    public IEnumerable<SerializedTelemetry> Collect() =>
        _exceptions.Count switch
        {
            0 => _telemetries.Select(SerializedTelemetryHelpers.Deserialize),
            1 => throw _exceptions[0],
            _ => throw new AggregateException(_exceptions)
        };
}