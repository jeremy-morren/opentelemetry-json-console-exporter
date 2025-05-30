# Console Json Exporter for OpenTelemetry .NET

[![NuGet](https://img.shields.io/nuget/v/JeremyMorren.OpenTelemetry.Exporter.Console.Json.svg)](https://www.nuget.org/packages/JeremyMorren.OpenTelemetry.Exporter.Console.Json)
[![NuGet](https://img.shields.io/nuget/dt/JeremyMorren.OpenTelemetry.Exporter.Console.Json.svg)](https://www.nuget.org/packages/JeremyMorren.OpenTelemetry.Exporter.Console.Json)

The json console exporter prints data to the Console/Debug output in JSON format.

All data is printed in JSON format with `OpenTelemetry` prefix e.g. `OpenTelemetry {"activity": ...}`.

> [!WARNING]
> This component is intended to be used while learning how telemetry data is
> created and exported. It is not recommended for any production environment.

Kudos to [Özkan Pakdil](https://github.com/ozkanpakdil/opentelemetry-json-console-exporter) for the original implementation.

## Installation

```shell
dotnet add package JeremyMorren.OpenTelemetry.Exporter.Console.Json
```

## Configuration

```csharp
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter.Console.Json;

var tracer = Sdk.CreateTracerProviderBuilder()
    .AddJsonConsoleExporter(o => o.Targets = ConsoleExporterOutputTargets.Debug)
    // Add activity sources to the provider
    .Build();

var metrics = Sdk.CreateMeterProviderBuilder()
    .AddJsonConsoleExporter(o => o.Targets = ConsoleExporterOutputTargets.Debug)
    // Add instrumentation sources to the provider
    .Build();
```

With ASP.NET Core:

```csharp
var builder = WebApplication.CreateBuilder(args);

if (Debugger.IsAttached)
{
    const ConsoleExporterOutputTargets target = ConsoleExporterOutputTargets.Debug;
    
    builder.Logging.ClearProviders()
        .AddOpenTelemetry(b => b.AddJsonConsoleExporter(o => o.Targets = target));

    builder.Services.AddOpenTelemetry()
        .WithTracing(b => b.AddJsonConsoleExporter(o => o.Targets = target))
        .WithMetrics(b => b.AddJsonConsoleExporter(o => o.Targets = target));
}
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


## Serilog

[![NuGet](https://img.shields.io/nuget/v/JeremyMorren.Serilog.Sinks.OpenTelemetry.Console.Json.svg)](https://www.nuget.org/packages/JeremyMorren.Serilog.Sinks.OpenTelemetry.Console.Json)
[![NuGet](https://img.shields.io/nuget/dt/JeremyMorren.Serilog.Sinks.OpenTelemetry.Console.Json.svg)](https://www.nuget.org/packages/JeremyMorren.Serilog.Sinks.OpenTelemetry.Console.Json)

A separate package is available to use with serilog.

```shell
dotnet add package JeremyMorren.Serilog.Sinks.OpenTelemetry.Console.Json
```

```csharp
using Serilog.Sinks.OpenTelemetry.Console.Json;

var logConf = new LoggerConfiguration()
    // Configure other sinks, enrichers, etc.
    // e.g. .WriteTo.Console()
    .Enrich.FromLogContext();

if (Debugger.IsAttached)
    logConf.WriteTo.OpenTelemetryConsoleJson(ConsoleExporterOutputTargets.Debug);

Log.Logger = logConf.CreateLogger();
```
