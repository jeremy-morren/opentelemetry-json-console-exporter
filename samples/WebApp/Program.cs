using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Exporter.Console.Json;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using WebApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<TelemetryLoop>();

const ConsoleExporterOutputTargets target = ConsoleExporterOutputTargets.Debug;

builder.Logging.ClearProviders()
    .AddOpenTelemetry(b =>
    {
        b.IncludeScopes = true;
        b.AddJsonConsoleExporter(o => o.Targets = target);
    });

builder.Services.AddOpenTelemetry()
    .WithTracing(b => b.AddJsonConsoleExporter(o => o.Targets = target))
    .WithMetrics(b => b.AddJsonConsoleExporter(o => o.Targets = target))

    .WithTracing(b => b
        .AddSource(TelemetryLoop.Source.Name)
        .AddSqlClientInstrumentation(o => o.SetDbStatementForText = true)
        .AddHttpClientInstrumentation()
        .AddNpgsql()
        .AddAspNetCoreInstrumentation())

    .WithMetrics(b => b
        .AddNpgsqlInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation()
        .AddProcessInstrumentation());


builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
