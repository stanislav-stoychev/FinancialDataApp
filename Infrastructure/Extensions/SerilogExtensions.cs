using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;

namespace Infrastructure.Extensions;

public static class SerilogExtensions
{
    public static IHostBuilder AddSerilog(
        this IHostBuilder host)
        => host.UseSerilog((ctx, cfg) =>
        {
            cfg.Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(ctx.Configuration["ElasticSearchOptions:Address"]))
                {
                    IndexFormat = ctx.Configuration["ElasticSearchOptions:IndexFormat"],
                    AutoRegisterTemplate = true,
                    NumberOfReplicas = int.Parse(ctx.Configuration["ElasticSearchOptions:Replicas"]),
                    NumberOfShards = int.Parse(ctx.Configuration["ElasticSearchOptions:Shards"])
                })
                .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
                .ReadFrom.Configuration(ctx.Configuration);
        });
}
