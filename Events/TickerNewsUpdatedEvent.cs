using Events.Models;

namespace Events;

public class NewsUpdatedEvent
   : Event
{
    public Content Content { get; set; }

    public List<Ticker> Tickers { get; set; }
}