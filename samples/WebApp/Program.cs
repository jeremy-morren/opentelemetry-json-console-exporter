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

if (Debugger.IsAttached)
{
    const ConsoleExporterOutputTargets target = ConsoleExporterOutputTargets.Debug;

    // builder.Logging.ClearProviders()
    //     .AddOpenTelemetry(b =>
    //     {
    //         b.IncludeScopes = true;
    //         b.AddJsonConsoleExporter(o => o.Targets = target);
    //     });

    builder.Host.UseSerilog((context, services, logger) =>
    {
        logger.WriteTo.OpenTelemetryConsoleJson(target)
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();
    });

    builder.Services.AddOpenTelemetry()
        .WithTracing(b => b.AddJsonConsoleExporter(o =>
        {
            o.Targets = target;
            o.Filter = a => true;
        }))
        .WithMetrics(b => b.AddJsonConsoleExporter(o =>
        {
            o.Targets = target;
            o.Filter = m => true;
        }));
}

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
