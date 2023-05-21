using Events.Models;

namespace FinancialDataApi.Models;

public class NewsMessage
{
    public Content Content { get; set; }

    public List<Ticker> Tickers { get; set; }
}
