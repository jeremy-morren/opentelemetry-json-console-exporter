using System.Diagnostics;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Exporter.Console.Json;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry.Console.Json;
using WebApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<TelemetryLoop>();

builder.Logging.ClearProviders();

if (Debugger.IsAttached)
{
    builder.Services.AddOpenTelemetry()
        .AddJsonConsoleExporter(
            o => o.Targets = ConsoleExporterOutputTargets.Debug,
            o => o.Filter = a => true,
            o => o.Filter = m => true);
}

builder.Host.UseSerilog((context, services, logger) =>
    {
        logger
            .WriteTo.Console()
            // .WriteTo.OpenTelemetryConsoleJson(ConsoleExporterOutputTargets.Debug)
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();
    },
    writeToProviders: true);

builder.Services.AddOpenTelemetry()
    .WithTracing(b => b
        .AddSource(TelemetryLoop.Source.Name)
        .AddSqlClientInstrumentation(o =>
        {
            o.SetDbStatementForText = true;
            o.RecordException = true;
        })
        .AddHttpClientInstrumentation(o => o.RecordException = true)
        .AddAspNetCoreInstrumentation(o => o.RecordException = true)
        .AddNpgsql())

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
