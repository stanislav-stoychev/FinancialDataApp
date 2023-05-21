using DataPullService.Extensions;
using DataPullService.Jobs;
using DataPullService.Options;
using DbContext;
using Infrastructure.Extensions;

namespace DataPullService;

public class Program
{
    public static async Task Main(string[] args)
    => await CreateHostBuilder(args)
        .Build()
        .RunAsync();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .AddSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                var options = hostContext.Configuration
                    .GetSection(typeof(DataPullServiceOptions).Name)
                    .Get<DataPullServiceOptions>();

                services
                    .AddHttpClient<DataPullJob>()
                    .ConfigureHttpClient(cfg =>
                        {
                            cfg.DefaultRequestHeaders.Authorization = new("Bearer", options.ApiKey);
                            cfg.BaseAddress = new(options.BaseAddress);
                        }
                     );

                services
                    .AddQuartzJobs(options)
                    .AddSingleton(options)
                    .AddSingleton<IDBContext>(new DatabaseContext(options.DataSource))
                    .AddServiceBus(options.RabbitMqOptions, options.EncryptKey)
                    .AddOTELTracing(options.TracingOptions, addSqlServerInstrumentarium: true);
            });
}