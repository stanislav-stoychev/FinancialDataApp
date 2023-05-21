using Infrastructure.Models;

namespace OutboxService.Options;

public class OutboxOptions
{
    public RabbitMqOptions RabbitMqOptions { get; set; }

    public TracingOptions TracingOptions { get; set; }

    public string EncryptKey { get; set; }

    public string DataSource { get; set; }

    public int DelayInMs{ get; set; }

    public int BatchSize { get; set; }
}