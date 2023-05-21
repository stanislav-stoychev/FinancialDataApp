using DbContext;
using Infrastructure.Extensions;
using OutboxService.Options;
using OutboxService.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var options = hostContext.Configuration
            .GetSection(typeof(OutboxOptions).Name)
            .Get<OutboxOptions>();

        services
            .AddSingleton(options)
            .AddSingleton<IDBContext>(new DatabaseContext(options.DataSource, true))
            .AddServiceBus(options.RabbitMqOptions, options.EncryptKey)
            .AddOTELTracing(options.TracingOptions, addSqlServerInstrumentarium: true)
            .AddHostedService<OutboxWorkerService>();
    })
    .Build();

host.Run();