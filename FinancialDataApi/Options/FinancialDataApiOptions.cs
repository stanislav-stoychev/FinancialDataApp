using Infrastructure.Models;

namespace FinancialDataApi.Options;

public class FinancialDataApiOptions
{
    public RabbitMqOptions RabbitMqOptions { get; set; }

    public TracingOptions TracingOptions { get; set; }

    public string EncryptKey { get; set; }

    public string Issuer { get; set; }

    public string Audience { get; set; }

    public string SigningKey { get; set; }

    public int TokenExpirationTimeInMinutes { get; set; }
}