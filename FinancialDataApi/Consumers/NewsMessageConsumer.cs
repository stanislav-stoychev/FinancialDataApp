using FinancialDataApi.Hubs;
using FinancialDataApi.Models;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace FinancialDataApi.Consumers;

public class NewsMessageConsumer 
    : IConsumer<NewsMessage>
{
    private readonly ILogger<NewsMessageConsumer> _logger;
    private readonly IHubContext<NewsHub> _hubContext;

    public NewsMessageConsumer(
        ILogger<NewsMessageConsumer> logger,
        IHubContext<NewsHub> hubContext
    )
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task Consume(
        ConsumeContext<NewsMessage> context
    )
    {
        try
        {
            foreach(var ticker in context.Message.Tickers)
                await _hubContext.Clients.Group(ticker.TickerCode)
                    .SendAsync(
                        "NewsUpdate",
                        context.Message,
                        context.CancellationToken
                );
        }
        catch(Exception ex )
        {
            _logger.LogError(ex, "Error while distributing message to subscribers.");
        }
    }
}