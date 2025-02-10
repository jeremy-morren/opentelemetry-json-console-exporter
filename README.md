# Console Json Exporter for OpenTelemetry .NET

[![NuGet](https://img.shields.io/nuget/v/JeremyMorren.OpenTelemetry.Exporter.Json.Console.svg)](https://www.nuget.org/packages/JeremyMorren.OpenTelemetry.Exporter.Json.Console)
[![NuGet](https://img.shields.io/nuget/dt/JeremyMorren.OpenTelemetry.Exporter.Json.Console.svg)](https://www.nuget.org/packages/JeremyMorren.OpenTelemetry.Exporter.Json.Console)

The json console exporter prints data to the Console/Debug output in JSON format. Currently, metrics and activities (traces) are supported.

All data is printed in JSON format with `OpenTelemetry` prefix e.g. `OpenTelemetry {"activity": ...}`.

> [!WARNING]
> This component is intended to be used while learning how telemetry data is
> created and exported. It is not recommended for any production environment.

Kudos to [Özkan Pakdil](https://github.com/ozkanpakdil/opentelemetry-json-console-exporter) for the original implementation.

## Installation

```shell
dotnet add package JeremyMorren.OpenTelemetry.Exporter.Json.Console
```

## Configuration

```csharp
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var tracer = Sdk.CreateTracerProviderBuilder()
    .AddJsonConsoleExporter(o => o.Targets = ConsoleExporterOutputTargets.Debug)
    // Add activity sources to the provider
    .Build();

var metrics = Sdk.CreateMeterProviderBuilder()
    .AddJsonConsoleExporter(o => o.Targets = ConsoleExporterOutputTargets.Debug)
    // Add instrumentation sources to the provider
    .Build();
```

which enables export to debug output in JSON format.

## Environment Variables

The following environment variables can be used to override the default
values of the `PeriodicExportingMetricReaderOptions`
(following
the [OpenTelemetry specification](https://github.com/open-telemetry/opentelemetry-specification/blob/v1.12.0/specification/sdk-environment-variables.md#periodic-exporting-metricreader)).

| Environment variable          | `PeriodicExportingMetricReaderOptions` property |
|-------------------------------|-------------------------------------------------|
| `OTEL_METRIC_EXPORT_INTERVAL` | `ExportIntervalMilliseconds`                    |
| `OTEL_METRIC_EXPORT_TIMEOUT`  | `ExportTimeoutMilliseconds`                     |

## References

* [OpenTelemetry Project](https://opentelemetry.io/)
