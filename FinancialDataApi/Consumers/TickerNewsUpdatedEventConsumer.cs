using Events;
using FinancialDataApi.Models;
using MassTransit;

namespace FinancialDataApi.Consumers;

public class TickerNewsUpdatedEventConsumer
    : IConsumer<NewsUpdatedEvent>
{
    private readonly ILogger<TickerNewsUpdatedEventConsumer> _logger;
    private readonly IBusControl _busControl;

    public TickerNewsUpdatedEventConsumer(
        ILogger<TickerNewsUpdatedEventConsumer> logger,
        IBusControl busControl
    )
    {
        _logger = logger;
        _busControl = busControl;
    }

    public async Task Consume(
        ConsumeContext<NewsUpdatedEvent> context
    )
    {
        try
        {
            await _busControl.Publish(
                new NewsMessage 
                { 
                    Tickers = context.Message.Tickers,
                    Content = context.Message.Content
                }, context.CancellationToken
            );
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Caught exception while pushing news feed data");
        }
    }
}
