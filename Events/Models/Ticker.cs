namespace Events.Models;

public class Ticker
{
    public string TickerCode { get; set; }

    public List<CandleStick> CandleSticks { get; set; }
}