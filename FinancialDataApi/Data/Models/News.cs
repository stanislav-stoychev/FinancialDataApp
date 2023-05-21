namespace FinancialDataApi.Data.Models;

public class News
{
    public long Id { get; set; }

    public string ExternalId { get; set; }

    public string Description { get; set; }

    public string Contents { get; set; }

    public DateTime Date { get; set; }

    public virtual ICollection<NewsTickerData> NewsTickerDatas { get; set; }
}