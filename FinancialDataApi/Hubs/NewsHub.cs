using Microsoft.AspNetCore.SignalR;

namespace FinancialDataApi.Hubs;

public class NewsHub
    : Hub
{
    public async Task Subscribe(
        string ticker
    )
    => await Groups.AddToGroupAsync(Context.ConnectionId, ticker);

    public async Task Unsubscribe(
        string ticker
    )
    => await Groups.RemoveFromGroupAsync(Context.ConnectionId, ticker);
}