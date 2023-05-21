using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Infrastructure.Extensions;

public static class OtelExtensions
{
    public static IServiceCollection AddOTELTracing(
        this IServiceCollection services,
        TracingOptions options,
        bool addSqlServerInstrumentarium = false,
        bool addHttpInstrumentarium = false
    )
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                if (addSqlServerInstrumentarium)
                    builder.AddSqlClientInstrumentation(cfg => cfg.SetDbStatementForText = true);

                if (addHttpInstrumentarium)
                    builder.AddHttpClientInstrumentation(cfg => cfg.RecordException = true);

                builder.AddAspNetCoreInstrumentation(opt => opt.RecordException = true)
                    .AddSource("MassTransit")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(options.Name))
                    .AddJaegerExporter(opts =>
                    {
                        opts.AgentHost = options.JaegerHost;
                        opts.AgentPort = options.JaegerPort;
                        opts.ExportProcessorType = ExportProcessorType.Simple;
                    });
            });

        return services;
    }
}