using Infrastructure.Models;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Utils;

namespace Infrastructure.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddServiceBus(
        this IServiceCollection services,
        RabbitMqOptions options,
        string encryptKey,
        bool registerConsumers = false,
        Action<IBusRegistrationConfigurator> additionalCfg = null
    )
    => services.AddMassTransit(x =>
    {
        x.SetKebabCaseEndpointNameFormatter();

        if (registerConsumers)
            x.AddConsumers(Assembly.GetEntryAssembly());

        additionalCfg?.Invoke(x);
        x.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(options.RabbitMQHost, options.RabbitMQVHost, hostConfigurator =>
            {
                hostConfigurator.Username(options.RabbitMQUser);
                hostConfigurator.Password(DataEncryption.DecryptString(options.RabbitMQPassword, encryptKey));
                hostConfigurator.PublisherConfirmation = false;
            });

            cfg.ConfigureEndpoints(ctx);
        });
    });
}