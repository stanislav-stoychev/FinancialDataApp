using Events.Models;
using Newtonsoft.Json.Linq;

namespace FinancialDataApi.Models;

public class TickerDto
{
    public string Ticker { get; set; }

    public List<CandleStick> CandleSticks { get; set; }
}
