using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace OpenTelemetry.Exporter.Console.Json;

/// <summary>
/// Extension methods to simplify registering the Console json exporter
/// </summary>
public static class ConsoleJsonExporterTracingExtensions
{
    /// <summary>
    /// Adds <see cref="ConsoleJsonActivityExporter"/> to the <see cref="TracerProviderBuilder"/> using default options.
    /// </summary>
    /// <param name="builder"><see cref="TracerProviderBuilder"/> builder to use.</param>
    /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
    public static TracerProviderBuilder AddJsonConsoleExporter(this TracerProviderBuilder builder)
        => AddJsonConsoleExporter(builder, name: null, configure: null);

    /// <summary>
    /// Adds <see cref="ConsoleJsonActivityExporter"/> to the <see cref="TracerProviderBuilder"/>
    /// </summary>
    /// <param name="builder"><see cref="TracerProviderBuilder"/> builder to use.</param>
    /// <param name="configure">Callback action for configuring <see cref="ConsoleJsonActivityExporterOptions"/>.</param>
    /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
    public static TracerProviderBuilder AddJsonConsoleExporter(this TracerProviderBuilder builder,
        Action<ConsoleJsonActivityExporterOptions> configure)
        => AddJsonConsoleExporter(builder, name: null, configure);

    /// <summary>
    /// Adds <see cref="ConsoleJsonActivityExporter"/> to the <see cref="TracerProviderBuilder"/>
    /// </summary>
    /// <param name="builder"><see cref="TracerProviderBuilder"/> builder to use.</param>
    /// <param name="name">Optional name which is used when retrieving options.</param>
    /// <param name="configure">Optional callback action for configuring <see cref="ConsoleJsonActivityExporterOptions"/>.</param>
    /// <returns>The instance of <see cref="TracerProviderBuilder"/> to chain the calls.</returns>
    public static TracerProviderBuilder AddJsonConsoleExporter(
        this TracerProviderBuilder builder,
        string? name,
        Action<ConsoleJsonActivityExporterOptions>? configure)
    {
        ArgumentNullException.ThrowIfNull(builder);

        name ??= Options.DefaultName;

        if (configure != null)
        {
            builder.ConfigureServices(services => services.Configure(name, configure));
        }

        return builder.AddProcessor(sp =>
        {
            var options = sp.GetRequiredService<IOptionsMonitor<ConsoleJsonActivityExporterOptions>>().Get(name);

            return new SimpleActivityExportProcessor(new ConsoleJsonActivityExporter(options));
        });
    }
}