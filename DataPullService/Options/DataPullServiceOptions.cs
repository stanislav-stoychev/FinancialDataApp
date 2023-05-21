using Infrastructure.Models;

namespace DataPullService.Options;

public class DataPullServiceOptions
{
    public RabbitMqOptions RabbitMqOptions { get; set; }

    public TracingOptions TracingOptions { get; set; }

    public string EncryptKey { get; set; }

    public string DataSource { get; set; }

    public string PullFinancialDataCronExpression { get; set; }

    public string ApiKey { get; set; }

    public string BaseAddress { get; set; }

    public int LookbackIntervalInHours { get; set; }

    public int BatchSize { get; set; }
}
