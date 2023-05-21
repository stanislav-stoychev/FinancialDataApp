using Events.Models;
using Newtonsoft.Json.Linq;

namespace FinancialDataApi.Models;

public class NewsDto
{
    public Content Content { get; set; }

    public List<TickerDto> Tickers { get; set; }
}