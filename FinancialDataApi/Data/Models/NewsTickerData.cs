namespace FinancialDataApi.Data.Models;

public class NewsTickerData
{
    public long Id { get; set; }

    public long NewsId { get; set; }

    public string Ticker { get; set; }

    public string CandleStickChart { get; set; }

    public virtual News News { get; set; }
}
