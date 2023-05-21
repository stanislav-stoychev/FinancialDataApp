namespace Infrastructure.Models;

public class RabbitMqOptions
{
    public string RabbitMQUser { get; set; }

    public string RabbitMQPassword { get; set; }

    public string RabbitMQHost { get; set; }

    public string RabbitMQVHost { get; set; }

    public int RabbitMQPort { get; set; }
}